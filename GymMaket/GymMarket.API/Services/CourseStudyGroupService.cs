using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseStudyGroups;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services;

public class CourseStudyGroupService : ICourseStudyGroupService
{
    private readonly GymMarketContext _context;
    private readonly INotificationRepository _notificationRepository;

    public CourseStudyGroupService(GymMarketContext context, INotificationRepository notificationRepository)
    {
        _context = context;
        _notificationRepository = notificationRepository;
    }

    public async Task<CourseStudyGroupDto?> EnsureDefaultCohortAsync(string courseId, string actorUserId)
    {
        var course = await _context.Courses
            .Include(c => c.Trainer)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
            return null;

        var ownerUserId = course.Trainer?.UserId ?? actorUserId;
        if (string.IsNullOrWhiteSpace(ownerUserId))
            return null;

        var group = await _context.CourseStudyGroups
            .Include(g => g.Conversation)
            .FirstOrDefaultAsync(g => g.CourseId == courseId && g.IsDefaultCohort);

        var now = DateTime.UtcNow;
        if (group == null)
        {
            var name = $"{(string.IsNullOrWhiteSpace(course.Title) ? "Course" : course.Title)} cohort";
            var conversation = new Conversation
            {
                Name = name,
                IsGroup = true,
                AvatarUrl = Defaults.AvatarUrl,
                CreatedById = ownerUserId,
                SenderId = ownerUserId,
                RecieveId = ownerUserId,
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            group = new CourseStudyGroup
            {
                StudyGroupId = Guid.NewGuid().ToString(),
                CourseId = courseId,
                ConversationId = conversation.Id,
                Conversation = conversation,
                Name = name,
                Description = "Automatically synced from paid course enrollments.",
                Kind = CourseStudyGroupKind.Cohort,
                IsDefaultCohort = true,
                IsActive = true,
                CreatedByUserId = ownerUserId,
                CreatedAt = now,
                UpdatedAt = now,
            };
            _context.CourseStudyGroups.Add(group);

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = ownerUserId,
                Role = ParticipantRoles.Owner,
                HasNewMessage = false,
                LastMessage = string.Empty,
                JoinedAt = now,
            });
            await _context.SaveChangesAsync();
            await AddSystemMessageAsync(conversation.Id, ownerUserId, "Course cohort created");
        }
        else if (!group.IsActive)
        {
            group.IsActive = true;
            group.UpdatedAt = now;
        }

        await EnsureParticipantAsync(group.ConversationId, ownerUserId, ParticipantRoles.Owner, hasNewMessage: false);
        await SyncDefaultLearnersAsync(group, ownerUserId);
        await _context.SaveChangesAsync();

        return await BuildDtoAsync(group.StudyGroupId, ownerUserId, canManage: true);
    }

    public async Task<List<CourseStudyGroupDto>> GetCourseGroupsAsync(string courseId, ClaimsPrincipal user, bool includeAll)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var query = _context.CourseStudyGroups
            .AsNoTracking()
            .Where(g => g.CourseId == courseId && g.IsActive);

        if (!includeAll)
        {
            query = query.Where(g => _context.ConversationParticipants
                .Any(p => p.ConversationId == g.ConversationId && p.UserId == userId));
        }

        var groups = await query
            .OrderByDescending(g => g.IsDefaultCohort)
            .ThenBy(g => g.Name)
            .ToListAsync();

        var result = new List<CourseStudyGroupDto>();
        foreach (var group in groups)
        {
            var dto = await BuildDtoAsync(group.StudyGroupId, userId, includeAll);
            if (dto != null)
                result.Add(dto);
        }

        return result;
    }

    public async Task<List<EligibleCourseLearnerDto>> GetEligibleLearnersAsync(string courseId, string? studyGroupId)
    {
        var inGroupUserIds = new HashSet<string>();
        if (!string.IsNullOrWhiteSpace(studyGroupId))
        {
            var conversationId = await _context.CourseStudyGroups
                .Where(g => g.StudyGroupId == studyGroupId && g.CourseId == courseId)
                .Select(g => (int?)g.ConversationId)
                .FirstOrDefaultAsync();

            if (conversationId.HasValue)
            {
                inGroupUserIds = (await _context.ConversationParticipants
                    .AsNoTracking()
                    .Where(p => p.ConversationId == conversationId.Value)
                    .Select(p => p.UserId)
                    .ToListAsync()).ToHashSet();
            }
        }

        return (await GetPaidLearnersAsync(courseId))
            .OrderBy(l => l.FullName)
            .Select(l => new EligibleCourseLearnerDto
            {
                StudentId = l.StudentId,
                UserId = l.UserId,
                FullName = l.FullName,
                Email = l.Email,
                Avatar = l.Avatar,
                IsInGroup = inGroupUserIds.Contains(l.UserId),
            })
            .ToList();
    }

    public async Task<CourseStudyGroupDto?> CreateStudyGroupAsync(string courseId, UpsertCourseStudyGroupDto dto, string actorUserId)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var validMemberIds = await GetEligibleUserIdsAsync(courseId, dto.MemberUserIds);
        validMemberIds.Remove(actorUserId);

        var now = DateTime.UtcNow;
        var conversation = new Conversation
        {
            Name = name,
            IsGroup = true,
            AvatarUrl = Defaults.AvatarUrl,
            CreatedById = actorUserId,
            SenderId = actorUserId,
            RecieveId = actorUserId,
        };
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        var group = new CourseStudyGroup
        {
            StudyGroupId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            ConversationId = conversation.Id,
            Name = name,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Kind = CourseStudyGroupKind.Normalize(dto.Kind),
            IsDefaultCohort = false,
            IsActive = dto.IsActive,
            CreatedByUserId = actorUserId,
            CreatedAt = now,
            UpdatedAt = now,
        };
        _context.CourseStudyGroups.Add(group);

        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = actorUserId,
            Role = ParticipantRoles.Owner,
            HasNewMessage = false,
            LastMessage = string.Empty,
            JoinedAt = now,
        });

        foreach (var memberId in validMemberIds)
        {
            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = memberId,
                Role = ParticipantRoles.Member,
                HasNewMessage = true,
                LastMessage = string.Empty,
                JoinedAt = now,
            });
        }

        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(conversation.Id, actorUserId, "Study group created");
        await NotifyAddedAsync(validMemberIds, name, actorUserId);

        return await BuildDtoAsync(group.StudyGroupId, actorUserId, canManage: true);
    }

    public async Task<CourseStudyGroupDto?> UpdateStudyGroupAsync(string studyGroupId, UpsertCourseStudyGroupDto dto, string actorUserId)
    {
        var group = await _context.CourseStudyGroups
            .Include(g => g.Conversation)
            .FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (group == null)
            return null;

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return null;

        group.Name = name;
        group.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        group.Kind = group.IsDefaultCohort ? CourseStudyGroupKind.Cohort : CourseStudyGroupKind.Normalize(dto.Kind);
        group.IsActive = group.IsDefaultCohort || dto.IsActive;
        group.UpdatedAt = DateTime.UtcNow;
        group.Conversation.Name = name;

        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(group.ConversationId, actorUserId, "Study group details updated");

        return await BuildDtoAsync(group.StudyGroupId, actorUserId, canManage: true);
    }

    public async Task<ApiResponse> AddMembersAsync(string studyGroupId, IEnumerable<string> userIds, string actorUserId)
    {
        var group = await _context.CourseStudyGroups.FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (group == null)
            return Error("STUDY_GROUP_NOT_FOUND", 404);
        if (group.IsDefaultCohort)
            return Error("DEFAULT_COHORT_SYNC_ONLY", 400);

        var validMemberIds = await GetEligibleUserIdsAsync(group.CourseId, userIds);
        var existingIds = await _context.ConversationParticipants
            .Where(p => p.ConversationId == group.ConversationId)
            .Select(p => p.UserId)
            .ToListAsync();
        validMemberIds.ExceptWith(existingIds);

        if (validMemberIds.Count == 0)
            return Error("NO_ELIGIBLE_NEW_MEMBERS", 400);

        var now = DateTime.UtcNow;
        foreach (var memberId in validMemberIds)
        {
            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = group.ConversationId,
                UserId = memberId,
                Role = ParticipantRoles.Member,
                HasNewMessage = true,
                LastMessage = string.Empty,
                JoinedAt = now,
            });
        }

        group.UpdatedAt = now;
        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(group.ConversationId, actorUserId, "New learners joined the study group");
        await NotifyAddedAsync(validMemberIds, group.Name, actorUserId);

        return Ok("SUCCESS");
    }

    public async Task<ApiResponse> RemoveMemberAsync(string studyGroupId, string userId, string actorUserId)
    {
        var group = await _context.CourseStudyGroups.FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (group == null)
            return Error("STUDY_GROUP_NOT_FOUND", 404);
        if (group.IsDefaultCohort)
            return Error("DEFAULT_COHORT_SYNC_ONLY", 400);

        var target = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == group.ConversationId && p.UserId == userId);
        if (target == null)
            return Error("MEMBER_NOT_FOUND", 404);
        if (target.Role == ParticipantRoles.Owner)
            return Error("CANNOT_REMOVE_OWNER", 400);

        _context.ConversationParticipants.Remove(target);
        group.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(group.ConversationId, actorUserId, "A learner was removed from the study group");

        return Ok("SUCCESS");
    }

    public async Task<ApiResponse> UpdateMemberRoleAsync(string studyGroupId, string userId, string role, string actorUserId)
    {
        var normalizedRole = string.Equals(role, ParticipantRoles.Admin, StringComparison.OrdinalIgnoreCase)
            ? ParticipantRoles.Admin
            : string.Equals(role, ParticipantRoles.Member, StringComparison.OrdinalIgnoreCase)
                ? ParticipantRoles.Member
                : null;

        if (normalizedRole == null)
            return Error("INVALID_ROLE", 400);

        var group = await _context.CourseStudyGroups.FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (group == null)
            return Error("STUDY_GROUP_NOT_FOUND", 404);

        var target = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == group.ConversationId && p.UserId == userId);
        if (target == null)
            return Error("MEMBER_NOT_FOUND", 404);
        if (target.Role == ParticipantRoles.Owner)
            return Error("CANNOT_CHANGE_OWNER", 400);

        target.Role = normalizedRole;
        group.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(group.ConversationId, actorUserId, normalizedRole == ParticipantRoles.Admin
            ? "A learner was promoted to group moderator"
            : "A learner moderator role was removed");

        return Ok("SUCCESS");
    }

    public async Task<ApiResponse> ArchiveStudyGroupAsync(string studyGroupId, string actorUserId)
    {
        var group = await _context.CourseStudyGroups
            .Include(g => g.Conversation)
            .FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (group == null)
            return Error("STUDY_GROUP_NOT_FOUND", 404);
        if (group.IsDefaultCohort)
            return Error("CANNOT_ARCHIVE_DEFAULT_COHORT", 400);

        group.IsActive = false;
        group.UpdatedAt = DateTime.UtcNow;
        group.Conversation.Name = $"{group.Name} (archived)";
        await _context.SaveChangesAsync();
        await AddSystemMessageAsync(group.ConversationId, actorUserId, "Study group archived");

        return Ok("SUCCESS");
    }

    private async Task SyncDefaultLearnersAsync(CourseStudyGroup group, string actorUserId)
    {
        var paidLearnerUserIds = (await GetPaidLearnersAsync(group.CourseId))
            .Select(l => l.UserId)
            .ToHashSet();

        var participants = await _context.ConversationParticipants
            .Where(p => p.ConversationId == group.ConversationId)
            .ToListAsync();

        var existingIds = participants.Select(p => p.UserId).ToHashSet();
        var now = DateTime.UtcNow;
        var added = new List<string>();
        foreach (var userId in paidLearnerUserIds.Where(userId => !existingIds.Contains(userId)))
        {
            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = group.ConversationId,
                UserId = userId,
                Role = ParticipantRoles.Member,
                HasNewMessage = true,
                LastMessage = string.Empty,
                JoinedAt = now,
            });
            added.Add(userId);
        }

        var removable = participants
            .Where(p => p.Role == ParticipantRoles.Member && !paidLearnerUserIds.Contains(p.UserId))
            .ToList();
        if (removable.Count > 0)
            _context.ConversationParticipants.RemoveRange(removable);

        if (added.Count > 0 || removable.Count > 0)
        {
            group.UpdatedAt = now;
            await _context.SaveChangesAsync();
            await AddSystemMessageAsync(group.ConversationId, actorUserId, "Cohort membership synced from enrollments");
            if (added.Count > 0)
                await NotifyAddedAsync(added, group.Name, actorUserId);
        }
    }

    private async Task EnsureParticipantAsync(int conversationId, string userId, string role, bool hasNewMessage)
    {
        var participant = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);
        if (participant != null)
        {
            if (participant.Role != ParticipantRoles.Owner)
                participant.Role = role;
            return;
        }

        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = userId,
            Role = role,
            HasNewMessage = hasNewMessage,
            LastMessage = string.Empty,
            JoinedAt = DateTime.UtcNow,
        });
    }

    private async Task<CourseStudyGroupDto?> BuildDtoAsync(string studyGroupId, string currentUserId, bool canManage)
    {
        var studyGroup = await _context.CourseStudyGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
        if (studyGroup == null)
            return null;

        var paidLearnerUserIds = (await GetPaidLearnersAsync(studyGroup.CourseId))
            .Select(l => l.UserId)
            .ToHashSet();

        var memberRows = await (from p in _context.ConversationParticipants.AsNoTracking()
                                join u in _context.Users.AsNoTracking() on p.UserId equals u.Id
                                join s in _context.Students.AsNoTracking() on u.Id equals s.UserId into studentJoin
                                from s in studentJoin.DefaultIfEmpty()
                                where p.ConversationId == studyGroup.ConversationId
                                orderby p.Role == ParticipantRoles.Owner descending, p.Role == ParticipantRoles.Admin descending, p.JoinedAt
                                select new
                                {
                                    p.UserId,
                                    StudentId = s != null ? s.StudentId : null,
                                    u.FullName,
                                    u.Email,
                                    u.Avatar,
                                    p.Role,
                                    p.JoinedAt,
                                }).ToListAsync();

        var members = memberRows.Select(row => new CourseStudyGroupMemberDto
        {
            UserId = row.UserId,
            StudentId = row.StudentId,
            FullName = row.FullName ?? string.Empty,
            Email = row.Email ?? string.Empty,
            Avatar = row.Avatar ?? Defaults.AvatarUrl,
            Role = row.Role,
            JoinedAt = DateTime.SpecifyKind(row.JoinedAt, DateTimeKind.Utc),
            IsEligibleLearner = paidLearnerUserIds.Contains(row.UserId),
        }).ToList();

        return new CourseStudyGroupDto
        {
            StudyGroupId = studyGroup.StudyGroupId,
            CourseId = studyGroup.CourseId,
            ConversationId = studyGroup.ConversationId,
            Name = studyGroup.Name,
            Description = studyGroup.Description,
            Kind = studyGroup.Kind,
            IsDefaultCohort = studyGroup.IsDefaultCohort,
            IsActive = studyGroup.IsActive,
            IsMember = members.Any(m => m.UserId == currentUserId),
            CanManage = canManage,
            MemberCount = members.Count,
            CreatedAt = DateTime.SpecifyKind(studyGroup.CreatedAt, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(studyGroup.UpdatedAt, DateTimeKind.Utc),
            Members = members,
        };
    }

    private async Task<HashSet<string>> GetEligibleUserIdsAsync(string courseId, IEnumerable<string> userIds)
    {
        var requested = userIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToHashSet();
        if (requested.Count == 0)
            return [];

        var eligible = (await GetPaidLearnersAsync(courseId)).Select(l => l.UserId).ToHashSet();
        eligible.IntersectWith(requested);
        return eligible;
    }

    private async Task<List<PaidLearner>> GetPaidLearnersAsync(string courseId)
    {
        var paidStatuses = new[] { PaymentStatus.Paid, PaymentStatus.Completed };

        var paymentStudentIds = _context.Payments
            .AsNoTracking()
            .Where(p => p.CourseId == courseId && paidStatuses.Contains(p.PaymentStatus!))
            .Select(p => p.StudentId);

        var registrationStudentIds = _context.CourseRegistrations
            .AsNoTracking()
            .Where(r => r.CourseId == courseId && paidStatuses.Contains(r.PaymentStatus!))
            .Select(r => r.StudentId);

        var paidStudentIds = paymentStudentIds.Concat(registrationStudentIds);

        var rows = await (from s in _context.Students.AsNoTracking()
                          join u in _context.Users.AsNoTracking() on s.UserId equals u.Id
                          where s.UserId != null && paidStudentIds.Contains(s.StudentId)
                          select new
                          {
                              s.StudentId,
                              UserId = u.Id,
                              FullName = u.FullName ?? s.Name ?? string.Empty,
                              Email = u.Email ?? s.Email ?? string.Empty,
                              Avatar = u.Avatar ?? s.ProfilePicture ?? Defaults.AvatarUrl,
                          }).ToListAsync();

        return rows
            .GroupBy(row => row.UserId)
            .Select(group => group.First())
            .Select(row => new PaidLearner(row.StudentId, row.UserId, row.FullName, row.Email, row.Avatar))
            .ToList();
    }

    private async Task AddSystemMessageAsync(int conversationId, string senderId, string content)
    {
        var message = new UserMessage
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            Type = MessageTypes.System,
            CreatedAt = DateTime.UtcNow,
        };
        _context.UserMessages.Add(message);

        var participants = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .ToListAsync();
        foreach (var participant in participants)
        {
            participant.LastMessage = content;
            participant.HasNewMessage = participant.UserId != senderId;
        }

        await _context.SaveChangesAsync();
    }

    private async Task NotifyAddedAsync(IEnumerable<string> userIds, string groupName, string actorUserId)
    {
        var targetIds = userIds.Where(id => id != actorUserId).Distinct().ToList();
        if (targetIds.Count == 0)
            return;

        await _notificationRepository.NotifyUsers(
            targetIds,
            NotificationTypes.StudyGroup,
            "Added to a course group",
            $"You were added to \"{groupName}\".",
            "/chat/chat-list");
    }

    private static ApiResponse Ok(string message) => new()
    {
        StatusCode = 200,
        Success = true,
        Message = message,
    };

    private static ApiResponse Error(string error, int statusCode) => new()
    {
        StatusCode = statusCode,
        Success = false,
        Errors = [error],
    };

    private sealed record PaidLearner(string StudentId, string UserId, string FullName, string Email, string Avatar);
}
