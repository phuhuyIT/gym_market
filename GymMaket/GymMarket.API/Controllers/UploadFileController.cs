using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadFileController : ControllerBase
    {
        private readonly MinIOService _minioService;

        public UploadFileController(MinIOService minIOService)
        {
            _minioService = minIOService;
        }

        [HttpPost("up-load-file")]
        public async Task<IActionResult> Upload([FromForm] FileAdd fileAdd)
        {
            var res = await _minioService.UploadFiles(fileAdd);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }

        [HttpPost("delete-file")]
        public async Task<IActionResult> DeleteFile(DeleteFile deleteFile)
        {
            var res = await _minioService.DeleteFile(deleteFile);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }
    }
}
