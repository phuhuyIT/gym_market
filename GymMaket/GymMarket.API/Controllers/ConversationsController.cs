using GymMarket.API.DTOs.Response.Account;
using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly MinIOService _minioService;

        public ConversationsController(IConversationRepository conversationRepository, MinIOService minioService)
        {
            _conversationRepository = conversationRepository;
            _minioService = minioService;
        }

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation(CreateConversationDto model)
        {
            var res = await _conversationRepository.CreateConversation(model);
            return StatusCode(res.StatusCode, new { res.Errors, res.Message });
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupDto model)
        {
            var res = await _conversationRepository.CreateGroup(model, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        [HttpPost("add-members")]
        public async Task<IActionResult> AddMembers(AddMembersDto model)
        {
            var res = await _conversationRepository.AddMembers(model, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        [HttpPost("remove-member/{conversationId}/{userId}")]
        public async Task<IActionResult> RemoveMember(int conversationId, string userId)
        {
            var res = await _conversationRepository.RemoveMember(conversationId, userId, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        [HttpPost("leave-group/{conversationId}")]
        public async Task<IActionResult> LeaveGroup(int conversationId)
        {
            var res = await _conversationRepository.LeaveGroup(conversationId, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        [HttpPost("update-member-role")]
        public async Task<IActionResult> UpdateMemberRole(UpdateMemberRoleDto model)
        {
            var res = await _conversationRepository.UpdateMemberRole(model, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        [HttpPost("update-group")]
        public async Task<IActionResult> UpdateGroup(UpdateGroupDto model)
        {
            var res = await _conversationRepository.UpdateGroup(model, GetUserId());
            return StatusCode(res.StatusCode, new { res.Errors, res.Message, res.Success });
        }

        // Uploads a group avatar image and returns its public URL. The URL is attached
        // to a conversation via create-group or update-group (which enforce permissions),
        // so this endpoint only stores the file.
        [HttpPost("group-avatar")]
        public async Task<IActionResult> UploadGroupAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new AvatarUploadResponse { StatusCode = 400, Success = false, Errors = ["NO_FILE"] });
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                return BadRequest(new AvatarUploadResponse { StatusCode = 400, Success = false, Errors = ["INVALID_IMAGE_FORMAT"] });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new AvatarUploadResponse { StatusCode = 400, Success = false, Errors = ["FILE_TOO_LARGE"] });
            }

            var avatarUrl = await _minioService.UploadSingleFileAsync(file, MinIOService.AVATARS);
            return Ok(new AvatarUploadResponse { StatusCode = 200, Success = true, Message = "AVATAR_UPLOADED", AvatarUrl = avatarUrl });
        }

        [HttpGet("group-members/{conversationId}")]
        public async Task<IActionResult> GetGroupMembers(int conversationId)
        {
            var members = await _conversationRepository.GetGroupMembers(conversationId);
            return Ok(members);
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query = "")
        {
            var users = await _conversationRepository.SearchUsers(query, GetUserId());
            return Ok(users);
        }

        [HttpGet("get-conversation-of-user/{userId}")]
        public async Task<IActionResult> GetConversationOfUser(string userId)
        {
            var conversations = await _conversationRepository.GetConversationOfUser(userId);
            return Ok(conversations);
        }

        [HttpGet("get-messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var messages = await _conversationRepository.GetMessages(conversationId);
            return Ok(messages);
        }

        [HttpPost("seen-message/{userId}/{conversationId}")]
        public async Task<IActionResult> SeenMessage(string userId, int conversationId)
        {
            await _conversationRepository.SeenMessage(userId, conversationId);
            return Ok();
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
    }
}
