using System.Text.Json;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class VendorRegistrationReview
{
    #region Parameters

    [Parameter]
    public required PreRegistrationRequest PreRegistration { get; init; }

    [Parameter]
    public required QuestionnaireRuntimeResponse Runtime { get; init; }

    [Parameter]
    public required EventCallback<VendorRegistrationDocumentItem> OnDownloadDocument { get; init; }
    [Parameter] public required EventCallback<RuntimeFileItem> OnDownloadFile { get; init; }

    [Parameter]
    public required EventCallback OnSendForVerification { get; init; }

    [Parameter]
    public required EventCallback OnPrevious { get; init; }

    #endregion

    #region Fields

    private bool _isConfirmed;

    #endregion

    #region Private Methods

    private string GetBrands()
    {
        return string.Join(", ", new[] { PreRegistration.BrandRepresentative }.Concat(PreRegistration.AdditionalBrands).Where(brand => !string.IsNullOrWhiteSpace(brand)));
    }

    private static string GetAnswer(RuntimeQuestionItem question)
    {
        var answer = question.Answer;
        if (answer?.OptionIds?.Count > 0)
        {
            var selected = answer.OptionIds.ToHashSet();
            return string.Join(", ", question.Options.Where(option => selected.Contains(option.Id)).Select(option => option.Label));
        }

        if (!string.IsNullOrWhiteSpace(answer?.AddressJson))
        {
            try
            {
                var address = JsonSerializer.Deserialize<QuestionnaireAddressRequest>(answer.AddressJson);
                return address is null ? "-" : string.Join(", ", new[] { address.Building, address.Street, address.Number, address.City, address.Country }.Where(value => !string.IsNullOrWhiteSpace(value)));
            }
            catch (JsonException)
            {
                return "-";
            }
        }

        return answer?.TextValue ?? answer?.IntegerValue?.ToString() ?? answer?.DecimalValue?.ToString() ?? answer?.DateValue?.ToString() ?? answer?.BooleanValue?.ToString() ?? (question.Files.Count > 0 ? question.Files[0].FileName : "-");
    }

    #endregion
}
