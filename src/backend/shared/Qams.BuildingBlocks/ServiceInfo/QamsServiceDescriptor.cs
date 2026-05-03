namespace Qams.BuildingBlocks.ServiceInfo;

public sealed record QamsServiceDescriptor(
    string ServiceName,
    string ServiceGroup,
    string Status,
    IReadOnlyCollection<string> Responsibilities);
