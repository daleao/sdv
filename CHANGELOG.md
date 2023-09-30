# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-changelogs).

## Patch 3.1.5 Highlights

* Changed core ticket from MARGX to MRG.
* Changed ticker for Professions module from PROFS to PRFS.
* [CMBT]: Neptune Glaive now requires the player have obtained the Skull Key before it will appear in Fishing Chests.
* [CMBT]: Wizard's special Blade of Ruin dialogue will now only occur once per day, so it will no longer prevent all other dialogues.
* [PRFS]: Spelunker ladder down chance changed from additive to multiplicative.
* [PRFS]: Spelunker prestige recovery increased from +2.5% to +5% health and from +1% to +2.5% stamina per level.
* [TOLS]: Fixed not being able to apply Mythicite upgrade at the Forge.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.1.4 Highlights

* Added translations for new chat notifications (missing JA, KO and ZH).
* [CMBT]: Receiving your final Galaxy weapon will now also reward a free pair of Space Boots.
* [CMBT]: Stabbing Sword special move will no longer clip through warps.
* [CMBT]: Fixed error thrown when trying to get Galaxy weapon with Iridium Bar config set to zero.
* [CMBT]: Fixed an issue where the player could not drift left or down using Slick Moves feature.
* [CMBT]: Fixed Savage Ring buff slowing down attack speed instead of boosting it up.
* [TXS]: Fixed debt not being collected when reloading.
* [TXS]: Default annual interest increased to 72% (was previously 12%).

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.1.3 Highlights

* Rolled back dependency updates due to conflicts with AtraCore.
* Added missing translations. Improved some Chinese translations, thanks to xuzhi1977.
* [CMBT]: Added chat notifications for when a virtue point is gained.
* [PRFS]: Changed the way Scavenger and Prospector treasures scale with the current streak. Players should now see significantly more treasure if they manage to keep their streaks high.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.1.2 Highlights

* Config menu now auto-detects gamepad mode and adapts accordingly.
* Added Korean GMCM translations by [Jun9273](https://github.com/Jun9273).
* Added Chinese GMCM translations by [Jumping-notes](https://github.com/Jumping-notes).
* [TWX]: Fixed Glowstone progression integration with Better Crafting.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Hotfix 3.1.1 Highlights

* Forgot to scale Garnet Node spawn back down to normal after debugging for 3.1.0.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.1.0 Highlights

* Optional files `[CP] Fish Pond Data` and `[CON] Garnet Node` are now embedded in the main mod.
* Updated mod dependencies.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.0.2 Highlights

* Fixed a few more translation issues.
* Added Combo Framework to the API.
* [PRFS]: Parallelized Luremaster logic to maybe improve performance on some systems.
* [CMBT]: Valor completion was reverted back to Monster Eradication Goals. But it can now also be completed by digging through the Mines without Staircases, or by reaching SVE's Treasure Cave.
* [CMBT]: Added new Virtue tags to several SVE events (most of them "Mature Events").
* [CMBT]: Honor can now be proven by returning the Mayor's purple shorts without shenanigans.
* [CMBT]: Changed SVE's Treasure Cave chest reward from Obsidian Edge to Lava Katana.

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Patch 3.0.1 Highlights

* Fixed some missing translations.
* [CMBT]: Fixed Piper's Slime ammo damage (increased from 5 to 10).
* [TOLS]: Fixed Master enchantment on tools other than Fishing Rod still increasing Fishing level by 1, and also not showing up as green in the skills page.

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