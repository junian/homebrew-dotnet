<div align="center">

<img src="https://upload.wikimedia.org/wikipedia/commons/7/7d/Microsoft_.NET_logo.svg" width="96px" alt="Microsoft dotnet logo" />

# Homebrew dotnet

Install .NET SDK side-by-side on macOS with Homebrew package manager.

</div>

## How to Use

The first thing you have to do is to Tap.

```shell
brew tap junian/homebrew-dotnet
```

> [!NOTE]
> 
> If you're currently using .NET formula from official `homebrew-cask`, you need to zap it first e.g., `brew uninstall --zap dotnet-sdk`.

### Installation

Then you can install .NET version as you wish.

```shell
brew install dotnet-sdk@<version>
```

Or

```shell
brew install junian/dotnet/dotnet-sdk@<version>
```

### Uninstallation

You can also uninstall .NET version as you wish.

```shell
brew uninstall junian/dotnet/dotnet-sdk@<version>
```

Or

```shell
brew uninstall junian/dotnet/dotnet-sdk@<version>
```

## .NET SDK Versions

| Version | Formula |
|---------|---------|
| .NET 10 RC-1 | `dotnet-sdk@10.0` |
| .NET 9 | `dotnet-sdk@9.0` |
| .NET 8 (LTS) | `dotnet-sdk@8.0` |
| .NET 7 | `dotnet-sdk@7.0` |
| .NET 6.0 (LTS) | `dotnet-sdk@6.0` |
| .NET 5.0 | `dotnet-sdk@5.0` |

## Where is .NET 4 / .NET Core 4?

There is no .NET 4 or .NET Core 4. To avoid confusion, version 4.x is strictly for .NET Framework, only on Windows.
Microsoft transition from **.NET Core** to _just_ **.NET** starting from version 5.

The closest thing with .NET Framework 4.X on macOS is [Mono MDK](#mono-mdk-version).

## .NET Core SDK Versions

| Version | Formula |
|---------|---------|
| .NET Core 3.1 (LTS) | `dotnet-sdk@3.1` |
| .NET Core 3.0 | `dotnet-sdk@3.0` |
| .NET Core 2.2 | `dotnet-sdk@2.2` |
| .NET Core 2.1 (LTS) | `dotnet-sdk@2.1` |
| .NET Core 2.0 | `dotnet-sdk@2.0` |
| .NET Core 1.1 | `dotnet-sdk@1.1` |
| .NET Core 1.0 | `dotnet-sdk@1.0` |

## Mono MDK Version

Before Microsoft created a cross-platform .NET, there is a community-developed .NET-compatible implementation called Mono.

| Version | Formula |
|---------|---------|
| Mono MDK 6.12 | `mono-mdk@6.12` |

## Xamarin Versions

Following packages depend on [Mono MDK](#mono-mdk-version).

| Version | Formula |
|---------|---------|
| Xamarin Mac | `xamarin-mac@9.3` |
| Xamarin iOS | `xamarin-ios@16.4` |
| Xamarin Android | `xamarin-android@13.2` |

## Visual Studio for Mac

Following apps requires [Mono MDK](#mono-mdk-version).

| Version | Formula |
|---------|---------|
| Visual Studio 2022 for Mac | `visual-studio@2022` |

## Brief History of .NET

If you're confused with .NET version and naming, here's a brief history:

1.  **2002:** .NET Framework (Windows-only, closed source)
2.  **2004:** Mono (Community-driven, cross-platform, open source)
3.  **2011:** Xamarin (Mono for Android, MonoTouch for iOS)
4.  **2016:** Microsoft acquires Xamarin & releases .NET Core (Modern, cross-platform, open source)
5.  **2020:** .NET 5 unifies everything into one platform, simply called **.NET** (cross-platform, open source)
