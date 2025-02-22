using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Endpoints.FishDataEndpoint;

public class DeleteFishData : EndpointWithoutRequest
{
    private readonly AppDbContext _db;

    public DeleteFishData(AppDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/fish-data/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("id");

        var data = await _db.FishData
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (data is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _db.FishData.Remove(data);
        await _db.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}