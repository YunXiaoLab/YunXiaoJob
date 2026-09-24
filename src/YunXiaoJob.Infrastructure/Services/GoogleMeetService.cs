using System.Net.Http.Json;
using System.Text.Json;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Infrastructure.Settings;

namespace YunXiaoJob.Infrastructure.Services;

public sealed class GoogleMeetService : IMeetingService
{
    private readonly HttpClient _http; private readonly GoogleMeetSettings _settings;
    public GoogleMeetService(HttpClient http, GoogleMeetSettings settings) { _http = http; _settings = settings; }

    public async Task<string> CreateGoogleMeetAsync(string title, string candidateEmail, DateTime startsAtUtc,
        DateTime endsAtUtc, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.ClientId) || string.IsNullOrWhiteSpace(_settings.ClientSecret) ||
            string.IsNullOrWhiteSpace(_settings.RefreshToken))
            throw new InvalidOperationException("Google Meet is not configured. Set GoogleMeet ClientId, ClientSecret and RefreshToken.");

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
        { Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret, ["refresh_token"] = _settings.RefreshToken,
            ["grant_type"] = "refresh_token" }) };
        using var tokenResponse = await _http.SendAsync(tokenRequest, cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();
        using var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync(cancellationToken));
        var token = tokenJson.RootElement.GetProperty("access_token").GetString()!;

        var eventBody = new { summary = title, start = new { dateTime = startsAtUtc.ToString("O"), timeZone = "UTC" },
            end = new { dateTime = endsAtUtc.ToString("O"), timeZone = "UTC" }, attendees = new[] { new { email = candidateEmail } },
            conferenceData = new { createRequest = new { requestId = Guid.NewGuid().ToString("N"), conferenceSolutionKey = new { type = "hangoutsMeet" } } } };
        using var eventRequest = new HttpRequestMessage(HttpMethod.Post,
            $"https://www.googleapis.com/calendar/v3/calendars/{Uri.EscapeDataString(_settings.CalendarId)}/events?conferenceDataVersion=1")
        { Content = JsonContent.Create(eventBody) };
        eventRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var eventResponse = await _http.SendAsync(eventRequest, cancellationToken);
        eventResponse.EnsureSuccessStatusCode();
        using var eventJson = JsonDocument.Parse(await eventResponse.Content.ReadAsStringAsync(cancellationToken));
        var root = eventJson.RootElement;
        var link = root.TryGetProperty("hangoutLink", out var hangout) ? hangout.GetString() : null;
        if (string.IsNullOrWhiteSpace(link) && root.TryGetProperty("conferenceData", out var conference) &&
            conference.TryGetProperty("entryPoints", out var points))
            link = points.EnumerateArray().FirstOrDefault(x => x.TryGetProperty("entryPointType", out var type) && type.GetString() == "video").GetProperty("uri").GetString();
        return !string.IsNullOrWhiteSpace(link) ? link : throw new InvalidOperationException("Google Calendar did not return a Meet link.");
    }
}
