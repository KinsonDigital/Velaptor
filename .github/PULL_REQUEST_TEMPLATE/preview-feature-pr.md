<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS MANAGE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    If you have contributions to make, use the "feature-to-develop" pull request template.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Preview Feature Pull Request</h1>


<h2 style="font-weight:bold" align="center">✅Development Checklist✅</h2>

<details open><summary>🌳Branching🌳</summary>

Does the name of the head(source) branch for this pull request have the correct **_preview feature branch_** syntax?

Syntax: _preview/feature/&lt;issue-id&gt;-&lt;description&gt;_  
Example: _preview/feature/123-my-preview-change_
- [ ] Yes
- [ ] No
 
Does the name of the base(target) branch for this pull request have the correct **_preview branch_** syntax?

Syntax: _preview/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;-preview.&lt;prev-number&gt;_  
Example: _preview/v1.2.3-preview.4_
- [ ] Yes
- [ ] No

Is the head(source) branch for this pull request created from a branch with the correct **_preview branch_** syntax?

Syntax: _preview/v&lt;major-number&gt;.&lt;minor-number&gt;.&lt;patch-number&gt;-preview.&lt;prev-number&gt;_  
Example: _preview/v1.2.3-preview.4_
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


<details open><summary>🧪Unit Testing🧪</summary>

My change requires unit tests to be written.
- [ ] Yes
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
- [ ] Pull request title matches the title of the linked issue.
- [ ] Associated issue exists and is linked to this pull request.
    - One issue per pull request.
- [ ] My code follows the coding style of this project.
    - This is enforced by the *.editorconfig* files in the project and displayed as warnings.  If there is an edge case with coding style that should be ignored or changed, reach out and let's discuss it.
- [ ] A ![preview-label](https://user-images.githubusercontent.com/85414302/150838564-33f6044b-55f9-4dd9-8783-1d739de9d92f.png) label has been added to the pull request.
- [ ] All tests passed locally.
    - This extra check is required because unit tests might pass locally but not in the CI environment during the status check process or vise-versa.  
      Tests might pass on the developer's machine but not necessarily on the code reviewer's machine.
    - Status checks are put in place for pull requests merging into the following branches.  These checks are run every single time a change is pushed to a pull request.  
      These checks validate things such as version syntax, tagging, builds, unit tests and more.
        - master
        - develop
        - release/v\*.\*.*
        - v\*.\*.\*-preview.*
