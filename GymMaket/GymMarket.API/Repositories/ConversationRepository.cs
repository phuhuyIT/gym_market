using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class ConversationRepository
    {
        private readonly GymMarketContext context;

        public ConversationRepository(GymMarketContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse> CreateConversation(CreateConversationDto model)
        {
            var sender = await context.Users.AsNoTrackingWithIdentityResolution()
                .Where(u => u.Id == model.SenderId)
                .FirstOrDefaultAsync();

            if (sender == null)
            {
                return new ApiResponse { Errors = ["Người gửi không tồn tại"], StatusCode = 400, Success = false };
            }

            var reviever = await context.Users.AsNoTrackingWithIdentityResolution()
                .Where(u => u.Id == model.RecieveId)
                .FirstOrDefaultAsync();

            if (reviever == null)
            {
                return new ApiResponse { Errors = ["Người nhận không tồn tại"], StatusCode = 400, Success = false };
            }


            var conversitionExists = await context.Conversations
                .AsNoTrackingWithIdentityResolution()
                .Where(c => (c.SenderId == model.SenderId || c.SenderId == model.RecieveId) && (c.SenderId == model.RecieveId || c.RecieveId == model.RecieveId))
                .FirstOrDefaultAsync();

            if(conversitionExists != null)
            {
                return new ApiResponse { Errors = [], StatusCode = 200, Success = false };
            }

            var conversition = new Conversation
            {
                Name = sender.FullName + " - " + reviever.FullName,
                RecieveId = reviever.Id,
                SenderId = sender.Id,
            };
            context.Conversations.Add(conversition);
            await context.SaveChangesAsync();

            var conversationParticipant1 = new ConversationParticipant
            {
                ConversationId = conversition.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = sender.Id,
            };

            var conversationParticipant2 = new ConversationParticipant
            {
                ConversationId = conversition.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = reviever.Id,
            };
            context.ConversationParticipants.Add(conversationParticipant1);
            context.ConversationParticipants.Add(conversationParticipant2);
            var r = await context.SaveChangesAsync();
            if (r > 0)
            {
                return new ApiResponse { Errors = [], StatusCode = 200, Success = false };
            }
            return new ApiResponse { Errors = ["Tạo đoạn chat thất bại. Vui lòng thử lại"], StatusCode = 400, Success = false };
        }

        public async Task<List<ConversitionDto>> GetConversationOfUser(string userId)
        {
            var list = await (from conversationUser in context.ConversationParticipants
                              join conversition in context.Conversations on conversationUser.ConversationId equals conversition.Id
                              join recieve in context.Users on conversition.RecieveId equals recieve.Id
                              join sender in context.Users on conversition.SenderId equals sender.Id
                              where conversationUser.UserId == userId
                              select new ConversitionDto
                              {
                                  ConversationId = conversationUser.ConversationId,
                                  ConversationName = conversition.Name,
                                  HasNewMessage = conversationUser.HasNewMessage,
                                  LastMessage = conversationUser.LastMessage,
                                  Avatar = recieve.Avatar != null && recieve.Id == userId && sender.Avatar != null ? sender.Avatar 
                                  : sender.Avatar != null && sender.Id == userId && recieve.Avatar != null ? recieve.Avatar
                                  : "https://cdn-icons-png.flaticon.com/512/1999/1999625.png"
                              }).ToListAsync();
            return list;
        }

        public async Task SeenMessage(string userId, int conversationId)
        {
            var conversationParticipant = await context.ConversationParticipants
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.UserId == userId && x.ConversationId == conversationId)
                .FirstOrDefaultAsync();

            if (conversationParticipant != null)
            {
                conversationParticipant.HasNewMessage = false;
                context.ConversationParticipants.Update(conversationParticipant);
                await context.SaveChangesAsync();
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

            context.UserMessages.Add(message);

            var conversationLastMessage = await context.ConversationParticipants
               .AsNoTrackingWithIdentityResolution()
               .Where(c => c.ConversationId == model.ConversationId)
               .ToListAsync();

            conversationLastMessage.ForEach((t) =>
            {
                t.LastMessage = model.Content;

                if(t.UserId != model.SenderId)
                {
                    t.HasNewMessage = true;
                }
            });
            context.ConversationParticipants.UpdateRange(conversationLastMessage);

            await context.SaveChangesAsync();
        }

        public async Task<List<UserMessageDto>> GetMessages(int conversationId)
        {
            var messages = await context.UserMessages
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
