# Modular Overhaul Change Logs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Minor Release 2.2.0 Highlights

* Added status conditions that will be used by various modules. Each status condition has a neat correponding animation.
    - **Bleeding:** Causes damage every second. Damage increases exponentially with each additionl stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
    - **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magama Sparkers).
    - **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
    - **Freeze:** Cannot move or attack for 30s. The next hit during the duration deals double damage and ends the effect.
    - **Poisoned:** Causes damage equal to 1/16 of max health every 3s, stacking up to 3x.
    - **Slowed:** Reduces movement speed by half for the duration.
    - **Stunned:** Cannot move or attack for the duration.
* [WPNZ]: All daggers can now inflict bleed if Rebalance option is enabled.
* [WPNZ]: Some weapons have suffered a tier demotion. All mythic weapons now carry a special effect:
    - The **Obsidian Edge**, in addition to ignoring enemy defense, now also applies Bleeding.
    - The **Lava Katana** can now cause Burning, and instantly incinerates Bugs and Flies.
    - The **Yeti Tooth** can now cause Chilled.
* [WPNZ]: In exchange, mythic weapons can no longer receive Prismatic Shard enchantments.
* [WPNZ]: Stabby sword thrust move will now auto-seek the hovered enemy if FaceMouseCursor setting is enabled.
* [ENCH]: Added the Steadfast enchantment: "Converts critical strike chance into bonus damage (multiplied by critical power)."
* [ENCH]: Artful can now be applied to Slingshots. Changes to Artful enchantment:
    - **Stabbing Sword**: Can dash twice in succession.s
    - **Defense Sword**: A successful parry ~~stuns enemies for 1s~~ guarantees a critical strike on the next attack (within 5s).
    - **Dagger**: Applies bleeding on every hit (if WPNZ is enabled with Rebalance).
    - **Club**: Now also stuns enemies for 2s.
    - **Slingshot**: Stunning smack becomes a stunning swipe (larger area, easier to hit).
* [ENCH]: Auto-fire mode with Gatling enchantment is now activated by double-pressing and holding the tool button. The auto-fire speed has also been increased to match the [Desperado Limit Break](Modules/Professions/README.md#limit-breaks).
* [ENCH]: Preserving enchantment now grants 100% chance to preserve (was 50%).
* [ENCH]: Spreading enchantment now consumes an additional ammo, but spread projectiles now deal 100% damage.
* [RNGS]: Thorns Ring can now cause Bleed (with Rebalance option). Renamed to "Ring of Thorns", because it just sounds better.
* [RNGS]: Ring of Yoba no longer grants invincibility; now grants a shield for 50% max health when your health drops below 30% (with Rebalance).
* [RNGS]: Immunity Ring now grants 100% immunity, instead of vanilla 40% (with Rebalance).
* [RNGS]: Warrior Ring now gains stacks on every kill (instead of 3 kills), but is capped at +20 attack (with Rebalance).

## Minor Release 2.1.0 Highlights

* [ENCH]: Added new Wabbajack enchantment.
* [WPNZ]: Fixed a long-standing overlooked issue with weapon hitboxes during combos. Combat should feel *significantly* better now.
* [PROFS]: Ultimate / Super Abilities / Special Abilities have been renamed to Limit Breaks. The HUD meter has been adjusted accordingly.
* Bugfixes.

## Major Release 2.0.0 Highlights

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
