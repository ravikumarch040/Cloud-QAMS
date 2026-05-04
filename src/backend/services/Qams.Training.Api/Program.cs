using Qams.BuildingBlocks.Common;
using Qams.Training.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TrainingStore>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Training.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/training-courses", (HttpRequest request, TrainingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var courses = store.ListCourses(context.TenantId);
    return Results.Ok(ApiResponse.Ok(courses, context.CorrelationId));
});

app.MapGet("/api/v1/training-courses/{courseId}", (string courseId, HttpRequest request, TrainingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var course = store.GetCourse(courseId, context.TenantId);
    if (course is null)
    {
        return Results.NotFound(ApiResponse.Fail<TrainingCourse>(new ApiError("course_not_found", "Training course not found."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(course, context.CorrelationId));
});

app.MapPost("/api/v1/training-courses", (CreateTrainingCourseRequest command, HttpRequest request, TrainingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingCourse>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingCourse>(new ApiError("tenant_mismatch", "Request tenant header must match course tenant."), context.CorrelationId));
    }

    var course = store.CreateCourse(command, Guid.NewGuid().ToString("N"), context.CorrelationId);
    return Results.Created($"/api/v1/training-courses/{course.CourseId}", ApiResponse.Ok(course, context.CorrelationId));
});

app.MapPost("/api/v1/training-courses/{courseId}/enroll", (string courseId, EnrollTrainingRequest command, HttpRequest request, TrainingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingEnrollment>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingEnrollment>(new ApiError("tenant_mismatch", "Request tenant header must match enrollment tenant."), context.CorrelationId));
    }

    var enrollment = store.Enroll(courseId, command, context.TenantId, context.CorrelationId);
    if (enrollment is null)
    {
        return Results.NotFound(ApiResponse.Fail<TrainingEnrollment>(new ApiError("course_not_found", "Training course not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Created($"/api/v1/training-courses/{courseId}/enrollments/{enrollment.EnrollmentId}", ApiResponse.Ok(enrollment, context.CorrelationId));
});

app.MapPost("/api/v1/training-courses/{courseId}/enrollments/{enrollmentId}/complete", (string courseId, string enrollmentId, CompleteTrainingRequest command, HttpRequest request, TrainingStore store) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingEnrollment>(headerError, context.CorrelationId));
    }

    if (!string.Equals(command.TenantId, context.TenantId, StringComparison.Ordinal))
    {
        return Results.BadRequest(ApiResponse.Fail<TrainingEnrollment>(new ApiError("tenant_mismatch", "Request tenant header must match completion tenant."), context.CorrelationId));
    }

    var completed = store.CompleteEnrollment(courseId, enrollmentId, command, context.TenantId, context.CorrelationId);
    if (completed is null)
    {
        return Results.NotFound(ApiResponse.Fail<TrainingEnrollment>(new ApiError("enrollment_not_found", "Enrollment not found or tenant mismatch."), context.CorrelationId));
    }

    return Results.Ok(ApiResponse.Ok(completed, context.CorrelationId));
});

app.Run();

public partial class Program { }
