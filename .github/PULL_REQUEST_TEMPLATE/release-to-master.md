<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->

<!-- Provide a short general summary of your changes in the Title above -->
## Production Release PR Description
This pull request performs a production release for version **_[add version here]_**

## How Has This Been Tested?
- [ ] Testing Application (Manual)
- [ ] No Testing Required

---

## Required Checklist (All Must Be Reviewed And Checked):
<!-- Go over all the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] PR title matches the example below but with proper version
  * **Example:** 🚀Release To Production - v1.2.3
- [ ] The **_[add version here]_** text in the PR description replaced with the version.
- [ ] Issue[s] exists and is linked to this PR.
- [ ] This PR is only for bringing changes from a **_release branch_** into the **_master_** branch
    - 💡 A **_release_** branch is the branch used for production releases and has a syntax of **_release/v\*.\*.\*_**
      - **Example:** release/v1.2.3
- [ ] My code follows the code style of this project.
- [ ] All tests passed locally.
  - 💡 Status checks are put in place to run unit tests every single time a change is pushed to a PR.  This does not mean that the tests pass in both the local and CI environment.
- [ ] Update library version by updating the **_\<Version/\>_** and **_\<FileVersion/\>_** tags in the **_.csproj_** file.
  - 💡 Every change to a PR will run a status check to confirm that the version has the correct syntax, a tag does not exist, and that it has not already been published to nuget.org
  - 💡 Make sure to remove the **_-preview.\<number\>_** section from the end of the version
    - **Example:**
      - The versions should be changed from:
        ``` html
        <Version>1.2.3-preview.4</Version>
        <FileVersion>1.2.3-preview.4</FileVersion>
        ```
      - To this:
        ``` html
        <Version>1.2.3</Version>
        <FileVersion>1.2.3</FileVersion>
        ```
- [ ] I have updated the release notes by creating a new production release notes file and adding it to the **_./Documentation/ReleaseNotes/ProductionReleases_** folder