using Bogus;
using WebApp.Endpoints.FishSamplingEndpoint.Dto;

namespace WebApp;

public class FakeDataService
{
    public List<FishSamplingDto> GetFishSamplings(int count = 100)
    {
        List<FishSamplingDto> dataset = new();

        // use bogus data for now
        var faker = new Faker<FishSamplingDto>()
            .RuleFor(x => x.Timestamp, f =>
            {
                var now = DateTimeOffset.Now;
                var pastSecond = f.Random.Number(0, 60 * 60 * 24 * 3);
                var date = now.AddSeconds(-pastSecond);
                return date.ToUnixTimeSeconds();
            })
            .RuleFor(x => x.FishCode, f => f.Random.ArrayElement(["FishA", "FishB"]))
            .RuleFor(x => x.Weight, f => f.Random.Number(0, 1400));

        for (var i = 0; i < count; i++)
        {
            dataset.Add(faker.Generate());
        }

        dataset = dataset.OrderBy(x => x.Timestamp).ToList();

        return dataset;
    }
}