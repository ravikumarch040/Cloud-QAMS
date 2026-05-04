using Qams.Forms.Api;
using Qams.BuildingBlocks.Common;

namespace Qams.BuildingBlocks.Tests;

public class FormStoreTests
{
    [Fact]
    public void CreateStoresFormDefinitionAndGetReturnsIt()
    {
        var store = new FormStore();
        var request = new CreateFormDefinitionRequest(
            "tenant-demo",
            "Test Form",
            "1.0",
            "Active",
            new[]
            {
                new FormFieldDefinition("field-1", "Text Field", FormFieldType.Text, true),
                new FormFieldDefinition("field-2", "Number Field", FormFieldType.Number, false)
            });

        var created = store.Create(request, "form-123", "corr-1");

        Assert.Equal("form-123", created.FormDefinitionId);
        Assert.Equal("tenant-demo", created.TenantId);
        Assert.Equal(2, created.Fields.Count);

        var retrieved = store.Get("form-123", "tenant-demo");
        Assert.NotNull(retrieved);
        Assert.Equal(created.Name, retrieved!.Name);
    }

    [Fact]
    public void ValidateReturnsInvalidWhenRequiredFieldMissing()
    {
        var store = new FormStore();
        var request = new CreateFormDefinitionRequest(
            "tenant-demo",
            "Required Form",
            "1.0",
            "Active",
            new[]
            {
                new FormFieldDefinition("field-1", "Required Field", FormFieldType.Text, true)
            });

        store.Create(request, "form-req", "corr-2");

        var submission = new ValidateFormSubmissionRequest(
            "tenant-demo",
            "form-req",
            new Dictionary<string, object?>());

        var result = store.Validate("form-req", submission, "tenant-demo");

        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("field_required", result.Errors.First().Code);
    }

    [Fact]
    public void ValidateReturnsValidWhenAllRequiredFieldsPresent()
    {
        var store = new FormStore();
        var request = new CreateFormDefinitionRequest(
            "tenant-demo",
            "Complete Form",
            "1.0",
            "Active",
            new[]
            {
                new FormFieldDefinition("field-1", "Required Field", FormFieldType.Text, true),
                new FormFieldDefinition("field-2", "Number Field", FormFieldType.Number, false)
            });

        store.Create(request, "form-good", "corr-3");

        var submission = new ValidateFormSubmissionRequest(
            "tenant-demo",
            "form-good",
            new Dictionary<string, object?>
            {
                ["field-1"] = "value",
                ["field-2"] = "123"
            });

        var result = store.Validate("form-good", submission, "tenant-demo");

        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
