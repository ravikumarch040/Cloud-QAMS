namespace Qams.AiAssistant.Api;

public sealed class AiAssistantStore
{
    private readonly object syncRoot = new();
    private readonly List<AssistantPrompt> prompts = new()
    {
        new("prompt-001", "tenant-life-sciences-demo", "Summarize quality issues and suggest next steps."),
        new("prompt-002", "tenant-life-sciences-demo", "Draft a CAPA action plan based on supplier nonconformance.")
    };

    public IReadOnlyCollection<AssistantPrompt> ListPrompts(string? tenantId)
    {
        lock (syncRoot)
        {
            return prompts
                .Where(prompt => string.IsNullOrWhiteSpace(tenantId) || prompt.TenantId == tenantId)
                .ToArray();
        }
    }

    public AssistantResponse CreateResponse(AssistantRequest request, string responseId, string correlationId)
    {
        lock (syncRoot)
        {
            var responseText = $"Assistant response for '{request.PromptText}'. Review and validate before action.";
            var response = new AssistantResponse(
                responseId,
                request.TenantId,
                request.UserId,
                request.PromptText,
                responseText,
                DateTimeOffset.UtcNow,
                correlationId);

            return response;
        }
    }
}

public sealed record AssistantPrompt(
    string PromptId,
    string TenantId,
    string PromptText);

public sealed record AssistantRequest(
    string TenantId,
    string UserId,
    string PromptText);

public sealed record AssistantResponse(
    string ResponseId,
    string TenantId,
    string UserId,
    string PromptText,
    string ResponseText,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);
