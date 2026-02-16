#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCS_DIR="$ROOT_DIR/docs"
error_count=0

required_sections=(
  "## Interview Answer Block"
  "## Interview Bad vs Strong Answer"
  "## Interview Timed Drill"
)

echo "Running interview section coverage checks..."

while IFS= read -r file; do
  for section in "${required_sections[@]}"; do
    if ! grep -Fq "$section" "$file"; then
      echo "MISSING INTERVIEW SECTION: $file -> $section"
      error_count=$((error_count + 1))
    fi
  done
done < <(find "$DOCS_DIR" -type f -name '*.md' | sort)

if [[ "$error_count" -gt 0 ]]; then
  echo "Interview section validation failed with $error_count issue(s)."
  exit 1
fi

echo "Interview section validation passed."
