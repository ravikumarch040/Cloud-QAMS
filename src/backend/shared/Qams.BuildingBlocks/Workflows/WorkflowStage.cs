namespace Qams.BuildingBlocks.Workflows;

public sealed record WorkflowStage(
    string StageId,
    string Name,
    int Sequence,
    bool RequiresESignature,
    TimeSpan? Sla);
