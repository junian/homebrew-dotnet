<div align="center">

<img src="https://raw.githubusercontent.com/junian/commons-media/refs/heads/master/svg/homebrew-logo.svg" width="96px" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="https://raw.githubusercontent.com/junian/commons-media/refs/heads/master/svg/microsoft-dotnet-logo.svg" width="96px" />

# Homebrew dotnet

Install the .NET SDK side-by-side on macOS using the Homebrew package manager.

</div>

## How to Use

First, tap the repository:

```shell
brew tap junian/homebrew-dotnet
````

> If you are currently using the .NET formula from the official `homebrew-cask`, you should uninstall it first. For example:
>
> ```shell
> brew uninstall --zap dotnet-sdk
> ```

### Installation

Install the desired .NET version:

```shell
brew install dotnet-sdk@<version>
```

Or:

```shell
brew install junian/dotnet/dotnet-sdk@<version>
```

### Uninstallation

Uninstall the desired .NET version:

```shell
brew uninstall dotnet-sdk@<version>
```

Or:

```shell
brew uninstall junian/dotnet/dotnet-sdk@<version>
```

## .NET SDK Versions

| Version               | Formula           |
| --------------------- | ----------------- |
| ⭐️ .NET 10 (LTS).     | `dotnet-sdk@10.0` |
| ⭐️ .NET 9             | `dotnet-sdk@9.0`  |
| ⭐️ .NET 8 (LTS)       | `dotnet-sdk@8.0`  |
| .NET 7                | `dotnet-sdk@7.0`  |
| .NET 6 (LTS)          | `dotnet-sdk@6.0`  |
| .NET 5                | `dotnet-sdk@5.0`  |

⭐️ [Supported versions](https://dotnet.microsoft.com/en-us/download/dotnet)

## Where is .NET 4 / .NET Core 4?

There is no **.NET 4** or **.NET Core 4**. Version 4.x is reserved for [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework), which only runs on Windows.

Microsoft transitioned from **.NET Core** to simply **.NET** starting with version 5.

The closest alternative to .NET Framework 4.x on macOS is [Mono MDK](#mono-mdk-version).

## .NET Core SDK Versions

| Version             | Formula          |
| ------------------- | ---------------- |
| .NET Core 3.1 (LTS) | `dotnet-sdk@3.1` |
| .NET Core 3.0       | `dotnet-sdk@3.0` |
| .NET Core 2.2       | `dotnet-sdk@2.2` |
| .NET Core 2.1 (LTS) | `dotnet-sdk@2.1` |
| .NET Core 2.0       | `dotnet-sdk@2.0` |
| .NET Core 1.1       | `dotnet-sdk@1.1` |
| .NET Core 1.0       | `dotnet-sdk@1.0` |

## Special Note for .NET Core 3.1 and .NET 5

On Apple Silicon Macs, if you install **.NET Core 3.1** or **.NET 5**, the `dotnet` binary is located at:

```
/usr/local/share/dotnet/x64/dotnet
```

You can create an alias:

```shell
alias dotnet-x64=/usr/local/share/dotnet/x64/dotnet
```

Then use it, for example:

```console
% dotnet-x64 --list-sdks
3.1.426 [/usr/local/share/dotnet/x64/sdk]
5.0.408 [/usr/local/share/dotnet/x64/sdk]
```

## Mono MDK Version

Before Microsoft created a cross-platform .NET, there was a community-developed .NET-compatible implementation called Mono.

| Version       | Formula         |
| ------------- | --------------- |
| Mono MDK 6.12 | `mono-mdk@6.12` |

## Xamarin Versions

These packages depend on [Mono MDK](#mono-mdk-version):

| Version         | Formula                |
| --------------- | ---------------------- |
| Xamarin.Mac     | `xamarin-mac@9.3`      |
| Xamarin.iOS     | `xamarin-ios@16.4`     |
| Xamarin.Android | `xamarin-android@13.2` |

## Visual Studio for Mac

This application also depends on [Mono MDK](#mono-mdk-version):

| Version                    | Formula              |
| -------------------------- | -------------------- |
| Visual Studio 2022 for Mac | `visual-studio@2022` |

## Brief History of .NET

If you’re confused about .NET versions and naming, here’s a quick history:

1. **2002:** .NET Framework (Windows-only, closed source)
2. **2004:** Mono (community-driven, cross-platform, open source)
3. **2011:** Xamarin (Mono for Android, MonoTouch for iOS)
4. **2016:** Microsoft acquires Xamarin and releases .NET Core (modern, cross-platform, open source)
5. **2020:** .NET 5 unifies everything into one platform, simply called **.NET** (cross-platform, open source)

## Cask Updater

A simple script is provided to automatically update the supported .NET Casks.

Install .NET 10, then run:

```bash
./update_casks.cs
```

If updates are available, the script will modify the cask files. Commit and push the changes afterward.

Most of the time, you don’t need to run it manually, as updates are checked every 6 hours via GitHub Workflow.
