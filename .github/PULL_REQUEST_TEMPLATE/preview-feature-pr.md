<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS CAN CREATE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    Please use the "preview-feature-pr" pull request template if you have contributions to make.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Preview Feature Pull Request</h1>
<h2 style="font-weight:bold" align="center">✅Code Review Checklist✅</h2>

- [ ] The **_head(source)_** branch for this pull request is a **_preview feature_** branch, with the correct naming syntax, in the following example:
  - A **_head(source)_** branch is the branch being merged into a target branch.
    <details closed><summary>Example</summary>

      ``` xml
      Syntax: preview/feature/<issue-num>-<description>
      Example: preview/feature/123-my-preview-feature
      ```
    </details>

- [ ] The **_head(source)_** branch for this pull request is created from a **_preview release_** branch, with the correct naming syntax, in the following example:
  <details closed><summary>Example</summary>

    ``` xml
    Syntax: preview/v<major>.<minor>.<patch>-preview.<prev-num>
    Example: preview/v1.2.3-preview.4
    ```
  </details>

- [ ] The **_base(target)_** branch for this pull request is a **_preview release_** branch, with the correct naming syntax, in the following example:
  - A **_base(target)_** branch is the branch that the **_head(source)_** branch is merging into.

    <details closed><summary>Example</summary>

      ``` xml
      Syntax: preview/v<major>.<minor>.<patch>-preview.<prev-num>
      Example: preview/v1.2.3-preview.4
      ```
    </details>

💡For more information on branching, refer to the projects [branching documentation](../../Documentation/Branching.md).

- [ ] Pull request title matches the title of the associated issue.

- [ ] A [![preview-label](https://user-images.githubusercontent.com/85414302/150838564-33f6044b-55f9-4dd9-8783-1d739de9d92f.png)](https://github.com/KinsonDigital/Velaptor/labels/preview) label has been added to the pull request.

- [ ] Associated issue exists and is linked to this pull request.
  - One issue per pull request.

- [ ] The labels attached to this PR match the labels attached to the associated issue.

- [ ] My code follows the coding style of this project.
  - The style is enforced by the **_.editorconfig_** files in the project and displayed as warnings.  If there is an edge case, reach out and let's discuss it.

- [ ] I have manually tested my code changes to the best of my ability.

- [ ] All tests passed locally.
  - This is required because unit tests might pass locally but not in the CI environment during the status check process or vice-versa.
  - Tests might pass on the developer's machine but not necessarily on the code reviewer's machine.
  - This does not mean that the tests pass in both the local and CI environment.
  - Status checks run for this pull request when merging into the **_preview/v\*.\*.\*-preview.\*_** branch.  These status checks run every time a change is pushed to the pull request.  These checks validate version syntax, tagging, builds, unit tests, and more.
