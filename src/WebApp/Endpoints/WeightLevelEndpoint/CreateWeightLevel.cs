using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.WeightLevelEndpoint.Dto;
using WebApp.Models;

namespace WebApp.Endpoints.WeightLevelEndpoint;

public class CreateWeightLevel : Ep
    .Req<List<WeightLevelDto>>
    .Res<Results<
        Ok<List<WeightLevelDto>>,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public CreateWeightLevel(AppDbContext db, FakeDataService fakeDataService)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/weight-level");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<List<WeightLevelDto>>, BadRequest<string>>> ExecuteAsync(
        List<WeightLevelDto> weightLevels,
        CancellationToken ct)
    {
        var entities = weightLevels.Select(x => new WeightLevel
        {
            Level = x.Level,
            LowerBound = x.LowerBound,
            UpperBound = x.UpperBound
        }).ToList();

        await _db.UpsertWeightLevel(entities, ct);

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