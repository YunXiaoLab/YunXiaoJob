using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Domain.Entities;

namespace YunXiaoJob.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<JobPostingSkill> JobPostingSkills => Set<JobPostingSkill>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<JobOffer> JobOffers => Set<JobOffer>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<RegistrationChallenge> RegistrationChallenges => Set<RegistrationChallenge>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PhoneNumber).HasMaxLength(30);
            entity.Property(x => x.PlatformRole).HasConversion<int>();
            entity.HasIndex(x => x.Email).IsUnique();

            entity.HasOne(x => x.CandidateProfile)
                .WithOne()
                .HasForeignKey<CandidateProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CandidateProfile>(entity =>
        {
            entity.ToTable("CandidateProfiles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Headline).HasMaxLength(250);
            entity.Property(x => x.Location).HasMaxLength(250);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasIndex(x => new { x.IsSearchable, x.Location });

            entity.HasMany(x => x.Resumes)
                .WithOne()
                .HasForeignKey(x => x.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Resume>(entity =>
        {
            entity.ToTable("Resumes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(250);
            entity.Property(x => x.FileUrl).IsRequired().HasMaxLength(1000);
            entity.HasIndex(x => new { x.CandidateProfileId, x.IsDefault });
        });

        builder.Entity<Company>(entity =>
        {
            entity.ToTable("Companies");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(250);
            entity.Property(x => x.LogoUrl).HasMaxLength(1000);
            entity.Property(x => x.Website).HasMaxLength(500);
            entity.Property(x => x.Address).HasMaxLength(500);
            entity.Property(x => x.Industry).HasMaxLength(150);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasIndex(x => x.Status);

            entity.HasMany(x => x.Members)
                .WithOne()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CompanyMember>(entity =>
        {
            entity.ToTable("CompanyMembers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Role).HasConversion<int>();
            entity.HasIndex(x => new { x.CompanyId, x.UserId }).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.IsActive });

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<JobPosting>(entity =>
        {
            entity.ToTable("JobPostings");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(300);
            entity.Property(x => x.Description).IsRequired();
            entity.Property(x => x.Requirements).IsRequired();
            entity.Property(x => x.Location).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Benefits).HasMaxLength(4000);
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            entity.Property(x => x.MinSalary).HasPrecision(18, 2);
            entity.Property(x => x.MaxSalary).HasPrecision(18, 2);
            entity.Property(x => x.EmploymentType).HasConversion<int>();
            entity.Property(x => x.WorkplaceType).HasConversion<int>();
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasIndex(x => new { x.Status, x.ApplicationDeadline });
            entity.HasIndex(x => new { x.CompanyId, x.Status });
            entity.HasIndex(x => x.Location);

            entity.HasOne<Company>()
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<JobCategory>()
                .WithMany()
                .HasForeignKey(x => x.JobCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByMemberId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.AssignedRecruiterMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Skills)
                .WithOne()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<JobPostingSkill>(entity =>
        {
            entity.ToTable("JobPostingSkills");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => new { x.JobPostingId, x.Name }).IsUnique();
        });

        builder.Entity<JobApplication>(entity =>
        {
            entity.ToTable("JobApplications");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.CoverLetter).HasMaxLength(4000);
            entity.HasIndex(x => new { x.JobPostingId, x.CandidateProfileId }).IsUnique();
            entity.HasIndex(x => new { x.JobPostingId, x.Status });
            entity.HasIndex(x => new { x.AssignedRecruiterMemberId, x.Status });

            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<CandidateProfile>()
                .WithMany()
                .HasForeignKey(x => x.CandidateProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Resume>()
                .WithMany()
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.AssignedRecruiterMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.StatusHistory)
                .WithOne()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Interviews)
                .WithOne()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Offers)
                .WithOne()
                .HasForeignKey(x => x.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ApplicationStatusHistory>(entity =>
        {
            entity.ToTable("ApplicationStatusHistories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FromStatus).HasConversion<int?>();
            entity.Property(x => x.ToStatus).HasConversion<int>();
            entity.Property(x => x.Note).HasMaxLength(2000);
            entity.HasIndex(x => new { x.JobApplicationId, x.CreatedAtUtc });

            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.ChangedByMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Interview>(entity =>
        {
            entity.ToTable("Interviews");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.LocationOrMeetingUrl).HasMaxLength(1000);
            entity.Property(x => x.Note).HasMaxLength(2000);
            entity.Property(x => x.EvaluationNote).HasMaxLength(4000);
            entity.HasIndex(x => new { x.JobApplicationId, x.StartsAtUtc });
            entity.HasIndex(x => new { x.ScheduledByMemberId, x.StartsAtUtc });

            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.ScheduledByMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<JobOffer>(entity =>
        {
            entity.ToTable("JobOffers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<int>();
            entity.Property(x => x.Salary).HasPrecision(18, 2);
            entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            entity.Property(x => x.Note).HasMaxLength(4000);
            entity.HasIndex(x => new { x.JobApplicationId, x.Status });

            entity.HasOne<CompanyMember>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SavedJob>(entity =>
        {
            entity.ToTable("SavedJobs");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.CandidateProfileId, x.JobPostingId }).IsUnique();

            entity.HasOne<CandidateProfile>()
                .WithMany()
                .HasForeignKey(x => x.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<JobPosting>()
                .WithMany()
                .HasForeignKey(x => x.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens"); entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.TokenHash).IsUnique(); entity.HasIndex(x => x.UserId);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("PasswordResetTokens"); entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.TokenHash).IsUnique(); entity.HasIndex(x => x.UserId);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<RegistrationChallenge>(entity =>
        {
            entity.ToTable("RegistrationChallenges"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(255); entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500); entity.Property(x => x.OtpHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(x => x.Email).IsUnique(); entity.HasIndex(x => x.ExpiresAtUtc);
        });
        builder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(250); entity.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.TargetUrl).HasMaxLength(500); entity.Property(x => x.Type).HasConversion<int>();
            entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAtUtc });
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<JobCategory>(entity => { entity.ToTable("JobCategories"); entity.HasKey(x => x.Id); entity.Property(x => x.Name).IsRequired().HasMaxLength(150); entity.HasIndex(x => x.Name).IsUnique(); });
        builder.Entity<Skill>(entity => { entity.ToTable("Skills"); entity.HasKey(x => x.Id); entity.Property(x => x.Name).IsRequired().HasMaxLength(100); entity.HasIndex(x => x.Name).IsUnique(); });
        builder.Entity<Location>(entity => { entity.ToTable("Locations"); entity.HasKey(x => x.Id); entity.Property(x => x.Name).IsRequired().HasMaxLength(150); entity.HasIndex(x => x.Name).IsUnique(); });
    }
}
