@echo off
setlocal

pushd %~dp0
dotnet build src\TriviaService.sln %*
popd

endlocal