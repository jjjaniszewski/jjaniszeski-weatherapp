<#
    .SYNOPSIS
    Meant to be executed in a local dev environment, this script is used to
    populate the local.settings.json file required from the WeatherApp Azure Function
#>
Param (
    [String] $weatherApiKey = ""
)

$azureWebJobsStorage = "UseDevelopmentStorage=true"

$localSettings = @{
    IsEncrypted = $False
    Values = @{
        AzureWebJobsStorage = $azureWebJobsStorage
        FUNCTIONS_WORKER_RUNTIME = "dotnet-isolated"
        WEATHER_UPDATER_CRON_SCHEDULE = "0 0 * * * *"
        WeatherApi__ApiKey = $weatherApiKey
    }
    ConnectionStrings = @{
        SqlServer = "Server=localhost,1433;Database=WeatherApp;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
    }
}

($localSettings | ConvertTo-Json) | Out-File -FilePath .\local.settings.json
Write-Output "The file was created $( (Resolve-Path .\).Path )\local.settings.json"
