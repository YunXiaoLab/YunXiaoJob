namespace YunXiaoJob.Application.Interfaces.Services;

public interface IMeetingService
{
    Task<string> CreateGoogleMeetAsync(string title, string candidateEmail, DateTime startsAtUtc,
        DateTime endsAtUtc, CancellationToken cancellationToken = default);
}
