using System.Text.Json;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using uMux.Models;
using uMux.Services;

namespace uMux.NotificationHandlers;

public abstract class NotificationHandlerBase
{
    private readonly IDataTypeService _dataTypeService;
    private readonly IMuxService _muxService;
    private readonly MediaFileManager _mediaFileManager;

    public NotificationHandlerBase(
        IDataTypeService dataTypeService,
        IMuxService muxService,
        MediaFileManager mediaFileManager
    )
    {
        _dataTypeService = dataTypeService;
        _muxService = muxService;
        _mediaFileManager = mediaFileManager;
    }

    /// <summary>
    /// Syncs the upload files to mux if the content has any Mux sync properties
    /// </summary>
    /// <param name="node"></param>
    /// <returns>boolean indicating if any changes were made</returns>
    public async Task<bool> SyncUploadFilesToMux(IContentBase node)
    {
        var isUpdated = false;

        var muxSyncProperties = node.Properties.Where(x =>
            x.PropertyType.PropertyEditorAlias == Constants.PropertyEditorSchema
        );

        foreach (var muxSyncProperty in muxSyncProperties)
        {
            var dataType = await _dataTypeService.GetAsync(
                muxSyncProperty.PropertyType.DataTypeKey
            );

            if (
                dataType is null
                || dataType.ConfigurationData.TryGetValue(
                    Constants.UploadPropertyAlias,
                    out var value
                )
                    is false
                || value is not string uploadPropertyAlias
                || string.IsNullOrWhiteSpace(uploadPropertyAlias)
            )
            {
                continue;
            }

            var existingStringValue = node.GetValue<string>(muxSyncProperty.Alias);

            var existingValue =
                existingStringValue.IsNullOrWhiteSpace() is false
                && existingStringValue.StartsWith("{")
                    ? JsonSerializer.Deserialize<MuxValue>(existingStringValue)
                    : null;

            var canContinue =
                node.IsPropertyDirty(uploadPropertyAlias)
                || existingValue?.Src != node.GetValue<string>(uploadPropertyAlias);

            if (canContinue == false)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(existingValue?.MuxAssetId) is false)
            {
                _muxService.DeleteAsset(existingValue.MuxAssetId);
                node.SetValue(muxSyncProperty.Alias, null);
                isUpdated = true;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                var fileStream = _mediaFileManager.GetFile(node, out var _, uploadPropertyAlias);
                fileStream.CopyTo(ms);
                var byteArray = ms.ToArray();

                if (byteArray is not null && byteArray.Length > 0)
                {
                    var asset = await _muxService.CreateAsset(
                        byteArray,
                        node.Name,
                        node.CreatorId.ToString(),
                        node.GetUdi().ToString().EnsureEndsWith($"/{uploadPropertyAlias}")
                    );

                    node.SetValue(
                        muxSyncProperty.Alias,
                        JsonSerializer.Serialize(
                            new MuxValue()
                            {
                                MuxAssetId = asset?.Data.Id,
                                PlaybackId = asset?.Data.PlaybackIds.FirstOrDefault()?.Id,
                                Src = node.GetValue<string>(uploadPropertyAlias),
                            }
                        )
                    );

                    isUpdated = true;
                }
            }
        }

        return isUpdated;
    }

    /// <summary>
    /// Deletes the files from Mux if the content has any Mux sync properties
    /// </summary>
    /// <param name="node"></param>
    /// <returns>boolean indicating if any changes were made</returns>
    public async Task<bool> DeleteSyncedUploadFilesFromMux(IContentBase node)
    {
        var isUpdated = false;

        var muxSyncProperties = node.Properties.Where(x =>
            x.PropertyType.PropertyEditorAlias == Constants.PropertyEditorSchema
        );

        foreach (var muxSyncProperty in muxSyncProperties)
        {
            var dataType = await _dataTypeService.GetAsync(
                muxSyncProperty.PropertyType.DataTypeKey
            );

            var existingStringValue = node.GetValue<string>(muxSyncProperty.Alias);

            var existingValue =
                existingStringValue.IsNullOrWhiteSpace() is false
                && existingStringValue.StartsWith("{")
                    ? JsonSerializer.Deserialize<MuxValue>(existingStringValue)
                    : null;

            if (existingValue is null || existingValue.MuxAssetId.IsNullOrWhiteSpace())
            {
                continue;
            }

            _muxService.DeleteAsset(existingValue.MuxAssetId);
            node.SetValue(muxSyncProperty.Alias, null);
            isUpdated = true;
        }

        return isUpdated;
    }

    public static bool ResetMuxValuesWithoutDeleting(IContentBase node)
    {
        var isUpdated = false;

        var muxSyncProperties = node.Properties.Where(x =>
            x.PropertyType.PropertyEditorAlias == Constants.PropertyEditorSchema
        );

        foreach (var muxSyncProperty in muxSyncProperties)
        {
            node.SetValue(muxSyncProperty.Alias, null);
            isUpdated = isUpdated || node.IsPropertyDirty(muxSyncProperty.Alias);
        }

        return isUpdated;
    }
}
