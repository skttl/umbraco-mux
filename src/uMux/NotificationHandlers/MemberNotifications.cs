using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using uMux.Services;

namespace uMux.NotificationHandlers;

public class MemberNotifications
    : NotificationHandlerBase,
        INotificationAsyncHandler<MemberDeletedNotification>,
        INotificationAsyncHandler<MemberSavingNotification>
{
    public MemberNotifications(
        ILogger<MemberNotifications> logger,
        IDataTypeService dataTypeService,
        IMuxService muxService,
        MediaFileManager mediaFileManager
    )
        : base(logger, dataTypeService, muxService, mediaFileManager) { }

    public Task HandleAsync(
        MemberDeletedNotification notification,
        CancellationToken cancellationToken
    ) => Task.WhenAll(notification.DeletedEntities.Select(TryDeleteSyncedUploadFilesFromMux));

    public async Task HandleAsync(
        MemberSavingNotification notification,
        CancellationToken cancellationToken
    ) => await Task.WhenAll(notification.SavedEntities.Select(TrySyncUploadFilesToMux));
}
