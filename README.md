# Muxy GameLink Unity FPS Demo

This demo is an example of Muxy GameLink integrated with the built in Unity FPS Microgame, it includes the game code and extension code.

## Requirements

Before you get started you will want to register your extension in the [Muxy Dashboard](dev.muxy.io)

If you wish to simply get the demo up and running without registering first, please use the Extension ID `ukylx425okgkrkrzg60epd7u9cehsf`.

## Unity Setup

Start a new `FPS Microgame` template project in the Unity Hub.

Drag and drop `GameLink.prefab` into `Assets\FPS\Prefabs`.
Drag and drop `GameLinkFPSBehaviour.cs` into `Assets\FPS\Scripts`.
Open `Assets\FPS\Scenes\IntroMenu.unity` and drag `GameLink.prefab` into the entity scene root.
Enter your extension id into the component on the GameLink prefab.

## Extension Setup

Please view the README inside the Extension folder on how to run it.

You can also refer to our [Unity GameLink Intro](muxy.io/docs/) documentation on how to setup and run an extension from scratch if you have no experience with npm at all.