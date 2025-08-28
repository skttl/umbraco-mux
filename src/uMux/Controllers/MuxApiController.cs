using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mux.Csharp.Sdk.Model;
using uMux.Enums;
using uMux.Services;

namespace uMux.Controllers;

[ApiVersion(Constants.Swagger.Version)]
[ApiExplorerSettings(GroupName = Constants.Swagger.GroupName)]
public class MuxApiController : MuxApiControllerBase
{
    private readonly IMuxService _muxService;

    public MuxApiController(IMuxService muxService) => _muxService = muxService;

    [HttpGet("status")]
    [ProducesResponseType<AssetStatus>(StatusCodes.Status200OK)]
    public async Task<AssetStatus> GetStatus(string assetId)
    {
        var asset = await _muxService.GetAsset(assetId);
        if (asset == null)
        {
            return AssetStatus.NotFound;
        }

        return asset.Data.Status switch
        {
            Asset.StatusEnum.Preparing => AssetStatus.Preparing,
            Asset.StatusEnum.Errored => AssetStatus.Errored,
            Asset.StatusEnum.Ready => AssetStatus.Ready,
            _ => AssetStatus.Unknown,
        };
    }
}
