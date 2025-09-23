cask "dotnet-sdk@1.1" do
  arch arm: "x64", intel: "x64"

  version "1.1.14"
  sha256   arm: "0769d804e226e96d377b48090718dc9b60c2ab9cc73e7a8ff65868d8e814b63a",
         intel: "0769d804e226e96d377b48090718dc9b60c2ab9cc73e7a8ff65868d8e814b63a"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-dev-osx-#{arch}.#{version}.pkg"
  name ".NET Core SDK 1.1"
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
