# MARGO Change Logs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Patch 2.4.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Added initial compatibility for [Archery](http://www.nexusmods.com/stardewvalley/mods/16767) to PROFS, SLNGS and ENCH (see the specific changelogs for details).
* Added shield VFX to Bloodthirsty enchant [ENCH] and Ring of Yoba's rebalance [RNGS].
* [CMBT]: Fixed the issue of DoT causing monsters to be removed before death. This means that the workaround, which forcefully respawned live monsters and caused issues in multiplayer, is no longer needed.
* [CMBT]: Added Fear status.
* [CMBT]: Retextured the Shadow Sniper arrow to make it look ethereal, to avoid completely breaking my immersion when it travels through walls.
* [ENCH]: Quincy enchantment can now travel through walls, no longer receives velocity from Desperado overcharge (since it receives Magnum effect instead), and no longer spams the annoying debuff spell sound.
* [PROFS]: Brute Frenzy now causes Fear status on activation.
* [PROFS]: There is now a very short delay before Desperado's overcharge begins. It should be barely noticeable while overcharging, but is *very* noticeable when you *don't* want to overcharge, so you don't hear the charging sound as often.
* [PROFS]: Fixed Limit Break gauge still rendering with Limit Breaks disabled.
* [PROFS]: Fixed an issue with Desperado's charging speed buff, which actualy reduced charging speed the player lost HP, instead of the opposite.
* [RNGS]: Removed the "+4 Immunity" text from Immunity Ring tooltip.
* [RNGS]: Power Chord Infinity Bands now normalize resonance correctly. This means that each gem will not get the full resonance effect from both resonant pairs, but rather shares the resonance with its equal. In simple terms, this nerfs Power Chords from 40% - 33% stats, to 30% - 26.7%. Monotone rings (all equal gems) are the only way to maximize a single stat.
* [TXS]: Fixed some translation keys.
* [TWX]: Mushroom Box age now uses Foraging level instead of Farming.
* [WPNZ]: Valor Trial progress now updates incrementally instead of only on completion. You must speak with Gil for the objective to update.
* Debug mode now also shows NPC names and current health in the case of monsters.

## Patch 2.3.1 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [CMBT]: Hotfix for memory leak issue.
* [WPNZ]: Removed uncredited and unlicensed artwork by [herbivoor](https://www.nexusmods.com/stardewvalley/mods/12004).

## Minor Release 2.3.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Migrated to Pathoschild's Translation Class Builder.
* Added all GMCM text to localization model.
* [PROFS]: Added config settings to displace the Limit Gauge bar.
* [CMBT]: Fixed a memory leak while saving.
* [WPNZ]: Now actually remembers when you start the Hero's Journey quest.
* [WPNZ]: Weapon sale price now considers Profit Margin settings as well as applied enchantments.
* [TXS]: Added income and property tax information to the API.

## Patch 2.2.8 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [CMBT]: For some reason, monsters suffering from damage-over-time are some times removed from the location before dying. This caused all sorts of strange behaviors, such as the status animation and Fly buzzing sound to keep playing forever. I cannot figure out why this is happening, so I've implemented a workaround; when a monster is removed from a location before dying, it is immediately re-added.
* [CMBT]: Insects no longer die instantly from Burn status, but suffer 4x damage.
* [PROFS]: Luremaster-owned Magnet-baited Crab Pots no longer produce trash, as stated in its description.
* [SLNGS]: Removed 2 frames from Special Move windup, so it should feel slightly better.
* [SLNGS]: Fixed a bug allowing slingshots to receiving infinite forges.
* [SLNGS]: Fixed FaceMouseCursor GMCM setting incorrectly mapped to the one from WPNZ module.
* [SLINGS + WPNZ]: Improved Slick Moves stop condtion, which fixes a weird bug where Weapon slick moves would interfere with Slingshot slick moves, and vive versa.

## Patch 2.2.7 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [WPNZ]: Fixed Blade of Ruin duplicating itself when stored in a chest.
* [WPNZ]: Fixed crashing when completing the Blade of Ruin's intro quest.
* [WPNZ]: You can now see and change all Stabbing Swords using a single config setting.
* [WPNZ]: Touched up some textures.

## Patch 2.2.6 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Fixed a bug preventing the config menu from reloading correctly.
* [WPNZ]: Added instrinsic enchantment to Neptune Glaive: feels like a crashing wave, dealing heavy knockback. Thanks to [Deadnoz](https://www.nexusmods.com/stardewvalley/users/9502763) for the idea.
* [WPNZ]: Added config setting to allow depositing the Blade of Ruin.
* [WPNZ]: Added compatibility for [Simple Weapons](https://www.nexusmods.com/stardewvalley/mods/16491?tab=posts&BH=0).
* [WPNZ][SLNGS]: When FaceMouseCursor is enabled, pressing the Action button will no longer cause the player to accidentally use a special move in another direction when trying to interact with something. If you've ever tried talking to an NPC, only to accidentally dash in a different direction while holding a Stabbing Sword, then you will certainly appreciate this change.

## Patch 2.2.5 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [WPNZ]: Fixed crashing issue in town and Spcial Order board.

## Patch 2.2.4 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [SLNGS]: Infinity Slingshot can now be created even if WPNZ module is disabled.

## Patch 2.2.3 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [ENCH]: Fixed a bug causing implicit Mythic enchantments from WPNZ to be removed on load.
* [SLNGS]: Fixed a bug preventing Galaxy Slingshot from receing Galaxy Souls.
* [WPNZ]: You can now change the difficulty of the Ruined Blade's damage-over-time.
* [WPNZ]: You can now change the difficulty of the Hero's Journey quest; i.e. the difficulty of completing each virtue trial (default is Medium).
* [WPNZ]: The Trial of Generosity can now also be completed by gifting NPCs. The trial will complete once the total value of gifts given reaches the specified gold amount. The host player can still complete this trial by purchasing the community upgrade (which will no longer count for farmhands).
* [WPNZ]: The Hero's Journey quest now displays as a single quest with multiple objectives (like a Special Order), rather than separate quests for each virtue.
* Because this is such a huge mod, and I'm a borderline narcissist, you can now set a keybind to directly open this mod's GMCM page (default LeftShift+F12, because I use F12 as the GMCM key).

## Patch 2.2.2 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* The new status conditions have been moved to their logical place in the CMBT module. So if that's not enabled, they won't work.
* Frozen damage bonus increased to x3.
* Fixes the issue causing players to receive the Blade of Ruin quest over again.

## Patch 2.2.1 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Hotfix for checksum validation not persisting until the next day.

## Minor Release 2.2.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

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

## Minor Release 2.1.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* [ENCH]: Added new Wabbajack enchantment.
* [WPNZ]: Fixed a long-standing overlooked issue with weapon hitboxes during combos. Combat should feel *significantly* better now.
* [PROFS]: Ultimate / Super Abilities / Special Abilities have been renamed to Limit Breaks. The HUD meter has been adjusted accordingly.
* Bugfixes.

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