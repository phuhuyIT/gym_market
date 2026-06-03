using GymMarket.API.Data;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using Minio;
using Minio.DataModel.Args;

namespace GymMarket.API.Services
{
    public class MinIOService
    {
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;
        private readonly GymMarketContext _context;
        private readonly ILogger<MinIOService> _logger;

        public static readonly string IMAGE_COURSES = "imagecourses";
        public static readonly string VIDEO_COURSES = "videocourses";
        public static readonly string AVATARS = "avatars";
        public static readonly string IMAGE_TYPE = "IMAGE";
        public static readonly string VIDEO_TYPE = "VIDEO";

        public MinIOService(IMinioClient minioClient,
            IConfiguration configuration,
            GymMarketContext context,
            ILogger<MinIOService> logger)
        {
            _minioClient = minioClient;
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse> UploadFiles(FileAdd fileAdd)
        {
            List<FileCourse> fileAdds = [];

            var imageResult = await UploadFilesToBucket(fileAdd.Images, IMAGE_COURSES, IMAGE_TYPE, fileAdd.CourseId);
            if (imageResult.error != null) return imageResult.error;
            fileAdds.AddRange(imageResult.files);

            var videoResult = await UploadFilesToBucket(fileAdd.Videos, VIDEO_COURSES, VIDEO_TYPE, fileAdd.CourseId);
            if (videoResult.error != null) return videoResult.error;
            fileAdds.AddRange(videoResult.files);

            _context.FileCourses.AddRange(fileAdds);
            var r = await _context.SaveChangesAsync();
            if(r > 0)
            {
                return new ApiResponse
                {
                    StatusCode = 200,
                    Message = "SUCCESS",
                };
            }

            return new ApiResponse
            {
                StatusCode = 400,
                Errors = ["UPLOAD_FAILED"],
            };
        }

        private async Task<(List<FileCourse> files, ApiResponse? error)> UploadFilesToBucket(
            List<IFormFile> files, string bucketName, string fileType, string courseId)
        {
            var results = new List<FileCourse>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    return (results, new ApiResponse
                    {
                        StatusCode = 400,
                        Errors = ["PLEASE_SELECT_AT_LEAST_ONE_FILE"]
                    });
                }

                await EnsureBucketExists(bucketName);

                string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithStreamData(stream)
                        .WithBucket(bucketName)
                        .WithObjectSize(file.Length)
                        .WithObject(objectName)
                        .WithContentType(file.ContentType);
                    await _minioClient.PutObjectAsync(putObjectArgs);

                    results.Add(new FileCourse
                    {
                        TypeFile = fileType,
                        ObjectId = objectName,
                        Url = BuildPublicUrl(bucketName, objectName),
                        CourseId = courseId,
                    });
                }
            }

            return (results, null);
        }

        private async Task EnsureBucketExists(string bucketName)
        {
            bool isExist = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (isExist) return;

            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));

            string publicPolicy = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": "*",
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{bucketName}}/*"]
                        }
                    ]
                }
                """;

            await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                .WithBucket(bucketName)
                .WithPolicy(publicPolicy));
        }

        public async Task<string> UploadSingleFileAsync(IFormFile file, string bucketName)
        {
            await EnsureBucketExists(bucketName);
            string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            using var stream = file.OpenReadStream();
            var putObjectArgs = new PutObjectArgs()
                .WithStreamData(stream)
                .WithBucket(bucketName)
                .WithObjectSize(file.Length)
                .WithObject(objectName)
                .WithContentType(file.ContentType);
            await _minioClient.PutObjectAsync(putObjectArgs);

            return BuildPublicUrl(bucketName, objectName);
        }

        // Builds the URL used by the browser to fetch an uploaded object.
        // When MinIO:PublicBaseUrl is set (e.g. a CDN or public MinIO host in production),
        // it is used as the absolute base. Otherwise a relative path is returned so the
        // object is fetched through the app's own origin (the dev-server proxy forwards
        // /{bucket}/* to MinIO), which works in local dev and forwarded environments alike.
        private string BuildPublicUrl(string bucketName, string objectName)
        {
            var publicBaseUrl = _configuration.GetSection("MinIO")["PublicBaseUrl"];
            if (!string.IsNullOrWhiteSpace(publicBaseUrl))
            {
                return $"{publicBaseUrl.TrimEnd('/')}/{bucketName}/{objectName}";
            }

            return $"/{bucketName}/{objectName}";
        }

        public async Task<ApiResponse> DeleteFile(DeleteFile deleteFile)
        {
            if (string.IsNullOrEmpty(deleteFile.ObjectName))
            {
                return new ApiResponse { Errors = ["FILE_NAME_REQUIRED"], StatusCode = 400 };
            }

            try
            {
                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(deleteFile.BucketName);

                bool isExist = await _minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    return new ApiResponse { Errors = ["BUCKET_NOT_FOUND"], StatusCode = 400 };
                }

                RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(deleteFile.BucketName)
                    .WithObject(deleteFile.ObjectName); // Fixed: was using BucketName as ObjectName

                await _minioClient.RemoveObjectAsync(removeObjectArgs);
                return new ApiResponse { Message = "FILE_DELETED_SUCCESSFULLY", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {ObjectName} from bucket {BucketName}", deleteFile.ObjectName, deleteFile.BucketName);
                return new ApiResponse { Errors = ["DELETE_FAILED"], StatusCode = 400 };
            }
        }
    }
}
