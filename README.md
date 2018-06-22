# FeatureBits

FeatureBits is a feature toggling system meant to support multiple platforms. 

## Features (no pun intended)
* FeatureBits can be used in .NET Core, and via your own Web API, through TypeScript/Angular. Currently supports SQL Server, Azure SQL Database, and Azure Data Tables on the back end.

## Limitations
* Azure Data Tables only lightly tested so far (see CONTRIBUTING.md to learn how you can help extend to other data platforms)

## Current Status
Public

## Contributing
See CONTRIBUTING.md and review the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

## Description
Feature Toggling is important to support Continuous Integration/Continuous Deployment. Features that may be experimental or incomplete can be hidden behind a FeatureBit for some, most, or all users. FeatureBits currently supports both .NET Core and TypeScript/Angular. 

## Reporting Security Issues

Security issues and bugs should be reported privately, via email, to the Microsoft Security
Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should
receive a response within 24 hours. If for some reason you do not, please follow up via
email to ensure we received your original message. Further information, including the
[MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in
the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).
