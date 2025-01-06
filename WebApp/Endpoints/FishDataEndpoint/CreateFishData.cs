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
    }

    public override async Task<Results<Ok, BadRequest<string>>> ExecuteAsync(FishDataDto req, CancellationToken ct)
    {
        if (await _db.FishData.AnyAsync(x => x.Id == req.Id, ct))
        {
            return TypedResults.BadRequest("Fish data with this ID already exists.");
        }

        var fishData = new FishData
        {
            Id = req.Id,
            Name = req.Name
        };

        _db.FishData.Add(fishData);
        await _db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}