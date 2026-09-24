# YunXiaoJob

YunXiaoJob is a multi-company job marketplace with a simple applicant tracking system (ATS).

The first implementation step is the Domain layer. See [the domain model](docs/domain-model.md) for the agreed business boundaries, roles, lifecycles and rules.

## Solution structure

```text
src/
  YunXiaoJob.Domain/   # Business entities, states and rules; no framework dependency
  YunXiaoJob.Application/ # Use cases and contracts for repositories and services
  YunXiaoJob.Infrastructure/ # EF Core persistence, repository and security-service implementations
  YunXiaoJob.API/      # ASP.NET Core controllers, middleware and composition root
client/                # React + Vite web client (see client/README.md)
docs/
  domain-model.md      # Domain design decisions
```

## Running locally

```bash
dotnet run --project src/YunXiaoJob.API   # http://localhost:5018
cd client && npm install && npm run dev   # http://localhost:5173
```

## Google Meet for interviews

Set the following environment variables on the API host before scheduling interviews:

```text
GoogleMeet__ClientId
GoogleMeet__ClientSecret
GoogleMeet__RefreshToken
GoogleMeet__CalendarId=primary
```

The Google OAuth client must authorize Google Calendar event creation. Scheduling an interview creates a Calendar event with a Google Meet conference, invites the candidate, and writes the resulting Meet URL to the candidate notification.
