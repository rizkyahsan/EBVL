using EBVL.Shared.Dto.Modules.MasterData.Lenders.AddLender;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Lenders.AddLender;

[AuthorizeRequest]
public sealed record AddLenderCommand : AddLenderRequest, IRequest<AddLenderResponse> { }

public sealed class AddLenderCommandValidator : AbstractValidatorBase<AddLenderCommand>
{
    public AddLenderCommandValidator()
    {
        Include(new AddLenderRequestValidator());
    }
}

public sealed class AddLenderCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<AddLenderCommand, AddLenderResponse>
{
    public async Task<AddLenderResponse> Handle(AddLenderCommand request, CancellationToken cancellationToken)
    {
        var checkDuplicate = await databaseService.Lenders
            .Where(x => !x.IsDeleted && x.Name.Trim() == request.Name.Trim())
            .AnyAsync(cancellationToken);

        if (checkDuplicate)
        {
            throw ExceptionFor.EntityAlreadyExists(LendersDisplayTextFor.Name, LendersDisplayTextFor.Name, request.Name);
        }

        var lender = new Lender
        {
            Name = request.Name,
            Address = request.Address,
            CountryId = request.CountryId,
            PhoneNumber = request.PhoneNumber,
            EmailAddress = request.EmailAddress,
            Website = request.Website
        };

        _ = await databaseService.Lenders.AddAsync(lender, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddLender), cancellationToken);

        return new AddLenderResponse
        {
            Item = new LenderItem
            {
                Id = lender.Id
            }
        };
    }
}
