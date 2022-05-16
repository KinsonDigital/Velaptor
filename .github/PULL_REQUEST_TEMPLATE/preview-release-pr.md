<!--
    !! NOTE !! - ONLY PROJECT OWNERS AND MAINTAINERS CAN CREATE PRODUCTION AND PREVIEW RELEASE PULL REQUESTS
    Please use the "preview-release-pr" pull request template if you have contributions to make.
-->
<!--suppress HtmlDeprecatedAttribute -->
<h1 style="font-weight:bold" align="center">Preview Release Pull Request</h1>
<h2 style="font-weight:bold" align="center">✅Code Review Checklist✅</h2>

- [ ] The **_head(source)_** branch for this pull request is a **_preview_** branch with the following example syntax.
  - A **_head(source)_** branch is the branch being merged into a target branch.
    <details closed><summary>Example</summary>

      ``` xml
      Syntax: preview/v<major>.<minor>.<patch>-preview.<prev-num>
      Example: preview/v1.2.3-preview.4
      ```
    </details>

- [ ] The **_base(target)_** branch for this pull request is a **_release_** branch with the following example syntax.
  - A **_base(target)_** branch is the branch that is a source branch being merged into it.
    <details closed><summary>Example</summary>

      ``` xml
      Syntax: release/v<major>.<minor>.<patch>
      Example: release/v1.2.3
      ```
    </details>

- [ ] The **_head(source)_** branch for this pull request is created from a **_release_** branch with the following example syntax.
  <details closed><summary>Example</summary>

    ``` xml
    Syntax: release/v<major>.<minor>.<patch>
    Example: release/v1.2.3
    ```
  </details>

- [ ] Pull request title matches the example below with the correct version.
  <details closed><summary>Example</summary>
    
    ``` xml
    Syntax: 🚀Release To Preview - v<major>.<minor>.<patch>-preview.<prev-num>
    Example: 🚀Release To Preview - v1.2.3-preview.4
    ```
  </details>

- [ ] A https://github.com/KinsonDigital/Velaptor/labels/%F0%9F%9A%80Preview%20Release label has been added to the this PR.

- [ ] A QA issue has been created and manual QA testing has been performed for the changes being released.
