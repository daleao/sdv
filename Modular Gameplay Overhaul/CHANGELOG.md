# MARGO Change Logs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Patch 2.2.5 Highlight

* [WPNZ]: Fixed crashing issue in town and Spcial Order board.

## Patch 2.2.4 Highlight

* [SLNGS]: Infinity Slingshot can now be created even if WPNZ module is disabled.

## Patch 2.2.3 Highlights

* [ENCH]: Fixed a bug causing implicit Mythic enchantments from WPNZ to be removed on load.
* [SLNGS]: Fixed a bug preventing Galaxy Slingshot from receing Galaxy Souls.
* [WPNZ]: You can now change the difficulty of the Ruined Blade's damage-over-time.
* [WPNZ]: You can now change the difficulty of the Hero's Journey quest; i.e. the difficulty of completing each virtue trial.
* [WPNZ]: The Trial of Generosity can now also be completed by gifting NPCs. The trial will complete once the total value of gifts given reaches the specified gold amount. The host player can still complete this trial by purchasing the community upgrade (which will no longer count for farmhands).
* [WPNZ]: The Hero's Journey quest now displays as a single quest with multiple objectives (like a Special Order), rather than separate quests for each virtue.
* Because this is such a huge mod, and I'm a borderline narcissist, you can now set a keybind to directly open this mod's GMCM page (default LeftShift+F12, because I use F12 as the GMCM key).

## Patch 2.2.2 Highlights

* The new status conditions have been moved to their logical place in the CMBT module. So if that's not enabled, they won't work.
* Frozen damage bonus increased to x3.
* Fixes the issue causing players to receive the Blade of Ruin quest over again.

## Patch 2.2.1 Highlights

* Hotfix for checksum validation not persisting until the next day.

## Minor Release 2.2.0 Highlights

* [CMBT]: Added status conditions that can be used by various modules. Each status condition has a neat correponding animation.
    - **Bleeding:** Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
    - **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magama Sparkers).
    - **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
    - **Freeze:** Cannot move or attack for 30s. The next hit during the duration deals double damage and ends the effect.
    - **Poisoned:** Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
    - **Slowed:** Reduces movement speed by half for the duration.
    - **Stunned:** Cannot move or attack for the duration.
* [WPNZ]: All daggers can now inflict bleed if Rebalance option is enabled.
* [WPNZ]: Some weapons have suffered a tier demotion. All mythic weapons now carry a special effect (Neptune Glaive is the only exception, because I still can't come up with an interesting effects for it):
    - The **Obsidian Edge**, in addition to ignoring enemy defense, now also applies Bleeding.
    - The **Lava Katana** can now cause Burning, and instantly incinerates Bugs and Flies.
    - The **Yeti Tooth** can now cause Chilled.
* [WPNZ]: In exchange, mythic weapons can no longer receive Prismatic Shard enchantments.
* [WPNZ]: Stabby sword thrust move will now automatically home-in on the hovered enemy if FaceMouseCursor setting is enabled.
* [ENCH]: Added the Steadfast enchantment: "Converts critical strike chance into bonus damage (multiplied by critical power)."
* [ENCH]: Artful can now be applied to Slingshots. Changes to Artful enchantment:
    - **Stabbing Sword**: Can dash twice in succession.
    - **Defense Sword**: A successful parry ~~stuns enemies for 1s~~ guarantees a critical strike on the next attack (within 5s).
    - **Dagger**: Applies bleeding on every hit (if WPNZ is enabled with Rebalance).
    - **Club**: Now also stuns enemies for 2s.
    - **Slingshot**: Stunning smack becomes a stunning swipe (larger area, easier to hit).
* [ENCH]: Auto-fire mode with Gatling enchantment is now activated by double-pressing and holding the tool button. The auto-fire speed has also been increased to match the [Desperado Limit Break](Modules/Professions/README.md#limit-breaks).
* [ENCH]: Preserving enchantment now grants 100% chance to preserve (was 50%).
* [ENCH]: Spreading enchantment now consumes an additional ammo, but spread projectiles also deal 100% damage (up from 60%).
* [RNGS]: Thorns Ring can now cause Bleed (with Rebalance option). Renamed to "Ring of Thorns", because it just sounds better.
* [RNGS]: Ring of Yoba no longer grants invincibility; now grants a shield for 50% max health when your health drops below 30% (with Rebalance).
* [RNGS]: Immunity Ring now grants 100% immunity, instead of vanilla 40% (with Rebalance).
* [RNGS]: Warrior Ring now gains stacks on every kill (instead of 3 kills), but is capped at +20 attack (with Rebalance).
* Bug fixes.

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

[🔼 Back to top](#margo-change-logs)