# Another Life (SP)

A single-player GTA V story-mode life sim built on ScriptHookVDotNet. You land at LSIA with a fake ID, $40 and a burner phone, work gig-app contracts, get licensed, and move up. Or you don't.

- [Design v0.3](docs/design.md)
- [Install](docs/install.md)
- [M0 test checklist](docs/test-checklist-m0.md)

## Status

**M0 skeleton built:** loads in story mode, JSON data with F9 hot-reload, save/load, d20 checks with practice-based skills, death/arrest/debt rules, addresses with postal codes, F10 debug menu. Next: **M1**, the character creator, arrival and crash pads.

## Build

Requires the .NET 8 SDK (builds the net48 game DLL via reference assemblies, on Windows or Linux).

```bash
dotnet test               # rules unit tests
build/package.sh m0       # -> dist/AnotherLife-m0.zip, unzip into the GTA V folder
```

On Windows, `powershell -ExecutionPolicy Bypass -File build\deploy.ps1` tests, builds and copies straight into the GTA V folder (`-GtaDir` or `GTA_DIR` to override the Steam path, `-NoData` to keep in-game JSON edits).

Every push also builds the zip in GitHub Actions (download it from the run's **Artifacts**).

## Layout

```
data/                     editable game data (copied to scripts/AnotherLife/data)
src/AnotherLife.Core/     rules, data loading, saves: no GTA references, unit-tested
src/AnotherLife.Game/     the ScriptHookVDotNet script (net48)
tests/                    xUnit tests for Core
build/package.sh          release zip
```

## Credits

- Postal codes: [DevBlocky/nearest-postal](https://github.com/DevBlocky/nearest-postal) `new-postals.json` (MIT, `licenses/nearest-postal.LICENSE`).
