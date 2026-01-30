set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\LubanTools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%WORKSPACE%/Assets/Scripts/LubanGenerated ^
    -x outputDataDir=%WORKSPACE%/Assets/StreamingAssets/LubanData

pause