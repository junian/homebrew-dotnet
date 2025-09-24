#!/usr/bin/env dotnet run

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

public class DotnetBuilds
{
    public partial class DotnetReleases
    {
        public string ChannelVersion { get; set; }
        public string LatestRelease { get; set; }
        public string LatestReleaseDate { get; set; }
        public string LatestRuntime { get; set; }
        public string LatestSdk { get; set; }
        public string SupportPhase { get; set; }
        public string ReleaseType { get; set; }
        public Uri LifecyclePolicy { get; set; }
        public List<Release> Releases { get; set; }
    }

    public partial class Release
    {
        public string ReleaseDate { get; set; }
        public string ReleaseVersion { get; set; }
        public bool Security { get; set; }
        // public List<object> CveList { get; set; }
        public Uri ReleaseNotes { get; set; }
        public Runtime Runtime { get; set; }
        public Sdk Sdk { get; set; }
    }

    public partial class Runtime
    {
        public string Version { get; set; }
        public string VersionDisplay { get; set; }
        public string VsVersion { get; set; }
        public string VsMacVersion { get; set; }
        public List<File> Files { get; set; }
    }

    public partial class File
    {
        public string Name { get; set; }
        public string Rid { get; set; }
        public Uri Url { get; set; }
        public string Hash { get; set; }
    }

    public partial class Sdk
    {
        public string Version { get; set; }
        public string VersionDisplay { get; set; }
        public string RuntimeVersion { get; set; }
        public string VsVersion { get; set; }
        public string VsMacVersion { get; set; }
        public string VsSupport { get; set; }
        public string VsMacSupport { get; set; }
        public string CsharpVersion { get; set; }
        public string FsharpVersion { get; set; }
        public string VbVersion { get; set; }
        public List<File> Files { get; set; }
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

  public static async Task<DotnetBuilds.DotnetReleases> GetDotnetReleasesAsync(string url)
    {
        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DotnetBuilds.DotnetReleases>(json);

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

    // Example usage
    public static async Task Main(string[] args)
    {
        try
        {
            string filePath = "./Casks/dotnet-sdk@10.0.rb"; // Path to your Ruby cask file

            // Read current values
            Console.WriteLine("Reading cask file...");
            var currentData = ReadCaskFile(filePath);

            Console.WriteLine($"Current version: {currentData.Version}");
            Console.WriteLine($"Current SHA256 ARM: {currentData.Sha256Arm}");
            Console.WriteLine($"Current SHA256 Intel: {currentData.Sha256Intel}");

            var dotnetRelease = await GetDotnetReleasesAsync("https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json");

            if (currentData.Version == dotnetRelease.LatestSdk)
                return;

            // Update with new values
            var newData = new CaskData
            {
                Version = dotnetRelease.LatestSdk, // New version
                Sha256Arm = "new_arm_sha256_value_here", // New ARM SHA256
                Sha256Intel = "new_intel_sha256_value_here" // New Intel SHA256
            };
        

            // Create backup
            // string backupPath = filePath + ".backup";
            // File.Copy(filePath, backupPath, true);
            // Console.WriteLine($"Backup created: {backupPath}");

            // Update the file
            UpdateCaskFile(filePath, newData);
            Console.WriteLine("Cask file updated successfully!");

            // Verify the changes
            var updatedData = ReadCaskFile(filePath);
            Console.WriteLine($"\nUpdated version: {updatedData.Version}");
            Console.WriteLine($"Updated SHA256 ARM: {updatedData.Sha256Arm}");
            Console.WriteLine($"Updated SHA256 Intel: {updatedData.Sha256Intel}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
