# Controls & Commands

In game, the same list arrives as phone-style texts on a new game. Reread them any time in **Pause > Brief > Notifications**, or resend with **F10 > Text me the controls**.

## In game

| Key | Does | From |
|---|---|---|
| **F10** | Another Life menu (M0 debug: status, money, checks, teleports, save) | Another Life |
| **F9** | Reload `scripts/AnotherLife/data/*.json` after editing | Another Life |
| **E** | Use an ATM or payphone when prompted | Another Life |
| **F8** | Open/close Menyoo (gamepad: RB + Left) | Menyoo |
| **F7** | Spooner mode (moved from F9) | Menyoo |
| **F3** | Free cam | Menyoo |
| **Shift + X** | Freemode Identity menu (look, wallet, master switch) | Freemode Identity |
| **F4** | SHVDN console (script errors show here) | ScriptHookVDotNet |
| **Backspace** | Back / close a menu | Menus |

### One-time character setup

1. **F8** Menyoo → Player Options → Model Changer → `mp_m_freemode_01` or `mp_f_freemode_01`.
2. Build the look; dress in grey sweats, plain T-shirt, white sneakers.
3. **Shift + X** Freemode Identity → master switch on → **Save**.
4. Quit, and have Claude set `[Penalties] Enabled = False` / `Confiscate = False` in `FreemodeIdentity.ini`.

## On the PC (PowerShell, from `C:\Dev\anotherlife`)

| Command | Does |
|---|---|
| `powershell -ExecutionPolicy Bypass -File build\deploy.ps1` | Test, build, copy the mod into the GTA folder |
| `... build\deploy.ps1 -NoData` | Same, but keep JSON you edited in game |
| `powershell -ExecutionPolicy Bypass -File build\install-prereqs.ps1` | Install SHV, SHVDN, Menyoo, Freemode Identity from `docs\files` |
| `... build\install-prereqs.ps1 -Uninstall` | Remove exactly what that installed |
| `dotnet test` | Rules unit tests |

## Files worth knowing (GTA V folder)

| File | What's in it |
|---|---|
| `scripts\AnotherLife\log.txt` | Another Life errors and events |
| `scripts\AnotherLife\positions.txt` | Spots captured with F10 → Log my position |
| `scripts\AnotherLife\save\slot1.json` | Your Another Life save (`.bak` = previous) |
| `ScriptHookV.log` | Missing after launch = BattlEye blocked the mods |
| `ScriptHookVDotNet.log` | C# script load errors |
| `asiloader.log` | Which `.asi` mods loaded |

## If something's off

- **Nothing loads** (no "Another Life loaded" message): BattlEye is on. Turn it off in the Rockstar Launcher settings and relaunch.
- **Right after a GTA update:** Script Hook V stops working until it's patched. Wait for the new version.
- **Backups** of your saves from before modding: `C:\Dev\gta-backups\2026-10-12`.
