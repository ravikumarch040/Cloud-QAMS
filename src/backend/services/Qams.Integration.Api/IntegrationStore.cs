namespace Qams.Integration.Api;

public sealed class IntegrationStore
{
    private readonly object syncRoot = new();
    private readonly List<ConnectorDefinition> connectors = new()
    {
        new(
            "connector-0001",
            "tenant-life-sciences-demo",
            "ERP Supplier Sync",
            "SAP",
            "https://erp.example.com/api/suppliers",
            ConnectorStatus.Active,
            DateTimeOffset.UtcNow.AddDays(-15),
            "seed-correlation-1")
    };

    public IReadOnlyCollection<ConnectorDefinition> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return connectors
                .Where(connector => string.IsNullOrWhiteSpace(tenantId) || connector.TenantId == tenantId)
                .OrderBy(connector => connector.Name)
                .ToArray();
        }
    }

    public ConnectorDefinition? Get(string connectorId, string? tenantId)
    {
        lock (syncRoot)
        {
            return connectors.FirstOrDefault(connector => connector.ConnectorId == connectorId &&
                (string.IsNullOrWhiteSpace(tenantId) || connector.TenantId == tenantId));
        }
    }

    public ConnectorDefinition Create(CreateConnectorRequest request, string connectorId, string correlationId)
    {
        lock (syncRoot)
        {
            var connector = new ConnectorDefinition(
                connectorId,
                request.TenantId,
                request.Name,
                request.Type,
                request.EndpointUrl,
                ConnectorStatus.Active,
                DateTimeOffset.UtcNow,
                correlationId);

            connectors.Add(connector);
            return connector;
        }
    }

    public ConnectorTestResult Test(string connectorId, TestConnectorRequest request, string tenantId, string correlationId)
    {
        lock (syncRoot)
        {
            var connector = connectors.FirstOrDefault(item => item.ConnectorId == connectorId && item.TenantId == tenantId);
            if (connector is null)
            {
                return new ConnectorTestResult(connectorId, false, "Connector not found.", correlationId);
            }

            var success = connector.EndpointUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            return new ConnectorTestResult(connectorId, success, success ? "Connector test succeeded." : "Connector endpoint is not secure.", correlationId);
        }
    }
}

public sealed record ConnectorDefinition(
    string ConnectorId,
    string TenantId,
    string Name,
    string Type,
    string EndpointUrl,
    ConnectorStatus Status,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateConnectorRequest(
    string TenantId,
    string Name,
    string Type,
    string EndpointUrl);

public sealed record TestConnectorRequest(
    string TenantId);

public sealed record ConnectorTestResult(
    string ConnectorId,
    bool Connected,
    string Message,
    string CorrelationId);

public enum ConnectorStatus
{
    Active,
    Disabled,
    Error
}
