<div align="center">

![logo](https://raw.githubusercontent.com/KinsonDigital/Velaptor/preview/Images/velaptor-logo.png)
</div>


<h1 style="border:0;font-weight:bold" align="center">Velaptor</h1>


<div align="center">

[![Build PR Status Check](https://img.shields.io/github/actions/workflow/status/KinsonDigital/Velaptor/build-status-check.yml?label=%E2%9A%99%EF%B8%8FBuild)](https://github.com/KinsonDigital/Velaptor/actions/workflows/build-status-check.yml)
[![Unit Test PR Status Check](https://img.shields.io/github/actions/workflow/status/KinsonDigital/Velaptor/unit-test-status-check.yml?label=%F0%9F%A7%AATests)](https://github.com/KinsonDigital/Velaptor/actions/workflows/unit-test-status-check.yml)

[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=bugs)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=KinsonDigital_Velaptor&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=KinsonDigital_Velaptor)

[![Codecov](https://img.shields.io/codecov/c/github/KinsonDigital/Velaptor?label=Code%20Coverage&logo=codecov)](https://app.codecov.io/gh/KinsonDigital/Velaptor/tree/preview)

[![Latest Nuget Release](https://img.shields.io/nuget/vpre/kinsondigital.Velaptor?label=Latest%20Release&logo=nuget)](https://www.nuget.org/packages/KinsonDigital.Velaptor)
[![Nuget Downloads](https://img.shields.io/nuget/dt/KinsonDigital.Velaptor?color=0094FF&label=nuget%20downloads&logo=nuget)](https://www.nuget.org/stats/packages/KinsonDigital.Velaptor?groupby=Version)

[![Good First GitHub Issues](https://img.shields.io/github/issues/kinsondigital/Velaptor/good%20first%20issue?color=7057ff&label=Good%20First%20Issues)](https://github.com/KinsonDigital/Velaptor/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
[![Discord](https://img.shields.io/discord/481597721199902720?color=%23575CCB&label=chat%20on%20discord&logo=discord&logoColor=white)](https://discord.gg/qewu6fNgv7)
</div>

<h2 style="font-weight:bold;" align="center" >NOTICE</h2>

This library is still under development and is not at v1.0.0 yet! However, all major features are available, so we encourage you to use **Velaptor** and provide feedback. That is what open source is all about. 

<h2 style="font-weight:bold;" align="center">About Velaptor</h2>

**Velaptor** is a 2D game development framework written in modern C# that strives to be simple and easy to use. Game development can be difficult, which is one reason why **Velaptor** was developed. It takes care of all the lower-level, more complicated things for creating a game, like playing sound, loading graphics, managing content, multi-platform, and dealing with input, just to name a few. This way, you can concentrate on developing your game or multi-media application.

<h2 style="font-weight:bold;" align="center">Features</h2>

### **Cross-Platform**

Our architecture prioritizes cross-platform dependencies to deliver a consistent environment across all operating systems. In cases where unified libraries do not exist, Velaptor leverages native, platform-specific implementations without compromising the core API.

> [!NOTE]  
> Velaptor currently supports Windows, Linux, and macOS.  There are plans to get support for Android and iOS in the future.

### **Easy To Use**

One of the goals of this library is to make it easy to use. We aim to ensure consistent naming conventions, documentation, and first-class API usability.

### **Flexible Content Loading System**

**Velaptor** can load texture atlas data for its rendering, as well as single images and sound content. The content loading system is flexible and has an API that can be extended to load custom content for your games.

### **Content Caching**

Assets like images and sounds are cached in memory for optimal performance, preventing redundant disk I/O when the same content is requested again. For specialized workflows, you can easily build and integrate a custom content loader.

<h2 style="font-weight:bold;" align="center">Feature Demo </h2>

<div align="center">

[![FeatureDemoVideo](https://raw.githubusercontent.com/KinsonDigital/Velaptor/preview/Images/demo-img.png)](https://www.youtube.com/watch?v=WGUdT9NPfb0)

</div>

<br/>

<h2 style="font-weight:bold;" align="center">Documentation and Resources</h2>

Complete API documentation, tutorials, and blogs can be found at the [Velaptor docs](https://docs.velaptor.io) website. **Velaptor** is powered by [CASL](https://github.com/KinsonDigital/CASL) and [Silk.NET](https://github.com/dotnet/Silk.NET).

<h2 style="font-weight:bold;" align="center">Contributing</h2>

These projects are fueled by personal passion and are actively maintained with regular updates, improvements as well as a commitment to code quality. As the ecosystem grows, I am looking to transition from a solo effort to a community-backed model. Whether you are interested in [contributing code](https://github.com/KinsonDigital/.github/blob/main/docs/CONTRIBUTING.md) to help build new features or reporting issues and sharing feedback, your involvement makes a positive impact.

We encourage and use early pull requests. Please do not wait until you are finished with your work before creating a PR!  Click [here](https://carlosperez.medium.com/pull-request-first-f6bb667a9b6) to learn how to create an early pull request.

<h2 style="font-weight:bold;" align="center">💖 Support Velaptor Development</h2>

Velaptor is a 100% free, open-source 2D game development framework built for modern .NET. It is independently developed and maintained to bring high-performance, developer-friendly 2D game architecture to the C# community.

If Velaptor helps you build your games, saves you development time, or empowers your studio, consider supporting its ongoing maintenance and roadmap!

<p align="center">
  <a href="https://github.com/sponsors/KinsonDigital">
    <img src="https://img.shields.io/badge/Sponsor%20on%20GitHub-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white" alt="Sponsor on GitHub" />
  </a>
</p>

### 🎯 Current Funding Goals

- [ ] **Goal 1: $100 / mo** — **Infrastructure & Testing Hardware** *(Covers domain registration, CI/CD automated test runners, and multi-platform testing devices).*
- [ ] **Goal 2: $250 / mo** — **Sustained Maintenance** *(Guarantees dedicated weekly hours for issue triage, bug fixes, and core framework updates).*
- [ ] **Goal 3: $500 / mo** — **Advanced Features & Ecosystem Expansion** *(Funds major roadmap pushes like advanced shader pipelines, mobile/WASM support, and extended tooling).*

Prefer a different platform? You can also support via [Open Collective](https://opencollective.com/kinsondigital) or [Ko-fi](https://ko-fi.com/kinsondigital) — one-time or recurring.

<h2 style="font-weight:bold;" align="center">Maintainers</h2>

![x-logo-dark-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-dark-mode.svg#gh-dark-mode-only)
![x-logo-light-mode](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/x-logo-16x16-light-mode.svg#gh-light-mode-only)
[Calvin Wilkinson](https://x.com/KDCoder) (KinsonDigital GitHub Organization - Owner)

![bluesky-logo](https://raw.githubusercontent.com/KinsonDigital/.github/main/Images/bluesky-logo-16x16.svg)
[Calvin Wilkinson](https://bsky.app/profile/kdcoder.bsky.social) (KinsonDigital GitHub Organization - Owner)

<br/>

---

### 🏆 Current Sponsors

Special thanks to the amazing individuals and organizations keeping Velaptor alive:

<!-- github-sponsors-start -->

[![GitHub Avatar](https://github.com/imgrammy.png?size=50)](https://github.com/imgrammy)

<!-- github-sponsors-end -->

<h2 style="font-weight:bold;" align="center">Licensing And Governance</h2>


<div align="center">

[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg?style=flat)](https://github.com/KinsonDigital/.github/blob/main/docs/code_of_conduct.md)
[![GitHub](https://img.shields.io/github/license/kinsondigital/Velaptor)](https://github.com/KinsonDigital/Velaptor/blob/preview/LICENSE.md)
</div>


This software is distributed under the very permissive MIT license, and all dependencies are distributed under MIT-compatible licenses.
This project has adopted the code of conduct defined by the **Contributor Covenant** to clarify the expected behavior in our community.
