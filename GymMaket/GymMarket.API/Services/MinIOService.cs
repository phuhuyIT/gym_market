
using GymMarket.API.Data;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using Minio;
using Minio.DataModel.Args;
using System.Security.AccessControl;

namespace GymMarket.API.Services
{
    public class MinIOService
    {
        private readonly IMinioClient minioClient;
        private readonly IConfiguration configuration;
        private readonly GymMarketContext context;

        public static readonly string IMAGE_COURSES = "imagecourses";
        public static readonly string VIDEO_COURSES = "videocourses";
        public static readonly string IMAGE_TYPE = "IMAGE";
        public static readonly string VIDEO_TYPE = "VIDEO";

        public MinIOService(IMinioClient minioClient, 
            IConfiguration configuration, 
            GymMarketContext context)
        {
            this.minioClient = minioClient;
            this.configuration = configuration;
            this.context = context;
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
                        Errors = ["Vui lòng chọn ít nhất 1 file"]
                    };
                }

                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(IMAGE_COURSES);

                // Kiểm tra nếu bucket chưa có, tạo mới
                bool isExist = await minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(IMAGE_COURSES);
                    await minioClient.MakeBucketAsync(makeBucketArgs);

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

                    // Áp dụng Access Policy
                    await minioClient.SetPolicyAsync(new SetPolicyArgs()
                        .WithBucket(IMAGE_COURSES)
                        .WithPolicy(publicPolicy));
                }

                string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Tải file lên MinIO
                using (var stream = file.OpenReadStream())
                {
                    PutObjectArgs putObjectArgs = new PutObjectArgs();
                    putObjectArgs
                        .WithStreamData(stream)
                        .WithBucket(IMAGE_COURSES)
                        .WithObjectSize(file.Length)
                        .WithObject(objectName)
                        .WithContentType(file.ContentType);
                    await minioClient.PutObjectAsync(putObjectArgs);

                    var fileResult = new FileCourse
                    {
                        TypeFile = IMAGE_TYPE,
                        ObjectId = objectName,
                        Url = $"{configuration.GetSection("MinIO")["Protocol"]}://{configuration.GetSection("MinIO")["Endpoint"]}/{IMAGE_COURSES}/{objectName}",
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
                        Errors = ["Vui lòng chọn ít nhất 1 file"]
                    };
                }

                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(VIDEO_COURSES);

                // Kiểm tra nếu bucket chưa có, tạo mới
                bool isExist = await minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(VIDEO_COURSES);
                    await minioClient.MakeBucketAsync(makeBucketArgs);

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

                    // Áp dụng Access Policy
                    await minioClient.SetPolicyAsync(new SetPolicyArgs()
                        .WithBucket(VIDEO_COURSES)
                        .WithPolicy(publicPolicy));
                }

                string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Tải file lên MinIO
                using (var stream = file.OpenReadStream())
                {
                    PutObjectArgs putObjectArgs = new PutObjectArgs();
                    putObjectArgs
                        .WithStreamData(stream)
                        .WithBucket(VIDEO_COURSES)
                        .WithObjectSize(file.Length)
                        .WithObject(objectName)
                        .WithContentType(file.ContentType);
                    await minioClient.PutObjectAsync(putObjectArgs);

                    var fileResult = new FileCourse
                    {
                        TypeFile = VIDEO_TYPE,
                        ObjectId = objectName,
                        Url = $"{configuration.GetSection("MinIO")["Protocol"]}://{configuration.GetSection("MinIO")["Endpoint"]}/{VIDEO_COURSES}/{objectName}",
                        CourseId = fileAdd.CourseId,
                    };
                    fileAdds.Add(fileResult);
                }
            }

            context.FileCourses.AddRange(fileAdds);
            var r = await context.SaveChangesAsync();
            if(r > 0)
            {
                return new ApiResponse
                {
                    StatusCode = 200,
                    Message = "",
                };
            }

            return new ApiResponse
            {
                StatusCode = 400,
                Errors = ["Vui lòng thử lại."],
            };
        }

        public async Task<ApiResponse> DeleteFile(DeleteFile deleteFile)
        {
            if (string.IsNullOrEmpty(deleteFile.ObjectName))
            {
                return new ApiResponse { Errors = ["Vui lòng chọn tên file"], StatusCode = 400 };
            }

            try
            {
                // Kiểm tra bucket có tồn tại hay không
                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(deleteFile.BucketName);

                bool isExist = await minioClient.BucketExistsAsync(bucketExistsArgs);
                if (!isExist)
                {
                    return new ApiResponse { Errors = ["Không tìm thấy bucket"], StatusCode = 400 };
                }

                // Xóa object từ bucket
                RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(deleteFile.BucketName)
                    .WithObject(deleteFile.BucketName);

                await minioClient.RemoveObjectAsync(removeObjectArgs);
                return new ApiResponse { Errors = ["Xóa file thành công"], StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Errors = ["Xóa file không thành công. Vui lòng thử lại"], StatusCode = 400 };
            }
        }
    }
}
