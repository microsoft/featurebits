[CmdletBinding()]
param(
    [Parameter(HelpMessage='NuGet package version')]
    [AllowEmptyString()]
    [string]
    $PackageVersion
)

If ($PackageVersion)
{
    Write-Host "Package-Nuget: building packages..."

    dotnet pack ".\src\FeatureBits.Core\FeatureBits.Core.csproj" /p:Configuration=Release --no-build /p:PackageVersion=$PackageVersion
    dotnet pack ".\src\FeatureBits.Data\FeatureBits.Data.csproj" /p:Configuration=Release --no-build /p:PackageVersion=$PackageVersion
    dotnet pack ".\src\FeatureBits.Console\FeatureBits.Console.csproj" /p:Configuration=Release --no-build /p:PackageVersion=$PackageVersion
}
else
{
    Write-Host "Package-Nuget: -PackageVersion not provided, skipping..." -ForegroundColor Black -BackgroundColor Yellow
}
