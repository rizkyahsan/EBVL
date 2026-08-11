using Microsoft.Extensions.Logging;
using EBVL.Shared.Dto.Modules.Examples.Dummies.GetDummies;

namespace EBVL.BackEnd.Logics.Modules.Examples.Dummies.GetDummies;

public sealed record GetDummiesQuery : GetDummiesRequest, IRequest<GetDummiesResponse>
{
}

public sealed class GetDummiesQueryValidator : AbstractValidatorBase<GetDummiesQuery>
{
    public GetDummiesQueryValidator()
    {
        Include(new GetDummiesRequestValidator());
    }
}

public sealed class GetDummiesQueryHandler(
    ILogger<GetDummiesQueryHandler> logger,
    IDateAndTimeService dateAndTimeService)
    : IRequestHandler<GetDummiesQuery, GetDummiesResponse>
{
    public async Task<GetDummiesResponse> Handle(GetDummiesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("SuatuDateOnly: {SuatuDateOnly}", $"{request.SuatuDateOnly:d MMMM yyyy}");
        logger.LogInformation("SuatuDateTime: {SuatuDateTime}", $"{request.SuatuDateTime:d MMMM yyyy HH:mm:ss}");
        logger.LogInformation("SuatuDateTimeOffset: {SuatuDateTimeOffset}", $"{request.SuatuDateTimeOffset:d MMMM yyyy HH:mm:ss zzz}");

        var waktuSekarang = dateAndTimeService.Now.ToDisplayText(DateTimeFormatFor.LongDateTime);
        var items = new List<DummyItem>();

        for (var i = 1; i <= request.Angka2; i++)
        {
            var angka1 = request.Angka1 + i;
            var angka2 = request.Angka2 - i;
            var hasil = angka1 + angka2;

            var dummyItem = new DummyItem
            {
                Iterasi = i,
                Angka1 = angka1,
                Angka2 = angka2,
                Hasil = hasil,
                Keterangan = $"Hasil Penjumlahan: {hasil} pada waktu {waktuSekarang}",
                SuatuDateOnly = request.SuatuDateOnly.AddDays(i),
                SuatuDateTime = request.SuatuDateTime.AddDays(i + 1),
                SuatuDateTimeOffset = request.SuatuDateTimeOffset.AddDays(i + 2)
            };

            items.Add(dummyItem);
        }

        await Task.CompletedTask;

        return new GetDummiesResponse
        {
            Items = items
        };
    }
}
