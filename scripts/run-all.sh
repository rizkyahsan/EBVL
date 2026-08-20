#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
api_project="$repo_root/src/02.BackEnd/05.BackEnd.WebApi/05.BackEnd.WebApi.csproj"
ui_project="$repo_root/src/03.FrontEnd/05.FrontEnd.WebUi/05.FrontEnd.WebUi.csproj"
api_secrets="$repo_root/src/02.BackEnd/05.BackEnd.WebApi/secrets.json"
ui_secrets="$repo_root/src/03.FrontEnd/05.FrontEnd.WebUi/secrets.json"

for secrets_file in "$api_secrets" "$ui_secrets"; do
  if [[ ! -f "$secrets_file" ]]; then
    echo "Missing required local configuration: $secrets_file" >&2
    exit 1
  fi
done

if ! docker info >/dev/null 2>&1; then
  echo "Docker Desktop is not running. Start Docker Desktop and retry this task." >&2
  exit 1
fi

if ! docker ps --format '{{.Names}}' | grep -qx 'sqlserver'; then
  echo "The required SQL Server container named 'sqlserver' is not running." >&2
  exit 1
fi

is_azurite_ready() {
  curl --silent --show-error --dump-header - --output /dev/null \
    'http://127.0.0.1:10000/devstoreaccount1?comp=list' 2>/dev/null |
    grep -qi '^Server: Azurite-Blob/'
}

if is_azurite_ready; then
  azurite_container="$(docker ps --filter publish=10000 --format '{{.Names}}' | head -n 1)"
  echo "Reusing Azurite on port 10000${azurite_container:+ (container: $azurite_container)} ..."
elif docker container inspect ebvl-azurite >/dev/null 2>&1; then
  echo "Starting the existing Azurite container ..."
  if ! docker start ebvl-azurite >/dev/null; then
    echo "Unable to start ebvl-azurite. Port 10000 may be used by a non-Azurite service." >&2
    echo "Stop that service or change its published port, then retry this task." >&2
    exit 1
  fi
else
  echo "Creating the Azurite container ..."
  docker run --detach \
    --name ebvl-azurite \
    --publish 10000:10000 \
    mcr.microsoft.com/azure-storage/azurite:latest \
    azurite-blob --blobHost 0.0.0.0 --skipApiVersionCheck >/dev/null
fi

for attempt in {1..30}; do
  if is_azurite_ready; then
    break
  fi

  if [[ "$attempt" -eq 30 ]]; then
    echo "Azurite did not become ready on port 10000." >&2
    exit 1
  fi

  sleep 1
done

api_pid=""
ui_pid=""

stop_apps() {
  trap - EXIT INT TERM

  if [[ -n "$api_pid" ]] && kill -0 "$api_pid" 2>/dev/null; then
    kill "$api_pid" 2>/dev/null || true
  fi

  if [[ -n "$ui_pid" ]] && kill -0 "$ui_pid" 2>/dev/null; then
    kill "$ui_pid" 2>/dev/null || true
  fi

  wait "$api_pid" "$ui_pid" 2>/dev/null || true
}

trap stop_apps EXIT INT TERM

echo "Restoring packages from the repository feeds ..."
dotnet restore "$repo_root/EBVL.slnx" \
  --configfile "$repo_root/NuGet.Offline.Config" \
  --ignore-failed-sources \
  -p:NuGetAudit=false

echo "Building the solution ..."
dotnet build "$repo_root/EBVL.slnx" \
  --no-restore \
  --disable-build-servers \
  -p:UseSharedCompilation=false \
  -nodeReuse:false \
  -m:1

echo "Starting WebApi at https://localhost:44421 ..."
dotnet run \
  --project "$api_project" \
  --launch-profile WebApi \
  --no-build \
  --no-restore \
  &
api_pid=$!

echo "Starting WebUi at https://localhost:44422/e-bvl ..."
dotnet run \
  --project "$ui_project" \
  --launch-profile WebUi \
  --no-build \
  --no-restore \
  &
ui_pid=$!

while kill -0 "$api_pid" 2>/dev/null && kill -0 "$ui_pid" 2>/dev/null; do
  sleep 1
done

if ! kill -0 "$api_pid" 2>/dev/null; then
  wait "$api_pid"
else
  wait "$ui_pid"
fi
