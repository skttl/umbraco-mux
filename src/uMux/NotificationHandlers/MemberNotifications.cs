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
        IDataTypeService dataTypeService,
        IMuxService muxService,
        MediaFileManager mediaFileManager
    )
        : base(dataTypeService, muxService, mediaFileManager) { }

    public Task HandleAsync(
        MemberDeletedNotification notification,
        CancellationToken cancellationToken
    ) => Task.WhenAll(notification.DeletedEntities.Select(DeleteSyncedUploadFilesFromMux));

    public async Task HandleAsync(
        MemberSavingNotification notification,
        CancellationToken cancellationToken
    ) => await Task.WhenAll(notification.SavedEntities.Select(SyncUploadFilesToMux));
}
