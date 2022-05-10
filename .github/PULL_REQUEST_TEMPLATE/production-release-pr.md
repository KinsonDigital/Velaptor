<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS CAN CREATE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    Please use the "production-release-pr" pull request template if you have contributions to make.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Production Release Pull Request</h1>


<h2 style="font-weight:bold" align="center">✅Development Checklist✅</h2>

<details open><summary>🌳Branching🌳</summary>

The release branch name for this pull request has the following the syntax.

Syntax: _release/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;_  
Example: _release/v1.2.3_
- [ ] Yes
- [ ] No

The name of the branch for this pull request is merging into the production branch with the name **_master_**.
- [ ] Yes
- [ ] No

Is the head(source) branch for this pull request created from a branch with the name **_develop_**?
- [ ] Yes
- [ ] No
</details>


<details open><summary>🐛Bugs🐛</summary>

Contains Bug Fix(es)
- [ ] Yes
  - [ ] A ![bug-label](https://user-images.githubusercontent.com/85414302/150812958-10b202a8-84ae-45fb-b7cb-7f4fb68e0e8c.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>🧨Breaking Change(s)🧨</summary>

Any changes, including behavioral, that prevent a library user's application from compiling or behaving correctly.
Refer to this [link](https://docs.microsoft.com/en-us/dotnet/core/compatibility/#modifications-to-the-public-contract) for more information.
- [ ] Yes
  - [ ] A ![breaking-change-label](https://user-images.githubusercontent.com/85414302/154378943-8e684157-2138-404d-ba19-b9d76061c12e.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>✨Enhancements✨</summary>

Contains enhancements that add a feature or behavior.
- [ ] Yes
  - [ ] An ![enhancement-label](https://user-images.githubusercontent.com/85414302/150804213-bd043c7b-54d2-4562-ad3f-69a07723a5ef.png) label has been added to the pull request.
- [ ]  No
</details>


<details open><summary>⚙️Workflow (CI/CD) Changes⚙️</summary>

These changes can only done by the project maintainers.
- [ ] Yes
  - [ ] A ![workflow-label](https://user-images.githubusercontent.com/85414302/150814606-314933ca-86c7-4edb-99cb-62d2198b20d9.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>📃Documentation Updates📃</summary>

Contains changes that require updates to existing **_code_** and/or **_Velaptor product_** documentation.
- [ ] Yes
  - [ ] I have updated the documentation accordingly.
    - Choose the type of documentation.
    - [ ] Code Documentation
      - [ ] A ![documentation-code-label](https://user-images.githubusercontent.com/85414302/154672489-8079ed03-b8ff-41ff-9864-1e2ae55300cc.png) label has been added to the pull request.
    - [ ] Product Documentation
      - [ ] A ![documentation-product-label](https://user-images.githubusercontent.com/85414302/154672508-5ac50eb7-67f8-4cdf-92b9-fe5fcbd93b14.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>🧪Manual Testing🧪</summary>

I have manually tested my changes.
This can be done by using the included testing application.
- [ ] Yes
- [ ] No
</details>


<h2 style="font-weight:bold" align="center">✅Code Review Checklist✅</h2>

<!-- Go over all of the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] Pull request title matches the example below with the correct version.
  - **Example:** 🚀Release To Production - v1.2.3
- [ ] Issues exist and are linked to this pull request.
- [ ] My code follows the coding style of this project.
  - The style is enforced by the *.editorconfig* files in the project and displayed as warnings.  If there is an edge case with a coding style that should be ignored or changed, reach out, and let's discuss it.
- [ ] A ![production-label](https://user-images.githubusercontent.com/85414302/154683121-8d8d90d0-a714-46e3-b13a-8a78d483f6ed.png) label has been added to the pull request.
- [ ] All tests passed locally.
  - This is required because unit tests might pass locally but not in the CI environment during the status check process or vise-versa.
  - Tests might pass on the developer's machine but not necessarily on the code reviewer's machine.
  - This does not mean that the tests pass in both the local and CI environment.
  - Status checks run for this pull request when merging into the **_master_** branch.  These status checks run every time a change is pushed to the pull request.  These checks validate version syntax, tagging, builds, unit tests, and more.
- [ ] Update library version by updating the **_\<Version/\>_** and **_\<FileVersion/\>_** tags in the **_Velaptor_** **_.csproj_** file.
  - Every change to a pull request will run a status check to confirm that the version has the correct syntax, a tag does not exist, and that it has not already been published to [nuget](https://www.nuget.org/)
    **Example:**
    ``` html
    <Version>1.2.3</Version>
    <FileVersion>1.2.3</FileVersion>
    ```
- [ ] I have updated the release notes by creating a new production release notes file and adding it to the **_./Documentation/ReleaseNotes/ProductionReleases_** folder.
