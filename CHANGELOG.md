# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-changelogs).

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

<div align="center">-- This is likely the final update before 1.6 --</div>

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