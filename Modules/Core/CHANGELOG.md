# Core Changelog

## 4.2.1

### Changed

* Updated Chinese translations by [Jumping-notes](https://github.com/Jumping-notes).

### Fixed

* Fixed GMCM issue in handheld systems removing all spaces from Dynamic List options.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.2.0

### Added

* Added new config presets which can be found in the "optionals" folder. The compatibility files have also been moved to this folder.
* Added Status Effect framework, previously from CMBT module.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.1.5

### Fixed

* GMCM Module Selection setting now translates correctly.
* Fixed an edge case where hitting Save on the menu would reset any DynamicList options that are **not** on the current menu page. Turns out GMCM doesn't quite behave in the way I expected, so had to implement a workaround.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.1.3

### Fixed

* Fixed GMCM list text boxes being limited to 13 characters.
* Fixed an edge case in SpanSplitter where splitting would fail if a substring is empty. This fixes an issue with Ridgeside's Fairy Needle weapon, which, for some reason, has no description.
* Fixed some translations. Added Chinese translation of GMCM strings by [Jumping-notes](https://github.com/Jumping-notes).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.1.2

### Changed

* Config options with custom logic now only trigger when the value changes. This avoids unecessarily triggering every single validation logic when saving via GMCM.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.1.1

### Changed

* Switched from storing menu initialization flag in custom data.json to storing it in config.json. This eliminates the extra file and allows users to set the flag as they wish. This also prevents users who carry over their config from seeing the initial module selection again. Honestly idk why I didn't do this in the first place.

### Fixed

* Fixed Debug objects being instantiated in Release mode.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.1.0

### Added

* Added dynamic list options to the GMCM, which means that any list config settings that previously had to be changed manually (like `CustomArtisanMachines`, `StabbingSwords`, `TaxRatePerBracket`, etc.) can now be changed in the menu in-game. With this, all settings are now available in the menu.

### Changed

* The GMCM is now self-generating (thanks to atravita for the idea and reference code). This does unfortunately mean that absolutely all translation keys for the menu have changed (sorry translators.)

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.0.1

### Added

* Added a warning to re-enable modules if checksum validation fails.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 4.0.0

### Added

* Debug mode now highlights every segment of the Royal Serpent.

### Changed

* Sprite assets organized into subfolders.
* Moved OutOfCombat MonsterTakeDamagePatcher to this module, where it should have been.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.1.4

### Added

* Re-added debug framework, which disappeared completely from the repo, apparently back in version 1.0?.

### Changed

* Now uses `IntegrationRegistry` from updated Shared lib.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.1.3

### Changed

* Rolled back dependency updates due to conflicts with AtraCore.

### Fixed

* Added missing translations. Improved some Chinese translations, thanks to xuzhi1977.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.1.2

### Added

* The config menu will now detect if gamepad mode is enabled and choose a controller-friendly layout. The detection only happens when the menu is constructed, and will not change dynamically if the player switches between gamepad mode and mouse/keyboard mode.
* Added Korean GMCM translations by [Jun9273](https://github.com/Jun9273).
* Added Chinese GMCM translations by [Jumping-notes](https://github.com/Jumping-notes).

### Changed

* Several lookups refactored to Shared lib.

### Fixed

* Added some missing translations and corrected some translation keys.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.2

### Added

* Added debuff framework to the API.

### Fixed

* Fixed a few more translation issues.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.1

### Fixed

* Fixed some missing translation issues.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.0

### Changed

* Changed several translation keys for better formatting with Pathoschild's Translation Generator. Possible mistakes may have been created.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.5.1

### Changed

* Lowered log level of mod conflicts to Debug.

### Fixed

* Fixed patching order of IClickableMenu.drawHoverText.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.5.0

### Changed

* Simplified the subfolders within assets. All textures are now consolidated under "sprites".

### Fixed

* Fixed an issue with patchers not obaying priority, which caused, for example, Automated Cheese Press to ignore Artisan bonuses when processing Large Milk, among most likely several other unidentified bugs.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.4.0

### Added

* Debug mode now also shows NPC names and current health in the case of monsters.

### Fixed

* Moved Textures out of Core and to each individual module, preventing a possible error when initializing if certain modules are disabled.
* Fixed core menu text not using correct translation keys for module selection option.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.3.0

### Added

* Added income and property tax information to the API.
* Added Pathoschild.Stardew.ModTranslationClassBuilder.
* Added all GMCM text options to i18n model, so the mod config menu can now be translated.

### Changed

* Cleaned up several config menus in the transition to Pathoschild.Stardew.ModTranslationClassBuilder.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.6

### Fixed

* Fixed a bug preventing the config menu from reloading correctly.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.5

### Fixed

* Made some changes to Integration framework to allow integrations to register when modules are activated via GMCM.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.3

### Added

* You can now set a key to directly open this mod's GMCM (default LeftShift + F12, because I use F12 as the GMCM key).
* Added some extra highlighting to debug mode for terrain features, large terrain features and objects.

### Changed

* Changed default DebugKey to "OemQuotes, OemTilde" (same as DebugMode mod).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.2

### Added

* Moved the API interface from the shared library to the mod source.

### Changed

* Moved some more stuff around.

### Removed

* Status conditions now live in CMBT. Because duh.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.1

### Fixed

* Checksum validation now happens on Saving instead of Saved, allowing it to persist immediately instead of only on the next day.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.0

### Added

* Added StackableBuff class.
* Added Status Effect framework. Each status condition has a neat correponding animation.
    - **Bleeding:** Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
    - **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magama Sparkers).
    - **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
    - **Freeze:** Cannot move or attack for 30s. The next hit during the duration deals double damage and ends the effect.
    - **Poisoned:** Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3×.
    - **Slowed:** Reduces movement speed by half for the duration.
    - **Stunned:** Cannot move or attack for the duration.

### Changed

* Debug mode is now a toggle.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.5

### Fixed

* Default value for Combat module has been correctly set to `false`. This would have prevented the initial setup wizard from triggering.
* Updated SpaceCore minimum version requirement.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.1

### Fixed

* Hotfix for GMCM crash.
* Added some missing GMCM config options and fixed some typos.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.0

### Added

* Added initial setup "wizard".
* Added Japanese translations by [sakusakusakuya](https://www.nexusmods.com/stardewvalley/users/155983153).


### Changed

* Improved GMCM menu with multi-column options and separators.
* Stun animation moved to Combat module.
* The entry commands for all modules have changed, now using the ticker-like acronyms, which are displayed in the titles of each module's README, in (parentheses).

### Fixed

* Added updated Russian localization by [romario314](https://www.nexusmods.com/stardewvalley/users/68548986).

### Removed

* Removed Hyperlinq library. *"Overoptimization is the root of all evil."*

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.2

### Added

* Added separators from GMCM options.

### Changed

* Added headers to GMCM menus.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.1

### Fixed

* Improvements to Chinese localization.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.0

### Added

* Added German translations by [FoxDie1986](https://www.nexusmods.com/stardewvalley/users/1369870).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.3

## Changed

* Caught some more indexed enumerables which has been replaced with for loops.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.2

### Fixed

* Fixed integer GMCM fields incorrectly displaying as decimals.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.0

## Added

* Added Hyperlinq library.

## Changed

* Optimized most iterations, removing excessive use of Linq and Enumerators to reduce allocation, and replacing some instances with Hyperlinq.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.1.0

### Changed

* Now using `ReadOnySpan` to split strings.
* Replace leftover reflected SpaceCore code with native.

### Fixed

* Now handles empty arguments in console commands.
* Players now hold their own mod data, rather than concentrating all data on the main player. This fixes some syncronization issues in splitscreen.
* Added parameterless constructors to mod projectiles, which apparently is required by the game for multiplayer syncronization. 

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.4

### Changed

* Default DebugKey changed to RightShift / RightShoulder.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.3

### Added

* Added Russian translations by [pavlo2906](https://www.nexusmods.com/stardewvalley/users/46516077).

### Fixed
* Added dependencies for Custom Ore Nodes and Custom Resource Clumps.
* Fixed update keys for this and optional files.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.2

### Added

* Added Chinese translations by [xuzhi1977](https://www.nexusmods.com/users/136644498).
* Added Korean translations by [BrightEast99](https://www.nexusmods.com/users/158443518).
* Added Spanish and French translations.
* Revalidate console command now also removes Dark Swords from chests.
* Added support for Better Chests to prevent accidentally depositing the Dark Sword.

### Changed

* Updated Portuguese translation.

### Fixed

* Fixed some typos in default (English) localization.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.0

### Changed

* Rebranded as MARGO.

### Fixed

* Fixed a possible memory leak in the shared Event Manager logic.

### Removed

* Removed Generic Mod Config Menu as a hard requirement.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.7

### Changed

* Mod integrations now use the Singleton pattern.

### Added

* Added FirstSecondUpdateTickedEvent.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.5

### Added

* Added Revert command, complementary to the Initialize command from Arsenal. This will undo the changes made by Arsenal to resolve possible issues after disabling the module.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.4

### Added

* Updated portuguese translations by [Onemix](https://www.nexusmods.com/stardewvalley/users/39429640)

### Changed

* Reverted some Virtual Properties back to PerScreen Mod State where more appropriate.

### Fixed

* Fixed SecondsOutOfCombat not reseting on damaging monsters (incorrect parameter name in Harmony Postfix).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.3

### Changed

* Renamed the mod folder to be less vague.

### Added

* Added asset invalidation when toggling a module.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.0 (Initial release)

* Initial Version

[🔼 Back to top](#core-changelog)