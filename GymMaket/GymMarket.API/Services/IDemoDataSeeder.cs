namespace GymMarket.API.Services;

public interface IDemoDataSeeder
{
    Task<DemoDataSeedResult> EnsureSeededAsync(CancellationToken cancellationToken = default);
}

public sealed record DemoDataSeedResult(bool Created, string CourseId, string TrainerEmail, string StudentEmail);
