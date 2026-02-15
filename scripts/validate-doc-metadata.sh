#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCS_DIR="$ROOT_DIR/Learning/docs"
error_count=0

trim() {
  local value="$1"
  # shellcheck disable=SC2001
  echo "$value" | sed -E 's/^[[:space:]]+//; s/[[:space:]]+$//'
}

echo "Running doc metadata checks..."

while IFS= read -r file; do
  if ! rg -q '^## Metadata$' "$file"; then
    echo "MISSING METADATA SECTION: $file"
    error_count=$((error_count + 1))
    continue
  fi

  owner_line="$(rg -m1 '^- Owner:' "$file" || true)"
  updated_line="$(rg -m1 '^- Last updated:' "$file" || true)"
  prereq_line="$(rg -m1 '^- Prerequisites:' "$file" || true)"
  related_line="$(rg -m1 '^- Related examples:' "$file" || true)"

  for line_name in owner_line updated_line prereq_line related_line; do
    if [[ -z "${!line_name}" ]]; then
      echo "MISSING METADATA FIELD: $file -> ${line_name/_line/}"
      error_count=$((error_count + 1))
    fi
  done

  owner="$(trim "${owner_line#- Owner:}")"
  updated="$(trim "${updated_line#- Last updated:}")"
  prereq="$(trim "${prereq_line#- Prerequisites:}")"
  related="$(trim "${related_line#- Related examples:}")"

  if [[ -z "$owner" || -z "$updated" || -z "$prereq" || -z "$related" ]]; then
    echo "EMPTY METADATA VALUE: $file"
    error_count=$((error_count + 1))
    continue
  fi

  if ! echo "$updated" | rg -q '^[A-Za-z]+ [0-9]{1,2}, [0-9]{4}$'; then
    echo "INVALID DATE FORMAT: $file -> '$updated' (expected 'Month D, YYYY')"
    error_count=$((error_count + 1))
  fi

  IFS=',' read -r -a related_items <<< "$related"
  for item in "${related_items[@]}"; do
    path="$(trim "$item")"
    if [[ -z "$path" ]]; then
      echo "EMPTY RELATED EXAMPLE PATH: $file"
      error_count=$((error_count + 1))
      continue
    fi

    if [[ ! -e "$ROOT_DIR/$path" ]]; then
      echo "MISSING RELATED EXAMPLE FILE: $file -> $path"
      error_count=$((error_count + 1))
    fi
  done
done < <(find "$DOCS_DIR" -type f -name '*.md' | sort)

if [[ "$error_count" -gt 0 ]]; then
  echo "Doc metadata validation failed with $error_count issue(s)."
  exit 1
fi

echo "Doc metadata validation passed."
