@echo off

set UNITYPROJECT_MODS="H:\Apps\Unity\Projects\LC\Lacrimosum\Assets\LethalCompany\Mods"
set UNITYPROJECT_BUILTBUNDLES="H:\Apps\Unity\Projects\LC\Lacrimosum\AssetBundles\StandaloneWindows"
set SOLUTION_DIR="I:\GitHub\Rider\Weather Electric\Lacrimosum"
set DOWNLOADS="C:\Users\Mabel\Downloads"

echo Netcode patching...
cd /d %SOLUTION_DIR%\NetcodePatcher
NetcodePatcher.Cli.exe -nv 1.5.2 plugins deps
COPY ".\plugins\Lacrimosum.dll" %SOLUTION_DIR%\Staging\BepInEx\plugins
echo Successfully patched!

cd /d %SOLUTION_DIR%

echo Copying bundles...
COPY %UNITYPROJECT_BUILTBUNDLES%\lacrimosum ".\Staging\BepInEx\Plugins\lacrimosum"
COPY %UNITYPROJECT_BUILTBUNDLES%\lacrimosumcosmetics ".\Staging\BepInEx\Plugins\lacrimosum.cosmetics"
echo Copied bundles!

echo Copying files to Unity project...
COPY %UNITYPROJECT_BUILTBUNDLES%\lacrimosum %UNITYPROJECT_MODS%\lacrimosum
COPY ".\NetcodePatcher\plugins\Lacrimosum.dll" %UNITYPROJECT_MODS%
echo Copied to Unity project!

echo Zipping mod and sending to Downloads...
powershell Compress-Archive -Path ".\Staging\*" -DestinationPath %DOWNLOADS%\Lacrimosum.zip -Force
echo All done :3