using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Endpoints.FishDataEndpoint.Dto;

namespace WebApp.Endpoints.FishDataEndpoint;

public class UpdateFishData : Ep
    .Req<FishDataDto>
    .Res<Results<
        Ok<FishDataDto>,
        BadRequest<string>
    >>
{
    private readonly AppDbContext _db;

    public UpdateFishData(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Put("/fish-data/{id}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<FishDataDto>, BadRequest<string>>> ExecuteAsync(FishDataDto req,
        CancellationToken ct)
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString();
        if (id == null)
        {
            return TypedResults.BadRequest("ID is missing in the route.");
        }

        var fishData = await _db.FishData.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (fishData == null)
        {
            return TypedResults.BadRequest("Fish data with this ID does not exist.");
        }

        fishData.FishCode = req.FishCode;
        fishData.Name = req.Name;
        await _db.SaveChangesAsync(ct);

        return TypedResults.Ok(req);
    }
}