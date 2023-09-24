# CMBT Changelog

## 3.1.4

### Added

* Receiving your final Galaxy weapon will now also reward a free pair of Space Boots. They can still be purchased at the Adventure Guild, as always.

### Fixed

* Added translations for new chat notifications (missing JA, KO and ZH).
* Stabbing Sword special move will no longer clip through warps.
* Fixed error thrown when trying to get Galaxy weapon with Iridium Bar config set to zero.
* Fixed an issue where the player could not drift left or down using Slick Moves feature.
* Fixed Savage Ring buff slowing down attack speed instead of boosting it up.

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.1.3

### Added

* Added chat notifications for when a virtue point is gained. This should help to make it less cryptic.

### Changed

* Changed data key for each virtue trial to simplify logic. Added temporary transition code to SaveLoaded.

### Fixed

* Mayor shorts quest now correctly gives Honor points instead of Valor.

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.1.1

### Fixed

* Forgot to scale Garnet Node spawn back down to normal after debugging for 3.1.0.

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.1.0

### Changed

* [CON] Garnet Node is now merged into the main mod, so no longer requires a separate download.

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.0.2

Exodus from Nexus.

### Added

* You can now prove your Honor (+2 points) by returning the Mayor's Purple Shorts, as long as you don't troll him at the Luau or the Grange.
* You can now prove your Valor by digging 100 (or 50, on Easy difficulty) consecutive floors in the Mines without using Staircases.
* Added Virtue completion tags to several SVE events. Most of these are "mature events", so you will need that option enabled to benefit from this.
* You can now prove your Valor by reaching the Treasure Cave in the Crimson Badlands.

### Changed 

* Base Valor completion objective was changed back to Monster Eradication Goals. Someone pointed out that slaying monsters for the sake of slaying monsters is not exactly "courageous".
* Changed SVE's Treasure Cave chest reward from Obsidian Edge to Lava Katana.

### Fixed

* Wabbajack end-of-day cleanup should now run correctly.

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.0.1

### Fixed

* Fixed Piper's Slime ammo damage (increased from 5 to 10).

<sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup>

## 3.0.0 <sup><sup><sup>[🔼 Back to top](#cmbt-changelog)</sup></sup></sup>

Merged WPNZ, SLNGS, RNGS and ENCH into the CMBT module. All of those modules essentially tackled different facets of combat, and WPNZ and SLNGS in particular shared many redundant patches and config settings. In that light, the unification streamlines a lot of the config schema and cuts down on the number of patches required.

Find the legacy pre-merge changelogs here:
* [CMBT](./resources/legacy/CHANGELOG_CMBT.md)
* [WPNZ](./resources/legacy/CHANGELOG_WPNZ.md)
* [SLNGS](./resources/legacy/CHANGELOG_SLNGS.md)
* [ENCH](./resources/legacy/CHANGELOG_ENCH.md)
* [RNGS](./resources/legacy/CHANGELOG_RNGS.md)

## Added

* Added enemy difficulty summands to config options and changed default config values for some multipliers.

## Changed

* Prismatic Shard ammo is no longer affected by Preserving Enchantment. That combination was broken AF.
* Improvements to the Blade of Ruin questline:
    * The player will now be prompted to pick up the Blade of Ruin immediately after taking the Gold Scythe, without need to interact with the Reaper Statue a second time.
    * After selecting to read any of the virtue inscriptions at the altar of Yoba, the player will now immediately be prompted to read a different inscription, until all 5 have been chosen. This should make it slightly more intuitive that all 5 dialogues must be seen to advance the quest.
    * The trials now display an objective text that should make it a bit more clear how to complete them (instead of simply "Prove your XYZ").
    * The town community upgrade now also counts towards the player's generosity.
    * The player's valor no longer depends on Monster Eradication quests. It's now a simple monster kill counter.
    * Tweaked the completion criteria for Generosity and Valor.
    * You can now offer a prayer to Yoba once a day to weaken the Blade of Ruin's curse by 20%, to a minimum of 50 points.
    * Slightly changed the flavor text when obtaining a Galaxy weapon and the Blade of Dawn.
* Blade of Dawn now also deals extra damage to shadow and undead monsters (vanilla Crusader effect, but weaker) and grants the effect of a small lightsource while held.
    * If you were not aware, the Blade of Dawn and Infinity weapons already all possess the ability to inflict perma-death on Mummies, replacing the vanilla Crusader enchantment.
* Blade of Dawn and Infinity weapon beams no longer cast a shadow.
* Lowered Wabbajack probability from 0.5 to about 0.309.

## Removed

* Removed temporary fixes for existing saves after previous changes to the Hero's Quest.


[🔼 Back to top](#cmbt-changelog)