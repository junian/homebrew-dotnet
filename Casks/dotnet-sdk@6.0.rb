cask "dotnet-sdk@6.0" do
  arch arm: "arm64", intel: "x64"

  version "6.0.428"
  sha256 arm:   "a613c068212533d6b034cd614daf63633abcfe6697bc016d76a4ece1ce577c7f",
         intel: "17bd1338041f37b242379b78a92ed9a9529c79894f13e47ae1ecf890a339d3b2"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-sdk-#{version}-osx-#{arch}.pkg"
  name ".NET SDK 6.0 (LTS)"
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
