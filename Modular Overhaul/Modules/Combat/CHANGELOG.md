# CMBT Changelog

## 2.6.0 <sup><sub><sup>[🔼 Back to top](#profs-change-log)</sup></sub></sup>

Merged WPNZ, SLNGS, RNGS and ENCH into the CMBT module. All of those modules essentially tackled different facets of combat, and WPNZ and SLNGS in particular shared many redundant patches and config settings. In that light, the unification streamlines a lot of the config schema and cuts down on the number of patches required.

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
    * You can now offer a prayer to Yoba once a day to weaken the Blade of Ruin's curse by 20% points, to a minimum of 50 points.
    * Slightly changed the flavor text when obtaining a Galaxy weapon and the Blade of Dawn.
* Blade of Dawn now also deals extra damage to shadow and undead monsters (vanilla Crusader effect, but weaker) and grants the effect of a small lightsource while held.
    * If you were not aware, the Blade of Dawn and Infinity weapons already all possess the ability to inflict perma-death on Mummies, replacing the vanilla Crusader enchantment.
* Blade of Dawn and Infinity weapon beams no longer cast a shadow.
* Lowered Wabbajack probability from 0.5 to about 0.309.

## Removed

* Removed temporary fixes for existing saves after previous changes to the Hero's Quest.


[🔼 Back to top](#cmbt-change-log)