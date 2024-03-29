# TriviaService

Simple background service to ask trivia questions via Windows notifications.


| ![Screenshot 1](./.github/screenshot1.png) | ![Screenshot 2](./.github/screenshot2.png) |
|:--:|:--:|

Trivia questions provided by the free [Open Trivia Database](https://opentdb.com/).

## Setup

### Dependencies
1. Download and install the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### Build

`dotnet build src\TriviaService.sln`

### Test

`dotnet test src\TriviaService.sln`

## Usage

1. Run `TriviaService.exe` to start
2. Press Ctrl+C to stop

## History

This project was built by [Jon Thysell](mailto://jthysell@microsoft.com) as a part of Microsoft's March 2024 Fix/Hack/Learn event.

### Goals

1. Practice some modern .NET development
2. Learn how to create a Windows Service
3. Learn how to call Windows App SDK's WinRT APIs from C#
4. Learn how to create and consume Windows Notifications
5. Learn how to connect to a web REST API

### Resources

* [Create Windows Service using `BackgroundService`](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)
* [Windows App SDK Unpackaged C# Console App Sample](https://github.com/microsoft/WindowsAppSDK-Samples/tree/main/Samples/Unpackaged/cs-console-unpackaged)
* [Quickstart: App notifications in the Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/notifications/app-notifications/app-notifications-quickstart?tabs=cs)
* [Tutorial: Make HTTP requests in a .NET console app using C#](https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient)
* [Open Trivia Database](https://opentdb.com/)
* [Logo Image](https://www.hiclipart.com/free-transparent-background-png-clipart-mthiw)
