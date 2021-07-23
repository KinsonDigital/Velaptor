<h2 align="center">
    <a href="#"><img align="center" src="./Documentation/Images/velaptor-logo.png" height="96"></a>
    <br />
</h2>

<h1 style="font-weight:bold" align="center">Velaptor</h1>

<div align="center">

[![codecov](https://codecov.io/gh/KinsonDigital/Velaptor/branch/feature/master/graph/badge.svg?token=gkqbQI7oCM)](https://codecov.io/gh/KinsonDigital/Velaptor)
[![Build](https://github.com/KinsonDigital/Velaptor/actions/workflows/main.yml/badge.svg)](https://github.com/KinsonDigital/Velaptor/actions/workflows/main.yml)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg)](code_of_conduct.md)

[![nuget-package](https://img.shields.io/badge/nuget-windows-orange)](asdf)
</div>

<h2 align="center" style="font-weight:bold">
    !!NOTE!!
</h2>
This library is still under develop and is not at v1.0.0 yet!!  All of the major features are there though so we encourage you to use it and report back on issues and improvements.  That is what open source is all about which can improve the library!!

<h1 style="font-weight:bold" align="center">Features</h1>

### Cross-Platform
We strive for **Velaptor** to be a cross platform library by running under **.NET v5.0**.  There are plans for this library to continually be updated as we approach **.NET 6.0** and beyond.

### Easy to use
One of the goals of this project is to make sure that the engine is easy to use.  Everything from naming, documentation and how the API is used needs to be simple and easy to use.

### Flexible Content Loading System
**Velaptor** has the ability to load texture atlas image and JSON data for texture atlas type rendering as well as single image content and sound content.  The content loading system is flexible and has an API that has the ability to be extended to load custom content for your media applications and games.

### Content Caching
Loaded content such as images and sounds are cached for reuse and performance reasons.  If the same content is loaded from disk after it has already been loaded, the same image content will be used for rendering.  This is the same for sounds and texture atlas data.

<h1 style="font-weight:bold" align="center">Maintainers</h1>

We currently have the following maintainers:
- [Calvin Wilkinson](https://github.com/Perksey) [<img src="https://about.twitter.com/etc/designs/about2-twitter/public/img/favicon.ico" alt="Follow Calvin Wilkinson on Twitter" width="16" />](https://twitter.com/KDCoder)

<h1 style="font-weight:bold" align="center">Building And Dev Environment Setup</h1>

1. Click [here](./EnvironmentSetup/WindowsDevEnvSetup.md) for instructions on development and environment setup for windows.

2. Building on linux machines still needs to be setup and defined.

<h1 style="font-weight:bold" align="center">Contributing</h1>

**Velaptor** encourages and uses [Early Pull Requests](https://medium.com/practical-blend/pull-request-first-f6bb667a9b6). Please don't wait until you're finished with your work before creating a PR.

1. We use a simple branching strategy that has 5 branches
   * <span style="color: #52D1A4;font-weight:bold">master</span> - This is is a long living branch for the purpose of stable production versions of the library.  Stable nuget packages are created from this branch.
   * <span style="color: #7BB0FC;font-weight:bold">develop</span> - This branch holds the current development work being produced for an upcoming release
   * <span style="color: #B18BE4;font-weight:bold">feature/my-features</span> - This branch is where development work will be performed and will be merged into the <span style="color: #7BB0FC;font-weight:bold">develop</span> or <span style="color: #E5D89C;font-weight:bold">release</span> branches via pull requests.  When this is created from the <span style="color: #7BB0FC;font-weight:bold">develop</span> branch, this will only be merged back into the <span style="color: #7BB0FC;font-weight:bold">develop</span> branch for fixing bugs or adding new features.  When created from a <span style="color: #E5D89C;font-weight:bold">release</span> branch, it will only be merged back into the <span style="color: #E5D89C;font-weight:bold">release</span> branch.  PR's tagged with **pre-release** will be used to make it easy to know that the <span style="color: #B18BE4;font-weight:bold">feature</span> branch's purpose is for making changes to a **pre-release**.
   * <span style="color: #E5D89C;font-weight:bold">release/v1.2.3</span> - This branch is created from the <span style="color: #7BB0FC;font-weight:bold">develop</span> branch to encompass a number of features that could be released to production and is where extensive testing will occur before a release.  Pre-release nuget packages will be created from this branch until deemed to be ready for a production release.  Once ready, this branch gets merged into the master branch which then creates a stable production release of the library.  This will also get merged into the develop branch to update current development and merge conflicts resolved.
2. Fork the repository
3. Add an empty commit to a new branch to start your work off: `git commit --allow-empty -m "start of [thing you're working on]"`
4. Once you've pushed a commit, open a [**draft pull request**](https://github.blog/2019-02-14-introducing-draft-pull-requests/). Do this **before** you actually start working.
5. Make your commits in small, incremental steps with clear descriptions.
6. All unit tests must pass before a PR will be completed.
7. Make sure that code follows the project set coding standards.
8. Tag a maintainer when you're done and ask for a review!

<h1 align="center">
   <div>
      <span>Branching Diagram</span>
   </div>
   <a href="#"><img style="width: 35%" align="center" src="Documentation/Images/github-branch-diagram.png"></a>
   <br/>
</h1>

<h2 align="left">Practices</h2>

1. The code base is highly tested using unit testing and a high level of code coverage is maintained.  When contributing, make sure to add or adjust the unit tests appropriately regarding your changes.
2. We use a combination of [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) and [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers) libraries for maintaining coding standards.
   * We understand that there are some exceptions to the rule and not all coding standards fit every situation.  In these scenarios, contact a maintainer and lets discuss it!!  Warnings can always be suppressed if need be.

<h1 style="font-weight:bold" align="center">Further Resources</h1>

- The sample project named **VelaptorPlayground** can be found in the [Testing Folder](https://github.com/KinsonDigital/CASPL/tree/master/Testing/VelaptorPlayground)
  * This project serves the purpose of a sample project as well as a simple way to do manual testing of the library
- [CASL](https://github.com/KinsonDigital/CASL) is used for audio.
- [Silk.NET](https://github.com/dotnet/Silk.NET) is used under the hood for rendering.

<h1 style="font-weight:bold" align="center">Licensing And Governance</h1>

**Velaptor** is distributed under the very permissive MIT license and all dependencies are distributed under MIT-compatible licenses.
This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community.
