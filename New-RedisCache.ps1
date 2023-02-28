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

function Get-VolumeState([string]$name) {
    $volumeName = docker volume inspect -f '{{.Name}}' $name
    if ($volumeName -eq $name) {
        return $volumeName.Trim()
    }
    else {
        return $null
    }
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
            $redisVolumes = docker inspect --format="{{.Mounts}}" $containerId | Out-String
            docker rm --force $name
            $newContainerId = $null            
            do {
                Write-Host "Waiting for the removal of the old container"
                $newContainerId = Get-ContainerState $name
                Start-Sleep -Seconds 0.2
            } while ($newContainerId)

            Write-Host "Cleaning up orphaned redis volumes"
            $volumeRegex = "volume ([^\r\n\s]+)"
            $redisVolumes | Select-String -Pattern $volumeRegex -AllMatches | Select-Object Matches | ForEach-Object {
                $_.Matches | ForEach-Object {
                    $volumeToRemove = $_.Groups[1].Value
                    Write-Host "Removing orphaned Docker volume $volumeToRemove"
                    docker volume rm $volumeToRemove --force
                    Write-Host "Waiting for volumes to be removed..."
                    do {
                        Start-Sleep -Seconds 0.2
                    } while ($null -ne (Get-VolumeState $volumeToRemove))
                }
            }
        }

        Write-Host "Creating new container"
        $volume = "$name-Data:/var/lib/"
        docker run -d --restart always --name $name -p $port`:6379 -v $volume redis --appendonly yes
    }

    do {
        $newContainerId = Get-ContainerState $name
        Start-Sleep -Seconds 0.2
    } while (!$newContainerId)

    Write-Host "Redis Docker container $name is running. You're set!" -ForegroundColor Green
}