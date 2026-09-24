# Đối chiếu 19 ca sử dụng và file Use Case

Danh sách này phản ánh mã nguồn hiện tại. Một số file gom nhiều class use case liên quan; các class đọc (`Get...UseCase`) nằm trong `Queries/QueryUseCases.cs` khi phù hợp.

| # | Ca sử dụng đã chuẩn hóa | File Use Case chứa xử lý |
|---:|---|---|
| UC-01 | Tài khoản & xác thực | `Application/UseCases/Accounts/RegistrationOtpUseCases.cs` — `StartRegistrationChallengeUseCase`, `VerifyRegistrationOtpUseCase`, `ResendRegistrationOtpUseCase`<br>`Application/UseCases/Accounts/AuthenticateUseCases.cs` — `LoginUseCase`, `RefreshAccessTokenUseCase`, `ResetPasswordUseCase`<br>`Application/UseCases/Accounts/ForgotPasswordUseCase.cs` |
| UC-02 | Khám phá việc làm | `Application/UseCases/JobPostings/SearchJobPostingsUseCase.cs`<br>`Application/UseCases/Queries/QueryUseCases.cs` — `GetJobPostingDetailUseCase`, `GetCompanyProfileUseCase` |
| UC-03 | Đăng ký doanh nghiệp | `Application/UseCases/CompanyRegistrations/SubmitCompanyRegistrationUseCase.cs` |
| UC-04 | Quản lý hồ sơ ứng viên | `Application/UseCases/Candidates/GetMyCandidateProfileUseCase.cs`<br>`Application/UseCases/Candidates/UpdateCandidateProfileUseCase.cs`<br>`Application/UseCases/Candidates/UpdateCandidateSectionsUseCases.cs` |
| UC-05 | Quản lý CV & việc đã lưu | `Application/UseCases/Candidates/AddResumeUseCase.cs`<br>`Application/UseCases/Candidates/SetDefaultResumeUseCase.cs`<br>`Application/UseCases/Candidates/SavedJobUseCases.cs` |
| UC-06 | Ứng tuyển & rút đơn | `Application/UseCases/Applications/ApplyForJobUseCase.cs`<br>`Application/UseCases/Applications/WithdrawApplicationUseCase.cs` |
| UC-07 | Quản lý đơn đã nộp & phản hồi Offer | `Application/UseCases/Queries/QueryUseCases.cs` — `GetMyApplicationsUseCase`, `GetApplicationDetailUseCase`<br>`Application/UseCases/Applications/RespondToJobOfferUseCase.cs` |
| UC-08 | Quản lý thông báo | `Application/UseCases/Notifications/NotificationUseCases.cs` — `GetMyNotificationsUseCase`, `MarkNotificationReadUseCase` |
| UC-09 | Quản lý công ty & thành viên | `Application/UseCases/Companies/CreateCompanyUseCase.cs`<br>`Application/UseCases/Companies/UpdateCompanyUseCase.cs`<br>`Application/UseCases/Companies/AddCompanyMemberUseCase.cs`<br>`Application/UseCases/Companies/CreateCompanyStaffAccountUseCase.cs`<br>`Application/UseCases/Queries/QueryUseCases.cs` — `GetMyCompaniesUseCase`, `GetCompanyMembersUseCase` |
| UC-10 | Quản lý tin tuyển dụng | `Application/UseCases/JobPostings/CreateJobPostingUseCase.cs`<br>`Application/UseCases/JobPostings/UpdateJobPostingUseCase.cs`<br>`Application/UseCases/JobPostings/SubmitJobPostingUseCase.cs`<br>`Application/UseCases/JobPostings/ManageJobPostingLifecycleUseCases.cs` — pause, resume, close<br>`Application/UseCases/Queries/QueryUseCases.cs` — `GetCompanyJobPostingsUseCase` |
| UC-11 | Quản lý đơn ứng tuyển (Employer) | `Application/UseCases/Applications/ChangeApplicationStatusUseCase.cs`<br>`Application/UseCases/Queries/QueryUseCases.cs` — `GetJobApplicationsUseCase`, `GetApplicationDetailUseCase` |
| UC-12 | Phân công Recruiter | `Application/UseCases/Applications/AssignRecruiterUseCase.cs` |
| UC-13 | Quản lý phỏng vấn & Offer | `Application/UseCases/Applications/ScheduleInterviewUseCase.cs`<br>`Application/UseCases/Applications/CompleteInterviewUseCase.cs`<br>`Application/UseCases/Applications/CreateJobOfferUseCase.cs` |
| UC-14 | Xem dashboard công ty | `Application/UseCases/Dashboards/DashboardUseCases.cs` — `GetCompanyDashboardUseCase` |
| UC-15 | Quản lý người dùng | `Application/UseCases/Administration/UserManagementUseCases.cs` — `GetUsersUseCase`, `SetUserActiveUseCase` |
| UC-16 | Xét duyệt đăng ký doanh nghiệp | `Application/UseCases/CompanyRegistrations/ReviewCompanyRegistrationUseCases.cs` — `GetCompanyRegistrationsUseCase`, `ApproveCompanyRegistrationUseCase`, `RejectCompanyRegistrationUseCase` |
| UC-17 | Kiểm duyệt công ty | `Application/UseCases/Administration/ApproveCompanyUseCase.cs`<br>`Application/UseCases/Administration/ModerateLifecycleUseCases.cs` — `SuspendCompanyUseCase`, `RejectCompanyUseCase` |
| UC-18 | Kiểm duyệt tin tuyển dụng | `Application/UseCases/Administration/ApproveJobPostingUseCase.cs`<br>`Application/UseCases/Administration/ModerateLifecycleUseCases.cs` — `RejectJobPostingUseCase`<br>`Application/UseCases/Queries/QueryUseCases.cs` — `GetAdminJobPostingsUseCase` |
| UC-19 | Xem dashboard nền tảng | `Application/UseCases/Dashboards/DashboardUseCases.cs` — `GetPlatformDashboardUseCase` |

## File bổ trợ, không phải Use Case trực tiếp

| Trách nhiệm | File |
|---|---|
| Tự expire job và offer | `API/Services/LifecycleExpiryService.cs` |
| Tạo Calendar event và Google Meet | `Infrastructure/Services/GoogleMeetService.cs` |
| Controller/API mapping | `API/Controllers/*.cs` |
| DTO request/response | `Application/DTOs/Requests/Requests.cs`, `Application/DTOs/Responses/Responses.cs` |
