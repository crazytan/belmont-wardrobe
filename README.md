# Belmont Wardrobe

An experimental BepInEx 6 IL2CPP plugin for **Castlevania: Belmont's Curse** on Windows.
Enables the five bonus-content entries found in the tested Steam build and asks
the game's own bonus manager to reconcile inventory when a save loads.

## Status

**Experimental prerelease.** Tested on Steam build **25975867**, Unity
**2022.3.62f2**, with BepInEx **6.0.0-be.788**.
The game log confirms that all five bonus packs report unlocked. Delivery into a
loaded save, costume selection, uninstall/relock behavior, and long-term save
compatibility have **not yet been verified**. This is not a fully tested release.

## Included unlocks

Two editions are built from this source. **Full** targets all five entries below.
**Story Costumes**, intended for Nexus, targets only Sonia, Doppelganger, and Full
Curse. It excludes both platform bonus IDs and does not bypass platform verification.
Install only one edition at a time; they share a GUID and DLL location.

| Bonus ID | Contents |
| --- | --- |
| `4708550` | Trevor and Sypha costumes; `Charm_TankOneHit` bonus relic |
| `4708530` | Alucard costume |
| `BonusContentKonamiCode` | Sonia costume |
| `BonusContentDoppleganger` | Doppelganger costume |
| `BonusContentBadEnding` | Full Curse costume |

The first two entries are platform bonus content. This version bypasses their
platform eligibility checks. The other three entries normally unlock through
in-game conditions. This plugin does not complete those story objectives or
unlock movement abilities, weapons, achievements, soundtrack files, or gallery files.
No game assets, game assemblies, loader, or other mods are included.

## Requirements

- Windows PC version of Castlevania: Belmont's Curse.
- [BepInEx for Castlevania](https://www.nexusmods.com/castlevaniabelmontscurse/mods/3),
  installed and initialized by starting the game once.

## Install

1. Close the game. Back up your saves and settings from
   `%USERPROFILE%\AppData\LocalLow\KONAMI\Castlevania Belmont's Curse`.
2. Download the ZIP attached to the GitHub release. Extract its contents into
   the game folder containing `Castlevania Belmont's Curse.exe`.
3. Confirm the path is `BepInEx\plugins\MidnightBonuses\MidnightBonuses.dll`.
4. Start the game and load a save. Check **System > Character Skin** and the
   relic inventory.

When updating, replace the old DLL; do not install a second copy elsewhere in
`BepInEx\plugins`. The plugin GUID remains `local.midnightbonuses` for compatibility
with the initial local version.

## Verification and troubleshooting

Look in `BepInEx\LogOutput.log` for:

```text
[Info   :Belmont Wardrobe] Verified: all 5 targeted bonus packs unlocked.
```

After loading a save, the plugin should also log:

```text
[Info   :Belmont Wardrobe] Checked bonus inventory after save loaded.
```

The first line confirms bonus flags, not visual inventory delivery. The second
confirms the inventory check ran, not that each item was independently verified.
Report the game build, BepInEx version, and relevant log lines with any issue.
Remove usernames and personal paths from logs before posting them publicly.

Game updates can invalidate the referenced methods or bonus IDs. Disable the
plugin if it causes errors. Compatibility with other unlockers is untested.

## Uninstall and rollback

Close the game and remove `BepInEx\plugins\MidnightBonuses\MidnightBonuses.dll`.
The game may remove or relock bonus items when it checks eligibility without the
plugin. If needed, restore your backed-up saves and settings while the game is
closed; Steam Cloud can affect which save copy is restored.

The plugin does not directly edit save files or request persistent bonus flags
(`savePlayerPrefs` is false), but normal game saving can persist inventory changes.

## Build from source

Install PowerShell 7 and a .NET SDK, initialize BepInEx in your local game copy,
then run:

```powershell
pwsh -File .\build.ps1 -GamePath 'X:\path\to\Castlevania Belmont''s Curse'
```

Add `-Edition Story` to build only the three story/secret costumes. The default
is `-Edition Full`.

Output: `build\Full\MidnightBonuses.dll` or `build\Story\MidnightBonuses.dll`.
Proprietary game/interop assemblies are read
from your installation at build time and are not distributed by this repository.

## Implementation

- Calls `BonusContentManager.UnlockBonusContent(id, false, true)` for the five
  known IDs after the manager initializes.
- Prevents `LockBonusContent` from relocking those five IDs while loaded.
- Runs the game's inventory reconciliation after `OnFirstRoomLoaded`.
- Checks flags every three seconds; stops its update loop on an exception.

In the Story edition, only the three non-platform IDs are compiled in, and
`skipPlatformVerification` is false.

## Credits and license

Original plugin source generated with OpenAI Codex and reviewed through local
compilation and runtime log checks. It has not received an independent code audit.
BepInEx, HarmonyX, and Il2CppInterop are external dependencies with their own
licenses. Castlevania belongs to its respective rights holders; this project is
unofficial and not endorsed by KONAMI or the game's developers.

The original source in this repository is offered under the MIT license.

