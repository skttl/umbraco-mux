import { ManifestPropertyEditorSchema, ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/property-editor'

const entryPointManifest = {
  name: 'uMux Entry Point',
  type: 'backofficeEntryPoint',
  alias: 'uMux.EntryPoint',
  js: () => import('./entrypoint.js'),
}

const editorManifest: ManifestPropertyEditorUi = {
  type: 'propertyEditorUi',
  alias: 'uMux.PropertyEditorUi.MuxSync',
  name: 'Mux Sync',
  element: () => import('./property-editor-ui-mux-sync.element.js'),
  meta: {
    label: 'Mux Sync',
    icon: 'icon-video',
    group: 'media',
    propertyEditorSchemaAlias: 'uMux.Sync',

    settings: {
      properties: [
        {
          alias: "uploadPropertyAlias",
          label: "Upload Property Alias",
          description: "Set the alias of the upload property editor to sync with",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox",
        },
      ],
    },
  },
};

const schemaManifest: ManifestPropertyEditorSchema = {
  type: 'propertyEditorSchema',
  name: 'Mux Sync',
  alias: 'uMux.Sync',
  meta: {
    defaultPropertyEditorUiAlias: 'uMux.PropertyEditorUi.MuxSync'
  },
};

export const manifests = [
  entryPointManifest,
  editorManifest,
  schemaManifest,
];
