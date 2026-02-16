#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCS_DIR="$ROOT_DIR/docs"

if [[ ! -d "$DOCS_DIR" ]]; then
  echo "ERROR: docs directory not found: $DOCS_DIR"
  exit 1
fi

error_count=0
all_refs_file="$(mktemp)"
indexed_refs_file="$(mktemp)"
trap 'rm -f "$all_refs_file" "$indexed_refs_file"' EXIT

extract_links() {
  local file="$1"
  perl -ne 'while (/\[[^\]]+\]\(([^)]+)\)/g) { print "$1\n"; }' "$file" 2>/dev/null || true
}

resolve_link() {
  local source_file="$1"
  local link="$2"

  # Remove anchor/query portion for filesystem resolution.
  link="${link%%#*}"
  link="${link%%\?*}"

  [[ -z "$link" ]] && return 1

  # Ignore template placeholder links.
  if [[ "$link" == *"<"* || "$link" == *">"* ]]; then
    return 1
  fi

  # Ignore remote links and non-file schemes.
  case "$link" in
    http://*|https://*|mailto:*|tel:*|javascript:*)
      return 1
      ;;
  esac

  local source_dir
  source_dir="$(cd "$(dirname "$source_file")" && pwd)"

  if [[ "$link" == /* ]]; then
    printf '%s\n' "$ROOT_DIR$link"
  else
    printf '%s\n' "$source_dir/$link"
  fi
}

check_file_links() {
  local file="$1"
  while IFS= read -r raw_link; do
    [[ -z "$raw_link" ]] && continue
    resolved="$(resolve_link "$file" "$raw_link" || true)"
    [[ -z "${resolved:-}" ]] && continue
    printf '%s\n' "$resolved" >> "$all_refs_file"
    if [[ ! -e "$resolved" ]]; then
      echo "BROKEN LINK: $file -> $raw_link"
      error_count=$((error_count + 1))
    fi
  done < <(extract_links "$file")
}

echo "Running docs link checks..."

# Validate links in root README and all markdown under Learning.
check_file_links "$ROOT_DIR/README.md"
while IFS= read -r md_file; do
  check_file_links "$md_file"
done < <(find "$ROOT_DIR/Learning" -type f -name '*.md' | sort)

echo "Running orphan docs checks..."

# Collect docs indexed from primary navigation files.
for index_file in "$ROOT_DIR/README.md" "$DOCS_DIR/README.md" "$DOCS_DIR/Learning-Path.md"; do
  while IFS= read -r raw_link; do
    [[ -z "$raw_link" ]] && continue
    resolved="$(resolve_link "$index_file" "$raw_link" || true)"
    [[ -z "${resolved:-}" ]] && continue
    printf '%s\n' "$resolved" >> "$indexed_refs_file"
  done < <(extract_links "$index_file")
done

sort -u "$indexed_refs_file" -o "$indexed_refs_file"

while IFS= read -r doc_page; do
  base="$(basename "$doc_page")"
  if [[ "$base" == "README.md" ]]; then
    continue
  fi

  if ! grep -Fxq "$doc_page" "$indexed_refs_file"; then
    echo "ORPHAN DOC: $doc_page is not linked from README.md, docs/README.md, or docs/Learning-Path.md"
    error_count=$((error_count + 1))
  fi
done < <(find "$DOCS_DIR" -type f -name '*.md' | sort)

if [[ "$error_count" -gt 0 ]]; then
  echo "Docs validation failed with $error_count issue(s)."
  exit 1
fi

echo "Docs validation passed."
