#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCS_DIR="$ROOT_DIR/docs"
error_count=0
HAS_RG=0
if command -v rg >/dev/null 2>&1; then
  HAS_RG=1
fi

trim() {
  local value="$1"
  # shellcheck disable=SC2001
  echo "$value" | sed -E 's/^[[:space:]]+//; s/[[:space:]]+$//'
}

echo "Running doc metadata checks..."

contains_pattern() {
  local pattern="$1"
  local file="$2"

  if [[ "$HAS_RG" -eq 1 ]]; then
    rg -q "$pattern" "$file"
  else
    grep -Eq "$pattern" "$file"
  fi
}

first_match() {
  local pattern="$1"
  local file="$2"

  if [[ "$HAS_RG" -eq 1 ]]; then
    rg -m1 "$pattern" "$file" || true
  else
    grep -Em1 "$pattern" "$file" || true
  fi
}

# Validate metadata blocks only on pages that already opt in with a metadata section.
# This keeps the gate strict for migrated pages while allowing incremental rollout.
while IFS= read -r file; do
  if ! contains_pattern '^## Metadata$' "$file"; then
    continue
  fi

  owner_line="$(first_match '^- Owner:' "$file")"
  updated_line="$(first_match '^- Last updated:' "$file")"
  prereq_line="$(first_match '^- Prerequisites:' "$file")"
  related_line="$(first_match '^- Related examples:' "$file")"

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

  if ! echo "$updated" | grep -Eq '^[A-Za-z]+ [0-9]{1,2}, [0-9]{4}$'; then
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
done < <(
  {
    find "$DOCS_DIR" -maxdepth 1 -type f -name '*.md'
    find "$DOCS_DIR" -type f -name 'README.md'
  } | sort -u
)

if [[ "$error_count" -gt 0 ]]; then
  echo "Doc metadata validation failed with $error_count issue(s)."
  exit 1
fi

echo "Doc metadata validation passed."
