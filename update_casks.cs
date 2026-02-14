#!/usr/bin/env dotnet run

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Security.Cryptography;

/// <summary>
/// JSON source generation context for System.Text.Json serialization.
/// Provides compile-time generated serialization code for .NET release metadata types.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DotnetBuilds.DotnetReleases))]
[JsonSerializable(typeof(DotnetBuilds.Release))]
[JsonSerializable(typeof(DotnetBuilds.Sdk))]
[JsonSerializable(typeof(DotnetBuilds.File))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

/// <summary>
/// Contains data models for .NET release metadata from the official .NET builds API.
/// </summary>
public class DotnetBuilds
{
    /// <summary>
    /// Represents the root object of .NET release metadata for a specific channel.
    /// </summary>
    public partial class DotnetReleases
    {
        /// <summary>
        /// Gets or sets the latest SDK version available in this release channel.
        /// </summary>
        [JsonPropertyName("latest-sdk")]
        public string? LatestSdk { get; set; }

        /// <summary>
        /// Gets or sets the list of all releases in this channel, ordered by release date.
        /// </summary>
        [JsonPropertyName("releases")]
        public List<Release>? Releases { get; set; }
    }

    /// <summary>
    /// Represents a single .NET release containing SDK and runtime information.
    /// </summary>
    public partial class Release
    {
        /// <summary>
        /// Gets or sets the SDK information for this release.
        /// </summary>
        [JsonPropertyName("sdk")]
        public Sdk? Sdk { get; set; }
    }

    /// <summary>
    /// Represents .NET SDK information including version and downloadable files.
    /// </summary>
    public partial class Sdk
    {
        /// <summary>
        /// Gets or sets the list of downloadable files for different platforms and architectures.
        /// </summary>
        [JsonPropertyName("files")]
        public List<File>? Files { get; set; }
    }

    /// <summary>
    /// Represents a downloadable file (installer, archive, etc.) for a specific platform.
    /// </summary>
    public partial class File
    {
        /// <summary>
        /// Gets or sets the filename of the downloadable file.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the download URL for this file.
        /// </summary>
        [JsonPropertyName("url")]
        public Uri? Url { get; set; }
    }
}

/// <summary>
/// Provides functionality to update Homebrew Cask files for .NET SDK installations.
/// Handles reading, parsing, updating cask files, and fetching latest .NET release information.
/// </summary>
public class RubyCaskUpdater
{
    /// <summary>
    /// Represents the data extracted from or to be written to a Homebrew Cask file.
    /// </summary>
    public class CaskData
    {
        /// <summary>
        /// Gets or sets the version string of the software package.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the SHA-256 hash for the ARM64 architecture installer.
        /// </summary>
        public string? Sha256Arm { get; set; }

        /// <summary>
        /// Gets or sets the SHA-256 hash for the Intel (x64) architecture installer.
        /// </summary>
        public string? Sha256Intel { get; set; }
    }

    /// <summary>
    /// Reads and parses a Homebrew Cask file to extract version and SHA-256 information.
    /// </summary>
    /// <param name="filePath">The path to the Ruby cask file.</param>
    /// <returns>A <see cref="CaskData"/> object containing the parsed information.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the cask file does not exist.</exception>
    public static CaskData ReadCaskFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cask file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        return ParseCaskContent(content);
    }

    /// <summary>
    /// Parses the content of a Homebrew Cask file to extract version and SHA-256 values.
    /// </summary>
    /// <param name="content">The raw content of the cask file.</param>
    /// <returns>A <see cref="CaskData"/> object with extracted values.</returns>
    public static CaskData ParseCaskContent(string content)
    {
        var caskData = new CaskData();

        // Parse version
        var versionMatch = Regex.Match(content, @"version\s+""([^""]+)""");
        if (versionMatch.Success)
            caskData.Version = versionMatch.Groups[1].Value;

        // Parse SHA256 values
        var sha256Pattern = @"sha256\s+arm:\s+""([^""]+)"",\s*intel:\s+""([^""]+)""";
        var sha256Match = Regex.Match(content, sha256Pattern);
        if (sha256Match.Success)
        {
            caskData.Sha256Arm = sha256Match.Groups[1].Value;
            caskData.Sha256Intel = sha256Match.Groups[2].Value;
        }

        return caskData;
    }

    /// <summary>
    /// Updates a Homebrew Cask file with new version and SHA-256 information.
    /// </summary>
    /// <param name="filePath">The path to the Ruby cask file to update.</param>
    /// <param name="newData">The new data to write to the cask file.</param>
    /// <exception cref="FileNotFoundException">Thrown when the cask file does not exist.</exception>
    public static void UpdateCaskFile(string filePath, CaskData newData)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cask file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        var updatedContent = UpdateCaskContent(content, newData);
        File.WriteAllText(filePath, updatedContent);
    }

    /// <summary>
    /// Updates the content of a Homebrew Cask file with new version and SHA-256 values using regex replacement.
    /// </summary>
    /// <param name="content">The original cask file content.</param>
    /// <param name="newData">The new data to replace in the content.</param>
    /// <returns>The updated cask file content.</returns>
    public static string UpdateCaskContent(string content, CaskData newData)
    {
        // Update version - FIXED: Use ${1} instead of $1 to avoid ambiguity with numbers
        if (!string.IsNullOrEmpty(newData.Version))
        {
            content = Regex.Replace(content,
                @"(version\s+"")[^""]+("")",
                "${1}" + newData.Version + "${2}");
        }

        // Update SHA256 values - FIXED: Use ${n} syntax
        if (!string.IsNullOrEmpty(newData.Sha256Arm) && !string.IsNullOrEmpty(newData.Sha256Intel))
        {
            content = Regex.Replace(content,
                @"(sha256\s+arm:\s+"")[^""]+("",\s*intel:\s+"")[^""]+("")",
                "${1}" + newData.Sha256Arm + "${2}" + newData.Sha256Intel + "${3}");
        }

        return content;
    }

    /// <summary>
    /// Fetches and deserializes .NET release metadata from the official builds API.
    /// </summary>
    /// <param name="url">The URL to the releases.json endpoint for a specific .NET version.</param>
    /// <returns>A <see cref="DotnetBuilds.DotnetReleases"/> object, or null if the request fails.</returns>
    public static async Task<DotnetBuilds.DotnetReleases?> GetDotnetReleasesAsync(string url)
    {
        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            // Console.WriteLine(json);
            var result = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.DotnetReleases);

            return result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Downloads a file from the specified URI and calculates its SHA-256 hash.
    /// </summary>
    /// <param name="fileUri">The URI of the file to download.</param>
    /// <returns>The SHA-256 hash as a lowercase hexadecimal string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when fileUri is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public static async Task<string> DownloadAndCalculateSha256Async(Uri fileUri)
    {
        ArgumentNullException.ThrowIfNull(fileUri);

        using var httpClient = new HttpClient();
        using var sha256 = SHA256.Create();
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        using var response = await httpClient.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        byte[] hashBytes = await sha256.ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// The filename for the .NET SDK ARM64 (Apple Silicon) installer package.
    /// </summary>
    const string DotnetArm64Filename = "dotnet-sdk-osx-arm64.pkg";

    /// <summary>
    /// The filename for the .NET SDK x64 (Intel) installer package.
    /// </summary>
    const string DotnetX64Filename = "dotnet-sdk-osx-x64.pkg";

    /// <summary>
    /// Main entry point that checks for new .NET SDK releases and updates corresponding Homebrew Cask files.
    /// Iterates through supported .NET versions, fetches latest release information, downloads installers,
    /// calculates SHA-256 hashes, and updates the cask files accordingly.
    /// </summary>
    /// <param name="args">Command line arguments (currently unused).</param>
    public static async Task Main(string[] args)
    {
        var supportedVersions = new List<string>
        {
            "11.0",
            "10.0",
            "9.0",
            "8.0",
            // "7.0", // Out of support May 14, 2024
            // "6.0", // Out of support November 12, 2024
        };

        foreach (var version in supportedVersions)
        {
            try
            {
                Console.WriteLine($"Checking .NET {version}");
                var filePath = $"./Casks/dotnet-sdk@{version}.rb"; // Path to your Ruby cask file

                // Read current values
                var currentData = ReadCaskFile(filePath);

                Console.WriteLine($"Current version: {currentData.Version}");
                Console.WriteLine($"Current SHA256 ARM: {currentData.Sha256Arm}");
                Console.WriteLine($"Current SHA256 Intel: {currentData.Sha256Intel}");
                Console.WriteLine("");

                var dotnetRelease = await GetDotnetReleasesAsync($"https://builds.dotnet.microsoft.com/dotnet/release-metadata/{version}/releases.json");
                if (dotnetRelease == null || dotnetRelease.LatestSdk == null || currentData.Version == dotnetRelease.LatestSdk)
                {
                    Console.WriteLine($"No new release for .NET {version}");
                    Console.WriteLine("");
                    continue;
                }

                var latestSdkVersion = dotnetRelease.LatestSdk;
                Console.WriteLine($"New Release for .NET {version} is Available: {latestSdkVersion}");

                var sdkList = (dotnetRelease?.Releases?.First()?.Sdk) ?? throw new Exception("SDK is NULL");

                var arm64Release = sdkList.Files?.SingleOrDefault(x => x.Name == DotnetArm64Filename);
                if (arm64Release == null || arm64Release.Url == null)
                    throw new Exception($".NET {version}-arm64 URL not found");

                Console.WriteLine($"Calculating SHA-256 {arm64Release.Url}");
                var sha256arm64 = await DownloadAndCalculateSha256Async(arm64Release.Url);
                Console.WriteLine(sha256arm64);
                Console.WriteLine("");

                var x64Release = sdkList.Files?.SingleOrDefault(x => x.Name == DotnetX64Filename);
                if (x64Release == null || x64Release.Url == null)
                    throw new Exception($".NET {version}-x64 URL not found");

                Console.WriteLine($"Calculating SHA-256 {x64Release.Url}");
                var sha256x64 = await DownloadAndCalculateSha256Async(x64Release.Url);
                Console.WriteLine(sha256x64);
                Console.WriteLine("");

                // Update with new values
                var newData = new CaskData
                {
                    Version = latestSdkVersion, // New version
                    Sha256Arm = sha256arm64, // New ARM SHA256
                    Sha256Intel = sha256x64 // New Intel SHA256
                };

                // Update the file
                UpdateCaskFile(filePath, newData);
                Console.WriteLine($".NET {version} Cask file updated successfully!");
                Console.WriteLine("");

                // Verify the changes
                var updatedData = ReadCaskFile(filePath);
                Console.WriteLine($"Updated version: {updatedData.Version}");
                Console.WriteLine($"Updated SHA256 ARM: {updatedData.Sha256Arm}");
                Console.WriteLine($"Updated SHA256 Intel: {updatedData.Sha256Intel}");
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
