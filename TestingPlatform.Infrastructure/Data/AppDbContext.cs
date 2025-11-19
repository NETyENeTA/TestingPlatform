
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets для всех моделей
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Direction> Directions { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Attempt> Attempts { get; set; }
    public DbSet<UserAttemptAnswer> UserAttemptAnswers { get; set; }
    public DbSet<UserSelectedOption> UserSelectedOptions { get; set; }
    public DbSet<UserTextAnswer> UserTextAnswers { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Login).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.UserId).IsUnique();
            entity.HasOne(s => s.User)
                  .WithOne(u => u.Student)
                  .HasForeignKey<Student>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(p => p.Name).IsUnique();
        });

        // Direction configuration
        modelBuilder.Entity<Direction>(entity =>
        {
            entity.HasIndex(d => d.Name).IsUnique();
        });

        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(g => g.Name).IsUnique();

            entity.HasOne(g => g.Direction)
                  .WithMany(d => d.Groups)
                  .HasForeignKey(g => g.DirectionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Course)
                  .WithMany(c => c.Groups)
                  .HasForeignKey(g => g.CourseId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Project)
                  .WithMany(p => p.Groups)
                  .HasForeignKey(g => g.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Many-to-many: Student-Group
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Groups)
            .WithMany(g => g.Students)
            .UsingEntity<Dictionary<string, object>>(
                "StudentGroup",
                j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                j => j.HasOne<Student>().WithMany().HasForeignKey("StudentId")
            );

        // Test configuration
        modelBuilder.Entity<Test>(entity =>
        {
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.IsRepeatable).HasDefaultValue(false);
            entity.Property(t => t.IsPublic).HasDefaultValue(false);
        });

        // Many-to-many relationships for Test
        modelBuilder.Entity<Test>()
            .HasMany(t => t.Students)
            .WithMany(s => s.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestStudent",
                j => j.HasOne<Student>().WithMany().HasForeignKey("StudentId"),
                j => j.HasOne<Test>().WithMany().HasForeignKey("TestId")
            );

        modelBuilder.Entity<Test>()
            .HasMany(t => t.Projects)
            .WithMany(p => p.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestProject",
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),
                j => j.HasOne<Test>().WithMany().HasForeignKey("TestId")
            );

        modelBuilder.Entity<Test>()
            .HasMany(t => t.Courses)
            .WithMany(c => c.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestCourse",
                j => j.HasOne<Course>().WithMany().HasForeignKey("CourseId"),
                j => j.HasOne<Test>().WithMany().HasForeignKey("TestId")
            );

        modelBuilder.Entity<Test>()
            .HasMany(t => t.Groups)
            .WithMany(g => g.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestGroup",
                j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                j => j.HasOne<Test>().WithMany().HasForeignKey("TestId")
            );

        modelBuilder.Entity<Test>()
            .HasMany(t => t.Directions)
            .WithMany(d => d.Tests)
            .UsingEntity<Dictionary<string, object>>(
                "TestDirection",
                j => j.HasOne<Direction>().WithMany().HasForeignKey("DirectionId"),
                j => j.HasOne<Test>().WithMany().HasForeignKey("TestId")
            );

        // Question configuration
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasIndex(q => new { q.TestId, q.Number }).IsUnique();

            entity.HasOne(q => q.Test)
                  .WithMany(t => t.Questions)
                  .HasForeignKey(q => q.TestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Answer configuration
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasOne(a => a.Question)
                  .WithMany(q => q.Answers)
                  .HasForeignKey(a => a.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Attempt configuration
        modelBuilder.Entity<Attempt>(entity =>
        {
            entity.Property(a => a.StartedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(a => a.Test)
                  .WithMany(t => t.Attempts)
                  .HasForeignKey(a => a.TestId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Student)
                  .WithMany(s => s.Attempts)
                  .HasForeignKey(a => a.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserAttemptAnswer configuration
        modelBuilder.Entity<UserAttemptAnswer>(entity =>
        {
            entity.HasIndex(ua => new { ua.AttemptId, ua.QuestionId }).IsUnique();

            entity.HasOne(ua => ua.Attempt)
                  .WithMany(a => a.UserAttemptAnswers)
                  .HasForeignKey(ua => ua.AttemptId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ua => ua.Question)
                  .WithMany(q => q.UserAttemptAnswers)
                  .HasForeignKey(ua => ua.QuestionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // UserSelectedOption configuration
        modelBuilder.Entity<UserSelectedOption>(entity =>
        {
            entity.HasOne(uso => uso.UserAttemptAnswer)
                  .WithMany(uaa => uaa.UserSelectedOptions)
                  .HasForeignKey(uso => uso.UserAttemptAnswerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uso => uso.Answer)
                  .WithMany(a => a.UserSelectedOptions)
                  .HasForeignKey(uso => uso.AnswerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // UserTextAnswer configuration
        modelBuilder.Entity<UserTextAnswer>(entity =>
        {
            entity.HasOne(uta => uta.UserAttemptAnswer)
                  .WithOne(uaa => uaa.UserTextAnswer)
                  .HasForeignKey<UserTextAnswer>(uta => uta.UserAttemptAnswerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TestResult configuration - ИСПРАВЛЕННАЯ ЧАСТЬ
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasIndex(tr => new { tr.TestId, tr.StudentId, tr.AttemptId }).IsUnique();

            entity.HasOne(tr => tr.Test)
                  .WithMany(t => t.TestResults) // Теперь Test имеет TestResults
                  .HasForeignKey(tr => tr.TestId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tr => tr.Attempt)
                  .WithMany(a => a.TestResults) // Attempt имеет TestResults
                  .HasForeignKey(tr => tr.AttemptId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tr => tr.Student)
                  .WithMany(s => s.TestResults) // Student имеет TestResults
                  .HasForeignKey(tr => tr.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}