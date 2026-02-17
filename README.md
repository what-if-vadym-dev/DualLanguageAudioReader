# ExamPrepar .NET 8 Solution Skeleton

## Overview
A Clean Architecture-style .NET 8 solution with ASP.NET Core Web API, application/domain/infrastructure class libraries, and xUnit tests.

## Structure
- src/ExamPrepar.Api — ASP.NET Core Web API (Minimal API template)
- src/ExamPrepar.Application — application layer (use cases, DTOs)
- src/ExamPrepar.Domain — domain entities and logic
- src/ExamPrepar.Infrastructure — persistence/integration adapters
- tests/ExamPrepar.UnitTests — unit tests (xUnit)

## Project References
- ExamPrepar.Api -> ExamPrepar.Application, ExamPrepar.Infrastructure
- ExamPrepar.Application -> ExamPrepar.Domain
- ExamPrepar.Infrastructure -> ExamPrepar.Application, ExamPrepar.Domain
- ExamPrepar.UnitTests -> ExamPrepar.Application, ExamPrepar.Domain

## Quick Commands
- dotnet restore
- dotnet build
- dotnet test
- dotnet run --project src/ExamPrepar.Api

## Run With Docker

Build the image:

```powershell
docker build -t dual-language-audio-reader .
```

Run the container (app on http://localhost:5180):

```powershell
docker run -d --name dual-language-audio-reader -p 5180:8080 dual-language-audio-reader
```

Stop and remove the container:

```powershell
docker stop dual-language-audio-reader
docker rm dual-language-audio-reader
```

### Docker Compose

Start via compose:

```powershell
docker compose up -d
```

Stop via compose:

```powershell
docker compose down
```

## Notes
- A repo-scoped nuget.config is included to use nuget.org and avoid private feed prompts.
- Solution file created as ExamPrepar.slnx (newer SDK). Use it with dotnet sln ExamPrepar.slnx ...

## Next Steps
- Add domain entities and services in Domain/Application.
- Implement persistence (e.g., EF Core) in Infrastructure.
- Replace the template WeatherForecast with your API endpoints.
- Add integration tests and CI pipeline as needed.
