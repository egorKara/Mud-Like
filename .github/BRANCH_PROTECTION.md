# Branch Protection Rules

## Main Branch Protection

The `main` branch is protected with the following rules:

### Required Status Checks
- ✅ **CodeQL Security Analysis** - Must pass before merging
- ✅ **Security Checks** - Must pass before merging
- ✅ **Unity Build** - Must pass before merging
- ✅ **Tests** - Must pass before merging

### Required Reviews
- ✅ **Require pull request reviews** - At least 1 reviewer
- ✅ **Dismiss stale reviews** - When new commits are pushed
- ✅ **Require review from code owners** - If CODEOWNERS file exists

### Branch Protection Settings
- ✅ **Require branches to be up to date** - Before merging
- ✅ **Require conversation resolution** - Before merging
- ✅ **Require signed commits** - For security
- ✅ **Require linear history** - No merge commits
- ✅ **Include administrators** - Apply rules to admins

### Restriction Settings
- ❌ **Allow force pushes** - Disabled
- ❌ **Allow deletions** - Disabled
- ❌ **Allow bypassing** - Disabled

## Develop Branch Protection

The `develop` branch has similar protection with:
- ✅ **Required status checks**
- ✅ **Required reviews** (1 reviewer)
- ✅ **Require up to date branches**
- ✅ **Require conversation resolution**

## Feature Branch Guidelines

### Naming Convention
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Critical fixes
- `security/description` - Security updates

### Requirements
- ✅ **Must be up to date** with target branch
- ✅ **Must pass all checks** before merging
- ✅ **Must have approval** from maintainer
- ✅ **Must have clear description** in PR

## Security Branch Protection

### Security Updates
- 🔒 **Immediate review required** for security PRs
- 🔒 **Automated security scanning** on all branches
- 🔒 **Secret scanning** enabled
- 🔒 **Dependency scanning** enabled

### Emergency Procedures
- 🚨 **Hotfix process** for critical security issues
- 🚨 **Direct push allowed** for security team only
- 🚨 **Immediate notification** to maintainers

## Code Quality Requirements

### Before Merging
- ✅ **No compilation errors**
- ✅ **No security vulnerabilities**
- ✅ **All tests passing**
- ✅ **Code coverage maintained**
- ✅ **Documentation updated**

### Code Review Checklist
- ✅ **Security implications reviewed**
- ✅ **Performance impact assessed**
- ✅ **Unity best practices followed**
- ✅ **ECS patterns correctly implemented**
- ✅ **No hardcoded secrets**
- ✅ **Proper error handling**

## Enforcement

### Automatic Enforcement
- 🤖 **GitHub Actions** enforce all rules
- 🤖 **Automated testing** on every PR
- 🤖 **Security scanning** on every push
- 🤖 **Dependency updates** via Dependabot

### Manual Enforcement
- 👥 **Code owners** review all changes
- 👥 **Security team** reviews security changes
- 👥 **Maintainers** approve all merges

## Exceptions

### Emergency Situations
- 🚨 **Critical security fixes** - Can bypass some checks
- 🚨 **Production issues** - Hotfix process available
- 🚨 **Maintainer override** - For urgent fixes only

### Documentation Only
- 📝 **Documentation updates** - Reduced review requirements
- 📝 **README changes** - Can be merged faster
- 📝 **Comment updates** - Minimal review needed

---

**Note**: These rules are enforced automatically by GitHub. Any exceptions must be approved by project maintainers.
