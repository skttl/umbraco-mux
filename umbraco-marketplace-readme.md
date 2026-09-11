# uMux

uMux connects Umbraco Media to [Mux Video](https://www.mux.com/video). Upload a video to Umbraco and uMux creates and maintains the corresponding Mux asset automatically.

## What uMux does

- Uploads videos from an Umbraco media item to Mux when the item is saved.
- Stores the Mux asset ID and playback ID on the media item.
- Keeps the local Umbraco file available alongside the Mux asset.
- Lets you use Mux playback URLs, thumbnails, storyboards, or the Mux Player in your frontend.

## Requirements

- Umbraco CMS 18 or newer
- A Mux account with API access

## Installation

Install the package in your Umbraco project:

```sh
dotnet add package Umbraco.Community.uMux
```

Add your Mux credentials to `appsettings.json` or configure them as environment variables:

```json
{
  "Umbraco": {
    "Mux": {
      "ApiTokenId": "YOUR MUX TOKEN ID",
      "ApiSecret": "YOUR MUX SECRET"
    }
  }
}
```

```text
Umbraco__Mux__ApiTokenId=YOUR MUX TOKEN ID
Umbraco__Mux__ApiSecret=YOUR MUX SECRET
```

## Configure the property editor

1. Go to **Settings > Data Types** in the Umbraco backoffice.
2. Create a data type using the **Mux Sync** property editor.
3. Enter the alias of the Upload property that contains the video file.
4. Add the data type to your media type, next to the Upload property.

The Mux Sync property must be on the same media type as the Upload property. Once configured, save a media item with a video to start the sync.

## Use the synced video

The property value contains a `MuxValue` with the local file path, Mux asset ID, and playback ID. For example, an iframe embed can use the playback ID like this:

```html
<iframe
  src="https://player.mux.com/@mediaVideo.MuxVideo.PlaybackId"
  style="width: 100%; border: none; aspect-ratio: 16/9;"
  allow="accelerometer; gyroscope; autoplay; encrypted-media; picture-in-picture;"
  allowfullscreen>
</iframe>
```

For more playback options and troubleshooting, see the [full documentation](https://github.com/skttl/umbraco-mux#readme).
