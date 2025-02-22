using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Endpoints.WeightLevelEndpoint;

public class DeleteWeightLevel : Ep
    .Req<List<int>>
    .Res<Results<
        Ok,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public DeleteWeightLevel(AppDbContext db, FakeDataService fakeDataService)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/weight-level");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, BadRequest<string>>> ExecuteAsync(
        List<int> weightLevelIds,
        CancellationToken ct)
    {
        var entities = await _db.WeightLevels
            .AsNoTracking()
            .Where(x => weightLevelIds.Contains(x.Level))
            .ToListAsync(cancellationToken: ct);

        _db.WeightLevels.RemoveRange(entities);

        await _db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}