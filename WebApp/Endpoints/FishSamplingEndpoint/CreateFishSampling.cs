using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.FishSamplingEndpoint.Dto;
using WebApp.Models;

namespace WebApp.Endpoints.FishSamplingEndpoint;

public class CreateFishSampling : Ep
    .Req<FishSamplingDto>
    .Res<Results<
        Ok,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public CreateFishSampling(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/fish-sampling");
    }

    public override async Task<Results<Ok, BadRequest<string>>> ExecuteAsync(FishSamplingDto req, CancellationToken ct)
    {
        var fish = await _db.FishData.FirstOrDefaultAsync(x => x.Id == req.FishId, ct);
        if (fish == null)
        {
            return TypedResults.BadRequest("Fish with this ID does not exist.");
        }

        var fishSampling = new FishSampling
        {
            Id = Guid.CreateVersion7().ToString(),
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(req.Timestamp).ToUnixTimeMilliseconds(),
            FishId = req.FishId,
            Weight = req.Weight
        };

        _db.FishSamplings.Add(fishSampling);
        await _db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}