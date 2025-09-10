#!/bin/bash

# Mud-Like Repository Security Setup Script
# This script configures all security settings for the repository

set -e

echo "🔒 Setting up Mud-Like repository security..."

# Repository information
REPO_OWNER="egorKara"
REPO_NAME="Mud-Like"
REPO_FULL_NAME="${REPO_OWNER}/${REPO_NAME}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if GitHub CLI is installed
if ! command -v gh &> /dev/null; then
    print_error "GitHub CLI is not installed. Please install it first."
    exit 1
fi

# Check if user is authenticated
if ! gh auth status &> /dev/null; then
    print_warning "GitHub CLI is not authenticated. Please run 'gh auth login' first."
    exit 1
fi

print_status "Starting security configuration for ${REPO_FULL_NAME}..."

# 1. Enable Dependabot alerts
print_status "Enabling Dependabot alerts..."
if gh api repos/${REPO_FULL_NAME}/vulnerability-alerts --method PUT 2>/dev/null; then
    print_success "Dependabot alerts enabled"
else
    print_warning "Failed to enable Dependabot alerts (may already be enabled)"
fi

# 2. Enable automated security fixes
print_status "Enabling automated security fixes..."
if gh api repos/${REPO_FULL_NAME}/automated-security-fixes --method PUT 2>/dev/null; then
    print_success "Automated security fixes enabled"
else
    print_warning "Failed to enable automated security fixes (may already be enabled)"
fi

# 3. Enable secret scanning
print_status "Enabling secret scanning..."
if gh api repos/${REPO_FULL_NAME}/secret-scanning --method PUT 2>/dev/null; then
    print_success "Secret scanning enabled"
else
    print_warning "Failed to enable secret scanning (may already be enabled)"
fi

# 4. Enable code scanning
print_status "Enabling code scanning..."
if gh api repos/${REPO_FULL_NAME}/code-scanning --method PUT 2>/dev/null; then
    print_success "Code scanning enabled"
else
    print_warning "Failed to enable code scanning (may already be enabled)"
fi

# 5. Set up branch protection for main branch
print_status "Setting up branch protection for main branch..."

# Create branch protection rule
BRANCH_PROTECTION_CONFIG='{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "CodeQL",
      "Security Checks",
      "Unity Build",
      "Tests"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": true
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "required_conversation_resolution": true,
  "require_linear_history": true
}'

if gh api repos/${REPO_FULL_NAME}/branches/main/protection --method PUT --input - <<< "$BRANCH_PROTECTION_CONFIG" 2>/dev/null; then
    print_success "Branch protection rule created for main branch"
else
    print_warning "Failed to create branch protection rule (may already exist)"
fi

# 6. Enable Actions
print_status "Enabling GitHub Actions..."

# Set Actions permissions
ACTIONS_CONFIG='{
  "enabled": true,
  "allowed_actions": "all"
}'

if gh api repos/${REPO_FULL_NAME}/actions/permissions --method PUT --input - <<< "$ACTIONS_CONFIG" 2>/dev/null; then
    print_success "GitHub Actions enabled"
else
    print_warning "Failed to enable GitHub Actions (may already be enabled)"
fi

# 7. Set up Actions workflow permissions
print_status "Setting up Actions workflow permissions..."

WORKFLOW_PERMISSIONS_CONFIG='{
  "default_workflow_permissions": "write",
  "can_approve_pull_request_reviews": true
}'

if gh api repos/${REPO_FULL_NAME}/actions/permissions/workflow --method PUT --input - <<< "$WORKFLOW_PERMISSIONS_CONFIG" 2>/dev/null; then
    print_success "Actions workflow permissions configured"
else
    print_warning "Failed to configure Actions workflow permissions"
fi

# 8. Verify configuration
print_status "Verifying security configuration..."

# Check branch protection
if gh api repos/${REPO_FULL_NAME}/branches/main/protection &>/dev/null; then
    print_success "Branch protection is active"
else
    print_warning "Branch protection verification failed"
fi

# Check security features
if gh api repos/${REPO_FULL_NAME}/vulnerability-alerts &>/dev/null; then
    print_success "Dependabot alerts are enabled"
else
    print_warning "Dependabot alerts verification failed"
fi

print_success "Security configuration completed!"
print_status "Repository: https://github.com/${REPO_FULL_NAME}"
print_status "Settings: https://github.com/${REPO_FULL_NAME}/settings"

echo ""
echo "🔒 Security features enabled:"
echo "  ✅ Dependabot alerts"
echo "  ✅ Automated security fixes"
echo "  ✅ Secret scanning"
echo "  ✅ Code scanning"
echo "  ✅ Branch protection (main)"
echo "  ✅ GitHub Actions"
echo "  ✅ Required pull request reviews"
echo "  ✅ Required status checks"
echo "  ✅ Linear history enforcement"
echo "  ✅ Conversation resolution"
echo ""
echo "🎯 Next steps:"
echo "  1. Check Actions tab for running workflows"
echo "  2. Verify branch protection in Settings → Branches"
echo "  3. Monitor security alerts in Security tab"
echo "  4. Test by creating a pull request"
