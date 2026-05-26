using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
