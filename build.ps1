param(
    [Parameter(Mandatory = $true)]
    [string]$GamePath,
    [ValidateSet('Full', 'Story')]
    [string]$Edition = 'Full'
)
$ErrorActionPreference = 'Stop'
$gameRoot = (Resolve-Path -LiteralPath $GamePath).Path
$outputRoot = Join-Path $PSScriptRoot "build\$Edition"
New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null

# Compile against the game's own .NET 6 runtime and generated interop assemblies.
# The repository and release do not redistribute these dependencies.
$references = @(Get-ChildItem -LiteralPath (Join-Path $gameRoot 'dotnet') -Filter '*.dll' |
    Where-Object { $_.Name -match '^(System\.|mscorlib|netstandard)' -and $_.Name -notmatch '\.Native\.dll$' })
foreach ($name in @('0Harmony.dll', 'BepInEx.Core.dll', 'BepInEx.Unity.IL2CPP.dll', 'BepInEx.Unity.Common.dll', 'Il2CppInterop.Runtime.dll')) {
    $references += Get-Item -LiteralPath (Join-Path $gameRoot "BepInEx\core\$name")
}
foreach ($name in @('Assembly-CSharp.dll', 'Il2Cppmscorlib.dll', 'UnityEngine.CoreModule.dll')) {
    $references += Get-Item -LiteralPath (Join-Path $gameRoot "BepInEx\interop\$name")
}
$sdkLine = @(& dotnet --list-sdks)[-1]
if ($LASTEXITCODE -ne 0 -or $sdkLine -notmatch '^(\S+)\s+\[(.+)\]') { throw 'A .NET SDK is required.' }
$compiler = Join-Path (Join-Path $Matches[2] $Matches[1]) 'Roslyn\bincore\csc.dll'
$response = @('/nostdlib+', '/target:library', '/langversion:latest', '/deterministic+', '/optimize+',
    ('/out:"' + (Join-Path $outputRoot 'MidnightBonuses.dll') + '"'))
if ($Edition -eq 'Story') { $response += '/define:STORY_ONLY' }
$response += $references | ForEach-Object { '/reference:"' + $_.FullName + '"' }
$response += '"' + (Join-Path $PSScriptRoot 'src\Plugin.cs') + '"'
$responsePath = Join-Path $outputRoot 'compile.rsp'
$response | Set-Content -LiteralPath $responsePath
& dotnet $compiler /noconfig "@$responsePath"
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Write-Output (Join-Path $outputRoot 'MidnightBonuses.dll')

