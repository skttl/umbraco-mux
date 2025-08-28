using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Mux.Csharp.Sdk.Api;
using Mux.Csharp.Sdk.Model;
using uMux.Configuration;

namespace uMux.Services;

public class MuxService : IMuxService
{
    private MuxSettings _settings;
    private readonly AssetsApi _assetsApi;
    private readonly DirectUploadsApi _directUploadsApi;
    private readonly IHttpClientFactory _httpClientFactory;

    public MuxService(IOptionsMonitor<MuxSettings> options, IHttpClientFactory httpClientFactory)
    {
        _settings = options.CurrentValue;
        options.OnChange(settings => _settings = settings);

        var config = GetConfiguration();
        _assetsApi = new AssetsApi(config);
        _directUploadsApi = new DirectUploadsApi(config);
        _httpClientFactory = httpClientFactory;
    }

    private Mux.Csharp.Sdk.Client.Configuration GetConfiguration()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_settings.ApiBasePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(_settings.ApiTokenId);
        ArgumentException.ThrowIfNullOrWhiteSpace(_settings.ApiSecret);

        return new Mux.Csharp.Sdk.Client.Configuration()
        {
            BasePath = _settings.ApiBasePath,
            Username = _settings.ApiTokenId,
            Password = _settings.ApiSecret,
        };
    }

    public async Task<AssetResponse> GetAsset(
        string assetId,
        CancellationToken cancellationToken = default
    )
    {
        return await _assetsApi.GetAssetAsync(assetId, cancellationToken);
    }

    public async Task<AssetResponse> CreateAsset(
        byte[] bytes,
        string? title = null,
        string? creatorId = null,
        string? externalId = null,
        CancellationToken cancellationToken = default
    )
    {
        var createRequest = new CreateUploadRequest
        {
            Test = _settings.TestModeEnabled,
            NewAssetSettings = new CreateAssetRequest(
                playbackPolicies: new List<PlaybackPolicy> { _settings.PlaybackPolicy },
                videoQuality: _settings.VideoQuality switch
                {
                    VideoQuality.Basic => CreateAssetRequest.VideoQualityEnum.Basic,
                    VideoQuality.Premium => CreateAssetRequest.VideoQualityEnum.Premium,
                    VideoQuality.Plus => CreateAssetRequest.VideoQualityEnum.Plus,
                    _ => CreateAssetRequest.VideoQualityEnum.Basic,
                },
                test: _settings.TestModeEnabled,
                meta: new AssetMetadata(title, creatorId, externalId)
            ),
        };

        var response = await _directUploadsApi.CreateDirectUploadAsync(createRequest);

        // put request to url with content type application/octet-stream
        using var client = _httpClientFactory.CreateClient(nameof(MuxService));
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var response2 = await client.PutAsync(response.Data.Url, content, cancellationToken);

        var upload = await _directUploadsApi.GetDirectUploadAsync(
            response.Data.Id,
            cancellationToken
        );

        return await GetAsset(upload.Data.AssetId, cancellationToken);
    }

    public async Task<AssetResponse> CopyAsset(
        string assetId,
        string? title = null,
        string? creatorId = null,
        string? externalId = null,
        CancellationToken cancellationToken = default
    )
    {
        var asset = await GetAsset(assetId, cancellationToken);
        ArgumentNullException.ThrowIfNull(asset, nameof(asset));

        var createRequest = new CreateAssetRequest()
        {
            Inputs = new List<InputSettings> { new InputSettings($"mux://assets/{asset.Data.Id}") },
            PlaybackPolicies = new List<PlaybackPolicy> { _settings.PlaybackPolicy },
            VideoQuality = _settings.VideoQuality switch
            {
                VideoQuality.Basic => CreateAssetRequest.VideoQualityEnum.Basic,
                VideoQuality.Premium => CreateAssetRequest.VideoQualityEnum.Premium,
                VideoQuality.Plus => CreateAssetRequest.VideoQualityEnum.Plus,
                _ => CreateAssetRequest.VideoQualityEnum.Basic,
            },
            Test = _settings.TestModeEnabled,
            Meta = new AssetMetadata(title, creatorId, externalId),
        };

        return await _assetsApi.CreateAssetAsync(createRequest, cancellationToken);
    }

    public void DeleteAsset(string assetId)
    {
        _assetsApi.DeleteAsset(assetId);
    }
}
