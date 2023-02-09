<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.17
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#506](https://github.com/KinsonDigital/Velaptor/issues/506) - Added the ability to create and manage scenes.  The following types have been added:
   - `IScene`
     >💡Used for creating your own completely custom scenes.
   - `SceneBase`  
     >💡This is an `abstract` class to create your scenes.  This comes with all of the required functionality to create scenes for normal circumstances.
   - `ISceneManager`
     >💡Automatically manages how scenes load and unload content, update, and render to the screen.  Use the `Window.SceneManager` property to add new scenes to the running application.
   - `SceneAlreadyExistsException`
     >💡This is thrown when adding a new scene to the `Window.SceneManager` if a scene already exists.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#504](https://github.com/KinsonDigital/Velaptor/issues/504) - Improved various types to increase performance.  The following breaking changes were added:
   - Changed the `Platform` class to `sealed`.
   - Changed the `PullResponses` class to `internal`.
   - Changed the class `KeyboardKeyStateData` to `internal`.
     >💡This was never meant to be a public API.  This was only used internally so this will mostly likely not affect your code base.
2. [#506](https://github.com/KinsonDigital/Velaptor/issues/506) - Added the following breaking changes:
   - Changed the following classes to sealed:
        - `EnumOutOfRangeException`
        - `InvalidRenderEffectsException`
        - `LoadEmbeddedResourceException`
        - `SystemMonitorException`
   - Changed the class `App` class to `internal`.
   - Changed the `FrameTime` struct to a `readonly record struct`.
     >💡This was done for performance gains.
   - Changed the `SizeU` struct to a `readonly record struct`.
     >💡This was done for performance gains.
   - Removed the `IWindowActions` interface.
     >💡The members of this interface were moved to the `IWindow` interface.
   - Removed the `IWindowProps` interface.
     >💡The members of this interface were moved to the `IWindow` interface.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#352](https://github.com/KinsonDigital/Velaptor/issues/352) - Refactored the unit test assertion code to use the [Fluent Assertions](https://fluentassertions.com/) library for the following classes:
   - `AtlasJSONDataPathResolverTests`
   - `AtlasLoaderTests`
   - `AtlasTexturePathResolverTests`
2. [#350](https://github.com/KinsonDigital/Velaptor/issues/350) - Refactored the unit test assertion code to use the [Fluent Assertions](https://fluentassertions.com/) library for the following classes:
   - `LoadFontExceptionTests`
   - `LoadTextureExceptionTests`
   - `ContentFontPathResolverTests`
   - `FontStatsServiceTests`
3. [#349](https://github.com/KinsonDigital/Velaptor/issues/349) - Refactored the unit test assertion code to use the [Fluent Assertions](https://fluentassertions.com/) library for the following classes:
   - `SoundCacheTests`
   - `TextureCacheTests`
   - `CachingExceptionTests`
   - `CachingMetaDataExceptionTests`
   - `LoadAtlasExceptionTests`
4. [#454](https://github.com/KinsonDigital/Velaptor/issues/454) - Added dependency management by setting up [dependabot](https://docs.github.com/en/rest/dependabot?apiVersion=2022-11-28).
5. [#507](https://github.com/KinsonDigital/Velaptor/issues/507) - Added automated code reviews by setting up [codacy](https://www.codacy.com/).
6. [#511](https://github.com/KinsonDigital/Velaptor/issues/511) - Refactored workflows to use environment files for step output.
   >💡This was required due to the old way of set outputs being deprecated.  Go [here](https://github.blog/changelog/2022-10-11-github-actions-deprecating-save-state-and-set-output-commands/) for more information.
