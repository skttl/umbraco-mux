using Mux.Csharp.Sdk.Model;

namespace uMux.Configuration;

public class MuxSettings
{
    public string ApiBasePath { get; set; } = "https://api.mux.com";
    public string? ApiTokenId { get; set; }
    public string? ApiSecret { get; set; }

    public string? SigningKeyId { get; set; }
    public string? SigningPrivateKey { get; set; }
    public int? SignedUrlExpiration { get; set; }

    public PlaybackPolicy PlaybackPolicy { get; set; } = PlaybackPolicy.Public;

    public VideoQuality VideoQuality { get; set; } = VideoQuality.Basic;

    public bool TestModeEnabled { get; set; } = false;
}

public enum VideoQuality
{
    /// <summary>
    /// For apps with simpler needs that need to save on bandwidth & cost
    /// </summary>
    Basic,

    /// <summary>
    /// For conistently high quality output, but incurs encoding cost
    /// </summary>
    Plus,

    /// <summary>
    /// For premium high-detail content like sports broadcasts
    /// </summary>
    Premium
}
