using EBVL.FrontEnd.Logics.Modules.Main.BrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistrations;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Services;

public sealed class BrandRegistrationState(ISender sender)
{
    private readonly List<BrandRegisterItem> _items = [];

    public IReadOnlyList<BrandRegisterItem> Items => _items;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var response = await sender.Send(new GetBrandRegistrationsQuery(), cancellationToken);
        _items.Clear();
        _items.AddRange(response.Items.Select(Map));
    }

    public async Task<BrandRegisterItem> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await sender.Send(new GetBrandRegistrationQuery(id), cancellationToken);
        var item = Map(response);
        Replace(item);
        return item;
    }

    public async Task<BrandRegisterItem> AddAsync(CreateBrandRegistrationRequest request, bool submit, CancellationToken cancellationToken = default)
    {
        var response = await sender.Send(new CreateBrandRegistrationCommand(request with { Submit = submit }), cancellationToken);
        var item = Map(response);
        Replace(item);
        return item;
    }

    public async Task<BrandRegisterItem> SaveInvitationAsync(Guid registrationId, string subject, string recipient, string cc, string body, string rowVersion, CancellationToken cancellationToken = default)
    {
        var response = await sender.Send(new SendBrandInvitationCommand(registrationId,
            new(subject, recipient, cc, body, rowVersion)), cancellationToken);
        var item = Map(response);
        Replace(item);
        return item;
    }

    public async Task<BrandRegisterItem> SetInvitationResponseAsync(Guid registrationId, Guid invitationId, BrandInvitationResponse response, string rowVersion, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new RespondBrandInvitationCommand(registrationId, invitationId,
            new(response, rowVersion)), cancellationToken);
        var item = Map(result);
        Replace(item);
        return item;
    }

    private void Replace(BrandRegisterItem item)
    {
        _ = _items.RemoveAll(x => x.Id == item.Id);
        _items.Insert(0, item);
    }

    private static BrandRegisterItem Map(BrandRegistrationListItem item)
    {
        return new(item.Id, item.ProductName, item.BrandName, item.Group, item.Status.ToString(),
            item.SubmittedAt ?? item.LastUpdatedAt, item.LastUpdatedAt, string.Empty, string.Empty, string.Empty, string.Empty, null);
    }

    private static BrandRegisterItem Map(BrandRegistrationItem item)
    {
        var invitation = item.Invitation is null ? null : new BrandInvitation(item.Invitation.Id, item.Invitation.Subject,
            item.Invitation.Recipient, item.Invitation.Cc, item.Invitation.Body, item.Invitation.Response?.ToString());
        return new(item.Id, item.ProductName, item.BrandName, item.Group, item.Status.ToString(),
            item.SubmittedAt ?? item.LastUpdatedAt, item.LastUpdatedAt, item.FactoryCountry, item.Category,
            item.ProductDescription, item.RowVersion, invitation);
    }
}

public sealed record BrandRegisterItem(Guid Id, string Product, string Brand, string Group, string Status,
    DateTimeOffset SubmittedAt, DateTimeOffset LastUpdatedAt, string Country, string Category,
    string ProductDescription, string RowVersion, BrandInvitation? Invitation);

public sealed record BrandInvitation(Guid Id, string Subject, string Recipient, string Cc, string Body, string? Response = null);
