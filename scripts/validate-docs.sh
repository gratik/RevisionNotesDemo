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
reachable_refs_file="$(mktemp)"
queue_file="$(mktemp)"
next_queue_file="$(mktemp)"
trap 'rm -f "$all_refs_file" "$indexed_refs_file" "$reachable_refs_file" "$queue_file" "$next_queue_file"' EXIT

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

# Seed recursive reachability from primary navigation files.
for index_file in "$ROOT_DIR/README.md" "$DOCS_DIR/README.md" "$DOCS_DIR/Learning-Path.md"; do
  printf '%s\n' "$index_file" >> "$indexed_refs_file"
done

while IFS= read -r seed_file; do
  [[ -z "$seed_file" ]] && continue
  if [[ -f "$seed_file" && "$seed_file" == *.md ]]; then
    printf '%s\n' "$seed_file" >> "$reachable_refs_file"
    printf '%s\n' "$seed_file" >> "$queue_file"
  fi
done < "$indexed_refs_file"

sort -u "$reachable_refs_file" -o "$reachable_refs_file"
sort -u "$queue_file" -o "$queue_file"

# Recursively walk markdown links so nested topic pages are considered reachable.
while [[ -s "$queue_file" ]]; do
  : > "$next_queue_file"

  while IFS= read -r current_file; do
    [[ -z "$current_file" ]] && continue
    [[ ! -f "$current_file" ]] && continue
    [[ "$current_file" != *.md ]] && continue

    while IFS= read -r raw_link; do
      [[ -z "$raw_link" ]] && continue
      resolved="$(resolve_link "$current_file" "$raw_link" || true)"
      [[ -z "${resolved:-}" ]] && continue
      [[ ! -f "$resolved" ]] && continue
      [[ "$resolved" != *.md ]] && continue

      if ! grep -Fxq "$resolved" "$reachable_refs_file"; then
        printf '%s\n' "$resolved" >> "$reachable_refs_file"
        printf '%s\n' "$resolved" >> "$next_queue_file"
      fi
    done < <(extract_links "$current_file")
  done < "$queue_file"

  if [[ ! -s "$next_queue_file" ]]; then
    break
  fi

  sort -u "$reachable_refs_file" -o "$reachable_refs_file"
  sort -u "$next_queue_file" -o "$next_queue_file"
  cp "$next_queue_file" "$queue_file"
done

while IFS= read -r doc_page; do
  base="$(basename "$doc_page")"
  if [[ "$base" == "README.md" ]]; then
    continue
  fi

  if ! grep -Fxq "$doc_page" "$reachable_refs_file"; then
    echo "ORPHAN DOC: $doc_page is not reachable from README.md, docs/README.md, or docs/Learning-Path.md"
    error_count=$((error_count + 1))
  fi
done < <(find "$DOCS_DIR" -type f -name '*.md' | sort)

if [[ "$error_count" -gt 0 ]]; then
  echo "Docs validation failed with $error_count issue(s)."
  exit 1
fi

echo "Docs validation passed."
