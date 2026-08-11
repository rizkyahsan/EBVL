using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

namespace EBVL.BackEnd.Logics.Modules.Administration.Configurations.GetConfigurations;

public sealed record GetConfigurationsQuery : IRequest<GetConfigurationsResponse>
{
}

public sealed class GetConfigurationsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetConfigurationsQuery, GetConfigurationsResponse>
{
    public async Task<GetConfigurationsResponse> Handle(GetConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var configurations = await databaseService.Configurations
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Key)
            .Select(x => new ConfigurationItem
            {
                Id = x.Id,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Key = x.Key,
                Value = x.Value
            })
            .ToListAsync(cancellationToken);

        return new GetConfigurationsResponse
        {
            Items = configurations
        };
    }
}
