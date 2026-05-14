using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly GymMarketContext _context;

        public ConversationRepository(GymMarketContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateConversation(CreateConversationDto model)
        {
            var sender = await _context.Users.AsNoTrackingWithIdentityResolution()
                .Where(u => u.Id == model.SenderId)
                .FirstOrDefaultAsync();

            if (sender == null)
            {
                return new ApiResponse { Errors = ["SENDER_NOT_FOUND"], StatusCode = 400, Success = false };
            }

            var receiver = await _context.Users.AsNoTrackingWithIdentityResolution()
                .Where(u => u.Id == model.RecieveId)
                .FirstOrDefaultAsync();

            if (receiver == null)
            {
                return new ApiResponse { Errors = ["RECEIVER_NOT_FOUND"], StatusCode = 400, Success = false };
            }


            var conversationExists = await _context.Conversations
                .AsNoTrackingWithIdentityResolution()
                .Where(c => (c.SenderId == model.SenderId || c.SenderId == model.RecieveId) && (c.RecieveId == model.RecieveId || c.RecieveId == model.SenderId))
                .FirstOrDefaultAsync();

            if(conversationExists != null)
            {
                return new ApiResponse { StatusCode = 200, Success = true };
            }

            var conversation = new Conversation
            {
                Name = sender.FullName + " - " + receiver.FullName,
                RecieveId = receiver.Id,
                SenderId = sender.Id,
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var conversationParticipant1 = new ConversationParticipant
            {
                ConversationId = conversation.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = sender.Id,
            };

            var conversationParticipant2 = new ConversationParticipant
            {
                ConversationId = conversation.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = receiver.Id,
            };
            _context.ConversationParticipants.Add(conversationParticipant1);
            _context.ConversationParticipants.Add(conversationParticipant2);
            var r = await _context.SaveChangesAsync();
            if (r > 0)
            {
                return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
            }
            return new ApiResponse { Errors = ["CONVERSATION_CREATION_FAILED"], StatusCode = 400, Success = false };
        }

        public async Task<List<ConversitionDto>> GetConversationOfUser(string userId)
        {
            var list = await (from conversationUser in _context.ConversationParticipants
                              join conversation in _context.Conversations on conversationUser.ConversationId equals conversation.Id
                              join receive in _context.Users on conversation.RecieveId equals receive.Id
                              join sender in _context.Users on conversation.SenderId equals sender.Id
                              where conversationUser.UserId == userId
                              select new ConversitionDto
                              {
                                  ConversationId = conversationUser.ConversationId,
                                  ConversationName = conversation.Name,
                                  HasNewMessage = conversationUser.HasNewMessage,
                                  LastMessage = conversationUser.LastMessage,
                                  Avatar = receive.Avatar != null && receive.Id == userId && sender.Avatar != null ? sender.Avatar 
                                  : sender.Avatar != null && sender.Id == userId && receive.Avatar != null ? receive.Avatar
                                  : "https://cdn-icons-png.flaticon.com/512/1999/1999625.png"
                              }).ToListAsync();
            return list;
        }

        public async Task SeenMessage(string userId, int conversationId)
        {
            var conversationParticipant = await _context.ConversationParticipants
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.UserId == userId && x.ConversationId == conversationId)
                .FirstOrDefaultAsync();

            if (conversationParticipant != null)
            {
                conversationParticipant.HasNewMessage = false;
                _context.ConversationParticipants.Update(conversationParticipant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendMessage(SendMessageDto model)
        {
            var message = new UserMessage
            {
                Content = model.Content,
                ConversationId = model.ConversationId,
                SenderId = model.SenderId,
            };

            _context.UserMessages.Add(message);

            var conversationParticipants = await _context.ConversationParticipants
               .AsNoTrackingWithIdentityResolution()
               .Where(c => c.ConversationId == model.ConversationId)
               .ToListAsync();

            foreach (var participant in conversationParticipants)
            {
                participant.LastMessage = model.Content;

                if(participant.UserId != model.SenderId)
                {
                    participant.HasNewMessage = true;
                }
            }
            _context.ConversationParticipants.UpdateRange(conversationParticipants);

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserMessageDto>> GetMessages(int conversationId)
        {
            var messages = await _context.UserMessages
                .AsNoTrackingWithIdentityResolution()
                .Include(m => m.AppUser)
                .Where(m => m.ConversationId == conversationId)
                .Select(m => new UserMessageDto
                {
                    Id = m.Id,
                    Content = m.Content,    
                    SenderId = m.SenderId
                })
                .ToListAsync();
            return messages;
        }
    }
}
