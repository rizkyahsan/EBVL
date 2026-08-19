using EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.GetUsers;

[AuthorizeRequest]
public sealed record GetUsersQuery : IRequest<GetUsersResponse>
{
}

public sealed class GetUsersQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var picCounts = await databaseService.Users
            .Where(x => !x.IsDeleted && x.IsPicLender)
            .GroupBy(x => x.LenderId)
            .Select(g => new
            {
                LenderId = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.LenderId, x => x.Count, cancellationToken);

        var users = await databaseService.Users
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Modified ?? x.Created)
            .Select(x => new UserItem
            {
                Id = x.Id,
                LenderId = x.LenderId,
                LenderName = x.Lender.Name,
                Username = x.Username,
                Name = x.DisplayName,
                Lender = x.Lender.Name,
                FullPhoneNumber = $"{x.PhoneCode} {x.PhoneNumber}",
                PhoneCode = x.PhoneCode ?? string.Empty,
                PhoneNumber = x.PhoneNumber ?? string.Empty,
                EmailAddress = x.EmailAddress,
                IsVerified = x.IsVerified,
                IsPicLender = x.IsPicLender
            })
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.CountPicLender = picCounts.GetValueOrDefault(user.LenderId, 0);
        }

        return new GetUsersResponse
        {
            Items = users
        };
    }
}
