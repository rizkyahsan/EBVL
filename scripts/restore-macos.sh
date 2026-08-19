#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

dotnet restore EBVL.slnx \
  --configfile NuGet.Offline.Config \
  --ignore-failed-sources

echo "Restore completed using the repository-local Pertamina 1.0.4 feed."
