using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.FishSamplingEndpoint.Dto;

namespace WebApp.Endpoints.FishSamplingEndpoint;

public class GetFishSampling : Ep
    .Req<FishSamplingRequest>
    .Res<Results<
        Ok<List<FishSamplingDto>>,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    private readonly FakeDataService _fakeDataService;

    public GetFishSampling(AppDbContext db, FakeDataService fakeDataService)
    {
        _db = db;
        _fakeDataService = fakeDataService;
    }

    public override void Configure()
    {
        Get("/fish-sampling");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<List<FishSamplingDto>>, BadRequest<string>>> ExecuteAsync(
        FishSamplingRequest request,
        CancellationToken ct)
    {
        var startTimestamp = request.Start;
        var endTimestamp = request.End;

        // var queryable = _db.FishSamplings
        //     .Select(x => new FishSamplingDto
        //     {
        //         Timestamp = x.Timestamp,
        //         FishCode = x.FishCode,
        //         Weight = x.Weight
        //     });
        //

        var rawData = new EnumerableQuery<FishSamplingDto>(_fakeDataService.GetFishSamplings());
        var queryable = rawData.AsQueryable();

        if (startTimestamp.HasValue)
        {
            queryable = queryable.Where(x => x.Timestamp >= startTimestamp);
        }

        if (endTimestamp.HasValue)
        {
            queryable = queryable.Where(x => x.Timestamp < endTimestamp);
        }

        if (request.FishCodes != null && request.FishCodes.Any())
        {
            queryable = queryable.Where(x => request.FishCodes.Contains(x.FishCode));
        }

        // var result = await queryable.ToListAsync(cancellationToken: ct);
        var result = queryable.ToList();

        return TypedResults.Ok(result);
    }
}