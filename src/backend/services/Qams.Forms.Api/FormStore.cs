using Qams.BuildingBlocks.Common;

namespace Qams.Forms.Api;

public sealed class FormStore
{
    private readonly object syncRoot = new();
    private readonly List<FormDefinition> definitions = [];

    public IReadOnlyCollection<FormDefinition> List(string? tenantId)
    {
        lock (syncRoot)
        {
            return definitions
                .Where(def => string.IsNullOrWhiteSpace(tenantId) || def.TenantId == tenantId)
                .OrderByDescending(def => def.CreatedAtUtc)
                .ToArray();
        }
    }

    public FormDefinition? Get(string formDefinitionId, string? tenantId)
    {
        lock (syncRoot)
        {
            return definitions.FirstOrDefault(def => def.FormDefinitionId == formDefinitionId &&
                (string.IsNullOrWhiteSpace(tenantId) || def.TenantId == tenantId));
        }
    }

    public FormDefinition Create(CreateFormDefinitionRequest request, string formDefinitionId, string correlationId)
    {
        lock (syncRoot)
        {
            var definition = new FormDefinition(
                formDefinitionId,
                request.TenantId,
                request.Name,
                request.Version,
                request.Status,
                request.Fields.ToArray(),
                DateTimeOffset.UtcNow,
                correlationId);

            definitions.Add(definition);
            return definition;
        }
    }

    public FormValidationResult? Validate(string formDefinitionId, ValidateFormSubmissionRequest request, string? tenantId)
    {
        lock (syncRoot)
        {
            var definition = definitions.FirstOrDefault(def => def.FormDefinitionId == formDefinitionId &&
                (string.IsNullOrWhiteSpace(tenantId) || def.TenantId == tenantId));

            if (definition is null)
            {
                return null;
            }

            var errors = new List<ApiError>();
            var values = request.Values ?? new Dictionary<string, object?>();

            foreach (var field in definition.Fields)
            {
                values.TryGetValue(field.FieldId, out var value);
                var valueText = value?.ToString();

                if (field.Required && string.IsNullOrWhiteSpace(valueText))
                {
                    errors.Add(new ApiError("field_required", $"Field '{field.Label}' is required.", field.FieldId));
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(valueText) && field.FieldType == FormFieldType.Number)
                {
                    if (!decimal.TryParse(valueText, out _))
                    {
                        errors.Add(new ApiError("field_invalid", $"Field '{field.Label}' must be a number.", field.FieldId));
                    }
                }
            }

            return new FormValidationResult(definition.FormDefinitionId, errors.Count == 0, errors);
        }
    }
}

public sealed record FormDefinition(
    string FormDefinitionId,
    string TenantId,
    string Name,
    string Version,
    string Status,
    IReadOnlyCollection<FormFieldDefinition> Fields,
    DateTimeOffset CreatedAtUtc,
    string CorrelationId);

public sealed record FormFieldDefinition(
    string FieldId,
    string Label,
    FormFieldType FieldType,
    bool Required,
    string? HelpText = null,
    IReadOnlyCollection<string>? Options = null);

public sealed record CreateFormDefinitionRequest(
    string TenantId,
    string Name,
    string Version,
    string Status,
    IReadOnlyCollection<FormFieldDefinition> Fields);

public sealed record ValidateFormSubmissionRequest(
    string TenantId,
    string FormDefinitionId,
    Dictionary<string, object?>? Values);

public sealed record FormValidationResult(
    string FormDefinitionId,
    bool IsValid,
    IReadOnlyCollection<ApiError> Errors);

public enum FormFieldType
{
    Text,
    Number,
    Date,
    Select,
    Checkbox
}