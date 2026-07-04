using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseStudyGroups;

public class CourseStudyGroupDto
{
    public string StudyGroupId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public int ConversationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Kind { get; set; } = string.Empty;

    public bool IsDefaultCohort { get; set; }

    public bool IsActive { get; set; }

    public bool IsMember { get; set; }

    public bool CanManage { get; set; }

    public int MemberCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<CourseStudyGroupMemberDto> Members { get; set; } = [];
}

public class CourseStudyGroupMemberDto
{
    public string UserId { get; set; } = string.Empty;

    public string? StudentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    public bool IsEligibleLearner { get; set; }
}

public class EligibleCourseLearnerDto
{
    public string StudentId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public bool IsInGroup { get; set; }
}

public class UpsertCourseStudyGroupDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? Kind { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> MemberUserIds { get; set; } = [];
}

public class ManageCourseStudyGroupMembersDto
{
    [Required]
    [MinLength(1)]
    public List<string> UserIds { get; set; } = [];
}

public class UpdateCourseStudyGroupMemberRoleDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
