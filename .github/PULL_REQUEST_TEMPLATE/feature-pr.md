<!--
    !! NOTE !! - ANY CONTRIBUTOR CAN CREATE FEATURE PULL REQUESTS
    Please use the "feature-pr" pull request template if you have contributions to make.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Feature Pull Request</h1>

<details><summary>📄Description📄</summary>
<!-- Provide a short, general summary of your changes in the Title above -->

</details>


<h2 style="font-weight:bold" align="center">✅Development Checklist✅</h2>

<details open><summary>🌳Branching🌳</summary>

Does the name of the head(source) branch for this pull request have the correct **_feature branch_** syntax?

Syntax: _feature/&lt;issue-id&gt;-&lt;description&gt;_  
Example: _feature/123-my-changes_
- [ ] Yes
- [ ] No

The pull request is merging into a branch named **_develop_**.
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


<details open><summary>🧪Unit Testing🧪</summary>

My change requires unit tests.
- [ ] Yes
- [ ] No
</details>


<details open><summary>🧪Manual Testing🧪</summary>

- [ ] Yes
- [ ] No
</details>


<h2 style="font-weight:bold" align="center">✅Code Review Checklist✅</h2>

<!-- Go over all of the following points, and put an `x` in all the boxes that apply. -->
<!-- If you're unsure about any of these, don't hesitate to ask. We're here to help! -->
- [ ] Pull request title matches the example below with the correct version.
  - **Example:** 🚀Release To Preview - v1.2.3-preview.4
- [ ] Issues exist and are linked to this pull request.
- [ ] My code follows the coding style of this project.
  - The style is enforced by the *.editorconfig* files in the project and displayed as warnings.  If there is an edge case with a coding style that should be ignored or changed, reach out, and let's discuss it.
- [ ] All tests passed locally.
  - This is required because unit tests might pass locally but not in the CI environment during the status check process or vise-versa.
  - Tests might pass on the developer's machine but not necessarily on the code reviewer's machine.
  - This does not mean that the tests pass in both the local and CI environment.
  - Status checks run for this pull request when merging into the **_develop_** branch.  These status checks run every time a change is pushed to the pull request.  These checks validate version syntax, tagging, builds, unit tests, and more.
