@echo off
setlocal
mkdir "C:\Program Files\CSVAnalyzerPro\"
copy %~dp0files.zip "C:\Program Files\CSVAnalyzerPro\"

cd /d %~dp0
Call :UnZipFile "C:\Program Files\CSVAnalyzerPro\" "C:\Program Files\CSVAnalyzerPro\files.zip"
exit /b

:UnZipFile <ExtractTo> <newzipfile>
set vbs="%temp%\_.vbs"
if exist %vbs% del /f /q %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo objShell.NameSpace(%1).CopyHere(FilesInZip)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
cscript //nologo %vbs%
if exist %vbs% del /f /q %vbs%

start /d "C:\Program Files\CSVAnalyzerPro\" setup.exe