# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security vulnerability, please follow these steps:

### 1. **DO NOT** create a public GitHub issue
### 2. **DO NOT** discuss the vulnerability publicly
### 3. **DO** report it privately to: security@mud-like.dev

### What to Include in Your Report

Please include the following information:

- **Description**: A clear description of the vulnerability
- **Steps to Reproduce**: Detailed steps to reproduce the issue
- **Impact**: Potential impact of the vulnerability
- **Environment**: Unity version, OS, and other relevant details
- **Proof of Concept**: If possible, include a minimal example

### Response Timeline

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Resolution**: Within 30 days (depending on severity)

### Security Best Practices

#### For Contributors

- **Never commit secrets** (API keys, passwords, tokens)
- **Use environment variables** for sensitive configuration
- **Validate all inputs** in ECS systems
- **Follow Unity security guidelines** for DOTS
- **Keep dependencies updated** regularly

#### For Users

- **Keep Unity updated** to the latest version
- **Use only official releases** from this repository
- **Report suspicious behavior** immediately
- **Don't run untrusted scripts** in your Unity project

### Security Measures

#### Code Security
- **Static Analysis**: Automated code scanning
- **Dependency Scanning**: Regular package updates
- **Secret Scanning**: Detection of exposed secrets
- **Code Review**: All changes require review

#### Runtime Security
- **Input Validation**: All user inputs are validated
- **Memory Safety**: ECS provides memory-safe operations
- **Deterministic Physics**: Prevents desync attacks
- **Network Security**: Encrypted multiplayer communication

### Contact Information

- **Security Email**: security@mud-like.dev
- **Project Maintainer**: Mud-Like Developer
- **GitHub Issues**: For non-security related issues only

### Acknowledgments

We appreciate the security research community and responsible disclosure practices. Contributors who report valid security vulnerabilities will be acknowledged in our security advisories.

---

**Thank you for helping keep Mud-Like secure!**
