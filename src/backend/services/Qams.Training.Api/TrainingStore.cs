namespace Qams.Training.Api;

public sealed class TrainingStore
{
    private readonly object syncRoot = new();
    private readonly List<TrainingCourse> courses = new()
    {
        new(
            "course-0001",
            "tenant-life-sciences-demo",
            "Good Manufacturing Practices",
            "Training on GMP requirements for manufacturing operations.",
            120,
            true,
            DateTimeOffset.UtcNow.AddDays(-30),
            new[]
            {
                new TrainingEnrollment("enroll-0001", "course-0001", "tenant-life-sciences-demo", "qa-technician-1", TrainingStatus.InProgress, null, null, "seed-correlation-1")
            },
            "seed-correlation-1")
    };

    public IReadOnlyCollection<TrainingCourse> ListCourses(string? tenantId)
    {
        lock (syncRoot)
        {
            return courses
                .Where(course => string.IsNullOrWhiteSpace(tenantId) || course.TenantId == tenantId)
                .OrderByDescending(course => course.CreatedAtUtc)
                .ToArray();
        }
    }

    public TrainingCourse? GetCourse(string courseId, string? tenantId)
    {
        lock (syncRoot)
        {
            return courses.FirstOrDefault(course => course.CourseId == courseId &&
                (string.IsNullOrWhiteSpace(tenantId) || course.TenantId == tenantId));
        }
    }

    public TrainingCourse CreateCourse(CreateTrainingCourseRequest request, string courseId, string correlationId)
    {
        lock (syncRoot)
        {
            var course = new TrainingCourse(
                courseId,
                request.TenantId,
                request.Title,
                request.Description,
                request.DurationMinutes,
                request.Required,
                DateTimeOffset.UtcNow,
                Array.Empty<TrainingEnrollment>(),
                correlationId);

            courses.Add(course);
            return course;
        }
    }

    public TrainingEnrollment? Enroll(string courseId, EnrollTrainingRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var course = courses.FirstOrDefault(item => item.CourseId == courseId && item.TenantId == tenantId);
            if (course is null)
            {
                return null;
            }

            var enrollment = new TrainingEnrollment(
                Guid.NewGuid().ToString("N"),
                course.CourseId,
                course.TenantId,
                request.TraineeUserId,
                TrainingStatus.InProgress,
                null,
                null,
                correlationId);

            var updatedCourse = course with { Enrollments = course.Enrollments.Concat(new[] { enrollment }).ToArray(), CorrelationId = correlationId };
            courses.Remove(course);
            courses.Add(updatedCourse);
            return enrollment;
        }
    }

    public TrainingEnrollment? CompleteEnrollment(string courseId, string enrollmentId, CompleteTrainingRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var course = courses.FirstOrDefault(item => item.CourseId == courseId && item.TenantId == tenantId);
            if (course is null)
            {
                return null;
            }

            var enrollment = course.Enrollments.FirstOrDefault(item => item.EnrollmentId == enrollmentId && item.TenantId == tenantId);
            if (enrollment is null)
            {
                return null;
            }

            var completed = enrollment with
            {
                Status = TrainingStatus.Completed,
                CompletedByUserId = request.CompletedByUserId,
                Score = request.Score,
                CompletedAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId
            };

            var updatedEnrollments = course.Enrollments.Select(item => item.EnrollmentId == enrollmentId ? completed : item).ToArray();
            var updatedCourse = course with { Enrollments = updatedEnrollments, CorrelationId = correlationId };

            courses.Remove(course);
            courses.Add(updatedCourse);
            return completed;
        }
    }
}

public sealed record TrainingCourse(
    string CourseId,
    string TenantId,
    string Title,
    string Description,
    int DurationMinutes,
    bool Required,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyCollection<TrainingEnrollment> Enrollments,
    string CorrelationId);

public sealed record TrainingEnrollment(
    string EnrollmentId,
    string CourseId,
    string TenantId,
    string TraineeUserId,
    TrainingStatus Status,
    string? CompletedByUserId,
    int? Score,
    string CorrelationId,
    DateTimeOffset? CompletedAtUtc = null);

public sealed record CreateTrainingCourseRequest(
    string TenantId,
    string Title,
    string Description,
    int DurationMinutes,
    bool Required);

public sealed record EnrollTrainingRequest(
    string TenantId,
    string TraineeUserId);

public sealed record CompleteTrainingRequest(
    string TenantId,
    string CompletedByUserId,
    int Score);

public enum TrainingStatus
{
    InProgress,
    Completed,
    Overdue
}
