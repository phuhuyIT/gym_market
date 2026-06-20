#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if command -v docker-compose >/dev/null 2>&1; then
  cd "$ROOT_DIR/MinIO" && docker-compose up -d
else
  cd "$ROOT_DIR/MinIO" && docker compose up -d
fi
