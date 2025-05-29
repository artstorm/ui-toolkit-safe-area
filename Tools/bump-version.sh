#!/bin/bash
set -euo pipefail

PACKAGE_JSON="Packages/com.bitbebop.ui-toolkit-safe-area/package.json"

if [[ ! -f "$PACKAGE_JSON" ]]; then
  echo "Error: $PACKAGE_JSON not found!"
  exit 1
fi

# Usage info
usage() {
  echo "Usage: $0 [major|minor|patch] [pre-release-suffix]"
  echo "Examples:"
  echo "  $0 patch"
  echo "  $0 minor -dev"
  echo "  $0 patch -alpha.123"
  exit 1
}

if [[ $# -lt 1 ]]; then
  usage
fi

BUMP_TYPE=$1
PRERELEASE_SUFFIX=${2:-}

# Read current version
CURRENT_VERSION=$(jq -r .version "$PACKAGE_JSON")
if [[ ! "$CURRENT_VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9\.\-]+)?$ ]]; then
  echo "Error: Current version '$CURRENT_VERSION' is not semver compatible."
  exit 1
fi

# Strip prerelease suffix if any
BASE_VERSION=$(echo "$CURRENT_VERSION" | cut -d- -f1)

# Split into parts
IFS='.' read -r MAJOR MINOR PATCH <<< "$BASE_VERSION"

case "$BUMP_TYPE" in
  major)
    ((MAJOR++))
    MINOR=0
    PATCH=0
    ;;
  minor)
    ((MINOR++))
    PATCH=0
    ;;
  patch)
    ((PATCH++))
    ;;
  *)
    echo "Error: Unknown bump type '$BUMP_TYPE'. Use major, minor, or patch."
    usage
    ;;
esac

NEW_VERSION="$MAJOR.$MINOR.$PATCH"

if [[ -n "$PRERELEASE_SUFFIX" ]]; then
  NEW_VERSION="$NEW_VERSION$PRERELEASE_SUFFIX"
fi

echo "Bumping version: $CURRENT_VERSION → $NEW_VERSION"

# Update package.json
jq --arg newver "$NEW_VERSION" '.version = $newver' "$PACKAGE_JSON" > "$PACKAGE_JSON.tmp" && mv "$PACKAGE_JSON.tmp" "$PACKAGE_JSON"

echo "Updated $PACKAGE_JSON with new version $NEW_VERSION"