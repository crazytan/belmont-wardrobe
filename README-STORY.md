# Belmont Wardrobe — Story Costumes

Experimental BepInEx plugin for Castlevania: Belmont's Curse on Windows.

Unlocks only these three normally obtainable in-game costumes:

- Sonia — the Konami Code bonus
- Doppelganger
- Full Curse — the bad-ending bonus

This edition does not unlock Alucard, Trevor, Sypha, or the Midnight bonus relic.
It does not bypass platform bonus ownership checks. It does not complete quests,
grant achievements, unlock weapons, or change movement abilities.

## Install

1. Close the game and back up saves/settings from
   `%USERPROFILE%\AppData\LocalLow\KONAMI\Castlevania Belmont's Curse`.
2. Install and initialize BepInEx for Castlevania (BepInEx 6 IL2CPP, build 788).
3. Extract this archive into the folder containing the game's EXE.
4. Verify `BepInEx\plugins\MidnightBonuses\MidnightBonuses.dll` exists.
5. Launch the game and load a save. Check System > Character Skin.

Install only one copy of this plugin. The `MidnightBonuses` DLL/folder name is
retained for upgrade compatibility with the original local plugin.

## Status and logs

Experimental release. Startup-tested on Steam build 25975867, Unity 2022.3.62f2,
with BepInEx 6.0.0-be.788. The bonus flags were verified through game logs;
visual costume selection, inventory delivery after loading a save, uninstall
behavior, and long-term save compatibility have not yet been verified.

Look in `BepInEx\LogOutput.log` for:
`Verified: all 3 targeted bonus packs unlocked.`
After a save loads, the plugin should log:
`Checked bonus inventory after save loaded.`
This confirms the inventory check ran, not that each item was independently verified.

## Uninstall

Close the game and remove `BepInEx\plugins\MidnightBonuses\MidnightBonuses.dll`.
Unlocks/items may remain in normal game saves or be relocked by the game. Restore
your backup if needed, accounting for Steam Cloud.

The plugin does not directly edit save files or request persistent bonus flags,
but normal game saving can persist inventory changes. No networking or bundled
game assets. Built with BepInEx, HarmonyX, and Il2CppInterop as external dependencies.
Original source was generated with OpenAI Codex. MIT licensed; see LICENSE.
Unofficial; not affiliated with KONAMI or the developers.
