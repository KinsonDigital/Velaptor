
<div align="center">
    <a href="#"><img align="center" src="./Documentation/Images/velaptor-logo.png" height="96"></a>
    <br />
  
</div>


<h1 style="border:0;font-weight:bold" align="center">Velaptor</h1>


<div align="center">

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/KinsonDigital/Velaptor/prod-release-pr-status-check.yml?color=2F8840&label=Prod%20CI%20Build&logo=GitHub)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/KinsonDigital/Velaptor/prev-release-pr-status-check.yml?color=2F8840&label=Preview%20CI%20Build&logo=GitHub)

![Codacy grade](https://img.shields.io/codacy/grade/ac26b169bfb54e0fad4433d538e7d397?color=2F8840&logo=codacy)
![Codecov](https://img.shields.io/codecov/c/github/KinsonDigital/Velaptor?color=2F8840&label=Code%20Coverage&logo=codecov)

[![Latest Nuget Release](https://img.shields.io/nuget/vpre/kinsondigital.Velaptor?label=Latest%20Release&logo=nuget)](https://www.nuget.org/packages/KinsonDigital.Velaptor)
![Nuget](https://img.shields.io/nuget/dt/KinsonDigital.Velaptor?color=0094FF&label=nuget%20downloads&logo=nuget)

[![Good First GitHub Issues](https://img.shields.io/github/issues/kinsondigital/Velaptor/good%20first%20issue?color=7057ff&label=Good%20First%20Issues)](https://github.com/KinsonDigital/Velaptor/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
[![Discord](https://img.shields.io/discord/481597721199902720?color=%23575CCB&label=chat%20on%20discord&logo=discord&logoColor=white)](https://discord.gg/qewu6fNgv7)
</div>


<h2 style="font-weight:bold;" align="center" >!! NOTICE !!</h2>

This library is still under development and is not at v1.0.0 yet!!  However, all of the major features are available, so we encourage you to use it and provide feedback.  That is what open source is all about. 🥳

<br/>

<h2 style="font-weight:bold;" align="center">📖 About Velaptor 📖</h2>

Velaptor is a 2D game development framework written in modern C# that strives to be simple and easy to use.  Game development can be difficult which is why **Velaptor** was developed.  It takes care of all of the lower level, more complicated things for creating a game like playing sound, loading graphics, managing content, multi-platform, and dealing with input, just to name a few.  This way you can just concentrate on developing your game or multi-media application.

<h2 style="font-weight:bold;" align="center">✨ Features ✨</h2>

### **Cross-Platform**
We strive for **Velaptor** to be a cross platform. We do this by using as many cross platform dependencies as possible. If a cross platform dependency cannot be found, then the appropriate platform specific implementations will be used.

### **Easy To Use**
One of the goals of this library is to make it easy to use.  We aim for naming, documentation and usability of the API to be first class.

### **Flexible Content Loading System**
**Velaptor** has the ability to load texture atlas data for its rendering as well as single image and sound content.  The content loading system is flexible and has an API with the ability to be extended to load custom content for your media applications and games.

### **Content Caching**
Loaded content such as images and sounds are cached for better performance.  If the same content is attempting to be loaded from its source after it has already been loaded, it will be used from memory instead of being reloaded. You can also load content by building your own content loader. 

<h2 style="font-weight:bold;" align="center">📽️ Feature Demo 📽️</h2>

<div align="center">

[![FeatureDemoVideo](https://img.youtube.com/vi/rcKi-eWeUuo/0.jpg)](https://www.youtube.com/watch?v=rcKi-eWeUuo)
</div>

<br/>

<h2 style="font-weight:bold;" align="center">📃 Documentation and Resources 📃</h2>

  Full API documentation and tutorials can be found at [docs.velaptor.io](https://docs.velaptor.io). Velaptor is powered by [CASL](https://github.com/KinsonDigital/CASL) and [Silk.NET](https://github.com/dotnet/Silk.NET).

<br/>

<h2 style="font-weight:bold;" align="center">🙏🏼 Contributing 🙏🏼</h2>

Interested in contributing? If so, click [here](https://github.com/KinsonDigital/.github/blob/master/docs/CONTRIBUTING.md) to learn how to contribute your time or [here](https://github.com/sponsors/KinsonDigital) if you are interested in contributing your funds via one-time or recurring donation.


<h2 style="font-weight:bold;" align="center">🔧 Maintainers 🔧</h2>

- [![twitter-logo](https://raw.githubusercontent.com/KinsonDigital/.github/master/Images/twitter-logo-16x16.svg)Calvin Wilkinson](https://twitter.com/KDCoder) (KinsonDigital GitHub Organization - Owner)
- [![twitter-logo](https://raw.githubusercontent.com/KinsonDigital/.github/master/Images/twitter-logo-16x16.svg)Kristen Wilkinson](https://twitter.com/kswilky) (KinsonDigital GitHub Organization - Project Management, Documentation, Tester)

<br/>

<h2 style="font-weight:bold;" align="center">🚔 Licensing And Governance 🚔</h2>


<div align="center">

[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg?style=flat)](code_of_conduct.md)
![GitHub](https://img.shields.io/github/license/kinsondigital/velaptor)
</div>


This software is distributed under the very permissive [MIT license](https://github.com/KinsonDigital/Velaptor/blob/release/v1.0.0/LICENSE.md) and all dependencies are distributed under MIT-compatible licenses.
This project has adopted the code of conduct defined by the **Contributor Covenant** to clarify expected behavior in our community.
