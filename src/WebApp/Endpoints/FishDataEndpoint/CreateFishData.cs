using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.FishDataEndpoint.Dto;
using WebApp.Models;

namespace WebApp.Endpoints.FishDataEndpoint;

public class CreateFishData : Ep
    .Req<FishDataDto>
    .Res<Results<
        Ok,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public CreateFishData(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/fish-data");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, BadRequest<string>>> ExecuteAsync(FishDataDto req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.FishCode) ||
            string.IsNullOrWhiteSpace(req.Name))
        {
            return TypedResults.BadRequest("Fish code and name are required.");
        }

        if (await _db.FishData.AnyAsync(x => x.FishCode == req.FishCode, ct))
        {
            return TypedResults.BadRequest("Fish data with this FishCode already exists.");
        }

        var fishData = new FishData
        {
            Id = Guid.CreateVersion7().ToString(),
            FishCode = req.FishCode,
            Name = req.Name
        };

        _db.FishData.Add(fishData);
        await _db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}