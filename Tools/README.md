# Dev Tools

Helper scripts for releasing and versioning Unity packages.

## Bump version

Use this before a release to increment the version in `package.json`.

```sh
./bump-version.sh [patch|minor|major] [optional-suffix]
```

Examples:

```sh
./bump-version.sh patch
./bump-version.sh minor -dev
./bump-version.sh patch -alpha.20250528
```

## Release current version

Tags the current version in `package.json` and pushes the tag to GitHub.

```sh
./release.sh
```

This creates an annotated Git tag like:

```
v1.0.1 Release version 1.0.1
```

> [!NOTE]
> 🚫 The script will abort if the tag already exists.
