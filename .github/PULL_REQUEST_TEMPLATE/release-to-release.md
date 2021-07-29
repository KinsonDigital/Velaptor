<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->

<!-- Provide a short general summary of your changes in the Title above -->

## Preview Release PR Description
This pull request performs a preview release for version [add version here]

## Related Issue
<!-- This project only accepts pull requests related to open issues -->
<!-- If suggesting a new feature or change, please discuss it in an issue first -->
<!-- If fixing a bug, there should be an issue describing it with steps to reproduce -->
<!-- Please provide a link to the issue here and the issue should be linked to the pull request -->

## Motivation and Context
<!-- Why is this change required? What problem does it solve? -->

## How Has This Been Tested?
<!-- Please describe in detail how you tested your changes. -->
<!--
    Include details of your testing environment, and the tests you ran to
    see how your change affects other areas of the code, etc.
-->

## Screenshots (if appropriate):
---

## Types of changes
<!-- What types of changes does your code introduce? Put an `x` in all the boxes that apply: -->
<!--
    If this change is a change for a release branch, features are not aloud.
    Release branches are mostly for small changes and bug fixes and is meant for preview releases.
-->
* [ ] Bug fix (non-breaking change which fixes an issue)
* [ ] Breaking change (fix or feature that would cause existing functionality to change)

---

## Optional Checklist:
* [ ] My change requires a change to the documentation.
  * [ ] I have updated the documentation accordingly.
  * [ ] If changes to documentation have been made, the PR contains the **documentation** label.
* [ ] I have added tests to cover my changes.

---

## Required Checklist (All Must Be Reviewed And Checked):
<!-- Go over all the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
* [ ] PR title matches the example below but with proper version
  * Release To Production - v1.2.3.preview.4
* [ ] The ***[add version here]*** text in the PR description replaced with the version.
* [ ] An issue exists and is related to this PR.
* [ ] This PR is only for bringing changes from a non-version-release branch into a version-release branch
    * 💡 A version-release branch is the branch used for preview releases and has a syntax of ***release/v1.2.3***
    * 💡 A non-version-release is a branch prefixed with ***release/*** and does NOT have a version after it.  non-version-release branches bring fixes and changes to a prerelease.
* [ ] My code follows the code style of this project.
* [ ] All tests passed.
* [ ] Update library version by updating the ***\<Version/\>*** and ***\<FileVersion/\>*** tags in the ***.csproj*** file.
  * 💡 Make sure to add the ***.preview.\<number\>*** syntax to the end of the version
  * **Example:**
    ``` html
    <Version>1.2.3.preview.4</Version>
    <FileVersion>1.2.3.preview.4</FileVersion>
    ```
* [ ] ***preview*** label has been added to the PR
---

## After Release Checklist:
* [ ] After PR has been completed and release to production has finished, I created a tag of the version on the master branch at the merge commit following the example below and have pushed it to the remote.
  * **Example:** v1.2.3.preview.4
* [ ] I have created a release that points to the newly created tag.
  * [ ] The release title follows the this example ***Velaptor - v1.2.3.preview.4***
  * [ ] The release description contains only the release notes for this release.
