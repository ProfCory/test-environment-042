<#
.SYNOPSIS
  Builds Another Life and copies it straight into the GTA V folder (Legacy).

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File build\deploy.ps1
  powershell -ExecutionPolicy Bypass -File build\deploy.ps1 -NoData     # keep in-game JSON edits
  powershell -ExecutionPolicy Bypass -File build\deploy.ps1 -GtaDir "D:\Games\GTAV"
#>
param(
    [string]$GtaDir = $env:GTA_DIR,
    [switch]$NoData,
    [switch]$SkipTests
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent

if (-not $GtaDir) { $GtaDir = 'C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V' }
if (-not (Test-Path (Join-Path $GtaDir 'GTA5.exe'))) { throw "GTA5.exe not found in '$GtaDir'. Pass -GtaDir or set GTA_DIR." }

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue).Source
if (-not $dotnet) { $dotnet = 'C:\Program Files\dotnet\dotnet.exe' }

# Build (and test) before touching the game folder.
if (-not $SkipTests) {
    & $dotnet test (Join-Path $repo 'AnotherLife.sln') --nologo -v quiet
    if ($LASTEXITCODE) { throw 'Tests failed; nothing deployed.' }
}
& $dotnet build (Join-Path $repo 'src\AnotherLife.Game') -c Release --nologo -v quiet
if ($LASTEXITCODE) { throw 'Build failed; nothing deployed.' }
$bin = Join-Path $repo 'src\AnotherLife.Game\bin\Release\net48'

$scripts = Join-Path $GtaDir 'scripts'
$dataDir = Join-Path $scripts 'AnotherLife\data'
New-Item -ItemType Directory -Force $dataDir | Out-Null

# Our own DLLs (+ pdbs so SHVDN error traces carry line numbers) always overwrite.
foreach ($f in 'AnotherLife.Game.dll', 'AnotherLife.Game.pdb', 'AnotherLife.Core.dll', 'AnotherLife.Core.pdb') {
    Copy-Item (Join-Path $bin $f) $scripts -Force
}

# Shared dependencies: never downgrade one another mod installed.
foreach ($f in 'Newtonsoft.Json.dll', 'LemonUI.SHVDN3.dll') {
    $src = Join-Path $bin $f
    $dst = Join-Path $scripts $f
    if (Test-Path $dst) {
        $have = [version](Get-Item $dst).VersionInfo.FileVersion
        $ours = [version](Get-Item $src).VersionInfo.FileVersion
        if ($have -ge $ours) { Write-Host "  keep  $f $have (ours $ours)"; continue }
    }
    Copy-Item $src $scripts -Force
}

if ($NoData) {
    Write-Host '  skip  data/*.json (-NoData)'
} else {
    Copy-Item (Join-Path $repo 'data\*.json') $dataDir -Force
}

Write-Host "Deployed to $scripts"

# Prerequisites the mod can't run without.
$missing = @()
foreach ($f in 'ScriptHookV.dll', 'dinput8.dll', 'ScriptHookVDotNet.asi', 'ScriptHookVDotNet3.dll') {
    if (-not (Test-Path (Join-Path $GtaDir $f))) { $missing += $f }
}
if (-not (Test-Path (Join-Path $scripts 'FreemodeIdentity.dll'))) { $missing += 'scripts\FreemodeIdentity.dll' }
if ($missing) { Write-Warning ("Missing prerequisites (see docs/install.md): " + ($missing -join ', ')) }
