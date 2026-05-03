using Qams.BuildingBlocks.Common;
using Qams.BuildingBlocks.Workflows;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var workflows = new List<WorkflowDefinition>
{
    new(
        "wf-capa-approval-v1",
        "CAPA Approval And Closure",
        "1.0.0",
        RecordLifecycleStatus.Effective.ToString(),
        new[]
        {
            new WorkflowStage("triage", "Triage", 1, false, TimeSpan.FromDays(2)),
            new WorkflowStage("investigation", "Investigation", 2, false, TimeSpan.FromDays(14)),
            new WorkflowStage("approval", "QA Approval", 3, true, TimeSpan.FromDays(5)),
            new WorkflowStage("effectiveness", "Effectiveness Check", 4, true, TimeSpan.FromDays(30))
        })
};

app.MapGet("/health", () => Results.Ok(new
{
    service = "Qams.Workflow.Api",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/v1/workflow-definitions", (HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    return Results.Ok(ApiResponse.Ok<IReadOnlyCollection<WorkflowDefinition>>(workflows, context.CorrelationId));
});

app.MapPost("/api/v1/workflow-definitions", (CreateWorkflowDefinitionRequest command, HttpRequest request) =>
{
    var context = QamsRequestContextFactory.From(request);
    var headerError = QamsRequestContextFactory.Validate(request, requireTenant: true, requireIdempotencyKey: true);
    if (headerError is not null)
    {
        return Results.BadRequest(ApiResponse.Fail<WorkflowDefinition>(headerError, context.CorrelationId));
    }

    var workflow = new WorkflowDefinition(
        $"wf-{command.RecordType.ToLowerInvariant()}-{Guid.NewGuid():N}",
        command.Name,
        "0.1.0-draft",
        RecordLifecycleStatus.Draft.ToString(),
        command.Stages);

    workflows.Add(workflow);
    return Results.Created($"/api/v1/workflow-definitions/{workflow.WorkflowDefinitionId}", ApiResponse.Ok(workflow, context.CorrelationId));
});

app.Run();

sealed record WorkflowDefinition(
    string WorkflowDefinitionId,
    string Name,
    string Version,
    string Status,
    IReadOnlyCollection<WorkflowStage> Stages);

sealed record CreateWorkflowDefinitionRequest(
    string RecordType,
    string Name,
    IReadOnlyCollection<WorkflowStage> Stages);
