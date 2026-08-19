using EBVL.Shared.Dto.Modules.MasterData.Lenders.DeleteLender;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Lenders.DeleteLender;

[AuthorizeRequest]
public sealed record DeleteLenderCommand : DeleteLenderRequest, IRequest { }

public sealed class DeleteLenderCommandValidator : AbstractValidatorBase<DeleteLenderCommand>
{
    public DeleteLenderCommandValidator()
    {
        Include(new DeleteLenderRequestValidator());
    }
}

public sealed class DeleteLenderCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<DeleteLenderCommand>
{
    public async Task Handle(DeleteLenderCommand request, CancellationToken cancellationToken)
    {
        var lender = await databaseService.Lenders
            .Where(x => !x.IsDeleted && x.Id == request.LenderId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(LendersDisplayTextFor.Lender, CommonDisplayTextFor.Id, request.LenderId);

        lender.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteLender), cancellationToken);
    }
}
