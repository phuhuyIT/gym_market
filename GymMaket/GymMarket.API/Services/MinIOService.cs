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

            foreach (var file in fileAdd.Images)
            {
                if (file == null || file.Length == 0)
                {
                    return new ApiResponse
                    {
                        StatusCode = 400,
                        Errors = ["PLEASE_SELECT_AT_LEAST_ONE_FILE"]
                    };
                }

                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(IMAGE_COURSES);

                bool isExist = await _minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(IMAGE_COURSES);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);

                    string publicPolicy = $@"
                                    {{
                                        ""Version"": ""2012-10-17"",
                                        ""Statement"": [
                                            {{
                                                ""Effect"": ""Allow"",
                                                ""Principal"": ""*"",
                                                ""Action"": [
                                                    ""s3:GetObject""
                                                ],
                                                ""Resource"": [
                                                    ""arn:aws:s3:::{IMAGE_COURSES}/*""
                                                ]
                                            }}
                                        ]
                                    }}";

                    await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                        .WithBucket(IMAGE_COURSES)
                        .WithPolicy(publicPolicy));
                }

                string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    PutObjectArgs putObjectArgs = new PutObjectArgs();
                    putObjectArgs
                        .WithStreamData(stream)
                        .WithBucket(IMAGE_COURSES)
                        .WithObjectSize(file.Length)
                        .WithObject(objectName)
                        .WithContentType(file.ContentType);
                    await _minioClient.PutObjectAsync(putObjectArgs);

                    var fileResult = new FileCourse
                    {
                        TypeFile = IMAGE_TYPE,
                        ObjectId = objectName,
                        Url = $"{_configuration.GetSection("MinIO")["Protocol"]}://{_configuration.GetSection("MinIO")["Endpoint"]}/{IMAGE_COURSES}/{objectName}",
                        CourseId = fileAdd.CourseId,
                    };
                    fileAdds.Add(fileResult);
                }
            }

            foreach (var file in fileAdd.Videos)
            {
                if (file == null || file.Length == 0)
                {
                    return new ApiResponse
                    {
                        StatusCode = 400,
                        Errors = ["PLEASE_SELECT_AT_LEAST_ONE_FILE"]
                    };
                }

                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(VIDEO_COURSES);

                bool isExist = await _minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(VIDEO_COURSES);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);

                    string publicPolicy = $@"
                                    {{
                                        ""Version"": ""2012-10-17"",
                                        ""Statement"": [
                                            {{
                                                ""Effect"": ""Allow"",
                                                ""Principal"": ""*"",
                                                ""Action"": [
                                                    ""s3:GetObject""
                                                ],
                                                ""Resource"": [
                                                    ""arn:aws:s3:::{VIDEO_COURSES}/*""
                                                ]
                                            }}
                                        ]
                                    }}";

                    await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                        .WithBucket(VIDEO_COURSES)
                        .WithPolicy(publicPolicy));
                }

                string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    PutObjectArgs putObjectArgs = new PutObjectArgs();
                    putObjectArgs
                        .WithStreamData(stream)
                        .WithBucket(VIDEO_COURSES)
                        .WithObjectSize(file.Length)
                        .WithObject(objectName)
                        .WithContentType(file.ContentType);
                    await _minioClient.PutObjectAsync(putObjectArgs);

                    var fileResult = new FileCourse
                    {
                        TypeFile = VIDEO_TYPE,
                        ObjectId = objectName,
                        Url = $"{_configuration.GetSection("MinIO")["Protocol"]}://{_configuration.GetSection("MinIO")["Endpoint"]}/{VIDEO_COURSES}/{objectName}",
                        CourseId = fileAdd.CourseId,
                    };
                    fileAdds.Add(fileResult);
                }
            }

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
