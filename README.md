# UI Toolkit Safe Area

<p align="center">
    <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/icon-dark.png">
    <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/icon-light.png">
    <img alt="Unity UI Toolkit Safe Area" src="https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/icon-light.png">
    </picture>
    <br/>
    <em>Provides a Safe Area Container for Unity's UI Toolkit.</em>
</p>

## About

The goal of the safe area container is to provide a custom control to simplify handling safe areas with UI Toolkit on relevant devices.

- Optional safe area and margin collapse.

## Usage

The SafeArea container is available in the Library in UI Builder and can be found under Project → Custom Controls and can be dragged into the UI hierarchy.

![UI Builder Hierarchy](https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/ui-builder-hierarchy.png)

The SafeArea container should be added as the top element in the hierarchy to ensure that it can fill up the entire screen. All child elements dropped into the SafeArea container will live inside `safe-area-content-container` and be adjusted accordingly depending on the current device SafeArea.

Margins can be set on the SafeArea container, useful when running on devices that does not utilize a safe area.

The container margins and the safe area is collapsed by default. That can be disabled in the inspector.

![UI Builder Inspector](https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/ui-builder-inspector.png)

Take a look at these screenshots which helps illustrate the differences how margin, safe area and collapse comes together. In each example the margin is set to `10px 10px 0px 10px`.

![UI Builder Inspector](https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/collapse-margin.png)

1. For a device without a safe area, the margin values are used as-is and collapse margins doesn't play a part as there is no safe area to collapse with.
2. For a device with a safe area the margins are collapsed with the safe area. The margin value is used if it would be larger than the safe area.
3. If `Collapse Margin` is disabled, the margin is added to the safe area.

## Installation

Requires Unity 2021.3 LTS or higher.

### Via Unity Package Manager and Git URL

Install the package directly in Unity Package Manger using this URL:

```
https://github.com/artstorm/ui-toolkit-safe-area.git?path=/Packages/com.bitbebop.ui-toolkit-safe-area
```

Open Unity Package Manager → <kbd>+</kbd> → Add package from git URL:

![Add package from git URL](https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/installation-git-1.png)

Paste URL:

![Paste git URL](https://raw.githubusercontent.com/artstorm/ui-toolkit-safe-area/main/.github/readme/installation-git-2.png)
