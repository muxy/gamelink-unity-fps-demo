# Muxy GameLink Unity FPS Demo

This demo is an example of Muxy GameLink integrated with the built in Unity FPS Microgame, it includes the game code and extension code.

You can also refer to our [Unity FPS Demo Code Walkthrough](muxy.io/docs/unity-fps-demo-code-walkthrough) of this demo. The walkthrough explains as much of the code interaction as possible.

## Requirements

Please view our [Quickstart Guide](https://docs.muxy.io/docs/quick-start) on how to make a Twitch Extension (or take an existing one) and register it with Muxy.

## Unity Setup

1. Start a new `FPS Microgame` template project in the Unity Hub.
2. Drag and drop `GameLink.prefab` into `Assets\FPS\Prefabs`.
3. Drag and drop `GameLinkFPSBehaviour.cs` into `Assets\FPS\Scripts`.
4. Open `Assets\FPS\Scenes\IntroMenu.unity` and drag `GameLink.prefab` into the hierarchy scene root.
5. Enter your extension id into the component field `GAMELINK_EXTENSION_ID` on the GameLink prefab.
6. Install the [GameLink Library](https://github.com/muxy/gamelink-unity) into the packages folder.

## Extension Setup

Please view the README inside the Extension folder on how to run it.