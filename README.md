# YunXiaoJob

YunXiaoJob is a multi-company job marketplace with a simple applicant tracking system (ATS).

The first implementation step is the Domain layer. See [the domain model](docs/domain-model.md) for the agreed business boundaries, roles, lifecycles and rules.

## Solution structure

```text
src/
  YunXiaoJob.Domain/   # Business entities, states and rules; no framework dependency
  YunXiaoJob.Application/ # Use cases and contracts for repositories and services
  YunXiaoJob.Infrastructure/ # EF Core persistence, repository and security-service implementations
docs/
  domain-model.md      # Domain design decisions
```
