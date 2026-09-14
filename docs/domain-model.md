# Domain model — YunXiaoJob

YunXiaoJob is a multi-company job marketplace with a simple applicant tracking system (ATS). The Domain project contains only plain entity classes, enums and properties. Business rules and state changes are implemented by Application Use Cases.

## Boundaries

| Boundary | Main entities | Ownership rule |
|---|---|---|
| Identity | `User`, `CandidateProfile`, `Resume` | A user is global to the platform. |
| Employer | `Company`, `CompanyMember`, `JobPosting` | Each employer-owned record belongs to exactly one company. |
| Recruitment | `JobApplication`, `Interview`, `JobOffer` | An application belongs to one job posting and therefore one company. |
| Marketplace | `SavedJob` | Candidates find, save and apply to published job postings. |

## Roles

- `Admin`: platform administrator; verifies companies and moderates job postings.
- `Owner`: company owner; manages the company and its members.
- `HR`: manages recruiting operations within the company.
- `Recruiter`: works only on assigned postings/applications.
- `Candidate`: creates a profile, manages CVs and applies for jobs.

`Owner`, `HR`, and `Recruiter` are membership roles on `CompanyMember`, not global user roles. This keeps every employer's data isolated.

## Core lifecycle

```text
Company: PendingVerification → Active | Rejected | Suspended
Job posting: Draft → PendingApproval → Published → Paused | Closed | Expired
Application: Submitted → Reviewing → Shortlisted → Interviewing → Offered → Hired
                                                      └──────────────→ Rejected
Candidate: may withdraw while the application has not ended
Offer: Draft → Sent → Accepted | Rejected | Expired | Withdrawn
```

## Essential rules

1. Only an active company may publish jobs.
2. A candidate may apply once to a job posting.
3. A resume selected for an application must belong to that candidate.
4. All ATS reads and writes must be constrained by the job posting's `CompanyId`.
5. Only public, published, non-expired jobs are searchable and applicable.
6. Only `Admin` may verify/suspend a company and approve/reject a job posting.
