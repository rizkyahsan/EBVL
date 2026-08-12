#!/usr/bin/env bash

set -euo pipefail

repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
api_project="$repository_root/src/02.BackEnd/05.BackEnd.WebApi/05.BackEnd.WebApi.csproj"
webui_project="$repository_root/src/03.FrontEnd/05.FrontEnd.WebUi/05.FrontEnd.WebUi.csproj"

cleanup() {
  if [[ -n "${api_pid:-}" ]]; then
    kill "$api_pid" 2>/dev/null || true
  fi

  if [[ -n "${webui_pid:-}" ]]; then
    kill "$webui_pid" 2>/dev/null || true
  fi
}

trap cleanup EXIT INT TERM

dotnet build "$repository_root/EBVL.slnx" \
  --no-restore \
  -p:NuGetAudit=false \
  -m:1

dotnet run \
  --project "$api_project" \
  --launch-profile WebApi \
  --no-build \
  --no-restore &
api_pid=$!

dotnet run \
  --project "$webui_project" \
  --launch-profile WebUi \
  --no-build \
  --no-restore &
webui_pid=$!

wait "$api_pid" "$webui_pid"
