using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Application.Interfaces;

public interface IRoutingService
{
    Task<RouteEtaResult> GetEtaAsync(GeoCoordinate origin, GeoCoordinate destination, CancellationToken ct);
}

public record RouteEtaResult(int EtaMinutes, double DistanceMeters);