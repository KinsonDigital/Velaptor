<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->

<!-- Provide a short general summary of your changes in the Title above -->
## Preview Release PR Description
This pull request performs a preview release for version **_[add version here]_**

## How Has This Been Tested?
- [ ] Testing Application (Manual)
- [ ] No Testing Required

---

## Optional Checklist:
- [ ] Bug Fix (non-breaking change which fixes an issue)
- [ ] Breaking change (fix or feature that would cause existing functionality to change)
- [ ] I have changes related to workflows (CI/CD)
- [ ] My change requires a change to the documentation.
- [ ] I have added tests to cover my changes.
  - [ ] I have updated the documentation accordingly.
  - [ ] If changes to documentation have been made, the PR contains the **documentation** label.

---

## Required Checklist (All Must Be Reviewed And Checked):
<!-- Go over all the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] PR title matches the example below but with proper version
  * **Example:** 🚀Release To Preview - v1.2.3.preview.4
- [ ] The **_[add version here]_** text in the PR description replaced with the version.
- [ ] Issue[s] exists and is linked to this PR.
- [ ] This PR is only for bringing changes from a **_preview branch_** into a **_release_** branch
    - 💡 A **_preview branch_** is used for preview releases and has a syntax like **_preview/v\*.\*.*-preview.\*_**
      - **Example:** preview/v1.2.3-preview.4
    - 💡 A **_release branch_** has the syntax like **_release/v\*.\*.\*_**.
      - **Example:** release/v1.2.3
- [ ] My code follows the code style of this project.
- [ ] All tests passed locally.
  - 💡 Status checks are put in place to run unit tests every single time a change is pushed to a PR.  This does not mean that the tests pass in both the local and CI environment.
- [ ] Update library version by updating the **_\<Version/\>_** and **_\<FileVersion/\>_** tags in the **_.csproj_** file.
  - 💡 Every change to a PR will run a status check to confirm that the version has the correct syntax and does not already exist in the repository.
  - 💡 Make sure to add the **_.preview.\<number\>_** syntax to the end of the version
    - **Example:**
      ``` html
      <Version>1.2.3.preview.4</Version>
      <FileVersion>1.2.3.preview.4</FileVersion>
      ```
- [ ] A **_preview_** label has been added to the PR
