param(
    [string]$Url = "http://localhost:5000"
)

function Get-PreferredLocalIPv4Address {
    $interfaces =
        [System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces() |
        Where-Object {
            $_.OperationalStatus -eq [System.Net.NetworkInformation.OperationalStatus]::Up -and
            $_.NetworkInterfaceType -ne [System.Net.NetworkInformation.NetworkInterfaceType]::Loopback -and
            $_.NetworkInterfaceType -ne [System.Net.NetworkInformation.NetworkInterfaceType]::Tunnel
        }

    $physicalInterfaces = $interfaces | Where-Object {
        $_.Name -notmatch '^(vEthernet|VirtualBox|VMware|WSL|Hyper-V)' -and
        $_.Description -notmatch '(Hyper-V|VirtualBox|VMware|WSL)'
    }

    $candidateInterfaces = if ($physicalInterfaces) { $physicalInterfaces } else { $interfaces }

    $candidates =
        $candidateInterfaces |
        ForEach-Object {
            $nic = $_
            $nic.GetIPProperties().UnicastAddresses | ForEach-Object {
                [pscustomobject]@{
                    InterfaceType = $nic.NetworkInterfaceType
                    IP = $_.Address.IPAddressToString
                }
            }
        } |
        Where-Object {
            $_.IP -notlike '127.*' -and
            $_.IP -notlike '169.254.*'
        }

    if (-not $candidates) {
        return $null
    }

    $preferred = $candidates | Where-Object {
        $_.InterfaceType -eq [System.Net.NetworkInformation.NetworkInterfaceType]::Wireless80211 -and
        ($_.IP.StartsWith("10.") -or
         $_.IP.StartsWith("192.168.") -or
         $_.IP -match '^172\.(1[6-9]|2[0-9]|3[0-1])\.')
    } | Select-Object -First 1

    if ($preferred) {
        return $preferred.IP
    }

    $preferred = $candidates | Where-Object {
        $_.InterfaceType -eq [System.Net.NetworkInformation.NetworkInterfaceType]::Ethernet -and
        ($_.IP.StartsWith("10.") -or
         $_.IP.StartsWith("192.168.") -or
         $_.IP -match '^172\.(1[6-9]|2[0-9]|3[0-1])\.')
    } | Select-Object -First 1

    if ($preferred) {
        return $preferred.IP
    }

    $preferred = $candidates | Where-Object {
        $_.IP.StartsWith("10.") -or
        $_.IP.StartsWith("192.168.") -or
        $_.IP -match '^172\.(1[6-9]|2[0-9]|3[0-1])\.'
    } | Select-Object -First 1

    if ($preferred) {
        return $preferred.IP
    }

    return ($candidates | Select-Object -First 1).IP
}

$exePath = Join-Path $PSScriptRoot "dist\student-projects.exe"

if (-not (Test-Path -LiteralPath $exePath)) {
    Write-Error "Published app not found at $exePath. Run 'dotnet publish -c Release -o dist' first."
    exit 1
}

$uri = [System.Uri]$Url
$localIp = Get-PreferredLocalIPv4Address

if ([string]::IsNullOrWhiteSpace($localIp)) {
    Write-Warning "Could not determine a local IPv4 address. Falling back to localhost."
    $localIp = "localhost"
}

$hostToUse = if ($uri.Host -in @("localhost", "127.0.0.1", "::1")) { $localIp } else { $uri.Host }
$bindingUrl = "{0}://{1}:{2}" -f $uri.Scheme, $hostToUse, $uri.Port
$browserUrl = "{0}{1}{2}" -f $bindingUrl, $uri.AbsolutePath, $uri.Query
$workingDirectory = Split-Path -Path $exePath -Parent

$originalAspNetCoreUrls = $env:ASPNETCORE_URLS
$env:ASPNETCORE_URLS = $bindingUrl

try {
    $process = Start-Process -FilePath $exePath -WorkingDirectory $workingDirectory -PassThru
}
finally {
    if ($null -eq $originalAspNetCoreUrls) {
        Remove-Item Env:ASPNETCORE_URLS -ErrorAction SilentlyContinue
    }
    else {
        $env:ASPNETCORE_URLS = $originalAspNetCoreUrls
    }
}

Start-Sleep -Seconds 3
Start-Process $browserUrl

Write-Host "Started student-projects (PID $($process.Id)) and opened $browserUrl"
