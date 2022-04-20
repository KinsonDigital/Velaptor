<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.3</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, there is always a chance for issues and bugs to exist with all releases.  It is also common to sometimes miss changes in the release notes when the amount of changes are large.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Greatly improved the testing application to help improve the process of manual testing
2. Created scene management system with various scenes for the testing application
   - Added a scene to test text rendering
   - Added a scene to test mouse functionality
   - Added a scene to test keyboard functionality
   - Added a scene to test graphics rendering
   - Created a simple scene management system to help easily add new scenes to the testing application
3. Created new abstract control base class named _`Velaptor.UI.ControlBase`_ for the purpose of creating custom controls
4. Created new _`Velaptor.UI.Label`_ control
5. Created new _`Velaptor.UI.Button`_ control
6. Created new _`Velaptor.UI.Textbox`_ control
   - Warning!! - This control is not finished and needs some work
7. Added ability to _`IContent`_ items to know if they have been created using **_Velaptor's_*** content pooling system.
   - This ability gives the user the ability to know if a content item was created and added tot he pooling system vs created as a custom instantiation by the user.
8. Add 2 new properties to the controls
   - _`MouseHoverColor`_ - This is the color that is applied on top of the control during rendering when the mouse hovers over the control.
   - _`MouseDown`_ - This is the color that is applied on top of the control during rendering when the left mouse button is in the down position over a control.
9.  Added new boolean property named _`IsPooled`_ to the _`IContent`_ interface and all implementation members
    - This property value is true if it was created using **_Velaptor's__** content creation system
10. Created new exception type named _`PooledDisposalException`_
   - This exception is thrown if an _`IContent`_ item is being disposed when it was created with **_Velaptor's_** content creation system which uses content pooling for performance
   - NOTE: To know if an item was created in a pool, query the boolean value of the _`IContent.IsPooled`_ property
11. Added ability to _`IContent`_ items to throw an exception of type _`PooledDisposalException`_
12. Added ability for the content loaders below to take in a content name with or without and extension.
    - NOTE: If a content name contains an extension, it is simply ignored.
    - _`AtlasLoader`_
    - _`FontLoader`_
    - _`TextureLoader`_
    - _`SoundLoader`_ - This loader has not been updated to do this.  There are plans to make some changes to the _`ISound`_ type in **_Velaptor_** which will possibly require some changes to the **_CASL_** library.  Coming soon!!

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. Fixed a bug with the _`MapValue`_ extension method
2. Fixed a bug with where clearing the screen to a set color was not working
   - This was a bug with the _`ISpriteBatch.Clear()`_ method implementation
3. Fixed a bug where in rare cases, the mouse setup process would try to duplicate setup of the mouse buttons causing an exception
   - This issue might actually of never occurred but was discovered during unit testing
4. Fixed some bugs with how _`IContent`_ items were being disposed in UI controls and scenes

---

<h2 style="font-weight:bold" align="center">Nuget/Library Additions 📦</h2>

1. Added nuget package **_Microsoft.CodeAnalysis.NetAnalyzers v5.0.3_**
   - This is for the purpose for code analysis during development
2. Added nuget package **_StyleCop.Analyzers v1.1.118_**
   - This is for the purpose for code analysis during development

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. Removed the following classes/types
   - _`RenderText`_
   - _`UIText`_
2. Refactored any public members in the code base that ended with **_ID_** to end with **_Id_** instead
   - This was to maintain consist clean code throughout the code base
3. Changed the constructor of the _`Velaptor.UI.Window`_ class from **_public_** to **_protected_**
4. Moved the types below from the _`Velaptor.Graphics`_ namespace to the _`Velaptor.Content`_ namespace
   - _`IAtlasData`_
   - _`AtlasData`_
   - _`IFont`_
   - _`Font`_
   - _`ITexture`_
   - _`Texture`_
5. Change the classes below to be sealed:
   - NOTE: This was done to make sure to now allow new types to inherit from these lower level library types.  This is to make sure that the content pooling and **_OpenGL_** calls do not put the system into a state of being unstable.  If user wants to create custom controls and content types, take advantage of all the various interfaces to do this as it was built for this. 
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

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Cleanup and refactoring of inline XML code documentation throughout entire library.
2. Refactoring to help aline code base with coding standards

---

<h2 style="font-weight:bold" align="center">CI/CD 🚀</h2>

1. Created workflow to automatically add triage labels to new and reopened issues
   - Contribution made by [@teezzan](https://github.com/teezzan) - Thanks!! 🙏
2. Fixed an issue with the **_QA_** and **_Production_** workflow template
   - This was a syntax error preventing the workflow from running

---

<h2 style="font-weight:bold" align="center">Other 👏</h2>

1. Increased code coverage
2. Miscellaneous cleanup and refactoring to meet coding standards
