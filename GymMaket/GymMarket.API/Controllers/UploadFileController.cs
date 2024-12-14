using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly MinIOService minIOService;

        public UploadFileController(MinIOService minIOService)
        {
            this.minIOService = minIOService;
        }

        [HttpPost("up-load-file")]
        public async Task<IActionResult> Upload([FromForm] FileAdd fileAdd)
        {
            var res = await minIOService.UploadFiles(fileAdd);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }

        [HttpPost("delete-file")]
        public async Task<IActionResult> DeleteFile(DeleteFile deleteFile)
        {
            var res = await minIOService.DeleteFile(deleteFile);
            return StatusCode(res.StatusCode, new { res.Message, res.Errors });
        }
    }
}
