# Installing Another Life (SP)

**Game:** GTA V **Legacy** (story mode). Enhanced is not supported yet.

## 1. One-time prerequisites

Install these into your GTA V folder (the one with `GTA5.exe`), skipping any you already have.

| What | Where to get it | Files that go in the GTA V folder |
|---|---|---|
| Script Hook V | dev-c.com/gtav/scripthookv | `ScriptHookV.dll`, `dinput8.dll` (the ASI loader) |
| ScriptHookVDotNet 3 (v3.6.0 or newer) | github.com/scripthookvdotnet/scripthookvdotnet/releases | `ScriptHookVDotNet.asi`, `ScriptHookVDotNet2.dll`, `ScriptHookVDotNet3.dll` |
| .NET Framework 4.8 | Built into Windows 10/11 | — |
| LemonUI (SHVDN3) | github.com/LemonUIbyLemon/LemonUI/releases | `scripts/LemonUI.SHVDN3.dll` (also shipped in our zip) |
| **Freemode Identity** 1.1.7+ | gta5-mods.com/scripts/freemode-identity | `FreemodeIdentity.dll` in `scripts/`, its `.asi` shim in the GTA V folder |
| **Menyoo PC** | gta5-mods.com (Menyoo PC) | Per its readme |

Make a `scripts` folder in the GTA V folder if there isn't one.

**Menyoo's Spooner key is F9 by default, which clashes with Another Life's data reload.** In `menyooStuff/menyooConfig.ini` set `SpoonerModeHotkey = 118` (F7).

**Developers:** put the downloaded files in `docs/files/` (gitignored, laid out like the GTA V folder) and run `build\install-prereqs.ps1`. It skips `xinput1_4.dll` (the Enhanced loader) and `NativeTrainer.asi` (F4 clashes with the SHVDN console), and records what it adds in `AnotherLife-prereqs.manifest.txt` so `-Uninstall` removes exactly those files.

### Your character (one time)

1. In story mode, open **Menyoo** → Player Options → Model Changer → `mp_m_freemode_01` (male) or `mp_f_freemode_01` (female).
2. Build the look in Menyoo (face, hair, skin). Dress them in **grey sweats, a plain T-shirt and white sneakers**; everything else gets bought in-game.
3. Open **Freemode Identity** (Shift + X), turn the master switch on, and **Save** the look.

### Freemode Identity settings (`FreemodeIdentity.ini`)

Another Life handles death and arrest itself, so turn off Freemode Identity's versions:

```ini
[Penalties]
Enabled = False
Confiscate = False
```

Keep `[Wallet] Enabled = True`. The wallet **is** your cash in Another Life: vanilla shops, Ammu-Nation and job mods all spend and pay into it. Bank and crypto live in Another Life.

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
