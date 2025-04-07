<#
    .SYNOPSIS
    Meant to be executed in a local dev environment, this script is used to
    populate the appsettings.Development.json file required for the WeatherApp Public API
#>

$settings = @{
    ConnectionStrings = @{
        SqlServer = "Server=localhost,1433;Database=WeatherApp;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
    }
    Logging = @{
        LogLevel = @{
            Default = "Information"
            "Microsoft.AspNetCore" = "Warning"
        }
    }
    AllowedHosts = "*"
}

($settings | ConvertTo-Json) | Out-File -FilePath .\appsettings.Development.json
Write-Output "The file was created $( (Resolve-Path .\).Path )\appsettings.Development.json" 