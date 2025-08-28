using System.Text.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using uMux.Models;

namespace uMux.PropertyEditors;

public class SyncPropertyValueConverter : PropertyValueConverterBase
{
    public override bool IsConverter(IPublishedPropertyType propertyType)
    {
        return propertyType.EditorAlias == Constants.PropertyEditorSchema;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) =>
        typeof(MuxValue);

    public override object? ConvertIntermediateToObject(
        IPublishedElement owner,
        IPublishedPropertyType propertyType,
        PropertyCacheLevel referenceCacheLevel,
        object? inter,
        bool preview
    ) =>
        inter is string value && value.StartsWith("{")
            ? JsonSerializer.Deserialize<MuxValue>(value)
            : null;
}
