#!/usr/bin/env dotnet run

#:package Newtonsoft.Json@13.0.4

using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class DotnetBuilds
{
    public partial class DotnetReleases
    {
        [JsonProperty("channel-version")]
        public string? ChannelVersion { get; set; }

        [JsonProperty("latest-release")]
        public string? LatestRelease { get; set; }

        [JsonProperty("latest-release-date")]
        public string? LatestReleaseDate { get; set; }

        [JsonProperty("latest-runtime")]
        public string? LatestRuntime { get; set; }

        [JsonProperty("latest-sdk")]
        public string? LatestSdk { get; set; }

        [JsonProperty("support-phase")]
        public string? SupportPhase { get; set; }

        [JsonProperty("release-type")]
        public string? ReleaseType { get; set; }

        [JsonProperty("releases")]
        public List<Release>? Releases { get; set; }
    }

    public partial class Release
    {
        [JsonProperty("release-date")]
        public string? ReleaseDate { get; set; }

        [JsonProperty("release-version")]
        public string? ReleaseVersion { get; set; }

        [JsonProperty("security")]
        public bool Security { get; set; }

        [JsonProperty("release-notes")]
        public Uri? ReleaseNotes { get; set; }

        [JsonProperty("runtime")]
        public Runtime? Runtime { get; set; }

        [JsonProperty("sdk")]
        public Sdk? Sdk { get; set; }
    }

    public partial class Runtime
    {
        [JsonProperty("version")]
        public string? Version { get; set; }

        [JsonProperty("version-display")]
        public string? VersionDisplay { get; set; }

        [JsonProperty("vs-version")]
        public string? VsVersion { get; set; }

        [JsonProperty("vs-mac-version")]
        public string? VsMacVersion { get; set; }

        [JsonProperty("files")]
        public List<File>? Files { get; set; }
    }

    public partial class File
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("rid")]
        public string? Rid { get; set; }

        [JsonProperty("url")]
        public Uri? Url { get; set; }

        [JsonProperty("hash")]
        public string? Hash { get; set; }
    }

    public partial class Sdk
    {
        [JsonProperty("version")]
        public string? Version { get; set; }

        [JsonProperty("version-display")]
        public string? VersionDisplay { get; set; }

        [JsonProperty("runtime-version")]
        public string? RuntimeVersion { get; set; }

        [JsonProperty("vs-version")]
        public string? VsVersion { get; set; }

        [JsonProperty("vs-mac-version")]
        public string? VsMacVersion { get; set; }

        [JsonProperty("vs-support")]
        public string? VsSupport { get; set; }

        [JsonProperty("vs-mac-support")]
        public string? VsMacSupport { get; set; }

        [JsonProperty("csharp-version")]
        public string? CsharpVersion { get; set; }

        [JsonProperty("fsharp-version")]
        public string? FsharpVersion { get; set; }

        [JsonProperty("vb-version")]
        public string? VbVersion { get; set; }

        [JsonProperty("files")]
        public List<File>? Files { get; set; }
    }
}

public class RubyCaskUpdater
{
    public class CaskData
    {
        public string Version { get; set; }
        public string Sha256Arm { get; set; }
        public string Sha256Intel { get; set; }
    }

    public static CaskData ReadCaskFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cask file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        return ParseCaskContent(content);
    }

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

    public static void UpdateCaskFile(string filePath, CaskData newData)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cask file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        var updatedContent = UpdateCaskContent(content, newData);
        File.WriteAllText(filePath, updatedContent);
    }

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

    public static async Task<DotnetBuilds.DotnetReleases?> GetDotnetReleasesAsync(string url)
    {
        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            // Console.WriteLine(json);
            var result = JsonConvert.DeserializeObject<DotnetBuilds.DotnetReleases>(json);

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

    const string DotnetArm64Filename = "dotnet-sdk-osx-arm64.pkg";
    const string DotnetX64Filename = "dotnet-sdk-osx-x64.pkg";

    // Example usage
    public static async Task Main(string[] args)
    {
        var supportedVersions = new List<string>
        {
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
