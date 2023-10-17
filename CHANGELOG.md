# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-changelogs).

## Minor Release 3.2.0 Highlights

* **[PRFS]: Major overhaul of most Combat professions!! Please see the [PRFS changelog](./Modules/Professions/CHANGELOG.md) for details.**
* **[CMBT]: New player status effects changes!!** Vanilla Burn, Freeze, Jinxed and Weakness debuffs have been reworked. Please see the [CMBT changelog](./Modules/Professions/CHANGELOG.md) for details.
* [CMBT]: Burn and Chill status effects now negate each other.
* [CMBT]: Added visual and audio queues to Freeze status. Improved status effect animations on Royal Serpent.
* [CMBT]: Blade of Dawn and Infinity weapons now also emit light.
* [CMBT]: Added new tooltips icons to rings and weapons to make them more easy to identify.
* [CMBT]: Added config options to set each weapon tier color. All Legendary-tier weapons now use the same title color, which by default is also the same as Masterwork tier.
* [CMBT]: Knockback damage no longer applies to gliders (flying enemies).
* [CMBT]: Wizard's summon letter for Blade of Ruin quest now uses Wizard's custom letter background.
* [CMBT]: Slightly increased the magnetism of tertian Infinity Band.
* [CMBT]: Removed the Magnum and Preserving enchantments for slingshots.
* [TWX]: Renamed `DairyYieldsQuantityOverQuality` setting to `ImmersiveDairyYield`, and made it so that Gold Eggs always yield iridium quality, instead of gold.
* Several translations keys changed or removed (translators please be noted).

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Major Release 3.0.0 Highlights

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