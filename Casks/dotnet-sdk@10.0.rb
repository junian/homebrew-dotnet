cask "dotnet-sdk@10.0" do
  arch arm: "arm64", intel: "x64"

  version "10.0.100-rc.2.25502.107"
  sha256 arm:   "cda7f488455749cfb33ede260d0a25715bd7b3588f67a3514107d76634e19c27",
         intel: "6e89ba33aa1bbaaaf3b108195e14e3ac317de9675788f3e58ab1c7b1a8c851b1"

  url "https://builds.dotnet.microsoft.com/dotnet/Sdk/#{version}/dotnet-sdk-#{version}-osx-#{arch}.pkg"
  name ".NET 10 RC-1 SDK"
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
