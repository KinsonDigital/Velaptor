<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Preview Release Pull Request</h1>


<h2 style="font-weight:bold" align="center">✅Development Checklist✅</h2>

<details open><summary>🌳Branching🌳</summary>

Does the name of the head(source) branch for this pull request have the correct **_preview branch_** syntax?

Syntax: _preview/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;-preview.&lt;prev-number&gt;_  
Example: _preview/v1.2.3-preview.4_
- [ ] Yes
- [ ] No

Does the name of the base(target) branch for this pull request have the correct **_release branch_** syntax?

Syntax: _release/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;_  
Example: _release/v1.2.3_
- [ ] Yes
- [ ] No

Is the head(source) branch for this pull request created from a branch with the correct **_release branch_** syntax?

Syntax: _release/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;_  
Example: _release/v1.2.3_
- [ ] Yes
- [ ] No
</details>


<details open><summary>🐛Bugs🐛</summary>

Contains Bug Fix(es)
- [ ] Yes
    - [ ] A ![bug-label](https://user-images.githubusercontent.com/85414302/150812958-10b202a8-84ae-45fb-b7cb-7f4fb68e0e8c.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>💣Breaking Change(s)💣</summary>

Contains [breaking change(s)](https://docs.microsoft.com/en-us/dotnet/core/compatibility/#modifications-to-the-public-contract)
- [ ] Yes
    - [ ] A ![breaking-change-label](https://user-images.githubusercontent.com/85414302/154378943-8e684157-2138-404d-ba19-b9d76061c12e.png) label has been added to the pull request.
- [ ] No
</details>


<details open><summary>✨Enhancements✨</summary>

Contains enhancement(s) that add a feature or behavior.
- [ ] Yes
    - [ ] An ![enhancement-label](https://user-images.githubusercontent.com/85414302/150804213-bd043c7b-54d2-4562-ad3f-69a07723a5ef.png) label has been added to the pull request.
- [ ]  No
</details>


<details open><summary>⚙️Workflow (CI/CD) Changes⚙️</summary>

Contains changes to workflow files. These changes can only done by the project maintainers.
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

I have manually tested my changes. (This can be done by using project named **_VelaptorTesting_**).
- [ ] Yes
- [ ] No
</details>


<h2 style="font-weight:bold" align="center">✅Code Review Checklist✅</h2>

<!-- Go over all of the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] Pull request title matches the example below with the correct version.
    - Example: 🚀Release To Preview - v1.2.3-preview.4
- [ ] Issues exist and are linked to this pull request.
- [ ] My code follows the coding style of this project.
    - This is enforced by the *.editorconfig* files in the project and displayed as warnings.  If there is an edge case with coding style that should be ignored or changed, reach out and let's discuss it.
- [ ] All tests passed locally.
    - Status checks are put in place to run unit tests every single time a change is pushed to a pull request.  This does not mean that the tests pass in both the local and CI environment.
- [ ] Update library version by updating the **_\<Version/\>_** and **_\<FileVersion/\>_** tags in the **_Velaptor_** **_.csproj_** file.
    - Every change to a pull request will run a status check to confirm that the version has the correct syntax, a tag does not exist, and that it has not already been published to [nuget](https://www.nuget.org/)
    - Make sure to add the **_-preview.\<number\>_** syntax to the end of the version.
      Example:
        ``` html
        <Version>1.2.3-preview.4</Version>
        <FileVersion>1.2.3-preview.4</FileVersion>
        ```
- [ ] I have updated the release notes by creating a new preview release notes file and adding it to the **_./Documentation/ReleaseNotes/PreviewReleases_** folder.
