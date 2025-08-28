const t = {
  name: "uMux Entry Point",
  type: "backofficeEntryPoint",
  alias: "uMux.EntryPoint",
  js: () => import("./entrypoint-Ds0YLQXJ.js")
}, e = {
  type: "propertyEditorUi",
  alias: "uMux.PropertyEditorUi.MuxSync",
  name: "Mux Sync",
  element: () => import("./property-editor-ui-mux-sync.element-CxO4A1Bf.js"),
  meta: {
    label: "Mux Sync",
    icon: "icon-video",
    group: "media",
    propertyEditorSchemaAlias: "uMux.Sync",
    settings: {
      properties: [
        {
          alias: "uploadPropertyAlias",
          label: "Upload Property Alias",
          description: "Set the alias of the upload property editor to sync with",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
        }
      ]
    }
  }
}, i = {
  type: "propertyEditorSchema",
  name: "Mux Sync",
  alias: "uMux.Sync",
  meta: {
    defaultPropertyEditorUiAlias: "uMux.PropertyEditorUi.MuxSync"
  }
}, o = [
  t,
  e,
  i
];
export {
  o as manifests
};
//# sourceMappingURL=u-mux.js.map
