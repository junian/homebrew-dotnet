cask "dotnet-sdk@11.0" do
  arch arm: "arm64", intel: "x64"

  version "11.0.100-preview.1.26104.118"
  sha256 arm:   "b30a0eda2dccd514421fa1bd22d1aa691fc6a857af5350968101b56dfa515bd4",
         intel: "b0e20f25cb0cc7464d77efcf29ab262129ec7e1d18193e616bb50816476ec1d4"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-sdk-#{version}-osx-#{arch}.pkg"
  name ".NET 11 SDK"
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
