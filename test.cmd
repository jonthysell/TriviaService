@echo off
setlocal

pushd %~dp0
dotnet test src\TriviaService.sln %*
popd

endlocal