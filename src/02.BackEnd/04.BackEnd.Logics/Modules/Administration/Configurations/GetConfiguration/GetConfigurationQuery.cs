using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

namespace EBVL.BackEnd.Logics.Modules.Administration.Configurations.GetConfiguration;

public sealed record GetConfigurationQuery : GetConfigurationRequest, IRequest<GetConfigurationResponse>
{
}

public sealed class GetConfigurationQueryValidator : AbstractValidatorBase<GetConfigurationQuery>
{
    public GetConfigurationQueryValidator()
    {
        Include(new GetConfigurationRequestValidator());
    }
}

public sealed class GetConfigurationQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetConfigurationQuery, GetConfigurationResponse>
{
    public async Task<GetConfigurationResponse> Handle(GetConfigurationQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Configuration) && audit.EntityId == request.ConfigurationId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var configuration = await databaseService.Configurations
            .Where(x => !x.IsDeleted && x.Id == request.ConfigurationId)
            .Select(x => new ConfigurationItem
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, CommonDisplayTextFor.Id, request.ConfigurationId);

        return new GetConfigurationResponse
        {
            Item = configuration
        };
    }
}
