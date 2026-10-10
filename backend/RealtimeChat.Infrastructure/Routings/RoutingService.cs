using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Infrastructure.Routings;

public class RoutingService : IRoutingService
{
    public async Task<RouteEtaResult> GetEtaAsync(GeoCoordinate origin, GeoCoordinate destination, CancellationToken ct)
    {
        // TODO: Thực hiện gọi API bên ngoài để tính toán

        return new RouteEtaResult(10, 100);
    }
}