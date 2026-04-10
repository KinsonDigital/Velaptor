---
description: "Generate xUnit tests for Velaptor C# classes. Use when: writing unit tests, creating test class, adding test methods, testing constructors, methods, or properties."
agent: "agent"
argument-hint: "Describe what to test (e.g. 'full test class for TextureRenderer' or 'test the Render method null guard')"
tools: [edit/createFile, edit/editFiles, edit/rename, search, web]
---

# Generate xUnit Unit Tests

You are generating unit tests for the **Velaptor** C# framework. Follow every convention below exactly.

## Input

The user will provide one of:
1. **A source file or class** → generate a full test class covering constructors, methods, and properties.
2. **A specific method, property, or scenario** → generate only the targeted test methods.
3. **Custom instructions related to testing** → follow the instructions while adhering to the conventions.

Use the source code and existing test files as context. Always read the source file under test before generating tests.

## Project Conventions

### File & Class Structure
- Test project: `Testing/VelaptorTests/`
- Test files mirror the source folder structure (e.g., `Velaptor/Services/Foo.cs` → `Testing/VelaptorTests/Services/FooTests.cs`)
- Class name: `[ClassName]Tests`
- Test class extends `TestsBase` (from `VelaptorTests.Helpers`)
- File header:
  ```csharp
  // <copyright file="[FileName].cs" company="KinsonDigital">
  // Copyright (c) KinsonDigital. All rights reserved.
  // </copyright>
  ```

### Frameworks & Imports
- **xUnit** (`[Fact]`, `[Theory]`, `[InlineData]`)
- **NSubstitute** for mocking (`Substitute.For<T>()`)
- **Shouldly** for assertions (`.ShouldBe()`, `.ShouldThrow<T>()`, `Should.Throw<T>()`)
- `AssertExtensions` from `VelaptorTests.Helpers` for `ThrowsWithMessage<T>(action, message)`

### Test Organization
- Group tests inside `#region` blocks by trait category:
  - `#region Constructor Tests` → `[Trait("Category", Ctor)]`
  - `#region Method Tests` → `[Trait("Category", Method)]`
  - `#region Property Tests` → `[Trait("Category", Prop)]`
  - `#region Reactable Subscription` → `[Trait("Category", Subscription)]`
- Every `[Fact]` or `[Theory]` must have a `[Trait("Category", ...)]` attribute using the `TestsBase` constants.

### Test Naming
- Pattern: `[MemberName]_[Scenario]_[ExpectedResult]`
- Examples:
  - `Ctor_WithNullFooParam_ThrowsException`
  - `Render_WhenBatchHasNotBegun_ThrowsException`
  - `Value_WhenGettingValueWithCachingOn_ReturnsCorrectResult`

### Test Body
- Use `// Arrange`, `// Act`, `// Assert` comments (or `// Arrange & Act` when combined).
- For exception tests:
  ```csharp
  // Arrange & Act
  var act = () => new Foo(null, mockBar);

  // Assert
  var exception = act.ShouldThrow<ArgumentNullException>();
  exception.Message.ShouldBe("Value cannot be null. (Parameter 'paramName')");
  ```
- For value assertions use Shouldly: `actual.ShouldBe(expected);`
- For mock verification use NSubstitute: `mockFoo.Received(1).Bar();` or `mockFoo.DidNotReceive().Bar();`

### Mock Setup & SUT Creation
- Declare mocks as `private readonly` fields, initialized in the test class constructor.
- Create a `private [ClassName] CreateSystemUnderTest()` helper method that instantiates the SUT with all mocks.
- Tests call `CreateSystemUnderTest()` rather than constructing the SUT inline (except for constructor-null-guard tests).

### Constructor Null Guard Tests
- Generate one test per nullable constructor parameter.
- Each test passes `null` for the target parameter and valid mocks for all others.
- Assert `ArgumentNullException` with the exact parameter name in the message.

### What NOT To Do
- DO NOT add XML doc comments to test methods.
- DO NOT use `Assert.Equal` / `Assert.Throws` — use Shouldly equivalents.
- DO NOT add `[ExcludeFromCodeCoverage]` to test classes unless otherwise instructed to do so.
- DO NOT use `DateTime.Now` or `Thread.Sleep` in tests.
- DO NOT create unnecessary abstractions or helper methods beyond `CreateSystemUnderTest()`.
