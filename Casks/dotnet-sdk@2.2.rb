cask "dotnet-sdk@2.2" do
  arch arm: "x64", intel: "x64"

  version "2.2.207"
  sha256   arm: "ca1fe559025ee3c41f1664eba26079a705c42865dbf5c83c0160e682e9ad83cb",
         intel: "ca1fe559025ee3c41f1664eba26079a705c42865dbf5c83c0160e682e9ad83cb"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-sdk-#{version}-osx-#{arch}.pkg"
  name ".NET Core 2.2 SDK"
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
