using System.Text.Json.Serialization;

namespace uMux.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetStatus
{
    NotFound = 0,
    Unknown = 1,
    Preparing = 2,
    Errored = 3,
    Ready = 4,
}
