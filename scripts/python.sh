#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

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

cd "$ROOT_DIR/Python_server"
exec "$(python_bin)" -m uvicorn app:app --reload
