# Installing Another Life (SP)

**Game:** GTA V **Legacy** (story mode). Enhanced is not supported yet.

## 1. One-time prerequisites

Install these into your GTA V folder (the one with `GTA5.exe`), skipping any you already have.

| What | Where to get it | Files that go in the GTA V folder |
|---|---|---|
| Script Hook V | dev-c.com/gtav/scripthookv | `ScriptHookV.dll`, `dinput8.dll` (the ASI loader) |
| ScriptHookVDotNet 3 (v3.6.0 or newer) | github.com/scripthookvdotnet/scripthookvdotnet/releases | `ScriptHookVDotNet.asi`, `ScriptHookVDotNet2.dll`, `ScriptHookVDotNet3.dll` |
| .NET Framework 4.8 | Built into Windows 10/11 | — |

Make a `scripts` folder in the GTA V folder if there isn't one.

## 2. Install the mod

Unzip `AnotherLife-<version>.zip` into the GTA V folder so you end up with:

```
GTA V/
  scripts/
    AnotherLife.Game.dll
    AnotherLife.Core.dll
    Newtonsoft.Json.dll
    LemonUI.SHVDN3.dll
    AnotherLife/
      data/        <- rules.json, checks.json, items.json, locations.json, crashpads.json
```

The mod creates these on first run:

```
    AnotherLife/
      save/slot1.json   <- your save (slot1.json.bak = the one before)
      log.txt           <- errors and events
      positions.txt     <- spots you capture with "Log my position"
```

If you already have a newer `LemonUI.SHVDN3.dll` or `Newtonsoft.Json.dll` from another mod, keep the newer one.

## 3. Play

Load story mode. You should see **"Another Life loaded"** in the bottom-left.

| Key | Does |
|---|---|
| F10 | Debug menu (M0) |
| F9 | Reload the data folder after editing JSON |

## 4. Tweaking

Edit any file in `scripts/AnotherLife/data/`, press **F9** in game, and changes apply without restarting. Typos show up as red "Data error" messages and in `log.txt`.

## Updating GTA V

Game updates break Script Hook V until it's patched, usually within a few days. Don't launch right after an update; wait for the new Script Hook V.

## Uninstall

Delete `scripts/AnotherLife*` and `scripts/AnotherLife/`. Your story-mode save is never touched.
