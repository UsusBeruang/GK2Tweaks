# GK2 Tweaks

GK2 Tweaks is a Graveyard Keeper 2 quality-of-life mod built for:

- BepInEx 5.4.23.5 (Windows x64)
- GK2 Mod Framework
- .NET Standard 2.1

The project consolidates several gameplay tweaks into one Framework-native mod with settings exposed through the Framework Mods UI.

## Development setup

1. Install BepInEx 5.4.23.5 into Graveyard Keeper 2.
2. Install `GK2.Framework.dll` into `BepInEx\plugins`.
3. Copy:

   ```text
   build\Local.props.example
   ```

   to:

   ```text
   build\Local.props
   ```

4. Set `GameDir` in `build\Local.props` to your local Graveyard Keeper 2 installation.
5. Optionally set `DeployToGame` to `true` to copy `GK2Tweaks.dll` into `BepInEx\plugins\GK2Tweaks` after a successful build.

`build\Local.props` is intentionally ignored by Git.

## Build

From the repository root:

```powershell
dotnet build .\GK2Tweaks.sln -c Debug
```

Release build:

```powershell
dotnet build .\GK2Tweaks.sln -c Release
```

The project validates the local BepInEx, GK2 Mod Framework, and Unity reference paths before compiling.

## Development workflow

Gameplay features are developed independently:

1. Create a feature branch from the current tested `main`.
2. Implement one feature only.
3. Open a pull request.
4. Build and runtime-test the PR.
5. Merge only after the feature passes in-game testing.

Planned feature areas:

- Auto Crafting
- Containers
- Item Stacks
- Player Inventory
- Big Item Stacking
- Player Movement
- Save Anywhere

Shared infrastructure changes use separate `chore/` branches and pull requests.

## Distribution

Release packages should contain only the files required by GK2 Tweaks. BepInEx, GK2 Mod Framework, Harmony, game assemblies, decompiled game source, build output, and IDE metadata are not distributed with the mod.
