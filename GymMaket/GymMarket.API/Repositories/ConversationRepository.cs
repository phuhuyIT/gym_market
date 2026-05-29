using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Hubs;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IHubContext<ChatHub> _hub;

        public ConversationRepository(GymMarketContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
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
                .Where(c => !c.IsGroup && (c.SenderId == model.SenderId || c.SenderId == model.RecieveId) && (c.RecieveId == model.RecieveId || c.RecieveId == model.SenderId))
                .FirstOrDefaultAsync();

            if (conversationExists != null)
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

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = sender.Id,
            });

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                HasNewMessage = true,
                LastMessage = "",
                UserId = receiver.Id,
            });
            var r = await _context.SaveChangesAsync();
            if (r > 0)
            {
                return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
            }
            return new ApiResponse { Errors = ["CONVERSATION_CREATION_FAILED"], StatusCode = 400, Success = false };
        }

        public async Task<ApiResponse> CreateGroup(CreateGroupDto model, string creatorId)
        {
            var creator = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == creatorId);
            if (creator == null)
            {
                return new ApiResponse { Errors = ["CREATOR_NOT_FOUND"], StatusCode = 400, Success = false };
            }

            // Keep only valid users, exclude the creator (added separately as Owner), de-dupe.
            var memberIds = await _context.Users.AsNoTracking()
                .Where(u => model.MemberIds.Contains(u.Id) && u.Id != creatorId)
                .Select(u => u.Id)
                .ToListAsync();

            var conversation = new Conversation
            {
                Name = model.Name,
                IsGroup = true,
                AvatarUrl = string.IsNullOrWhiteSpace(model.AvatarUrl) ? Defaults.AvatarUrl : model.AvatarUrl,
                CreatedById = creatorId,
                // Legacy non-null FK columns are unused for groups; point them at the creator.
                SenderId = creatorId,
                RecieveId = creatorId,
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = creatorId,
                Role = ParticipantRoles.Owner,
                HasNewMessage = false,
                LastMessage = "",
                JoinedAt = DateTime.Now,
            });

            foreach (var memberId in memberIds)
            {
                _context.ConversationParticipants.Add(new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    UserId = memberId,
                    Role = ParticipantRoles.Member,
                    HasNewMessage = true,
                    LastMessage = "",
                    JoinedAt = DateTime.Now,
                });
            }
            await _context.SaveChangesAsync();

            await AddSystemMessage(conversation.Id, $"{creator.FullName} created the group");

            return new ApiResponse { StatusCode = 200, Success = true, Message = conversation.Id.ToString() };
        }

        public async Task<ApiResponse> AddMembers(AddMembersDto model, string actorId)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == model.ConversationId && c.IsGroup);
            if (conversation == null)
            {
                return new ApiResponse { Errors = ["GROUP_NOT_FOUND"], StatusCode = 404, Success = false };
            }

            var actor = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == model.ConversationId && p.UserId == actorId);
            if (actor == null || (actor.Role != ParticipantRoles.Owner && actor.Role != ParticipantRoles.Admin))
            {
                return new ApiResponse { Errors = ["FORBIDDEN"], StatusCode = 403, Success = false };
            }

            var existingIds = await _context.ConversationParticipants
                .Where(p => p.ConversationId == model.ConversationId)
                .Select(p => p.UserId)
                .ToListAsync();

            var newUsers = await _context.Users.AsNoTracking()
                .Where(u => model.UserIds.Contains(u.Id) && !existingIds.Contains(u.Id))
                .ToListAsync();

            if (newUsers.Count == 0)
            {
                return new ApiResponse { Errors = ["NO_NEW_MEMBERS"], StatusCode = 400, Success = false };
            }

            foreach (var user in newUsers)
            {
                _context.ConversationParticipants.Add(new ConversationParticipant
                {
                    ConversationId = model.ConversationId,
                    UserId = user.Id,
                    Role = ParticipantRoles.Member,
                    HasNewMessage = true,
                    LastMessage = "",
                    JoinedAt = DateTime.Now,
                });
            }
            await _context.SaveChangesAsync();

            var actorUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == actorId);
            var names = string.Join(", ", newUsers.Select(u => u.FullName));
            await AddSystemMessage(model.ConversationId, $"{actorUser?.FullName} added {names}");
            await BroadcastGroupUpdated(model.ConversationId);

            return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
        }

        public async Task<ApiResponse> RemoveMember(int conversationId, string userId, string actorId)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.IsGroup);
            if (conversation == null)
            {
                return new ApiResponse { Errors = ["GROUP_NOT_FOUND"], StatusCode = 404, Success = false };
            }

            var isSelf = userId == actorId;

            var actor = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == actorId);
            if (actor == null)
            {
                return new ApiResponse { Errors = ["NOT_A_MEMBER"], StatusCode = 403, Success = false };
            }

            if (!isSelf && actor.Role != ParticipantRoles.Owner && actor.Role != ParticipantRoles.Admin)
            {
                return new ApiResponse { Errors = ["FORBIDDEN"], StatusCode = 403, Success = false };
            }

            var target = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);
            if (target == null)
            {
                return new ApiResponse { Errors = ["MEMBER_NOT_FOUND"], StatusCode = 404, Success = false };
            }

            // The owner can only be removed by leaving themselves.
            if (target.Role == ParticipantRoles.Owner && !isSelf)
            {
                return new ApiResponse { Errors = ["CANNOT_REMOVE_OWNER"], StatusCode = 400, Success = false };
            }

            var wasOwner = target.Role == ParticipantRoles.Owner;
            _context.ConversationParticipants.Remove(target);
            await _context.SaveChangesAsync();

            // If the owner left, hand ownership to the longest-standing remaining member.
            if (wasOwner)
            {
                var successor = await _context.ConversationParticipants
                    .Where(p => p.ConversationId == conversationId)
                    .OrderByDescending(p => p.Role == ParticipantRoles.Admin)
                    .ThenBy(p => p.JoinedAt)
                    .FirstOrDefaultAsync();
                if (successor != null)
                {
                    successor.Role = ParticipantRoles.Owner;
                    await _context.SaveChangesAsync();
                }
            }

            var targetUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (isSelf)
            {
                await AddSystemMessage(conversationId, $"{targetUser?.FullName} left the group");
            }
            else
            {
                var actorUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == actorId);
                await AddSystemMessage(conversationId, $"{actorUser?.FullName} removed {targetUser?.FullName}");
            }
            await BroadcastGroupUpdated(conversationId);

            return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
        }

        public Task<ApiResponse> LeaveGroup(int conversationId, string userId)
            => RemoveMember(conversationId, userId, userId);

        public async Task<ApiResponse> UpdateMemberRole(UpdateMemberRoleDto model, string actorId)
        {
            if (model.Role != ParticipantRoles.Admin && model.Role != ParticipantRoles.Member)
            {
                return new ApiResponse { Errors = ["INVALID_ROLE"], StatusCode = 400, Success = false };
            }

            var actor = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == model.ConversationId && p.UserId == actorId);
            if (actor == null || actor.Role != ParticipantRoles.Owner)
            {
                return new ApiResponse { Errors = ["FORBIDDEN"], StatusCode = 403, Success = false };
            }

            var target = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == model.ConversationId && p.UserId == model.UserId);
            if (target == null)
            {
                return new ApiResponse { Errors = ["MEMBER_NOT_FOUND"], StatusCode = 404, Success = false };
            }
            if (target.Role == ParticipantRoles.Owner)
            {
                return new ApiResponse { Errors = ["CANNOT_CHANGE_OWNER"], StatusCode = 400, Success = false };
            }

            target.Role = model.Role;
            await _context.SaveChangesAsync();

            var targetUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == model.UserId);
            var verb = model.Role == ParticipantRoles.Admin ? "is now an admin" : "is no longer an admin";
            await AddSystemMessage(model.ConversationId, $"{targetUser?.FullName} {verb}");
            await BroadcastGroupUpdated(model.ConversationId);

            return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
        }

        public async Task<List<GroupMemberDto>> GetGroupMembers(int conversationId)
        {
            var members = await (from p in _context.ConversationParticipants
                                 join u in _context.Users on p.UserId equals u.Id
                                 where p.ConversationId == conversationId
                                 orderby p.JoinedAt
                                 select new GroupMemberDto
                                 {
                                     UserId = p.UserId,
                                     FullName = u.FullName ?? string.Empty,
                                     Avatar = u.Avatar ?? Defaults.AvatarUrl,
                                     Role = p.Role,
                                 }).ToListAsync();
            return members;
        }

        public async Task<List<UserSearchDto>> SearchUsers(string query, string currentUserId)
        {
            query = (query ?? string.Empty).Trim();
            var users = _context.Users.AsNoTracking().Where(u => u.Id != currentUserId);

            if (!string.IsNullOrEmpty(query))
            {
                users = users.Where(u =>
                    (u.FullName != null && u.FullName.Contains(query)) ||
                    (u.Email != null && u.Email.Contains(query)));
            }

            return await users
                .OrderBy(u => u.FullName)
                .Take(20)
                .Select(u => new UserSearchDto
                {
                    Id = u.Id,
                    FullName = u.FullName ?? string.Empty,
                    Avatar = u.Avatar ?? Defaults.AvatarUrl,
                    Email = u.Email ?? string.Empty,
                })
                .ToListAsync();
        }

        public async Task<List<ConversationDto>> GetConversationOfUser(string userId)
        {
            var myParts = await (from p in _context.ConversationParticipants
                                 join c in _context.Conversations on p.ConversationId equals c.Id
                                 where p.UserId == userId
                                 select new
                                 {
                                     c.Id,
                                     c.IsGroup,
                                     c.Name,
                                     c.AvatarUrl,
                                     p.HasNewMessage,
                                     p.LastMessage,
                                     p.Role,
                                 }).ToListAsync();

            var conversationIds = myParts.Select(x => x.Id).ToList();

            var allParts = await (from p in _context.ConversationParticipants
                                  join u in _context.Users on p.UserId equals u.Id
                                  where conversationIds.Contains(p.ConversationId)
                                  select new
                                  {
                                      p.ConversationId,
                                      p.UserId,
                                      FullName = u.FullName,
                                      Avatar = u.Avatar,
                                  }).ToListAsync();

            var result = new List<ConversationDto>();
            foreach (var conv in myParts)
            {
                var participants = allParts.Where(x => x.ConversationId == conv.Id).ToList();

                string name;
                string avatar;
                if (conv.IsGroup)
                {
                    name = conv.Name;
                    avatar = string.IsNullOrEmpty(conv.AvatarUrl) ? Defaults.AvatarUrl : conv.AvatarUrl;
                }
                else
                {
                    var other = participants.FirstOrDefault(x => x.UserId != userId);
                    name = other?.FullName ?? conv.Name;
                    avatar = string.IsNullOrEmpty(other?.Avatar) ? Defaults.AvatarUrl : other!.Avatar!;
                }

                result.Add(new ConversationDto
                {
                    ConversationId = conv.Id,
                    ConversationName = name,
                    HasNewMessage = conv.HasNewMessage,
                    LastMessage = conv.LastMessage,
                    Avatar = avatar,
                    IsGroup = conv.IsGroup,
                    Role = conv.Role,
                    MemberCount = participants.Count,
                });
            }

            return result;
        }

        public async Task SeenMessage(string userId, int conversationId)
        {
            var conversationParticipant = await _context.ConversationParticipants
                .Where(x => x.UserId == userId && x.ConversationId == conversationId)
                .FirstOrDefaultAsync();

            if (conversationParticipant != null)
            {
                conversationParticipant.HasNewMessage = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserMessageDto?> SendMessage(SendMessageDto model)
        {
            var message = new UserMessage
            {
                Content = model.Content,
                ConversationId = model.ConversationId,
                SenderId = model.SenderId,
                CreatedAt = DateTime.Now,
                Type = MessageTypes.Text,
            };

            _context.UserMessages.Add(message);

            var conversationParticipants = await _context.ConversationParticipants
               .Where(c => c.ConversationId == model.ConversationId)
               .ToListAsync();

            foreach (var participant in conversationParticipants)
            {
                participant.LastMessage = model.Content;

                if (participant.UserId != model.SenderId)
                {
                    participant.HasNewMessage = true;
                }
            }

            await _context.SaveChangesAsync();

            var sender = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == model.SenderId);

            return new UserMessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = sender?.FullName ?? string.Empty,
                Avatar = string.IsNullOrEmpty(sender?.Avatar) ? Defaults.AvatarUrl : sender!.Avatar!,
                SentAt = message.CreatedAt,
                Type = message.Type,
            };
        }

        public async Task<List<UserMessageDto>> GetMessages(int conversationId)
        {
            var messages = await (from m in _context.UserMessages.AsNoTracking()
                                  join u in _context.Users on m.SenderId equals u.Id into su
                                  from u in su.DefaultIfEmpty()
                                  where m.ConversationId == conversationId
                                  orderby m.CreatedAt
                                  select new UserMessageDto
                                  {
                                      Id = m.Id,
                                      ConversationId = m.ConversationId,
                                      Content = m.Content,
                                      SenderId = m.SenderId,
                                      SenderName = u != null ? (u.FullName ?? string.Empty) : string.Empty,
                                      Avatar = u != null && u.Avatar != null ? u.Avatar : Defaults.AvatarUrl,
                                      SentAt = m.CreatedAt,
                                      Type = m.Type,
                                  }).ToListAsync();
            return messages;
        }

        public async Task<List<int>> GetConversationIdsOfUser(string userId)
        {
            return await _context.ConversationParticipants
                .Where(p => p.UserId == userId)
                .Select(p => p.ConversationId)
                .ToListAsync();
        }

        private async Task AddSystemMessage(int conversationId, string text)
        {
            var message = new UserMessage
            {
                Content = text,
                ConversationId = conversationId,
                SenderId = string.Empty,
                CreatedAt = DateTime.Now,
                Type = MessageTypes.System,
            };
            _context.UserMessages.Add(message);

            var participants = await _context.ConversationParticipants
                .Where(p => p.ConversationId == conversationId)
                .ToListAsync();
            foreach (var participant in participants)
            {
                participant.LastMessage = text;
            }
            await _context.SaveChangesAsync();

            var dto = new UserMessageDto
            {
                Id = message.Id,
                ConversationId = conversationId,
                Content = text,
                SenderId = string.Empty,
                SenderName = string.Empty,
                Avatar = Defaults.AvatarUrl,
                SentAt = message.CreatedAt,
                Type = MessageTypes.System,
            };
            await _hub.Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", dto);
        }

        private async Task BroadcastGroupUpdated(int conversationId)
        {
            await _hub.Clients.Group(conversationId.ToString()).SendAsync("GroupUpdated", conversationId);
        }
    }
}
