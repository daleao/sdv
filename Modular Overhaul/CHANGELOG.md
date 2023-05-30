# MARGO Change Logs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-change-logs).

## Minor Release 2.4.0 Highlights <sup><sub><sup>[🔼](#margo-change-logs)</sup></sub></sup>

* Added initial compatibility for [Archery](http://www.nexusmods.com/stardewvalley/mods/16767) to [PROFS](Modules/Professions/CHANGELOG.md), [SLNGS](Modules/Slingshots/CHANGELOG.md) and ENCH (see the specific changelogs for details).
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