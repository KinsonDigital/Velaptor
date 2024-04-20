<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.35
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Added a new `enum` named `AudioBuffer` for choosing audio buffer types.
2. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Added a new property with the name `BufferType` to the `IAudio` interface and `Audio` class.
    - This is used to know what type of buffering is being used for the audio.  This is a new feature that comes from _**CASL**_ _**v1.0.0-preview.18**_.
1. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Added a new `Load(string atlasPathOrName)` method to the `ILoader<IAtlasDta>` interface for loading texture atlas data.
2. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Added a new `Load(string audioPathOrName, AudioBuffer bufferType)` method to the `ILoader<IAudio>` interface for loading audio.
3. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Added a new `Load(string texturePathOrName)` method to the `ILoader<ITexture>` interface for loading textures.

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#882](https://github.com/KinsonDigital/Velaptor/issues/882) - Fixed the following bugs:
    - Fixed a bug where invoking the `ImageData.FlipHorizontally()` method would throw a null reference exception when an instance of `ImageData` struct was created via a default constructor or the `default` keyword.
    - Fixed a bug where invoking the `ImageData.FlipVertically()` method would throw a null reference exception when an instance of `ImageData` struct was created via a default constructor or the `default` keyword.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#947](https://github.com/KinsonDigital/Velaptor/issues/947) - 1. Removed the following constructors from the `Texture` class:
    - Removed constructor with the signature `Texture(string name, ImageData imageData)`.
    - Removed constructor with the signature `Texture(string name, string filePath)`.
2. [#933](https://github.com/KinsonDigital/Velaptor/issues/933) - Removed the deprecated controls UI API.
3. [#882](https://github.com/KinsonDigital/Velaptor/issues/882) - Removed the `width` and `height` ctor params from the `ImageData` struct.
    - The dimensions are now internally pulled from the `pixels` parameter.
4. [#938](https://github.com/KinsonDigital/Velaptor/issues/938) - Removed the following constructors in the content API.
    - Removed the `AtlasLoader` class from `public` to `internal`.
    - Removed the `TextureLoader` class from `public` to `internal`.
    - Removed the `AudioLoader` class from `public` to `internal`.
    - Removed the `FontLoader` class from `public` to `internal`.
    - Removed the `AtlasData` class constructor from `public` to `internal`.

5. [#934](https://github.com/KinsonDigital/Velaptor/issues/934) - Introduced the following breaking changes related to CASL audio API updates.
   - Renamed the `ISound` interface to `IAudio`.
   - Renamed the `Sound` class to `Audio`.
   - Changed the data type of the `Position` and `Length` properties to `TimeSpan`.
   - Replaced the `State` property from the `ISound` interface and `Sound` class with the following bool properties to represent the state of the audio.
      - `IsPlaying`
      - `IsPaused`
      - `IsStopped`
      - The `State` property was removed due to the unintentional exposure of the _**CASL**_ API.
   - Removed the `Reset()` method from the `ISound` interface and `Sound` class.  This method was performing the same operation as the `Stop()` method.
   - Removed the `public` constructor from the `Sound` class.
      - This was done to force users to use the content loader system.
   - Refactored the name of the `LoadSoundException` to `LoadAudioException`.
   - Refactored the name of the `SoundLoader` class to `AudioLoader`.
   - Refactored the name of the `PathResolverFactory.CreateSoundPathResolver()` method to `PathResolverFactory.CreateAudioPathResolver()`.
   - Refactored the name of the `ContentLoaderFactory.CreateSoundLoader()` method to `ContentLoaderFactory.CreateAudioLoader()`.
   - The default name of the content folder `Sound` which is where audio content is located has been changed to `Audio`.
   - Refactored the `ILoader<IAudio>.Unload()` method parameter named `sound` to `audio`.


<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#946](https://github.com/KinsonDigital/Velaptor/pull/946) - Updated dependency _**freetypesharp**_ to _**v3.0.0**_
2. [#942](https://github.com/KinsonDigital/Velaptor/pull/942) - Updated dependency _**xunit**_ to _**v2.7.1**_
3. [#942](https://github.com/KinsonDigital/Velaptor/pull/942) - Updated dependency _**xunit.runner.visualstudio**_ to _**v2.5.8**_
4. [#940](https://github.com/KinsonDigital/Velaptor/pull/940) - Updated dependency _**sixlabors.imagesharp**_ to _**v3.1.4**_
5. [#936](https://github.com/KinsonDigital/Velaptor/pull/936) - Updated dependency _**CASL**_ to _**v1.0.0-preview.19**_
6. [#932](https://github.com/KinsonDigital/Velaptor/pull/932) - Updated dependency _**KinsonDigital.Carbonate**_ to _**v1.0.0-preview.18**_
7. [#929](https://github.com/KinsonDigital/Velaptor/pull/929) - Updated dependency _**system.io.abstractions**_ to _**v21.0.0**_
8. [#928](https://github.com/KinsonDigital/Velaptor/pull/928) - Updated dependency _**freetypesharp**_ to _**v2.0.0**_
9. [#927](https://github.com/KinsonDigital/Velaptor/pull/927) - Updated dependency _**coverlet.msbuild**_ to _**v6.0.2**_
10. [#926](https://github.com/KinsonDigital/Velaptor/pull/926) - Updated dependency _**coverlet.collector**_ to _**v6.0.2**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#924](https://github.com/KinsonDigital/Velaptor/issues/924) - Removed reporator badge.
2. [#909](https://github.com/KinsonDigital/Velaptor/issues/909) - Updated demo video link.
3. [#897](https://github.com/KinsonDigital/Velaptor/issues/897) - Updated animation scene.
4. [#881](https://github.com/KinsonDigital/Velaptor/issues/881),[#868](https://github.com/KinsonDigital/Velaptor/issues/868) - Refactored moq code to nsubstitute.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**
5. [#783](https://github.com/KinsonDigital/Velaptor/issues/783) - Replaced custom guards.
    > **Thank you [@thestbar](https://github.com/thestbar)!!**
