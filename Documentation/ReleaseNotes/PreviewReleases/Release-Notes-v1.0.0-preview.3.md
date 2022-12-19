<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
   Velaptor Preview Release Notes - v1.0.0-preview.3
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. Greatly improved the process of the manual testing application.
2. Created scene management systems for the following testing applications:
   - Text rendering
   - Mouse functionality
   - Keyboard functionality
   - Graphics rendering
3. Created a scene management system to help easily add new scenes to the testing application.
4. Created new abstract control base class named _`Velaptor.UI.ControlBase`_ for the purpose of creating custom controls.
5. Created new _`Velaptor.UI.Label`_ control.
6. Created new _`Velaptor.UI.Button`_ control.
7. Created new _`Velaptor.UI.Textbox`_ control.
   > ⚠️ - This control is not finished and needs some work.
8. Added ability to the _`IContent`_ items to know whether or not they have been created using **_Velaptor's_*** content pooling system.
   - This enables the user to know if a content item was created and added to the pooling system or created as a custom instantiation by the user.
9. Added 2 new properties to the controls.
   - _`MouseHoverColor`_ - This is the color that is applied on top of the control during rendering when the mouse hovers over the control.
   - _`MouseDown`_ - This is the color that is applied on top of the control during rendering when the left mouse button is in the down position over a control.
10. Added new boolean property named _`IsPooled`_ to the _`IContent`_ interface and all implementation members.
    - This property value is true if it was created using **_Velaptor's__** content creation system.
11. Created new exception type named _`PooledDisposalException`_.
    - This exception is thrown if an _`IContent`_ item is being disposed when it was created with **_Velaptor's_** content creation system which uses content pooling for performance.
      > **💡** To know if an item was created in a pool, query the boolean value of the _`IContent.IsPooled`_ property.
       
12. Added ability to _`IContent`_ items to throw an exception of type _`PooledDisposalException`_.
13. Added ability for the content loaders below to take in a content name with or without and extension.
      > 💡 If a content name contains an extension, it is ignored.
    - _`AtlasLoader`_
    - _`FontLoader`_
    - _`TextureLoader`_
    - _`SoundLoader`_ - This loader has not been updated yet.  There are plans to make some changes to the _`ISound`_ type in **_Velaptor_** which may require some changes to the **_CASL_** library.  Coming soon!!

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. Fixed a bug with the _`MapValue`_ extension method.
2. Fixed a bug where clearing the screen to a set color was not working.
   - This was a bug with the _`ISpriteBatch.Clear()`_ method implementation.
3. Fixed a bug where in rare cases, the mouse setup process would try to duplicate setup of the mouse buttons causing an exception.
   - This issue may not have occurred, but was discovered during unit testing.
4. Fixed some bugs with how _`IContent`_ items were being disposed in UI controls and scenes.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Additions 📦</h2>

1. Added NuGet package **Microsoft.CodeAnalysis.NetAnalyzers** **_v5.0.3_**
   - This is for the purpose for code analysis during development.
2. Added NuGet package **StyleCop.Analyzers** **_v1.1.118_**
   - This is for the purpose for code analysis during development.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. Removed the following classes/types:
   - _`RenderText`_
   - _`UIText`_
2. Refactored any public members in the code base that ended with **_ID_** to end with **_Id_** instead.
   - This was to maintain consist clean code throughout the code base.
3. Changed the constructor of the _`Velaptor.UI.Window`_ class from **_public_** to **_protected_**.
4. Moved the types below from the _`Velaptor.Graphics`_ namespace to the _`Velaptor.Content`_ namespace:
   - _`IAtlasData`_
   - _`AtlasData`_
   - _`IFont`_
   - _`Font`_
   - _`ITexture`_
   - _`Texture`_
5. Changed the classes below to be sealed:
      >💡This was done to make sure new types do not inherit from these lower level library types.  This is to make sure that the content pooling and **_OpenGL_** calls do not put the system into a state of being unstable.  If the user wants to create custom controls and content types, take advantage of all the interfaces. 
   - _`AtlasData`_
   - _`AtlasLoader`_
   - _`ContentLoader`_
   - _`Font`_
   - _`FontLoader`_
   - _`Sound`_
   - _`SoundLoader`_
   - _`Texture`_
   - _`TextureLoader`_
   - _`Button`_
   - _`Label`_

---

<h2 style="font-weight:bold" align="center">CI/CD 🚀</h2>

1. Created workflow to automatically add triage labels to new and reopened issues.
   - Contribution made by [@teezzan](https://github.com/teezzan) - Thanks!! 🙏
2. Fixed an issue with the **_QA_** and **_Production_** workflow template.
   - This was a syntax error preventing the workflow from running.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. Cleanedup and refactored inline XML code documentation throughout entire library.
2. Refactored to align code base with coding standards.
3. Increased code coverage.
4. Miscellaneous cleanup and refactoring to meet coding standards.
