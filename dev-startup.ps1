#Requires -Version 7.2
# Script dev-startup
<# .SYNOPSIS
     Developer Machine Setup Automation
.DESCRIPTION
     Installs all the dependencies needed to run PaulsRedditFeed Locally.
     This script is idempotent, so you can run it as many times as you want
     without hurting anything
.NOTES
     Author     : Paul Miller - paulstark256@gmail.com
.LINK
     https://github.com/PaulStarkX/PaulsTwitterFeed
#>
param(
    [switch]$Rebuild
)

$newRedisCacheModule = Join-Path $PSScriptRoot "New-RedisCache.ps1"
Import-Module $newRedisCacheModule -Force

Write-Host "`nBeginning Developer Machine Setup" -ForegroundColor Cyan
Write-Host "-----------------------------------------------------------------------------------------" -ForegroundColor Cyan

if ($Rebuild) {
    Write-Host "-Rebuild option was specified."
    Write-Host "Local Redis caches for PaulsRedditFeed will be rebuilt and all the data in them will be lost." -ForegroundColor Yellow
}

$dockerFound = Get-IsCommandFound "docker" "Docker"
if (!$dockerFound) {
    Write-Host "Docker was not found and is required to host the redis cache for this project." -ForegroundColor Yellow
    Write-Host "Please install docker and rerun this script. You might have to restart your computer after the docker install.`n"
    Write-Host "https://www.docker.com/`n`n" -ForegroundColor Blue
    Exit 1
}

New-RedisCache PaulsRedditFeedCache $Rebuild
New-RedisCache PaulsRedditFeedTestCache $Rebuild

Write-Host "`nMachine Setup Completed`n" -ForegroundColor Cyan