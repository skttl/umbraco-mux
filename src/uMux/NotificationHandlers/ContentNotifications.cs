using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using uMux.Services;

namespace uMux.NotificationHandlers;

public class ContentNotifications
    : NotificationHandlerBase,
        INotificationHandler<ContentCopyingNotification>,
        INotificationAsyncHandler<ContentDeletedBlueprintNotification>,
        INotificationAsyncHandler<ContentDeletedNotification>,
        INotificationHandler<ContentSavedBlueprintNotification>,
        INotificationAsyncHandler<ContentSavingNotification>
{
    private readonly IContentService _contentService;

    public ContentNotifications(
        IContentService contentService,
        IDataTypeService dataTypeService,
        IMuxService muxService,
        MediaFileManager mediaFileManager
    )
        : base(dataTypeService, muxService, mediaFileManager)
    {
        _contentService = contentService;
    }

    public void Handle(ContentCopyingNotification notification) =>
        ResetMuxValuesWithoutDeleting(notification.Copy);

    public async Task HandleAsync(
        ContentDeletedBlueprintNotification notification,
        CancellationToken cancellationToken
    ) => await Task.WhenAll(notification.DeletedBlueprints.Select(DeleteSyncedUploadFilesFromMux));

    public async Task HandleAsync(
        ContentDeletedNotification notification,
        CancellationToken cancellationToken
    ) => await Task.WhenAll(notification.DeletedEntities.Select(DeleteSyncedUploadFilesFromMux));

    public void Handle(ContentSavedBlueprintNotification notification)
    {
        if (ResetMuxValuesWithoutDeleting(notification.SavedBlueprint))
        {
            _contentService.SaveBlueprint(
                notification.SavedBlueprint,
                notification.CreatedFromContent
            );
        }
    }

    public async Task HandleAsync(
        ContentSavingNotification notification,
        CancellationToken cancellationToken
    ) => await Task.WhenAll(notification.SavedEntities.Select(SyncUploadFilesToMux));
}
