@echo on

IF NOT EXIST "%1%2\build\" (
    mkdir "%1%2\build\"
)

IF NOT "%RoR2Location%" == "" (
    copy /Y "%1%2\bin\netstandard2.0\%3.dll" "%RoR2Location%BepInEx\plugins\%3.dll";
)

copy /Y "%1%2\bin\netstandard2.0\%3.dll" "%1%2\build\%3.dll";

copy /Y "%1%2\manifest.json" "%1%2\build\manifest.json";

copy /Y "%1%2\icon.png" "%1%2\build\icon.png";

copy /Y "%1\README.md" "%1%2\build\README.md";

copy /Y "%1\LICENSE.txt" "%1%2\build\LICENSE.txt";

IF NOT "%SevZipLocation%" == "" (
    "%SevZipLocation%7z.exe" a "%1%2\build\build.zip" "%1%2\build\*" -x!build.zip
)

for /f "delims== tokens=1,2" %%G in (./version.txt) do set %%G=%%H

call :FindReplace "%Build%" "<version>" ./manifest.json

call :FindReplace "%Build%" "<version>" ./CharacterCustomizerPlus.cs

exit /b 

:FindReplace <findstr> <replstr> <file>
set tmp="%temp%\tmp.txt"
If not exist %temp%\_.vbs call :MakeReplace
for /f "tokens=*" %%a in ('dir "%3" /s /b /a-d /on') do (
  for /f "usebackq" %%b in (`Findstr /mic:"%~1" "%%a"`) do (
    echo(&Echo Replacing "%~1" with "%~2" in file %%~nxa
    <%%a cscript //nologo %temp%\_.vbs "%~1" "%~2">%tmp%
    if exist %tmp% move /Y %tmp% "%%~dpnxa">nul
  )
)
del %temp%\_.vbs
exit /b

:MakeReplace
>%temp%\_.vbs echo with Wscript
>>%temp%\_.vbs echo set args=.arguments
>>%temp%\_.vbs echo .StdOut.Write _
>>%temp%\_.vbs echo Replace(.StdIn.ReadAll,args(0),args(1),1,-1,1)
>>%temp%\_.vbs echo end with