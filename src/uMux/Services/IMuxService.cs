using Mux.Csharp.Sdk.Model;

namespace uMux.Services;

public interface IMuxService
{
    public Task<AssetResponse> CopyAsset(
        string assetId,
        string? title = null,
        string? creatorId = null,
        string? externalId = null,
        CancellationToken cancellationToken = default
    );
    public Task<AssetResponse> CreateAsset(
        byte[] bytes,
        string? title = null,
        string? creatorId = null,
        string? externalId = null,
        CancellationToken cancellationToken = default
    );
    public void DeleteAsset(string assetId);
    public Task<AssetResponse> GetAsset(
        string assetId,
        CancellationToken cancellationToken = default
    );
}
