<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->

<!-- Provide a short general summary of your changes in the Title above -->
## Preview Release PR Description
This pull request performs a preview release for version **_[add version here]_**

## How Has This Been Tested?
- [ ] Testing Application (Manual)
- [ ] No Testing Required

---

## Development Checklist:
**Types Of Changes:**
- Bug Fix(es)
  - [ ] A **_bug_** label has been added to the PR
- [ ] Contains breaking change(s)
  - 💡 This would force library users to change their code.  This would involve a public facing API change, behavior that would force a change to the users code base, or even an update in behavior that could force a change to the library users code base.  Sometimes this it is unclear if the change is a breaking change.  If it is unclear, reach out so we can discuss and investigate if it is indeed a breaking change.
- Additional feature and/or behavior added.~~~~
  - [ ] Yes
    - An **_enhancement_** label has been added to the PR
      - [ ] Yes
      - [ ] No
  - [ ] No
- I have changes related to workflows (CI/CD)
  - 💡 These kinds of changes are only done by the project owner and maintainers that are allowed to make changes to workflows
  - [ ] Yes
    - A **_workflow_** label has been added to the PR
      - [ ] Yes
      - [ ] No
  - [ ] No

**Documentation:**
- [ ] My change requires a change to the documentation.
  - [ ] I have updated the documentation accordingly.
  - [ ] A **_documentation_** label has been added to the PR

**Testing:**
- My change requires unit tests to be written
  - [ ] Yes
    - [ ] I have added tests to cover my changes.
  - [ ] No
- [ ] I have manually tested my changes to the best of my ability
  - NOTE: This can be done by using the included testing application

---

## Review Checklist:
<!-- Go over all the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] PR title matches the example below with the correct version
  * 💡 **Example:** 🚀Release To Preview - v1.2.3-preview.4
- [ ] The **_[add version here]_** text in the PR description replaced with the version.
- [ ] Issues exist and are linked to this PR.
- [ ] My code follows the coding style of this project.
  - 💡 This is enforced by the *.editorconfig* files in the project and displayed as warnings.  If there is an edge case with coding style that should be ignored or changed, reach out and lets discuss it.
- [ ] All tests passed locally.
  - 💡 Status checks are put in place to run unit tests every single time a change is pushed to a PR.  This does not mean that the tests pass in both the local and CI environment.
- [ ] Update library version by updating the **_\<Version/\>_** and **_\<FileVersion/\>_** tags in the **_Velaptor_** **_.csproj_** file.
  - 💡 Every change to a PR will run a status check to confirm that the version has the correct syntax, a tag does not exist, and that it has not already been published to nuget.org
  - 💡 Make sure to add the **_-preview.\<number\>_** syntax to the end of the version
    - **Example:**
      ``` html
      <Version>1.2.3-preview.4</Version>
      <FileVersion>1.2.3-preview.4</FileVersion>
      ```
- [ ] I have updated the release notes by creating a new preview release notes file and adding it to the **_./Documentation/ReleaseNotes/PreviewReleases_** folder
- [ ] A **_preview_** label has been added to the PR
