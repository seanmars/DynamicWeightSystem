using WebApp.Service.Dto;

namespace WebApp.Service;

public class DeviceService
{
    public async Task<List<DeviceWeightLevelDto>> GetWeightLevel(CancellationToken ct = default)
    {
        await Task.Delay(0, ct);
        return
        [
            new DeviceWeightLevelDto() { Level = 1, LowerBound = 0, UpperBound = 100 },
            new DeviceWeightLevelDto() { Level = 2, LowerBound = 101, UpperBound = 200 },
            new DeviceWeightLevelDto() { Level = 3, LowerBound = 201, UpperBound = 300 },
            new DeviceWeightLevelDto() { Level = 4, LowerBound = 301, UpperBound = 400 },
            new DeviceWeightLevelDto() { Level = 5, LowerBound = 401, UpperBound = 500 },
            new DeviceWeightLevelDto() { Level = 6, LowerBound = 501, UpperBound = 600 },
            new DeviceWeightLevelDto() { Level = 7, LowerBound = 601, UpperBound = 700 }
        ];
    }
}