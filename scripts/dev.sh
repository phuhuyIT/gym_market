#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PIDS=()

cleanup() {
  if [ "${#PIDS[@]}" -gt 0 ]; then
    kill "${PIDS[@]}" 2>/dev/null || true
  fi
}
trap cleanup EXIT INT TERM

compose() {
  if command -v docker-compose >/dev/null 2>&1; then
    docker-compose "$@"
  else
    docker compose "$@"
  fi
}

wait_for_port() {
  local host="$1"
  local port="$2"
  local name="$3"
  local max_attempts="${4:-60}"

  printf 'Waiting for %s on %s:%s' "$name" "$host" "$port"
  for _ in $(seq 1 "$max_attempts"); do
    if (echo >"/dev/tcp/$host/$port") >/dev/null 2>&1; then
      printf '\n'
      return 0
    fi
    printf '.'
    sleep 1
  done

  printf '\n%s did not become available on %s:%s\n' "$name" "$host" "$port" >&2
  return 1
}

python_bin() {
  if [ -x "$ROOT_DIR/Python_server/.venv/bin/python" ]; then
    printf '%s\n' "$ROOT_DIR/Python_server/.venv/bin/python"
  elif [ -x "$ROOT_DIR/venv/bin/python" ]; then
    printf '%s\n' "$ROOT_DIR/venv/bin/python"
  elif command -v python3 >/dev/null 2>&1; then
    printf '%s\n' "python3"
  else
    printf '%s\n' "python"
  fi
}

printf 'Starting Docker infrastructure...\n'
(cd "$ROOT_DIR/MinIO" && compose up -d)

wait_for_port 127.0.0.1 1433 "SQL Server" 90
wait_for_port 127.0.0.1 9000 "MinIO" 60

if [ "${SKIP_MIGRATIONS:-0}" != "1" ]; then
  printf 'Applying EF migrations...\n'
  (cd "$ROOT_DIR/GymMaket/GymMarket.API" && dotnet ef database update)
else
  printf 'Skipping EF migrations because SKIP_MIGRATIONS=1.\n'
fi

printf 'Starting Python server...\n'
(cd "$ROOT_DIR/Python_server" && "$(python_bin)" -m uvicorn app:app --reload) &
PIDS+=("$!")

printf 'Starting .NET API...\n'
(cd "$ROOT_DIR/GymMaket/GymMarket.API" && dotnet run --launch-profile http) &
PIDS+=("$!")

wait_for_port 127.0.0.1 5284 ".NET API" 90

printf 'Starting Angular client...\n'
(cd "$ROOT_DIR/gym_market_client" && npm start) &
PIDS+=("$!")

printf '\nGymMarket is starting:\n'
printf '  Frontend:   http://localhost:4200\n'
printf '  .NET API:   http://localhost:5284/swagger\n'
printf '  Python API: http://127.0.0.1:8000/docs\n'
printf '  MinIO UI:   http://localhost:9001\n\n'
printf 'Press Ctrl+C to stop the app servers. Docker containers stay running for faster next starts.\n'

wait -n "${PIDS[@]}"
