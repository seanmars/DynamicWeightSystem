using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.FishDataEndpoint.Dto;

namespace WebApp.Endpoints.FishDataEndpoint;

public class GetFishData : Ep
    .NoReq
    .Res<Results<
        Ok<List<FishDataDto>>,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public GetFishData(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/fish-data");
    }

    public override async Task<Results<Ok<List<FishDataDto>>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        var result = await _db.FishData
            .Select(x => new FishDataDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken: ct);

        return TypedResults.Ok(result);
    }
}