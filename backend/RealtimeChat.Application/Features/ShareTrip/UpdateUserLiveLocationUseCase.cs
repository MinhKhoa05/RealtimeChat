using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.ShareTrip;

public class UpdateUserLiveLocationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;

    public UpdateUserLiveLocationUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(CreateShareTripRequest request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var shareTripSessions = await _context.ShareTripSessions
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .Where(x => x.OwnerId == currentUserId)
            .ToListAsync(ct);

        if (shareTripSessions.Count == 0)
        {
            throw new NotFoundException("Không có trip session nào đang hoạt động");
        }

        // Cập nhật location hiện tại
        var userLiveLocation = await _context.UserLiveLocations
            .FindAsync(currentUserId, ct)
            ?? throw new NotFoundException();

        userLiveLocation.UpdateLocation(request.CurrentLatitude, request.CurrentLongitude, request.AccuracyMeters, request.RecordedAt, _timeProvider.GetUtcNow().UtcDateTime);
        await _context.SaveChangesAsync(ct);

        foreach (var shareTripSession in shareTripSessions)
        {
            await _notifier.NotifyToConversationAsync(shareTripSession.ConversationId, "location_update", userLiveLocation, ct);
        }

        // Tính toán ETA.
        // foreach (var session in shareTripSessions)
        // {
        //     // Kiểm tra có đến lúc tính lại ETA chưa.
        //     if (!_tripEtaPolicy.ShouldRecalculateEta(session, latitude, longitude, now))
        //     {
        //         continue;
        //     }

        //     try
        //     {
        //         // Gọi Routing API để tính ETA.
        //         var etaMinutes = await _tripEtaProvider.GetEtaMinutesAsync(latitude, longitude, session.DestinationLatitude, session.DestinationLongitude, ct);

        //         // Chỉ cập nhật khi Routing API trả về kết quả hợp lệ.
        //         if (etaMinutes is null)
        //             continue;

        //         session.UpdateEta(etaMinutes.Value, latitude, longitude, _timeProvider.GetUtcNow().UtcDateTime);

        //         await _context.SaveChangesAsync(ct);

        //         // Chỉ thông báo ETA sau khi đã lưu thành công.
        //         await _notifier.NotifyToConversationAsync(session.ConversationId, "share_trip_eta_updated",
        //             new ShareTripEtaUpdatedResponse
        //             {
        //                 SessionId = session.Id,
        //                 EtaMinutes = session.EtaMinutes,
        //                 EtaCalculatedAt = session.EtaCalculatedAt
        //             },
        //             ct);
        //     }
        //     catch (RoutingApiException ex)
        //     {
        //         // Ghi log lỗi, giữ nguyên ETA gần nhất.
        //         // Không cập nhật EtaBaseLatitude/Longitude
        //         // hoặc EtaCalculatedAt nếu tính ETA thất bại.
        //         _logger.LogWarning(ex, "Failed to calculate ETA for session {SessionId}", session.Id);
        //     }
        // }
    }
}