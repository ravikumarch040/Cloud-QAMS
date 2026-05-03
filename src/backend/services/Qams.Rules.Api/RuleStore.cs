namespace Qams.Rules.Api;

public sealed class RuleStore
{
    private readonly object syncRoot = new();
    private readonly List<RuleDefinition> rules = new()
    {
        new("rule-capa-close-notify", "tenant-life-sciences-demo", "CAPA Case", "CapaCase", "CapaClosed", "close", "Send notification when CAPA closes", "SendNotification", true, DateTimeOffset.UtcNow, "seed-correlation-1"),
        new("rule-capa-approval-check", "tenant-life-sciences-demo", "CAPA Approval", "CapaCase", "CapaApproval", "approve", "Require qa-manager role for CAPA approval", "RequireRole:qa-manager", true, DateTimeOffset.UtcNow, "seed-correlation-2"),
    };

    public IReadOnlyCollection<RuleDefinition> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return rules
                .Where(rule => string.IsNullOrWhiteSpace(tenantId) || rule.TenantId == tenantId)
                .OrderByDescending(rule => rule.CreatedAtUtc)
                .ToArray();
        }
    }

    public RuleDefinition Create(CreateRuleRequest request, string ruleId, string correlationId)
    {
        lock (syncRoot)
        {
            var rule = new RuleDefinition(
                ruleId,
                request.TenantId,
                request.Name,
                request.ResourceType,
                request.TriggerEvent,
                request.Action,
                request.Description,
                request.Effect,
                request.Enabled,
                DateTimeOffset.UtcNow,
                correlationId);

            rules.Add(rule);
            return rule;
        }
    }

    public RuleEvaluationResult Evaluate(RuleEvaluationRequest request)
    {
        lock (syncRoot)
        {
            var applicable = rules
                .Where(rule => rule.Enabled && rule.TenantId == request.TenantId &&
                    string.Equals(rule.ResourceType, request.ResourceType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(rule.Action, request.Action, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (applicable.Length == 0)
            {
                return new RuleEvaluationResult(request.ResourceType, request.Action, false, "No matching rules found.", Array.Empty<string>());
            }

            var matchingRules = new List<string>();
            var allowed = true;

            foreach (var rule in applicable)
            {
                matchingRules.Add(rule.RuleId);
                if (rule.Effect.StartsWith("RequireRole:", StringComparison.OrdinalIgnoreCase))
                {
                    var role = rule.Effect.Split(':', 2)[1];
                    if (!request.UserRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                    {
                        allowed = false;
                    }
                }
            }

            return new RuleEvaluationResult(
                request.ResourceType,
                request.Action,
                allowed,
                allowed ? "Rules evaluation passed." : "Rules evaluation denied.",
                matchingRules);
        }
    }

    public RuleTriggerResult Trigger(RuleTriggerRequest request)
    {
        lock (syncRoot)
        {
            var triggered = rules
                .Where(rule => rule.Enabled && rule.TenantId == request.TenantId &&
                    string.Equals(rule.ResourceType, request.ResourceType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(rule.TriggerEvent, request.TriggerEvent, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (triggered.Length == 0)
            {
                return new RuleTriggerResult(request.ResourceType, request.TriggerEvent, false, "No trigger rules matched.", Array.Empty<string>());
            }

            var actions = triggered.Select(rule => rule.Effect).ToArray();
            return new RuleTriggerResult(request.ResourceType, request.TriggerEvent, true, "Triggered automation actions.", actions);
        }
    }
}

public sealed record RuleDefinition(
    string RuleId,
    string TenantId,
    string Name,
    string ResourceType,
    string TriggerEvent,
    string Action,
    string Description,
    string Effect,
    bool Enabled,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record CreateRuleRequest(
    string TenantId,
    string Name,
    string ResourceType,
    string TriggerEvent,
    string Action,
    string Description,
    string Effect,
    bool Enabled);

public sealed record RuleEvaluationRequest(
    string TenantId,
    string ResourceType,
    string Action,
    string[] UserRoles,
    Dictionary<string, string>? Attributes);

public sealed record RuleEvaluationResult(
    string ResourceType,
    string Action,
    bool Allowed,
    string Reason,
    IReadOnlyCollection<string> MatchingRuleIds);

public sealed record RuleTriggerRequest(
    string TenantId,
    string ResourceType,
    string TriggerEvent,
    Dictionary<string, string>? Payload);

public sealed record RuleTriggerResult(
    string ResourceType,
    string TriggerEvent,
    bool Triggered,
    string Reason,
    IReadOnlyCollection<string> Actions);