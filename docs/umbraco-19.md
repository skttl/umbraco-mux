# Umbraco 19 RC

`main` targets Umbraco 19.0.0-rc1. `v18/main` preserves the Umbraco 18 version at commit `d9953e4`.

Build requirements:

- .NET 11 SDK
- Node.js 24.13 or later and npm 11 or later

Build the backoffice client before packing:

```sh
cd src/uMux/Client
npm ci
npm run build
cd ../../..
dotnet build src/uMux.sln --configuration Release
dotnet pack src/uMux/uMux.csproj --configuration Release --output .artifacts/packages /p:PackageVersion=19.0.0-rc1
```

The client now authenticates with the backoffice cookie. It no longer reads an access token from the auth context. The OpenAPI generator uses `/umbraco/openapi/umux.json`.

Before running the test site, back up its database. Umbraco 19 migrates the database and Umbraco 18 cannot reopen it afterwards. No database migration is performed by the build commands above.

The checked-in `src/uMux.TestSite/uSync/v17` and `v18` exports are historical fixtures. Do not import them into a migrated v19 database. Umbraco 19 changes built-in property editor aliases and configuration, so old exports can undo those migrations. After upgrading a backed-up test site with uSync 19, export fresh v19 fixtures and verify the upload and uMux data types before importing elsewhere.

Manual verification still required on a backed-up test site:

- Sign in to the backoffice and open a video with the uMux property editor. Confirm the status API succeeds using cookies and no bearer token.
- Upload and save a video. Verify Mux synchronization, status polling, and playback information.
- Check migrated data types before exporting new uSync fixtures.

References: [release candidate announcement](https://umbraco.com/blog/umbraco-19-release-candidate/) and [v19 upgrade notes](https://docs.umbraco.com/umbraco-cms/19.latest/get-started/upgrading-and-migrating/version-specific).
