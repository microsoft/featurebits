[![Build status](https://ci.appveyor.com/api/projects/status/v3inx358w36q49wl/branch/master?svg=true)](https://ci.appveyor.com/project/ynauls/featurebits/branch/master)

# FeatureBits

FeatureBits is a feature toggling system meant to support multiple platforms. 

## How to use
A better usage story is coming in the next few days as we publish the packages to the public NuGet feed. Currently they're only published to a private feed of the original team.

## I can't wait! What if I want to use it now?
The easiest way to use FeatureBits right now is to create a new SQL DB and use `Update-Database` on the FeatureBits.Data project. This will create your initial DB. From there, add some feature bits definitions in the FeatureBitDefinitions table (The CLI won't be very usable until we get the public NuGet package published - that's why we're recommending updating the DB directly). Next, in your code, create Features.cs, which contains an enum corresponding to each of your feature bit Ids. Now download the source code for FeatureBits and compile the solution. You can then reference the generated assembly, `FeatureBits.Core`, to include in your project. Instantiate in instance of `FeatureBitEvaluator` with the appropriate SQL connection string and you should then be able to use code like `if (_evaluator.IsEnabled(Features.ExportAccess, CurrentPermissionLevel))` to determine whether a feature is "on" or "off".

## What are the different kinds of FeatureBitDefinitions?
1) On/Off (column OnOff) - if (and only if) no other columns are populated, then the OnOff column determines whether the feature is on or off.
2) ExcludedEnvironments - Comma separated list of environements for which the feature should be turned off. Attempts to read the `ASPNETCORE_ENVIRONMENT` environment variable to determine whether the bit should be on or off.
3) MinimumAllowedPermissionLevel - If the user's permission level (as uniquely determine by your application) is greater than or equal to a certain integer value, then the feature bit is "on".
4) ExactAllowedPermissionLevel - Same as the last one, but the user's permission level must exactly match (equals).

You can also take a look at the file `FeatureBitEvaluatorTests.cs` to see how the different kinds of feature bit definitions are used.

Other feature bit definitions are in the works.

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
