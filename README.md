<div align="center">
    <a href="#"><img align="center" src="./Documentation/Images/velaptor-logo.png" height="96"></a>
    <br />
  
</div>

<h1 style="border:0;font-weight:bold" align="center">Velaptor</h1>

<div align="center">

![](https://img.shields.io/github/workflow/status/KinsonDigital/Velaptor/%F0%9F%9A%80Production%20Release?label=Production%20Release%20%F0%9F%9A%80&logo=GitHub&style=flat)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/kinsondigital/velaptor/%F0%9F%9A%80Preview%20Release?color=%23238636&label=Preview%20Release%20%F0%9F%9A%80&logo=github)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/kinsondigital.velaptor?label=Latest%20Release&logo=nuget)
</div>

<div align="center">

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/kinsondigital/velaptor/%E2%9C%94Unit%20Testing%20Status%20Check?color=%23238636&label=Unit%20Tests)
![](https://img.shields.io/codecov/c/github/KinsonDigital/Velaptor/master?label=Code%20Coverage&logo=CodeCov&style=flat)
![GitHub issues by-label](https://img.shields.io/github/issues/kinsondigital/velaptor/good%20first%20issue?color=%23238636&label=Good%20First%20Issues)
</div>

<div align="center">

![Discord](https://img.shields.io/discord/481597721199902720?color=%23575CCB&label=discord&logo=discord&logoColor=white)
![Twitter URL](https://img.shields.io/twitter/url?color=%235c5c5c&label=Follow%20%40KDCoder&logo=twitter&url=https%3A%2F%2Ftwitter.com%2FKDCoder)
</div>

<h2 style="font-weight:bold;border:0" align="center" >!! NOTICE !!</h2>

This library is still under development and is not at v1.0.0 yet!!  However, all of the major features are available, so we encourage you to use it and report back any issues and improvements you may have.  That is what open source is all about. 🥳

<h2 style="font-weight:bold;border:0" align="center">📖 About Velaptor</h2>

Velaptor is a 2D game development framework written in modern C# that strives to be simple and easy to use.  Game development can be difficult which is why **Velaptor** was developed.  It takes care of all of the lower level, more complicated things for creating a game like playing sound, loading graphics, managing content, multi-platform, and dealing with input, just to name a few.  This way you can just concentrate on developing your game or multi-media application.

<h2 style="font-weight:bold;border:0" align="center">✨Features</h2>

### **Cross-Platform**
We strive for **Velaptor** to be a cross platform library by running under **.NET v5.0**.  There are plans for this library to continually be updated as we approach **.NET 6.0** and beyond.

### **Easy To Use**
One of the goals of this library is to make sure that the it is easy to use.  Everything from naming, documentation and the usability of the how API needs to be simple and easy.

### **Flexible Content Loading System**
**Velaptor** has the ability to load texture atlas images and JSON data for texture atlas type rendering as well as single image and sound content.  The content loading system is flexible and has an API with the ability to be extended to load custom content for your media applications and games.

### **Content Caching**
Loaded content such as images and sounds are cached for performance reasons.  If the same content is attempting to be loaded from disk after it has already been loaded, that same content will be used from memory instead of being reloaded.  Sounds and texture atlas data have the same functionality.  Custom content loading will not come with caching and will have to be added manually.

### **Feature Demo Application**
https://user-images.githubusercontent.com/85414302/150527337-6d872768-73dc-4603-82e8-37c691c78d4d.mp4

<h2 style="font-weight:bold;border:0" align="center">🔧Maintainers</h2>

We currently have the following maintainers:
- [Calvin Wilkinson](https://twitter.com/KDCoder) [<img src="https://about.twitter.com/etc/designs/about2-twitter/public/img/favicon.ico" alt="Follow Calvin Wilkinson on Twitter" width="16" />](https://twitter.com/KDCoder) (GitHub Organization / Owner)
- [Kristen Wilkinson](https://twitter.com/kswilky) [<img src="https://about.twitter.com/etc/designs/about2-twitter/public/img/favicon.ico" alt="Follow Calvin Wilkinson on Twitter" width="16" />](https://twitter.com/KDCoder) (GitHub Organization / Documentation Maintainer / Tester)

<h2 style="font-weight:bold;border:0" align="center">📄Documentation</h2>

- Go to the [Table Of Contents](./Documentation/TableOfContents.md) for instructions on various topics such as:
  - Branching
  - Release Process
  - Environment Setup
  - and more . . .

<h2 style="font-weight:bold;border:0" align="center">🙏🏼Contributing</h2>

**Velaptor** encourages and uses [Early Pull Requests](https://medium.com/practical-blend/pull-request-first-f6bb667a9b6). Please don't wait until you're finished with your work before creating a PR.

1. Fork the repository.
2. Create a feature branch following the feature branch section in the documentation [here](./Documentation/Branching.md).
3. Add an empty commit to the new feature branch to start your work off.
   * Use this git command: `git commit --allow-empty -m "start work for issue #<issue-number-here>"`
   * Example: `git commit --allow-empty -m "start work for issue #123"`
4. Once you've pushed a commit, open a [**draft pull request**](https://github.blog/2019-02-14-introducing-draft-pull-requests/). Do this **before** you start working.
5. Make your commits in small, incremental steps with clear descriptions.
6. All unit tests must pass before a PR will be completed.
7. Make sure that the code follows the the coding standards.
   * Pay attention to the warnings in **Visual Studio**!!
   * Refer to the *.editorconfig* files in the code base for rules.
8. Tag a maintainer when you're done and ask for a review!

If you have any questions, please reach out to a project maintainer.

<h2 style="font-weight:bold;border:0" align="center">Practices</h2>

- The code base is highly tested by using unit tests while maintaining a high level of code coverage.  Manual testing is performed using the included testing application built specifically for manually testing the library.  When contributing, make sure to add or adjust the unit tests appropriately regarding your changes and perform manual testing.
- We use a combination of [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) and [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers) libraries for maintaining coding standards.
   - We understand that there are some exceptions to the rule and not all coding standards fit every situation.  In these scenarios, contact a maintainer and lets discuss it!!  Warnings can always be suppressed if need be.
- We use [semantic versioning 2.0](https://semver.org/) for versioning.
- Branching model below.
  - [Branching Diagram (GitHub Dark Mode)](./Documentation/Images/BranchingDiagram-DarkMode.png)
  - [Branching Diagram (GitHub Light Mode)](./Documentation/Images/BranchingDiagram-LightMode.png)

<h2 style="font-weight:bold;border:0" align="center">Further Resources</h2>

- The sample project named **VelaptorTesting** can be found in the [Testing Folder](https://github.com/KinsonDigital/Velaptor/tree/preview/master/Testing/VelaptorTesting)
  - This is a sample project for the purpose of performing manual testing of the library as well as understanding how to use it.
- [CASL](https://github.com/KinsonDigital/CASL) is used for audio.
- Powered by [Silk.NET](https://github.com/dotnet/Silk.NET).

<h2 style="font-weight:bold;border:0" align="center">Licensing And Governance</h2>

<div align="center">

[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg?style=flat)](code_of_conduct.md)
![GitHub](https://img.shields.io/github/license/kinsondigital/velaptor)
</dic>

**Velaptor** is distributed under the very permissive **MIT license** and all dependencies are distributed under MIT-compatible licenses.
This project has adopted the code of conduct defined by the **Contributor Covenant** to clarify expected behavior in our community.
