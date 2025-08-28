using Umbraco.Cms.Core.PropertyEditors;

namespace uMux.PropertyEditors;

[DataEditor(
    Constants.PropertyEditorSchema,
    ValueEditorIsReusable = true,
    ValueType = ValueTypes.Json
)]
public class SyncDataEditor : DataEditor
{
    public SyncDataEditor(IDataValueEditorFactory dataValueEditorFactory)
        : base(dataValueEditorFactory) { }
}
