# Branch Protection Setup Instructions

## Manual Setup (Recommended)

### 1. Branch Protection Rules
1. Go to: `Settings → Branches`
2. Click: `Add rule`
3. Branch name pattern: `main`
4. Enable:
   - ✅ Require a pull request before merging
   - ✅ Require approvals (1 reviewer)
   - ✅ Dismiss stale PR approvals when new commits are pushed
   - ✅ Require status checks to pass before merging
   - ✅ Require branches to be up to date before merging
   - ✅ Require conversation resolution before merging
   - ✅ Require signed commits
   - ✅ Require linear history
   - ✅ Include administrators

### 2. Security & Analysis
1. Go to: `Settings → Security & analysis`
2. Enable:
   - ✅ Dependabot alerts
   - ✅ Dependabot security updates
   - ✅ Code scanning
   - ✅ Secret scanning

### 3. Actions Settings
1. Go to: `Settings → Actions → General`
2. Artifact and log retention: `90 days`
3. Click: `Save`

## Automated Setup (Alternative)

### Using GitHub CLI
```bash
# Install GitHub CLI
curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null
sudo apt update
sudo apt install gh

# Authenticate
gh auth login

# Enable branch protection
gh api repos/egorKara/Mud-Like/branches/main/protection \
  --method PUT \
  --field required_status_checks='{"strict":true,"contexts":["CodeQL","Security Checks","Unity Build","Tests"]}' \
  --field enforce_admins=true \
  --field required_pull_request_reviews='{"required_approving_review_count":1,"dismiss_stale_reviews":true}' \
  --field restrictions=null

# Enable security features
gh api repos/egorKara/Mud-Like/vulnerability-alerts \
  --method PUT

gh api repos/egorKara/Mud-Like/automated-security-fixes \
  --method PUT
```

## Verification

### Check Branch Protection
```bash
gh api repos/egorKara/Mud-Like/branches/main/protection
```

### Check Security Features
```bash
gh api repos/egorKara/Mud-Like/vulnerability-alerts
gh api repos/egorKara/Mud-Like/automated-security-fixes
```

## Expected Results

After setup, you should see:
- ✅ Branch protection active on `main`
- ✅ Required status checks: CodeQL, Security Checks, Unity Build, Tests
- ✅ Required reviews: 1 reviewer
- ✅ Dependabot alerts enabled
- ✅ Code scanning enabled
- ✅ Secret scanning enabled
- ✅ Actions workflows running automatically

## Troubleshooting

### If Actions don't run:
1. Check `Settings → Actions → General`
2. Ensure "Allow all actions" is selected
3. Check workflow files in `.github/workflows/`

### If Branch Protection fails:
1. Check `Settings → Branches`
2. Verify rule is applied to `main` branch
3. Check required status checks are available

### If Security features don't work:
1. Check `Settings → Security & analysis`
2. Ensure all features are enabled
3. Wait 24 hours for initial scan
