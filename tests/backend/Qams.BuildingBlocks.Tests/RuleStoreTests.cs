using Qams.Rules.Api;

namespace Qams.BuildingBlocks.Tests;

public class RuleStoreTests
{
    private static readonly string[] NoRoles = Array.Empty<string>();
    private static readonly string[] QaManagerRoles = new[] { "qa-manager" };

    [Fact]
    public void CreateAddsRuleAndListIncludesRule()
    {
        var store = new RuleStore();
        var request = new CreateRuleRequest(
            "tenant-demo",
            "Approval Rule",
            "CapaCase",
            "CapaApproval",
            "approve",
            "Require sign-off",
            "RequireRole:qa-manager",
            true);

        var rule = store.Create(request, "rule-100", "corr-1");

        Assert.Equal("rule-100", rule.RuleId);
        Assert.Equal("tenant-demo", rule.TenantId);
        Assert.Contains(store.List("tenant-demo"), candidate => candidate.RuleId == "rule-100");
    }

    [Fact]
    public void EvaluateDeniesWhenRoleMissing()
    {
        var store = new RuleStore();
        store.Create(new CreateRuleRequest(
            "tenant-demo",
            "Approval Rule",
            "CapaCase",
            "CapaApproval",
            "approve",
            "Require sign-off",
            "RequireRole:qa-manager",
            true),
            "rule-200",
            "corr-2");

        var result = store.Evaluate(new RuleEvaluationRequest(
            "tenant-demo",
            "CapaCase",
            "approve",
            NoRoles,
            null));

        Assert.False(result.Allowed);
        Assert.Equal("Rules evaluation denied.", result.Reason);
    }

    [Fact]
    public void EvaluateAllowsWhenRequiredRolePresent()
    {
        var store = new RuleStore();
        store.Create(new CreateRuleRequest(
            "tenant-demo",
            "Approval Rule",
            "CapaCase",
            "CapaApproval",
            "approve",
            "Require sign-off",
            "RequireRole:qa-manager",
            true),
            "rule-201",
            "corr-3");

        var result = store.Evaluate(new RuleEvaluationRequest(
            "tenant-demo",
            "CapaCase",
            "approve",
            QaManagerRoles,
            null));

        Assert.True(result.Allowed);
        Assert.Equal("Rules evaluation passed.", result.Reason);
    }

    [Fact]
    public void TriggerReturnsNoMatchWhenNoTriggerRules()
    {
        var store = new RuleStore();

        var result = store.Trigger(new RuleTriggerRequest(
            "tenant-demo",
            "CapaCase",
            "CapaClosed",
            null));

        Assert.False(result.Triggered);
        Assert.Equal("No trigger rules matched.", result.Reason);
    }
}
