<#
.SYNOPSIS
  Installs the third-party prerequisites kept in docs\files into the GTA V (Legacy) folder,
  recording every file it adds so -Uninstall can remove exactly those.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File build\install-prereqs.ps1
  powershell -ExecutionPolicy Bypass -File build\install-prereqs.ps1 -Uninstall
#>
param(
    [string]$GtaDir = $env:GTA_DIR,
    [switch]$Uninstall
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$src = Join-Path $repo 'docs\files'

if (-not $GtaDir) { $GtaDir = 'C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V' }
if (-not (Test-Path (Join-Path $GtaDir 'GTA5.exe'))) { throw "GTA5.exe not found in '$GtaDir'. Pass -GtaDir or set GTA_DIR." }
$manifest = Join-Path $GtaDir 'AnotherLife-prereqs.manifest.txt'

# Not installed:
#   xinput1_4.dll     the ENHANCED asi loader; Legacy uses dinput8.dll
#   NativeTrainer.asi F4 clashes with the SHVDN console; Menyoo covers it
#   menyoolog.txt, License.txt, Readme.txt, *.pdb  docs/logs/debug symbols
$skip = 'xinput1_4.dll', 'NativeTrainer.asi', 'menyoolog.txt', 'License.txt', 'Readme.txt',
        'ScriptHookVDotNet.pdb', 'ScriptHookVDotNet2.pdb', 'ScriptHookVDotNet3.pdb'

# Files the prerequisites write on their own while running; removed on uninstall too.
$generated = 'ScriptHookV.log', 'asiloader.log', 'ScriptHookVDotNet.log', 'menyoolog.txt',
             'FreemodeIdentity.log', 'scripts\FreemodeIdentity.ini', 'FreemodeIdentity.ini'

if ($Uninstall) {
    if (-not (Test-Path $manifest)) { throw "No manifest at $manifest; nothing recorded to remove." }
    $entries = Get-Content $manifest | Where-Object { $_ -and -not $_.StartsWith('#') }
    # Files first, then directories deepest-first (only if left empty).
    foreach ($rel in $entries | Where-Object { -not $_.EndsWith('\') }) {
        $p = Join-Path $GtaDir $rel
        if (Test-Path $p) { Remove-Item $p -Force; Write-Host "  del  $rel" }
    }
    foreach ($rel in $generated) {
        $p = Join-Path $GtaDir $rel
        if (Test-Path $p) { Remove-Item $p -Force; Write-Host "  del  $rel (generated)" }
    }
    foreach ($rel in $entries | Where-Object { $_.EndsWith('\') } | Sort-Object Length -Descending) {
        $p = Join-Path $GtaDir $rel
        if ((Test-Path $p) -and -not (Get-ChildItem $p -Force)) { Remove-Item $p -Force; Write-Host "  rmdir $rel" }
        elseif (Test-Path $p) { Write-Warning "Left $rel (not empty, e.g. Menyoo saves or Another Life files)" }
    }
    Remove-Item $manifest -Force
    Write-Host 'Prerequisites removed. Another Life itself: delete scripts\AnotherLife*.'
    return
}

if (-not (Test-Path (Join-Path $src 'ScriptHookV.dll'))) { throw "Expected the mod files in $src." }

$added = New-Object System.Collections.Generic.List[string]
if (Test-Path $manifest) { $added.AddRange([string[]](Get-Content $manifest | Where-Object { $_ -and -not $_.StartsWith('#') })) }

foreach ($file in Get-ChildItem $src -Recurse -File) {
    if ($skip -contains $file.Name) { continue }
    $rel = $file.FullName.Substring($src.Length + 1)
    $dst = Join-Path $GtaDir $rel

    # Record each directory we create (trailing \) so uninstall can remove it.
    $dir = Split-Path $dst -Parent
    $chain = @()
    while (-not (Test-Path $dir)) { $chain = ,$dir + $chain; $dir = Split-Path $dir -Parent }
    foreach ($d in $chain) {
        New-Item -ItemType Directory $d | Out-Null
        $added.Add($d.Substring($GtaDir.Length + 1) + '\')
    }

    if (Test-Path $dst) {
        # Never take ownership of a file that was already there (vanilla or another mod's).
        if (-not $added.Contains($rel)) { Write-Host "  keep $rel (already present, not ours)"; continue }
    } else {
        $added.Add($rel)
    }
    Copy-Item $file.FullName $dst -Force
}

$header = "# Files added by build\install-prereqs.ps1 on $(Get-Date -Format s). Remove with -Uninstall."
Set-Content $manifest -Encoding utf8 -Value (@($header) + ($added | Select-Object -Unique))
Write-Host "Installed $(($added | Where-Object { -not $_.EndsWith('\') }).Count) files; manifest: $manifest"
