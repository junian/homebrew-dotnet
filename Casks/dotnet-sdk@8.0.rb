cask "dotnet-sdk@8.0" do
  arch arm: "arm64", intel: "x64"

  version "8.0.415"
  sha256 arm:   "06da68eaf72faec6a44afb5f352114c2230e0f8f1b784a24fe992fbfb83aa308",
         intel: "382d281c793379e052454702c682aa0739d8eb15a79f9fe4a9cb67f117276ab0"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-sdk-#{version}-osx-#{arch}.pkg"
  name ".NET 8 SDK (LTS)"
  desc "Developer platform"
  homepage "https://www.microsoft.com/net/core#macos"

  # This identifies releases with the same major/minor version as the current
  # cask version. New major/minor releases occur annually in November and the
  # check will automatically update its behavior when the cask is updated.
  livecheck do
    url "https://builds.dotnet.microsoft.com/dotnet/release-metadata/#{version.major_minor}/releases.json"
    regex(/^v?(\d+(?:\.\d+)+)$/i)
    strategy :json do |json, regex|
      json["releases"]&.map do |release|
        version = release.dig("sdk", "version")
        next unless version&.match(regex)

        version
      end
    end
  end

  depends_on macos: ">= :ventura"

  pkg "dotnet-sdk-#{version}-osx-#{arch}.pkg"

  uninstall pkgutil: [
    "com.microsoft.dotnet.*#{version.major_minor}*#{arch}",
  ]

  zap pkgutil: [
        "com.microsoft.dotnet.*",
        "com.microsoft.netstandard.pack.targeting.*",
      ],
      delete:  [
        "/etc/paths.d/dotnet",
        "/etc/paths.d/dotnet-cli-tools",
      ],
      trash:   [
        "~/.dotnet",
        "~/.nuget",
      ]
end
