namespace Qams.Policy.Api;

public sealed class PolicyStore
{
    private readonly List<PolicyRule> rules = new()
    {
        new("qa-approve-capa", "tenant-life-sciences-demo", "CapaCase", "approve", ["qa-manager", "quality-director"]),
        new("qa-close-capa", "tenant-life-sciences-demo", "CapaCase", "close", ["qa-manager"]),
        new("audit-create-finding", "tenant-life-sciences-demo", "AuditFinding", "create", ["auditor", "lead-auditor"])
    };

    public PolicyEvaluationResult Evaluate(PolicyEvaluationRequest request, string? tenantId)
    {
        var applicableRules = rules
            .Where(r => (string.IsNullOrWhiteSpace(tenantId) || r.TenantId == tenantId) &&
                       r.ResourceType == request.ResourceType &&
                       r.Action == request.Action)
            .ToList();

        if (applicableRules.Count == 0)
        {
            return new PolicyEvaluationResult(false, "No applicable policy rules found.", []);
        }

        var allowedRoles = applicableRules.SelectMany(r => r.AllowedRoles).Distinct().ToArray();
        var hasPermission = request.UserRoles.Any(role => allowedRoles.Contains(role));

        return new PolicyEvaluationResult(hasPermission, hasPermission ? "Access granted." : "Access denied.", allowedRoles);
    }
}

public sealed record PolicyRule(string PolicyId, string TenantId, string ResourceType, string Action, string[] AllowedRoles);

public sealed record PolicyEvaluationRequest(string ResourceType, string Action, string[] UserRoles);

public sealed record PolicyEvaluationResult(bool Allowed, string Reason, string[] RequiredRoles);