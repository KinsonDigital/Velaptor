<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.20
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#594](https://github.com/KinsonDigital/Velaptor/issues/594) - Added new method overloads for rendering textures to the `ITextureRenderer` interface and `TextureRenderer` class.  This was done to give users the option to use a `System.Numerics.Vector2` parameter which is highly used when developing games.
   - `void Render(ITexture texture, Vector2 pos, int layer = 0)`
   - `void Render(ITexture texture, Vector2 pos, float angle, int layer = 0)`
   - `void Render(ITexture texture, Vector2 pos, RenderEffects effects, int layer = 0)`
   - `void Render(ITexture texture, Vector2 pos, Color color, int layer = 0)`
   - `void Render(ITexture texture, Vector2 pos, Color color, RenderEffects effects, int layer = 0)`

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#593](https://github.com/KinsonDigital/Velaptor/issues/593) - Updated the [CICD](https://github.com/KinsonDigital/CICD) dotnet tool from version `v1.0.0-preview.21` to `v1.0.0-preview.22`
2. [#372](https://github.com/KinsonDigital/Velaptor/issues/372) - Refactored unit test assertion code for the following files:
   - _CachedValueTests.cs_
   - _InternalExtensionMethodsTests.cs_
   - _PublicExtensionMethodsTests.cs_
   - _SizeUTests.cs_
   - _WindowTests.cs_
