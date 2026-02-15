#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LEARNING_DIR="$ROOT_DIR/Learning"
PROJECT_FILE="$ROOT_DIR/RevisionNotesDemo.csproj"

error_count=0

echo "Running topic README structure checks..."
while IFS= read -r topic_dir; do
  readme="$topic_dir/README.md"
  if [[ ! -f "$readme" ]]; then
    echo "MISSING README: $topic_dir/README.md"
    error_count=$((error_count + 1))
    continue
  fi

  for section in \
    "## Learning goals" \
    "## Prerequisites" \
    "## Runnable examples" \
    "## Bad vs good examples summary" \
    "## Related docs"; do
    if ! grep -Fq "$section" "$readme"; then
      echo "README SECTION MISSING: $readme -> $section"
      error_count=$((error_count + 1))
    fi
  done
done < <(find "$LEARNING_DIR" -mindepth 1 -maxdepth 1 -type d ! -name docs | sort)

echo "Running DataAccess/Database ownership checks..."

if ! grep -Fq 'Compile Remove="Learning/Database/**/*.cs"' "$PROJECT_FILE"; then
  echo "PROJECT STRUCTURE ISSUE: RevisionNotesDemo.csproj must exclude Learning/Database/**/*.cs from compile items."
  error_count=$((error_count + 1))
fi

while IFS= read -r db_file; do
  base="$(basename "$db_file")"
  canonical="$LEARNING_DIR/DataAccess/$base"
  if [[ ! -f "$canonical" ]]; then
    echo "MISSING CANONICAL MATCH: $db_file expected $canonical"
    error_count=$((error_count + 1))
  fi
done < <(find "$LEARNING_DIR/Database" -maxdepth 1 -type f -name '*.cs' | sort)

if [[ "$error_count" -gt 0 ]]; then
  echo "Content structure validation failed with $error_count issue(s)."
  exit 1
fi

echo "Content structure validation passed."
