# MARGO Change Logs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Patch 2.5.7 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Hotfix for Binning and Socializing skills not working correctly.

## Patch 2.5.6 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Added some new screenshots.
* [CMBT]: Added a "fix" for Lava Lurk's hitbox. In vanilla, the hitbox is always completely below the Lurk's head (the part of the body that would be submerged). This change makes it so that the hitbox moves depending on the Lurk's state; when it emerges, the hitbox changes to match the head that's above the surface, making it much more intuitive to fight against (especially with slingshots).
* [PROFS]: Aquarist reverted to pre-version 2.5.0 as some users pointed out that the change made it impossible to get pirate treasure.
* [RNGS]: Small compatibility patch for Identifiable Combined Rings.
* [SLNGS]: Small fix for Basic Slingshot tooltip.
* [SLNGS]: EnableSpecialMove config now actually does something.

## Patch 2.5.5 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [CMBT]: Fixed an issue with frozen enemies being invincible.
* [ENCH]: Added a fix for transfigured enemies causing save issues in custom maps, like Ridgeside Forest.
* [PNDS]: Fixed an issue with legendary fish not retaining quality. Apparently.
* [PROFS]: Can now choose to disable level 20 progression.
* [PROFS]: Fixed off-screen tracking of panning points by Prospector.
* [PROFS]: Fixed integration with Mushroom Propagator.
* [SLNGS]: Explosive ammo now slays Mummies.
* [TXS]: You can now choose what percentage of each business expense is tax-deductible.
    YOU MUST EITHER DELETE YOUR CONFIGS, OR MANUALLY CHANGE THE FOLLOWING VALUES FROM "true/false" to "1.0/0.0":
    * DeductibleAnimalExpenses
    * DeductibleBuildingExpenses
    * DeductibleSeedExpenses
    * DeductibleToolExpenses
* [TXS]: Now actually uses ConservationistTaxBonusCeiling setting from PROFS, instead of setting it to 100%.
* [TOLS]: Added some missing GMCM options relating to Watering Can.
* [TWX]: Fixed Tea Bush Age incorrectly using Tree Aging Factor.
* [WPNZ]: Removed the Dwarven Forge Part 2 quest.
* Some minor niche bugfixes.

## Patch 2.5.4 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Fixed a compatibility issue with Flexible Sprinklers.

## Patch 2.5.3 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Fixed issue with Hoe and Watering Can settings validations.
* Fixed Null-Reference exception in Slingshot tooltip.

## Patch 2.5.2 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Hotfix for crops withering even when watered.

## Patch 2.5.1 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* First hotfix for few issues in 2.5.0.

## Minor Release 2.5.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [PNDS]: Mr. and Ms. Angler can now mate when placed together in a pond.
* [PROFS]: Major overhaul of Fishing professions. Please see the dedicated [changelog](Modules/Professions/CHANGELOG.md).
* [PROFS]: Prospector Hunt streak now also buffs the chance of spawning panning points.
* [PROFS]: Slingshot projectiles now leave a trail effect when overcharged.
* [PROFS]: SpaceCore skill prestige choices can now also be respecced at the Statue of Prestige (untested).
* [SLNGS]: Removed gemstone enchantments and replaced with gemstones as ammo. Major new Archery compatibility changes, including a new Misc. download for compatibility with [Archery Starter Pack](https://www.nexusmods.com/stardewvalley/mods/16768).
* [SLNGS]: Ranged weapons now display the equipped ammo (can be disabled).
* [TXS]: Now persists data in the Mod Data Dictionary, which fixes a possible issue if the player reset the game after saving on the night of the 1st, but before completing the 2nd.
* [TOLS]: Added Radioactive tool upgrades and Volcano Forge Upgrading. Please see the dedicated [changelog](Modules/Tools/CHANGELOG.md).
* [TOLS]: Added option to reward experience for using the Watering Can, and another to prevent refilling the Watering Can with salt water.**
* [TWX]: Added option to wither un-watered crops, and another to prevent bait consumption when a Crab Pot produces trash.**

** Yes, I know these already exist in other mods. But I prefer to avoid downloading small immersion tweaks when they can be added directly to the modular overhaul.

## Major Release 2.0.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Weapon, Slingshot and Enchantment-related functionalities have been refactored to specific modules.
    * Added Weapons module.
    * Added Slingshots module.
    * Added Enchantments module.
    * Added Combat module.
    * Removed Arsenal module.
* Now forcefully opens a setup "wizard" on first start-up.
* [WPNZ]: Certain weapons have been rebalanced, including Galaxy and Infinity weapons.
* [WPNZ]: Several tweaks to make new weapons significantly easier to come by.
* [WPNZ]: No longer requires manual execution of `revalidate` command.
* [ENCH]: Added new Explosive / Blasting enchantment.
* [ENCH]: Several rebalancing changes to Energized and Bloodthirsty enchantments.
* [ENCH]: Artful for Stabbing Swords now allows turning mid-dash.
* [ENCH]: Now replaces the generic "Forged" text with actual gemstone sockets in weapon and slingshot tooltips.
* [RNGS]: Adjustments to Chord Harmonization logic, leading to a rebalance of certain gemstone combinations. The tetrads, in particular, have become significantly stronger.
* [TXS]: Added property taxes.
* [TOLS]: Now includes the popular Harvest With Scythe feature, so you no longer have to rely on CJB Cheats Menu's implementation. The implementation is similar to Yet Another Harvest With Scythe Mod, which means they will conflict if installed together.
* [TWX]: Added new tweak for spawning crows in Ginger Island farm and other custom maps.
* Bug fixes.

## Detailed Change Logs

* [Core](Modules/Core/CHANGELOG.md)
* [Professions](Modules/Professions/CHANGELOG.md)
* [Combat](Modules/Combat/CHANGELOG.md)
* [Weapons](Modules/Weapons/CHANGELOG.md)
* [Slingshots](Modules/Slingshots/CHANGELOG.md)
* [Tools](Modules/Tools/CHANGELOG.md)
* [Enchantments](Modules/Enchantments/CHANGELOG.md)
* [Rings](Modules/Rings/CHANGELOG.md)
* [Ponds](Modules/Ponds/CHANGELOG.md)
* [Taxes](Modules/Taxes/CHANGELOG.md)
* [Tweaks](Modules/Tweex/CHANGELOG.md)

[🔼 Back to top](#margo-change-logs)