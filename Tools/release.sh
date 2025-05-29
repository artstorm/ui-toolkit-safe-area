#!/bin/bash
set -euo pipefail

PACKAGE_JSON="Packages/com.bitbebop.ui-toolkit-safe-area/package.json"

if [[ ! -f "$PACKAGE_JSON" ]]; then
  echo "Error: $PACKAGE_JSON not found!"
  exit 1
fi

# Ensure we're on the main branch
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [[ "$CURRENT_BRANCH" != "main" ]]; then
  echo "Error: You must be on the 'main' branch to release. Current branch: $CURRENT_BRANCH"
  exit 1
fi

VERSION=$(jq -r .version < "$PACKAGE_JSON")

if git rev-parse "v$VERSION" >/dev/null 2>&1; then
  echo "Tag v$VERSION already exists. Aborting."
  exit 1
fi

echo "Releasing version $VERSION"

# Annotated tag with message
git tag -a "v$VERSION" -m "Release version $VERSION"

# For now, let's not push the tag automatically.
#git push origin "v$VERSION"
#echo "Tag v$VERSION pushed!"
echo "v$VERSION tagged!"
