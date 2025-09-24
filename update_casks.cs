#!/usr/bin/env dotnet run

using System;
using System.IO;
using System.Text.RegularExpressions;

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

    // Example usage
    public static void Main(string[] args)
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

            // Update with new values
            var newData = new CaskData
            {
                Version = "10.0.100-rc.1.25451.108", // New version
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
