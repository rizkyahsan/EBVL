using EBVL.Shared.Dto.Modules.Examples.Dummies.PostDummy;

namespace EBVL.BackEnd.Logics.Modules.Examples.Dummies.PostDummy;

//[AuthorizeRequest]
public sealed record PostDummyCommand : PostDummyRequest, IRequest<PostDummyResponse>
{
}

public sealed class PostDummyCommandValidator : AbstractValidatorBase<PostDummyCommand>
{
    public PostDummyCommandValidator()
    {
        Include(new PostDummyRequestValidator());
    }
}

public sealed class PostDummyCommandHandler(
    IDateAndTimeService dateAndTimeService)
    : IRequestHandler<PostDummyCommand, PostDummyResponse>
{
    public async Task<PostDummyResponse> Handle(PostDummyCommand request, CancellationToken cancellationToken)
    {
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

        return new PostDummyResponse
        {
            Items = items
        };
    }
}
