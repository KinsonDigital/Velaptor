<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.6</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, including releases, there is always a chance for issues and bugs.  It is also common to miss changes in the release notes when there are many.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Implemented specialized version of the .NET observable interfaces. This replaces the use of `System.IObservable` and `System.IObserver`.
    - This gives the ability to add features to the observable pattern.
    - NOTE: This provides the ability to do [Reactive Programming](https://en.wikipedia.org/wiki/Reactive_programming) by using the [Observable Pattern](https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj81ZHM3dL1AhVtk4kEHQkbCygQFnoECCEQAQ&url=https%3A%2F%2Fdocs.microsoft.com%2Fen-us%2Fdotnet%2Fstandard%2Fevents%2Fobserver-design-pattern&usg=AOvVaw1J3tfvEfjKtYOxvZEXY3sk).
2. Added a new property named `IsDefaultFont` to the `IFont` and `Font` types.
    -  This represents whether or not the font was loaded using the default font.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 💣</h2>

1. Refactored name of the `Observable` class to `Reactor`
    - This was done due to prevent naming clashes in the future with `System.IObservable<T>`, `System.IObserver` types.  This was discovered when creating the new type `IReactor`.  Originally the name was `Velaptor.Observables.Core.IObservable<T>`.  This was the same name as `System.IObservable<T>` and was causing too many uses of `using alias` across the code base to prevent naming clashes between **Velaptor** and **.NET**.
    - The new name `Reactor` means that it reacts to push notifications for the purpose of **_reactive_** programming.
2. Change the scope of the methods below for the `Window` class from `public` to `protected`:
    - `OnLoad()`
    - `OnDraw()`
    - `OnUnload()`
    - `OnResize()`
3. Refactored name of interface `IObservable` to `IReactable`.
4. Refactored name of interface `IObserver` to `IReactor`.
5. Refactored name of class `Observable` to `Reactable`.
6. Refactored name of class `Observer` to `Reactor`.
7. Refactored name of class `ObserverUnsubscriber` to `ReactorUnsubscriber`.
8. Refactored namespace `Velaptor.Observables` to `Velaptor.Reactables`.
9. Removed `IDisposable` interface from the `IContent` interface.
10. Removed the `Dispose()` method from all of the `IContent` implementations below:
    - `Texture`
    - `AtlasData`
    - `Sound`
    - `Font`

---

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Greatly improved the branching diagram to help contributors understand the Git branching process.
2. Updated documentation to point to the new branching diagram.

---

<h2 style="font-weight:bold" align="center">Other 👏</h2>

1. Added a new discord badge to README file to join the discord server.
