## Brief description
"Scales" in name of this project refers to weight scales, as this project aims to determine runtime memory consumption of managed objects, using reflection.

## Disclaimer
This project in no way production ready, and was developed as my personal research device for CoreCLR memory usage. But if it will satiate some else's curiosity on how objects get laid out, then great!

## Usage
This project exposes single extension method for [object](https://docs.microsoft.com/en-us/dotnet/api/system.object?view=netcore-3.1) called `GetExclusiveSize`.

All source code located in file `ReflectionObjectScales/ObjectSizeExtension.cs`.

## To Do
If somebody decide to elevate this project to production, they should expand on testing. Current implementation tested only on x64 Windows system, and control values for size got extracted manually, from debugger.

Theoretically its possible to automatically extract memory sizes using [debugger API](https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/icordebugprocess5-gettypelayout-method) on [hosted application](https://docs.microsoft.com/en-US/dotnet/core/tutorials/netcore-hosting).  