Clear-Host;

Write-Host -NoNewline "Please enter an issue number: " -ForegroundColor Cyan;
$issueNumber = Read-Host;

if ($issueNumber -notmatch '^\d+$') {
    Write-Error "User input is not a number";
    exit 1;
}

Write-Host -NoNewline "Please enter a branch name: " -ForegroundColor Cyan;
$branchDescrip = Read-Host;

$branchDescrip = $branchDescrip.ToLower();
$branchDescrip = $branchDescrip.Replace(" ", "-").Replace("_", "-");
$branchDescrip = $branchDescrip.TrimStart("-");
$branchDescrip = $branchDescrip.TrimEnd("-");

$headBranch = "feature/$issueNumber-$branchDescrip";
$commitMsg = "Start owrk for issue #$issueNumber";

$destBranch = "not-set";

$baseBranches = @("main", "preview");
Write-Host -NoNewline "Please choose a base branch from the list [$($baseBranches -join ', ')]: " -ForegroundColor Cyan;
$chosenBaseBranch = Read-Host;

if ($baseBranches -contains $chosenBaseBranch) {
    $destBranch = $chosenBaseBranch;
} else {
    Write-Error "Invalid base branch.";
    exit 1;
}

Write-Host "`n--------------------------------`n";

Write-Host "Creating branch. . ." -ForegroundColor Yellow;
git checkout -B "$headBranch";
Write-Host "";

Write-Host "Creating empty commit. . ." -ForegroundColor Yellow;
git commit --allow-empty -m $commitMsg;
Write-Host "";

Write-Host "Pushing branch to remote. . ." -ForegroundColor Yellow;
git push --set-upstream origin "$headBranch";
Write-Host "";

Write-Host "Creating PR. . ." -ForegroundColor Yellow;
gh pr create -B $destBranch -b "" -t "new pr" -d;
