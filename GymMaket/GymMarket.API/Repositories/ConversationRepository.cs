using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.UserMessage;
using GymMarket.API.Hubs;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IPresenceTracker _presenceTracker;
        private readonly INotificationRepository _notificationRepository;

        public ConversationRepository(GymMarketContext context, IHubContext<ChatHub> hub, IPresenceTracker presenceTracker, INotificationRepository notificationRepository)
        {
            _context = context;
            _hub = hub;
            _presenceTracker = presenceTracker;
            _notificationRepository = notificationRepository;
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
                JoinedAt = DateTime.UtcNow,
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
                    JoinedAt = DateTime.UtcNow,
                });
            }
            await _context.SaveChangesAsync();

            await AddSystemMessage(conversation.Id, $"{creator.FullName} created the group");

            await _notificationRepository.NotifyUsers(
                memberIds,
                NotificationTypes.Chat,
                "Added to a group",
                $"{creator.FullName} added you to the group \"{model.Name}\"",
                "/chat/chat-list");

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
                    JoinedAt = DateTime.UtcNow,
                });
            }
            await _context.SaveChangesAsync();

            var actorUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == actorId);
            var names = string.Join(", ", newUsers.Select(u => u.FullName));
            await AddSystemMessage(model.ConversationId, $"{actorUser?.FullName} added {names}");
            await BroadcastGroupUpdated(model.ConversationId);

            await _notificationRepository.NotifyUsers(
                newUsers.Select(u => u.Id),
                NotificationTypes.Chat,
                "Added to a group",
                $"{actorUser?.FullName} added you to the group \"{conversation.Name}\"",
                "/chat/chat-list");

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

        public async Task<ApiResponse> UpdateGroup(UpdateGroupDto model, string actorId)
        {
            var newName = model.Name?.Trim();
            var newAvatarUrl = model.AvatarUrl?.Trim();
            if (string.IsNullOrEmpty(newName) && string.IsNullOrEmpty(newAvatarUrl))
            {
                return new ApiResponse { Errors = ["NOTHING_TO_UPDATE"], StatusCode = 400, Success = false };
            }

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

            var renamed = !string.IsNullOrEmpty(newName) && newName != conversation.Name;
            var avatarChanged = !string.IsNullOrEmpty(newAvatarUrl) && newAvatarUrl != conversation.AvatarUrl;
            if (!renamed && !avatarChanged)
            {
                return new ApiResponse { StatusCode = 200, Success = true, Message = "SUCCESS" };
            }

            if (renamed)
            {
                conversation.Name = newName!;
            }
            if (avatarChanged)
            {
                conversation.AvatarUrl = newAvatarUrl;
            }
            await _context.SaveChangesAsync();

            var actorUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == actorId);
            if (renamed)
            {
                await AddSystemMessage(model.ConversationId, $"{actorUser?.FullName} renamed the group to \"{newName}\"");
            }
            if (avatarChanged)
            {
                await AddSystemMessage(model.ConversationId, $"{actorUser?.FullName} changed the group photo");
            }
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

            var lastMessageTimes = await _context.UserMessages.AsNoTracking()
                .Where(m => conversationIds.Contains(m.ConversationId))
                .GroupBy(m => m.ConversationId)
                .Select(g => new { ConversationId = g.Key, At = g.Max(m => m.CreatedAt) })
                .ToDictionaryAsync(x => x.ConversationId, x => x.At);

            var allParts = await (from p in _context.ConversationParticipants
                                  join u in _context.Users on p.UserId equals u.Id
                                  where conversationIds.Contains(p.ConversationId)
                                  select new
                                  {
                                      p.ConversationId,
                                      p.UserId,
                                      FullName = u.FullName,
                                      Avatar = u.Avatar,
                                      LastSeen = u.LastSeen,
                                  }).ToListAsync();

            var result = new List<ConversationDto>();
            foreach (var conv in myParts)
            {
                var participants = allParts.Where(x => x.ConversationId == conv.Id).ToList();

                string name;
                string avatar;
                string? otherUserId = null;
                bool isOnline;
                DateTime? lastSeen = null;
                if (conv.IsGroup)
                {
                    name = conv.Name;
                    avatar = string.IsNullOrEmpty(conv.AvatarUrl) ? Defaults.AvatarUrl : conv.AvatarUrl;
                    // A group is "active" when any member other than the current user is online.
                    isOnline = participants.Any(x => x.UserId != userId && _presenceTracker.IsOnline(x.UserId));
                }
                else
                {
                    var other = participants.FirstOrDefault(x => x.UserId != userId);
                    name = other?.FullName ?? conv.Name;
                    avatar = string.IsNullOrEmpty(other?.Avatar) ? Defaults.AvatarUrl : other!.Avatar!;
                    otherUserId = other?.UserId;
                    isOnline = otherUserId != null && _presenceTracker.IsOnline(otherUserId);
                    // Stored as UTC, but SQL roundtrips lose the Kind; re-stamp so the JSON
                    // carries 'Z' and clients convert to their local time.
                    lastSeen = other?.LastSeen == null ? null : DateTime.SpecifyKind(other.LastSeen.Value, DateTimeKind.Utc);
                }

                result.Add(new ConversationDto
                {
                    ConversationId = conv.Id,
                    ConversationName = name,
                    HasNewMessage = conv.HasNewMessage,
                    LastMessage = conv.LastMessage,
                    LastMessageAt = lastMessageTimes.TryGetValue(conv.Id, out var lastAt)
                        ? DateTime.SpecifyKind(lastAt, DateTimeKind.Utc)
                        : null,
                    Avatar = avatar,
                    IsGroup = conv.IsGroup,
                    Role = conv.Role,
                    MemberCount = participants.Count,
                    OtherUserId = otherUserId,
                    IsOnline = isOnline,
                    LastSeen = lastSeen,
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
                CreatedAt = DateTime.UtcNow,
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

            await NotifyMessageRecipients(model, sender, conversationParticipants);

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

        // Bell notification for everyone else in the conversation. Upsert keyed on the
        // conversation link so a burst of messages collapses into one unread entry
        // showing the latest preview instead of one row per message.
        private async Task NotifyMessageRecipients(SendMessageDto model, AppUser? sender, List<ConversationParticipant> participants)
        {
            var conversation = await _context.Conversations.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == model.ConversationId);

            var title = conversation?.IsGroup == true
                ? $"New message in \"{conversation.Name}\""
                : $"New message from {sender?.FullName ?? "someone"}";

            var preview = model.Content.Length > 120 ? model.Content[..120] + "…" : model.Content;
            if (conversation?.IsGroup == true)
            {
                preview = $"{sender?.FullName}: {preview}";
            }

            var link = $"/chat/chat-list?conversationId={model.ConversationId}";

            foreach (var participant in participants.Where(p => p.UserId != model.SenderId))
            {
                await _notificationRepository.NotifyUserUpsert(
                    participant.UserId, NotificationTypes.Chat, title, preview, link);
            }
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
            // Stored as UTC, but SQL roundtrips lose the Kind; re-stamp so the JSON
            // carries 'Z' and clients convert to their local time.
            foreach (var message in messages)
            {
                message.SentAt = DateTime.SpecifyKind(message.SentAt, DateTimeKind.Utc);
            }
            return messages;
        }

        public async Task<List<int>> GetConversationIdsOfUser(string userId)
        {
            return await _context.ConversationParticipants
                .Where(p => p.UserId == userId)
                .Select(p => p.ConversationId)
                .ToListAsync();
        }

        public async Task UpdateLastSeen(string userId, DateTime lastSeen)
        {
            await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastSeen, lastSeen));
        }

        private async Task AddSystemMessage(int conversationId, string text)
        {
            var message = new UserMessage
            {
                Content = text,
                ConversationId = conversationId,
                SenderId = string.Empty,
                CreatedAt = DateTime.UtcNow,
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
