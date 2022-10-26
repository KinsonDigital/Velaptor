<h1 style="border:0;font-weight:bold" align="center">Contribution Guide</h1>

Hello!!  Thanks for your interest in contributing to **Velaptor**!!  Any help you can provide is greatly appreciated.  We are looking for ways to make the contribution process as simple and as straightforward as possible. In addition, we want to make sure that the required workflow and development process restrictions are followed to ensure high software quality.

If you have any ideas and/or comments on how to simplify the process for outside contributions, please don't hesitate to contact one of the project maintainers.

Below you will find a detailed step-by-step guide on how to contribute to the project!!

<h2 style="border:0;font-weight:bold" align="center">Branching</h2>

<details closed><summary>TLDR</summary>

Go [here](./Branching.md) for more information on <span style="color: #66B2FF;font-weight:bold">feature</span> and <span style="color: #9E269E;font-weight:bold">preview feature</span> branches.

When it comes to branching, this project follows a strict branch naming policy.  The purpose of this is to make sure that consistent branch names will not only keep things clear and tie together the information in GIT to the issues and pull requests in GitHub but also help provide our CI/CD system with a means to perform its job such as validating issue and PR setup as well as running PR status checks.

The branching model for the project is strict and more complicated than most OS projects but this is for good reason.  The good news is that as an outside contributor, you only have to worry about 2 types of branches.

Before you decide if you should make a <span style="color: #FFB366;font-weight:bold">develop</span> or <span style="color: #9E269E;font-weight:bold">preview feature</span> branch, you need to figure out if the project is currently in preview.  If the software is in preview, you create a <span style="color: #9E269E;font-weight:bold">preview feature</span> branch from the latest <span style="color: #CC99FF;font-weight:bold">preview</span> branches.  If not in the preview, you create a <span style="color: #FFB366;font-weight:bold">develop</span> branch from the <span style="color: #FFB366;font-weight:bold">develop</span> branch.

Below is a list of the various ways you can find out if the project is in preview.
1. Check on [nuget.org](https://www.nuget.org/packages/KinsonDigital.Velaptor) to see what the latest version is. If the latest version is a preview release value, then the software is probably in preview.
2. Check for the most recent branch in GIT and see if it is a <span style="color: #CC99FF;font-weight:bold">preview</span> branch.
3. Check for the most recent tag in GIT and see if it is a preview release version.
4. Check if the [latest milestone](https://github.com/KinsonDigital/Velaptor/milestones) contains a preview release or production release as its title.
   - Production release example: v1.2.3
   - Preview release example: v1.2.3-preview.4
5. Reach out to a maintainer and ask.


Please make sure that you take the time and verify that the branch name is correct.  Branch names must be all lower case and the issue number in the branch name must be a valid GitHub issue number.  If the branch naming syntax, casing, or issue number is incorrect, the status checks for the CI/CD system will detect this and prevent the PR from being merged.

Of course, we are all human and people make mistakes!!  This is ok and no harm is done.  If you do accidentally create a pull request with a head(source) branch that is incorrect, simply tag a maintainer in the pull request about the mistake and recreate the pull request.  If for some reason you used the incorrect base(target) branch, the pull request does not have to be created.  This can be updated by a maintainer with ease.
</details>


<h2 style="border:0;font-weight:bold" align="center">Creating a Pull Request</h2>

<details closed><summary>TLDR</summary>

When creating a pull request, please use early pull requests.  This helps with the pull request system in GitHub for recording all commits and the history of the development process.  This helps encourage more transparency and collaboration with the project. Refer to [Early Pull Requests](https://medium.com/practical-blend/pull-request-first-f6bb667a9b6) for more information.

Make sure that you leave a comment in the issue that you want to contribute to.  This is important because without a comment, the GitHub notification system will not come into play and the maintainers of the project might not know that you are interested.  This also allows GitHub to let the maintainers assign the issue to you.

Also, make sure that you get confirmation from a project maintainer that you can work on a particular issue, no matter how it is labeled.  Sometimes certain issues are simply not meant for outside contributors due to complexity, project context, or other reasons.

If you have any questions or need clarification about the pull request, please use the comments section in the pull request.  This not only keeps things in the open but also helps the project maintainers stay up to date due to the GitHub notification system.  For any other questions, hop into the [discord server](https://discord.gg/qewu6fNgv7)!!

Please make the pull request title exactly matches the title of the associated issue. This helps with searching and maintaining a contextual link between the issue and the pull request.  If you do forget to do this or simply make a mistake, you do not have to recreate the pull request.  Just tag a maintainer in the pull request about the issue and they will fix it.
</details>

**Velaptor** encourages and uses [Early Pull Requests](https://medium.com/practical-blend/pull-request-first-f6bb667a9b6). Please don't wait until you're finished with your work before creating a PR!

1. Please leave a comment about your interest in the issue comments.
   - Please do not start working on an issue until you have got confirmation that you can from a project maintainer.
2. Fork and clone the repository.
   - Don't know how to fork and clone a repository? Go [here](https://docs.github.com/en/get-started/quickstart/fork-a-repo).
   - Please make sure to uncheck the _**branch only checkbox**_. Refer to [this image](./Images/create-fork.jpg) for an example.
3. Create a feature or preview feature branch using the syntax requirements in the _**Branching**_ section above and push it to remove. 
4. Add an empty commit to the new branch to start your work.
   1. Use this git command: `git commit --allow-empty -m "Start work for issue #<issue-number-here>"`.
      - Example: `git commit --allow-empty -m "Start work for issue #123"`.
   2. `git push`
5. Once you've pushed the commit, open a [**draft pull request**](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests#draft-pull-requests). Do this **BEFORE** you start working.
6. Make your commits in small, incremental steps with clear descriptions.
7. All unit tests must pass before a PR will be completed.
8. Make sure that the code follows the coding standards.
   * Pay attention to the warnings in **Visual Studio** and/or **JetBrains Rider**!!
   * Refer to the *.editorconfig* files in the code base for rules.
9. When you are finished with your changes, tag a maintainer in the PR comments and ask for a review!


<h2 style="border:0;font-weight:bold" align="center">Resources</h2>

1. [Forking a Repository](https://docs.github.com/en/get-started/quickstart/fork-a-repo)
2. [Early Pull Requests](https://medium.com/practical-blend/pull-request-first-f6bb667a9b6)
3. [Discord Server](https://discord.gg/qewu6fNgv7)
4. [Main Branching Docs](./Branching.md)
5. [Draft Pull Requests](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests#draft-pull-requests)
