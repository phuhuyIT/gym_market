using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly ConversationRepository conversationRepository;

        public ConversationsController(ConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation(CreateConversationDto model)
        {
            var res = await conversationRepository.CreateConversation(model);
            return StatusCode(res.StatusCode, new { res.Errors, res.Message });
        }

        [HttpGet("GetConversationOfUser/{userId}")]
        public async Task<IActionResult> GetConversationOfUser(string userId)
        {
            var conversations = await conversationRepository.GetConversationOfUser(userId);
            return Ok(conversations);
        }

        [HttpGet("get-messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var messages = await conversationRepository.GetMessages(conversationId);
            return Ok(messages);
        }

        [HttpGet("seen-message/{userId}/{conversationId}")]
        public async Task<IActionResult> SeenMessage(string userId, int conversationId)
        {
            await conversationRepository.SeenMessage(userId, conversationId);
            return Ok();
        }
    }
}
