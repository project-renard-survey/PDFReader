@title WiX Build - PDF Reader 3

@echo off

set SIGNTOOL=C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\signtool.exe

@echo Signing EXE
if exist "%SIGNTOOL%" (
	rem OK
	) else ( 
	echo Signtool.exe is not where the batch file thinks it is. You'll have to edit the batch file where it says SIGNTOOL=. SIGNTOOL.EXE is in the Windows SDK.
	goto:END
)
:sign
"%SIGNTOOL%" sign /sha1 11326c841787d9ba95bebc258ec9a8ffd11057e9 "..\PDFReader\bin\Release\PDFReader.exe"
if ERRORLEVEL 0 goto:SIGNEDOK
echo Failed to sign the executable. Probably the WebbIE signing certificate is not installed on your machine, which
echo means you're not Alasdair. Delete the signtool line from the build.bat batch script.
goto:END
:SIGNEDOK
echo Candle
if exist PDFReader.wixobj del PDFReader.wixobj
"%WIX%\bin\candle.exe" PDFReader.wxs -nologo -ext WixNetfxExtension -ext WixUtilExtension -ext wixTagExtension -ext WixUiExtension
echo.
echo Light
if exist PDFReader.msi del PDFReader.msi 
"%WIX%\bin\light.exe" PDFReader.wixobj -spdb -sice:ICE91 -nologo -ext WixNetfxExtension -ext WixUtilExtension -ext wixTagExtension -ext WixUiExtension
echo.
echo All done. 
:END
@pause

