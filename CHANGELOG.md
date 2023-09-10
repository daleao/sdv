# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Patch 3.0.2 Highlights <sup><sub><sup>[🔼 Back to top](#margo-change-logs)</sup></sub></sup>

* Fixed a few more translation issues.
* [PROFS]: Parallelized Luremaster logic to maybe improve performance on some systems.

## Patch 3.0.1 Highlights <sup><sub><sup>[🔼 Back to top](#margo-change-logs)</sup></sub></sup>

* Fixed some missing translations.
* [CMBT]: Lowered ammo damage:
    * Gemstones: 50 -> 45
    * Diamond: 120 -> 90
    * Prismatic Shard: 250 -> 120
* [CMBT]: Fixed Piper's Slime ammo damage (increased from 5 to 10).
* [TOLS]: Fixed Master enchantment on tools other than Fishing Rod still increasing Fishing level by 1, and also not showing up as green in the skills page.

## Major Release 3.0.0 Highlights <sup><sub><sup>[🔼 Back to top](#margo-change-logs)</sup></sub></sup>

* Re-unification of all the combat-oriented modules: CMBT, WPNZ, SLNGS, RNGS and ENCH are now collectively known as CMBT.
    * Several redundant config settings (like those related to Auto-Selection) were consolidated.
    * Only the Glowstone Ring features from RNGS were moved to TWX instead.
* Changed several translation keys for better formatting with Pathoschild's Translation Generator. This may lead to missing translation issues, so please report if you see any.
* [CMBT]: Improvements to the Blade of Ruin questline. See the [CMBT changelog](Modules/Combat/CHANGELOG.md#3_0_0).
* [CMBT]: Blade of Dawn now also deals extra damage to shadow and undead monsters and grants a small light while held.
* [CMBT]: Blade of Dawn and Infinity weapon beams no longer cast a shadow.
* [CMBT]: Prismatic Shard ammo is no longer affected by Preserving Enchantment. That combination was broken AF.
* [CMBT]: Lowered Wabbajack probability from 0.5 to about 0.309.
* [CMBT]: Added enemy difficulty summands to config options and changed default config values for some multipliers.
* [TWX]: Re-organized config settings by skill.

[🔼 Back to top](#cmbt-change-log)

## Detailed Changelogs

* [Core](Modules/Core/CHANGELOG.md)
* [Professions](Modules/Professions/CHANGELOG.md)
* [Combat](Modules/Combat/CHANGELOG.md)
* [Tools](Modules/Tools/CHANGELOG.md)
* [Ponds](Modules/Ponds/CHANGELOG.md)
* [Taxes](Modules/Taxes/CHANGELOG.md)
* [Tweaks](Modules/Tweex/CHANGELOG.md)

[🔼 Back to top](#margo-change-logs)