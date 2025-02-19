using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.WeightLevelEndpoint.Dto;

namespace WebApp.Endpoints.WeightLevelEndpoint;

public class GetWeightLevel : Ep
    .NoReq
    .Res<Results<
        Ok<List<WeightLevelDto>>,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public GetWeightLevel(AppDbContext db, FakeDataService fakeDataService)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/weight-level");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<List<WeightLevelDto>>, BadRequest<string>>> ExecuteAsync(
        CancellationToken ct)
    {
        var result = await _db.WeightLevels
            .Select(x => new WeightLevelDto
            {
                Level = x.Level,
                LowerBound = x.LowerBound,
                UpperBound = x.UpperBound
            })
            .ToListAsync(cancellationToken: ct);

        return TypedResults.Ok(result);
    }
}