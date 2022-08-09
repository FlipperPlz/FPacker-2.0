del /S /Q $(OutDir)\TestFiles
xcopy /S /Y "$(ProjectDir)TestFiles" "$(OutDir)\TestFiles"
exit /b 10