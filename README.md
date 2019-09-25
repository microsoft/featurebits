# FeatureBits - A .NET (and multi-platform) feature toggling system

[![Build status](https://ci.appveyor.com/api/projects/status/hykv6phe9u4kljc5/branch/master?svg=true)](https://ci.appveyor.com/project/dseelinger/featurebits/branch/master)
[![NuGet](https://img.shields.io/nuget/v/FeatureBits.Core.svg)](https://www.nuget.org/packages/FeatureBits.Core/)
[![NuGet](https://img.shields.io/nuget/dt/FeatureBits.Core.svg)](https://www.nuget.org/packages/FeatureBits.Core/)

FeatureBits is a feature toggling system to support Continuous Delivery on multiple platforms.

FeatureBits is now available in NuGet!

```install-package featurebits.core```

For more information about how feature toggling works, please see [Feature Toggles (aka Feature Flags)](https://www.martinfowler.com/articles/feature-toggles.html)

## How to use

Note: A complete sample application of the steps below can be found at [feature-bits-sample](https://github.com/dseelinger/feature-bits-sample).

1) Create a .NET Core 2.X Web Application (Web API) application.
1) In the Package Manager Console,  ```Install-Package featurebits.core -IncludePrerelease```
1) Edit your .csproj, add the following ```<ItemGroup>``` section:

```csharp

  <ItemGroup>
    <DotNetCliToolReference Include="dotnet-fbit" Version="0.4.1" />
    <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="*" />
  </ItemGroup>

```

The SecretManager tool is not absolutely necessary, but will keep you from having to continually enter your SQL or Azure Table connection strings when using the command-line interface.

Next:

1) Create your SQL database or Azure Table to store the FeatureBit definitions. For SQL, you can use (or modify for your particular environment) ```FeatureBitDefinitions.SQL```, found in this repository.
1) Right-click your web project in Visual Studio 2017 or greater and select "Manage User Secrets"
1) Add the following into your User Secrets file:

```JSON

{
  "fbit:-s":  "[your SQL Connection string goes here]",
  "ConnectionStrings": {
    "FeatureBitsDbContext": "[your SQL Connection string goes here]"
  }

}

```

If you plan to use Azure Table storage, use "fbit:-a"

```JSON

{
  "fbit:-a":  "[your Azure Table Storage Connection string goes here]"
}

```

The sample application assumes a local SQL Server from this point.

1) Follow the instructions in [Safe storage of app secrets in development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) to set up your web applciation to consume User Secrets from the web application's configuration.
1) Navigate a console window to the web application's folder and execute the following command, which creates an initial FeatureBit definition that is always on: `dotnet fbit add -n DummyOn -o true`
1) Add another FeatureBit definition, which always evaluates to off, as follows: `dotnet fbit add -n DummyOff -o false`
1) Now generate an enum for your new feature bit definitions with: `dotnet fbit generate -n "SampleWeb"`
1) Edit your `Startup.cs` file for dependency injection of your SQL connection string and the FeatureBitEvaluator:

```csharp

public void ConfigureServices(IServiceCollection services)
{
    string featureBitsConnectionString = Configuration.GetConnectionString("FeatureBitsDbContext");
    services.AddDbContext<FeatureBitsEfDbContext>(options => options.UseSqlServer(featureBitsConnectionString));
    services.AddTransient<IFeatureBitsRepo, FeatureBitsEfRepo>((serviceProvider) =>
    {
        DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
        options.UseSqlServer(featureBitsConnectionString);
        var context = new FeatureBitsEfDbContext(options.Options);
        return new FeatureBitsEfRepo(context);
    });
    services.AddTransient<IFeatureBitEvaluator, FeatureBitEvaluator>();

    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}

```

Now add a simple controller that can handle requests for evaluating FeatureBits to your web application:

```csharp

using System.Linq;
using FeatureBits.Core;
using Microsoft.AspNetCore.Mvc;

namespace SampleWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureBitsController : ControllerBase
    {

        private readonly IFeatureBitEvaluator _evaluator;

        public FeatureBitsController(IFeatureBitEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        [HttpGet("/{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var definition = _evaluator.Definitions.SingleOrDefault(d => d.Id == id);

            if (definition != null)
            {
                bool isEnabled = _evaluator.IsEnabled((Features) id, 0);
                return new JsonResult(isEnabled);
            }

            return NotFound();
        }
    }
}

```

In ValuesController.cs, replace the first two methods with the following code:

```csharp

private readonly IFeatureBitEvaluator _evaluator;

public ValuesController(IFeatureBitEvaluator evaluator)
{
    _evaluator = evaluator;
}

// GET api/values
[HttpGet]
public ActionResult<IEnumerable<string>> Get()
{
    if (_evaluator.IsEnabled(Features.DummyOn))
    {
        return new string[] { "value1", "value2" };
    }
    else
    {
        return new string[] { };
    }
}

// GET api/values/5
[HttpGet("{id}")]
public ActionResult<string> Get(int id)
{
    if (_evaluator.IsEnabled(Features.DummyOff))
    {
        return "value";
    }
    else
    {
        return "";
    }
}

```

## FeatureBit Definitions

1) On/Off (column OnOff) - if (and only if) no other columns are populated, then the OnOff column determines whether the feature is on or off.
2) ExcludedEnvironments - Comma separated list of environments for which the feature should be turned off. Attempts to read the `ASPNETCORE_ENVIRONMENT` environment variable to determine whether the bit should be on or off.  IncludedEnvironments (when supplied) supercedes ExcludedEnvironments.
3) IncludedEnvironments - Comma separated list of environments for which the feature should be turned on. Attempts to read the `ASPNETCORE_ENVIRONMENT` environment variable to determine whether the bit should be on or off.
4) MinimumAllowedPermissionLevel - If the user's permission level (as uniquely determine by your application) is greater than or equal to a certain integer value, then the feature bit is "on".
5) ExactAllowedPermissionLevel - Same as the last one, but the user's permission level must exactly match (equals).


You can also take a look at the file `FeatureBitEvaluatorTests.cs` to see how the different kinds of feature bit definitions are used.

Other feature bit definitions are in the works.

## fbit .NET Core Console Application

If you include a `<DotNetCliToolReference>` to `dotnet-fbit` as mentioned above, you'll be able to take advantage of the FeatureBit CLI rather than manipulating the SQL DB or Azure Table directly. Help is enabled for the CLI and any of its 'verbs'. For example, `dotnet fbit --help`


## Features

* FeatureBits can be used in .NET Core, and via your own Web API, through TypeScript/Angular. Currently supports SQL Server, Azure SQL Database, and Azure Data Tables on the back end.

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
