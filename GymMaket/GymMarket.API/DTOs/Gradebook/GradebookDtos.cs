namespace GymMarket.API.DTOs.Gradebook;

public class GradeCategoryDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal WeightPercent { get; set; }
    public int Order { get; set; }
    public bool IsDefault { get; set; }
}

public class GradeItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ItemType { get; set; } = "Quiz";
    public string? CategoryId { get; set; }
    public decimal PointsPossible { get; set; }
    public bool IsPublished { get; set; }
}

public class UpdateGradeCategoryDto
{
    public string? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal WeightPercent { get; set; }
    public int Order { get; set; }
}

public class UpdateGradeItemCategoryDto
{
    public string ItemId { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
}

public class UpdateGradebookPolicyDto
{
    public List<UpdateGradeCategoryDto> Categories { get; set; } = new();
    public List<UpdateGradeItemCategoryDto> Items { get; set; } = new();
}

public class GradebookPolicyDto
{
    public string CourseId { get; set; } = string.Empty;
    public string? CourseTitle { get; set; }
    public List<GradeCategoryDto> Categories { get; set; } = new();
    public List<GradeItemDto> Items { get; set; } = new();
}

public class GradeItemScoreDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ItemType { get; set; } = "Quiz";
    public string CategoryId { get; set; } = string.Empty;
    public decimal PointsPossible { get; set; }
    public decimal? Score { get; set; }
    public decimal? TotalPoints { get; set; }
    public decimal? ScorePercent { get; set; }
    public string Status { get; set; } = "Missing";
    public DateTime? SubmittedAt { get; set; }
}

public class CategoryGradeDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal WeightPercent { get; set; }
    public decimal? CurrentPercent { get; set; }
    public decimal FinalPercent { get; set; }
    public decimal WeightedPoints { get; set; }
    public int GradedItems { get; set; }
    public int TotalItems { get; set; }
}

public class StudentGradeSummaryDto
{
    public string StudentId { get; set; } = string.Empty;
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public decimal? CurrentPercent { get; set; }
    public decimal FinalPercent { get; set; }
    public string LetterGrade { get; set; } = "F";
    public List<CategoryGradeDto> Categories { get; set; } = new();
    public List<GradeItemScoreDto> Items { get; set; } = new();
}

public class CourseGradebookDto
{
    public string CourseId { get; set; } = string.Empty;
    public string? CourseTitle { get; set; }
    public List<GradeCategoryDto> Categories { get; set; } = new();
    public List<GradeItemDto> Items { get; set; } = new();
    public List<StudentGradeSummaryDto> Students { get; set; } = new();
    public decimal? CurrentAveragePercent { get; set; }
    public decimal FinalAveragePercent { get; set; }
}
