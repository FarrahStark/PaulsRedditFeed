function Get-IsCommandFound([string]$command, [string]$name) {
    try {
        if (Get-Command $command) {
            Write-Host "$name found"
        }
        return $true
    }
    catch {
        Write-Host "$name not found"
        return $false
    }
}

function Get-ContainerState([string]$name) {
    $containerState = docker inspect -f '{{.State.Status}}' $name
    return $containerState.Trim()
}

function New-RedisCache([string]$name, [int]$port, [boolean]$rebuild) {
    Write-Host "`nEnsuring Redis Docker container $name is created and running" -ForegroundColor Cyan
    $containerState = Get-ContainerState $name
    if ($rebuild) {
        $containerState = "needs rebuild"
    }

    if ($containerState -eq "running") {
        Write-Host "Container is already running. You're set!" -ForegroundColor Green
        return
    }
    elseif ($containerState -eq "exited") {
        Write-Host "Restarting container." -ForegroundColor Magenta
        docker start $name
    }
    else {
        $containerId = docker ps -a -q -f name=$name | Out-String
        $containerId = ($containerId ?? "").Trim()
        if ($containerId) {
            Write-Host "Container needs to be rebuilt." -ForegroundColor Yellow
            docker rm --force $name
        }

        Write-Host "Creating new container"
        docker run -d --restart always --name $name -p $port`:6379 redis
    }

    do {
        $newContainerId = Get-ContainerState $name
        Start-Sleep -Seconds 0.2
    } while (!$newContainerId)

    Write-Host "Redis Docker container $name is running. You're set!" -ForegroundColor Green
}