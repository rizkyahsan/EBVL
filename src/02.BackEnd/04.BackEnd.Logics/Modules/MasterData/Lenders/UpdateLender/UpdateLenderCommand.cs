using EBVL.Shared.Dto.Modules.MasterData.Lenders.UpdateLender;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Lenders.UpdateLender;

[AuthorizeRequest]
public sealed record UpdateLenderCommand : UpdateLenderRequest, IRequest { }

public sealed class UpdateLenderCommandValidator : AbstractValidatorBase<UpdateLenderCommand>
{
    public UpdateLenderCommandValidator()
    {
        Include(new UpdateLenderRequestValidator());
    }
}

public sealed class UpdateLenderCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateLenderCommand>
{
    public async Task Handle(UpdateLenderCommand request, CancellationToken cancellationToken)
    {
        var lender = await databaseService.Lenders
            .Where(x => !x.IsDeleted && x.Id == request.LenderId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(LendersDisplayTextFor.Lender, CommonDisplayTextFor.Id, request.LenderId);

        var checkDuplicate = await databaseService.Lenders
            .Where(x => !x.IsDeleted && x.Name.Trim() == request.Name.Trim() && x.Id != request.LenderId)
            .AnyAsync(cancellationToken);

        if (checkDuplicate)
        {
            throw ExceptionFor.EntityAlreadyExists(LendersDisplayTextFor.Name, LendersDisplayTextFor.Name, request.Name);
        }

        lender.Name = request.Name;
        lender.Address = request.Address;
        lender.CountryId = request.CountryId;
        lender.PhoneNumber = request.PhoneNumber;
        lender.EmailAddress = request.EmailAddress;
        lender.Website = request.Website;

        _ = await databaseService.SaveAsync(nameof(UpdateLender), cancellationToken);
    }
}
