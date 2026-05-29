using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories.IRepositories;
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

        public ConversationsController(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
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
