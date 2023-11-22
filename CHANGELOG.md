# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-changelogs).

## Patch 4.1.3 Highlights

* Fixed GMCM list options reseting when hitting Save.
* The module selection menu now translates correctly.
* Fixed multiplayer error caused when a peer fires a custom projectile.
* [CMBT]: You can now hear the chords in your Infinity Bands.
* [PRFS]: Reblanced the impact of Treasure Hunt streak.
* [PRFS]: Fixed issue of farmhands being unable to complete treasure hunts.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

* [PROFS]: Fixed typo in `ProspectorHunt.TryStart` which caused SO exception.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 4.1.3 Highlights

* Fixed GMCM list text boxes being limited to 13 characters.
* [CMBT]: Fixed an edge case that caused an error to be thrown with Ridgeside's Fairy Needle, which, for some reason, does not have a description.
* [CMBT]: Stabbing Swords are no longer persisted as such, to avoid the issue of the game freezing if the mod is removed abruptly.
* [CMBT]: Weapon Overhaul is now a hard requirement for both Stabbing Swords and Melee Combo Framework.
* [CMBT]: Removed the crit. boost given to daggers in vanilla.
* [CMBT]: Added new failsafes for Community Upgrades applying to farmhand Hero Quest.
* [PRFS]: Removed patch to Content Patcher. Please manually edit SVE's files as described in the [detailed changelog](Modules/Professions/CHANGELOG.md).
* [PRFS]: Desperado's quick-shot perk now displays as a buff.
* [PRFS]: Fixed Prospector Hunt weapons spawning out-of-bounds.
* [PROFS]: Treasure Hunts now play quest complete sound on completion.
* [TOLS]: Axe and Pickaxe shockwave no longer cost stamina with Efficient enchantment
* [TWX]: Removed Spawn Crows feture.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 4.1.2 Highlights

* [CMBT]: Fixed issues with weapons stats being set incorrectly.
* [PRFS]: Fixed Null-Reference exception caused by `Monster.FindPlayer`.
* Config options with custom logic now only trigger when the value changes. This avoids unecessarily triggering every single validation logic when saving via GMCM.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Hotfix 4.1.1 Highlights

* Hotfix for all weapons being turned into swords when Combat module not enabled.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Minor Release 4.1.0 Highlights

* The GMCM is now self-generating, which is awesome (for me) but does unfortunately mean that absolutely all translation keys for the menu have changed (sorry translators). The ZH and KO menu translations had to be discarded.
* Added dynamic list options to the GMCM, which means that any list config settings that previously had to be changed manually (like `CustomArtisanMachines`, `StabbingSwords`, `TaxRatePerBracket`, etc.) can now be changed in the menu in-game. With this, all settings are now available in the menu.
* [CMBT]: Made several minor improvements to animations and sound effects when acquiring Galaxy Blade / Blade of Dawn.
* [PRFS]: Fixed Aquarist bug causing it to always consider the `FishPondCeiling` setting instead of the actual number of constructed Fish Ponds.
* [PRFS]: Hopefully fixed an issue with CP skill level conditions not working at levels above 10.
* [PRFS]: Several PPJA dairy products are now also considered "animal-derived" for the Producer profession. The list of `AnimalDerivedProducts` has been added to the configs.
* Added compatibility for [More New Fish](https://www.nexusmods.com/stardewvalley/mods/3578).
    * [CMBT]: The Sword Fish weapon is now a Mythic-tier Stabbing Sword with scaling damage based on caught fish species.
    * [PRfS]: Tui and La can be raised in Fish Ponds with the Aquarist profession. They produce essence instead of roe.
    * [PNDS]: Tui and La can be raised together in the same Fish Pond, unlocking a low chance to produce Galaxy Soul.

<div align="center">-- This is likely the final content release before 1.6 --</div>

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Major Release 4.0.0 Highlights

* **[PRFS]:**
    * Major rework of most Combat professions. I encourage you to go over the detailed changelog for PRFS module.
* **[CMBT]:**
    * Major rework of all Ranged prismatic enchantments. Removed all the boring old enchantments in favor of much better new ones.
    * Reworked some player status effects to be more consistent with the ones applied to monsters. Also tweaked some monster status effects.
    * Added new visuals and sound effects to certain status effects.
    * Several other tweaks and minor improvements. I encourage you to go over the detailed changelog for CMBT module.
* Added, removed and changed several to translation keys.
* Some bug fixes.
* Changed to custom [license](LICENSE).

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Detailed Changelogs

In-depth changelogs for existing modules.

* [Core](Modules/Core/CHANGELOG.md)
* [Professions](Modules/Professions/CHANGELOG.md)
* [Combat](Modules/Combat/CHANGELOG.md)
* [Tools](Modules/Tools/CHANGELOG.md)
* [Ponds](Modules/Ponds/CHANGELOG.md)
* [Taxes](Modules/Taxes/CHANGELOG.md)
* [Tweex](Modules/Tweex/CHANGELOG.md)

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Legacy Changelogs

Changelogs for modules that have been merged and no longer exist.

* [Weapons](Modules/Combat/resources/legacy/CHANGELOG_WPNZ.md)
* [Slingshots](Modules/Combat/resources/legacy/CHANGELOG_SLNGS.md)
* [Enchantments](Modules/Combat/resources/legacy/CHANGELOG_ENCH.md)
* [Rings](Modules/Combat/resources/legacy/CHANGELOG_RNGS.md)

[🔼 Back to top](#margo-changelogs)