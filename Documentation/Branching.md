<h1 align="center">Branching</h1>

**Velaptor** uses a more complicated branching model, but it gives you more control of the SDLC (Software Development Life Cycle).  This branching model allows a clear purpose for adding features, bug fixes, preview releases, QA releases, and standard releases.

As a standard contributor, all you have to worry about is creating <span style="color: #66B2FF;font-weight:bold">feature</span> branches and creating pull requests to merge those branches into the <span style="color: #FFB366;font-weight:bold">develop</span> branch.  The rest is taken care of by a solid CI/CD system as well as the maintainers of the project.  Only the organization owner and designated team members will manage the release process.  So, contributing is very easy!!🥳

**_NOTE_:** As you know, everything in software is subject to change, including the branching model.  If too many issues or complications occur with the current branching model and/or release process, it will be changed accordingly.

---

<h2 align="center">Branches Used</h2>


<h3 align="left" style="color: #82B366;font-weight:bold">Master Branch</h3>

Long living branch that represents stable production versions of **Velaptor**:
- **Branch Syntax:** master
- **Branches That Can Merge Into Master:**
  - <span style="color: #00CCCC;font-weight:bold">Release</span> branches via pull request
- **Created From:** none
- **Merges Into:** none
- **Environment:** Production
- **Required Casing:** all lowercase
- **CI/CD:**
  - Upon pull request completion, the <span style="color: #00CCCC;font-weight:bold">release</span> branches are merged into the <span style="color: #82B366;font-weight:bold">master</span> branch and are automatically built, tested, and released to production as a nuget package.
  - The testing application is attached as an artifact to the <span style="color: #00CCCC;font-weight:bold">release</span> branch for the purpose of testing.


<h3 align="left" style="color: #FFB366;font-weight:bold">Develop Branch</h3>

Long living branch that represents the most current development in progress:
- **Branch Syntax:** develop
- **Branches That Can Merge Into Develop Branch:**
  - <span style="color: #66B2FF;font-weight:bold">Feature</span> branches via pull requests
- **Created From:** none
- **Merges Into:** none
- **Environment:** QA
- **Required Casing:** all lowercase
- **CI/CD:**
  - Automatically built, tested, and deployed as a QA release upon pull request completion.
  - The testing application is attached as an artifact to the QA release for the purpose of testing.
   

<h3 align="left" style="color: #66B2FF;font-weight:bold">Feature Branches</h3>

Short living branch where a developer's work will be performed and merged back into the <span style="color: #FFB366;font-weight:bold">develop</span> branch via a pull request:
- **Branch Syntax:** feature/\<issue id\>-\<description\>
  - Example: feature/123-my-branch
- **Branches That Can Merge Into Feature Branches:** None
- **Created From:** <span style="color: #FFB366;font-weight:bold">develop</span>
- **Merges Into:** <span style="color: #FFB366;font-weight:bold">develop</span>
- **Environment:** none
- **Required Casing:** all lowercase
- **CI/CD:**
  - Build and unit test status checks are automatically run for each change to the pull request.
  - All status checks must pass before a pull request will be completed.


<h3 align="left" style="color: #B84949;font-weight:bold">Hotfix Branches</h3>

Short living branch where urgent bug fixes or changes will be performed:

**_NOTE_:** Hotfix branches should be carefully reviewed and only used when the software is considered **broken** and/or **unusable**.  Changes to this branch should be absolutely minimal and merged directly into the <span style="color: #82B366;font-weight:bold">master</span> branch via a pull request.
- **Branch Syntax:** hotfix/\<issue id\>-\<description\>
  - Example: hotfix/123-my-hotfix
- **Branches That Can Merge Into Hotfix Branches:** none
- **Created From:** <span style="color: #82B366;font-weight:bold">master</span>
- **Merges Into:** <span style="color: #82B366;font-weight:bold">master</span>
- **Environment:** none
- **Required Casing:** all lowercase
- **CI/CD:**
  - Build and unit test status checks are automatically run for each change to the pull request.
  - All status checks must pass before a pull request will be completed.


<h3 align="left" style="color: #00CCCC;font-weight:bold">Release Branches</h3> 

Represents <span style="color: #66B2FF;font-weight:bold">features</span> and/or bug fixes to be released as a <span style="color: #82B366;font-weight:bold">production</span> or <span style="color: #CC99FF;font-weight:bold">preview</span> release:
- **Branch Syntax:** release/v\<major\>.\<minor\>.\<patch\>
  - Example: release/v1.2.3
- **Branches That Can Merge Into Release Branches:**
  - <span style="color: #CC99FF;font-weight:bold">Preview</span> branches via pull request
- **Created From:** <span style="color: #FFB366;font-weight:bold">develop</span> branch
- **Merged Into:** <span style="color: #FFB366;font-weight:bold">develop</span> and <span style="color: #82B366;font-weight:bold">master</span> branches
- **Environment:** none
- **Required Casing:** all lowercase
- **CI/CD:**
  - Can be a major, minor, or patch release.
  - Can be used for preview releases.
  - Preview releases are only done manually.
  - Build, unit test, and version validation status checks are automatically run for each change to the pull request.
  - All status checks must pass before a pull request will be completed.
  - When a release is performed, 2 pull requests are created.  One for a merge into the <span style="color: #FFB366;font-weight:bold">develop</span> branch and one for a merge into the <span style="color: #82B366;font-weight:bold">master</span> branch.
  - Upon merging into the <span style="color: #FFB366;font-weight:bold">develop</span> (QA) branch, a QA release will be automatically performed.
  - Upon merging into the <span style="color: #82B366;font-weight:bold">master</span> (Production) branch, a production release will be automatically performed.


<h3 align="left" style="color: #CC99FF;font-weight:bold">Preview Branches</h3>

Holds minimal changes for upcoming production release stability.

**_NOTE_:** Used for refactoring, bug fixes, and changes related to making an upcoming release more stable and to give users the chance to utilize the software and provide feedback before a major release.  Introducing major features outside of the changes in the upcoming release is not allowed. These kinds of changes are performed on the <span style="color: #CC99FF;font-weight:bold">preview</span> branch by using <span style="color: #9E269E;font-weight:bold">preview feature</span> branches.
- **Branch Syntax:** preview/v\<major\>.\<minor\>.\<patch\>-preview.\<prev number\>
  - Example: preview/v1.2.3-preview.4
- **Branches That Can Merge Into Preview Branches:** <span style="color: #9E269E;font-weight:bold">preview feature</span> branches
- **Created From:** <span style="color: #00CCCC;font-weight:bold">release</span> branches
- **Merged Into:** <span style="color: #00CCCC;font-weight:bold">release</span> branches
- **Environment:** none
- **Required Casing:** all lowercase
- **CI/CD:**
  - The major, minor, and patch numbers of the preview branch and the release branch it was created from, must match. 
  - Build, unit test, and version validation status checks are automatically run for each change to the pull request.
  - All status checks must pass before a pull request will be completed.


<h3 align="left" style="color: #9E269E;font-weight:bold">Preview Feature Branches</h3>

Where a developer's work will be performed when implementing changes for a <span style="color: #CC99FF;font-weight:bold">preview</span> branch via a pull request.
- **Branch Syntax:** preview/feature/\<issue id\>-\<description\>
  - Example: preview/feature/123-my-branch
- **Branches That Can Merge Into Preview Feature Branches:** none
- **Created From:** <span style="color: #CC99FF;font-weight:bold">preview</span> branches
- **Merged Into:** <span style="color: #CC99FF;font-weight:bold">preview</span> branches
- **Environment:** none
- **Required Casing:** all lowercase
- **CI/CD:**
  - Build and unit test status checks are automatically run for each change to the pull request.
  - All status checks must pass before a pull request will be completed.

---

<h2 align="center">
   <div>
      <span style="font-weight:bold">Branching Diagram</span>
   </div>

![BranchingDiagram](./Images/BranchingDiagram-DarkMode-v1.1.png#gh-dark-mode-only)
![BranchingDiagram](./Images/BranchingDiagram-LightMode-v1.1.png#gh-light-mode-only)
</h2>

<div align="right">

   [< Linux Dev Env Setup](./EnvironmentSetup/LinuxDevEnvSetup.md)
   <br/>
</div>
