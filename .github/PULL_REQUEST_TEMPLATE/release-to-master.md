<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
    
    This PR is ONLY for merging release branches into the master branch to perform production releases
-->

## Production PR Description
This pull request performs a production release for version [add version here]

---

## Types of changes
<!-- What types of changes does your code introduce? Put an `x` in all the boxes that apply: -->
* [ ] Bug fixes (non-breaking change which fixes an issue)
* [ ] Contains new features (non-breaking change which adds functionality)
* [ ] Contains breaking changes (fix or feature that would cause existing functionality to change)

---

## Required Checklist (All Must Be Reviewed And Checked):
💡 PR will not be completed until this list is complete

<!-- Go over all the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
* [ ] PR title matches the example below but with proper version
  * Release To Production - v1.2.3
* [ ] The ***[add version here]*** text in the PR description replaced with the version.
* [ ] An issue exists using the production release template for this PR.
* [ ] This PR is only for bringing changes from ***release*** branches into the ***master*** branch.
* [ ] All tests passed.
* [ ] All issues have been linked to this PR.
  * 💡 These include all issues created and implemented in preview releases
* [ ] Update library version by updating the ***\<Version/\>*** and ***\<FileVersion/\>*** tags in the ***.csproj*** file.
  * 💡 Most of the time the major, minor, and patch numbers are correct and only the ***.preview.\<number\>*** section needs to be removed.
  * **Example:**
    ``` html
    <Version>1.2.3</Version>
    <FileVersion>1.2.3</FileVersion>
    ```

## After Release Checklist:
* [ ] After PR has been completed and release to production has finished, I created a tag of the version on the master branch at the merge commit following the example below and have pushed it to the remote.
  * **Example:** v1.2.3
* [ ] I have created a release that points to the newly created tag.
  * [ ] The release title follows the this example ***Velaptor - v1.2.3***
  * [ ] The release description contains only the release notes for this release.
