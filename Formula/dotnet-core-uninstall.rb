class DotnetCoreUninstall < Formula
  desc "Tool for uninstalling .NET SDKs and runtimes"
  homepage "https://github.com/dotnet/cli-lab"
  # version "1.7.661902"
  license "MIT"

  on_arm do
    url "https://github.com/dotnet/cli-lab/releases/download/1.7.661902/dotnet-core-uninstall-osx-arm64.tar.gz"
    sha256 "fa410a8f506f833b3ef211671f1fc4e44a1d6f53473162fdf1e84c91463b6c3e"
  end

  on_intel do
    url "https://github.com/dotnet/cli-lab/releases/download/1.7.661902/dotnet-core-uninstall-osx-x64.tar.gz"
    sha256 "7f46a4f6556a03d2dab7ff72871e298aa721fe408aa3dabb3f6932bb1683a02c"
  end

  livecheck do
    url :stable
    strategy :github_latest
  end

  def install
    bin.install "dotnet-core-uninstall"
  end

  test do
    system bin/"dotnet-core-uninstall", "--help"
  end
end
