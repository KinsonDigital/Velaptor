<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.21
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#212](https://github.com/KinsonDigital/Velaptor/issues/212) - Added the ability to cache measurements when measuring text.  The `IFont` interface and `Font` class now contain two more properties named `CacheEnabled` and `MaxCacheSize` to control if and how caching is done when measuring text size.
   > The adding caching capability has increased performance of the text measuring process by up to 33.5%.
2. [#155](https://github.com/KinsonDigital/Velaptor/issues/155) - Added the ability to render primitive circle shapes with the following features:
   >💡This can be done by using the `IShapeRenderer.Render()` method overload that uses the new `CircleShape` struct.
   - Circle diameter and/or radius.
   - Solid colored circles.
   - Bordered circles with an empty center.
   - Horizontal and vertical color gradients.

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#155](https://github.com/KinsonDigital/Velaptor/issues/155) - Fixed an issue with the border thickness being reset to the maximum amount.
   > 💡This was occurring when a `RectShape` width and height was being reduced to a minimum value of 1.  After the full reduction in size, the increasing of size would then reset the border thickness back to the maximum amount instead of the border thickness that was applied before the reduction in rectangle size.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#515](https://github.com/KinsonDigital/Velaptor/issues/515) - Added breaking changes to the following type members:
   > 💡This was due to changing the return type from `ReadOnlyCollection` to `IReadOnlyCollection` for performance gains.
   - `Label.CharacterBounds`
   - `IAtlasData.SubTextureNames`
   - `AtlasData.SubTextureNames`
   - `IFont.Metrics`
   - `Font.Metrics`
   - `IItemCache<T>.CacheKeys`
2. [#155](https://github.com/KinsonDigital/Velaptor/issues/155) - Added breaking changes to the following types:
   - Changed the name of the `IRendererFactory.CreateRectangleRenderer()` method to `IRendererFactory.CreateShapeRenderer()`.
   - Changed the name of the `RendererFactory.CreateRectangleRenderer()` method to `RendererFactory.CreateShapeRenderer()`.
   - Changed the name of the `IRectangleRenderer` interface to `IShapeRenderer`.
   - Changed the name of the `RectShape.IsFilled` property to `RectShape.IsSolid`.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#515](https://github.com/KinsonDigital/Velaptor/issues/515) - Increased performance in various areas when processing and/or using collections.
2. [#155](https://github.com/KinsonDigital/Velaptor/issues/155) - Changed the `RectShape` from a standard `struct` to a `record struct`.
   >💡This increases performance when comparing if two `RectShape`'s are equal.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#369](https://github.com/KinsonDigital/Velaptor/issues/369) - Updated **FluentAssertions** from _**v6.10.0**_ to _**v6.11.0**_

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#603](https://github.com/KinsonDigital/Velaptor/issues/603) - Fixed a broken logo link in the NuGet package readme.
2. [#369](https://github.com/KinsonDigital/Velaptor/issues/369) - Refactored XUnit unit test assertions to use [Fluent Assertions](https://fluentassertions.com/).