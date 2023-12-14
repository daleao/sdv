# PRFS Changelog

## 4.1.8

### Added

* Added new Prestige mode options:
    * **Standard** - Prestige levels (11-20) are unlocked individually per skill by resetting the skill 3 times to acquire all 10th-level professions within that skill. *This is the standard paradigm that has been available until this point.**
    * **Challenge** - Prestige levels (11-20) are unlocked simultaneously for all skills only after all skills have been reset 3 times each and all 10th-level professions have been acquired across all skills. *This was my original plan, before receving mostly negative feedback.*
    * **Streamlined** - Prestige levels (11-20) are unlocked immediately upon reaching level 10. The Skill Reset mechanic is eliminated altogether. But this also means you can obtain only one profession per skill. *This is meant for casual players who don't appreciate the grind, or anyone who wants the extended progression but without becoming overpowered.*
    * **All Professions** - This is the opposite of Streamlined; Prestige levels are eliminated altogether, but the Skill Reset mechanic is maintained. The level cap remains at the regular 10, but the Statue of Prestige will still allow you to obtain all the non-prestige professions.

* Added the following translation keys:
    * "gmcm.prestige_progression_mode.title"
    * "gmcm.prestige_progression_mode.desc"
    * "gmcm.prestige_mode.none"
    * "gmcm.prestige_mode.standard"
    * "gmcm.prestige_mode.challenge"
    * "gmcm.prestige_mode.streamlined"
    * "gmcm.prestige_mode.all_professions"
    * "prestige.level_up.unlocked.challenge"

### Changed

* The Oceanographer (Prestiged Conservationist) perk has been changed to fit the profession's title.
    * ~~Tax deductions build 50% faster~~ Crab pots without bait have a chance to trap fish. Special conditions apply during rainy or full-moon nights.

    *Your deep knowledge of current and tidal patterns allow your crab pots to catch entire schools of fish during days when the weather is rainy or stormy, or during the day of the full-moon (15th). All fish caught during these days receive a dramatic boost to quantity and a +1 boost to quality.*

    <div align="center">

    \*~~ With this, I think I am finally happy with all 60 profession perks. ~~\*
    </div>
* The Prestige achievement "Master of the Five Trades" is now awarded when all 5 vanilla skills have reached level 20 (previously was level 10).
* Changed the following translation keys (asterisk indicates a wildcard):
    * ~~"gmcm.progression_style.\*"~~ -> "gmcm.ribbon_style.\*"
    * ~~ "prestige.level_up.unlocked"~~ -> "prestige.level_up.unlocked.standard"

### Fixed

* Fixed SpaceCore skill levels being offset by -1.

### Removed

* Removed the following translation keys:
    * "gmcm.enable_prestige.title"
    * "gmcm.enable_prestige.desc"
    * "gmcm.enable_extended_progression.title"
    * "gmcm.enable_extended_progression.desc"

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.7

### Added

* Added new titles for Prestiged profession variants.
  (**English only!! Translations will NOT show in-game, so please don't even try.**)

### Changed

* Female counterpart to Slimed Piper has been renamed to Slimed Siren.
* Experience curve is now an Array instead of a Dictionary.

### Fixed

* `ForgetRecipesOnSkillReset` setting should now apply to SpaceCore skills when ebaled.
* SpaceCore skills should now be properly capped at level 10 when not fully reset.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.6

### Added

* Added extended prestige levels as part of the requirement for perfection. This replaces the vanilla "Farmer Level" requirement, replacing it with "Skills Mastered". If Extended Level Progression enabled, mastery means having reached level 20. Otherwise, mastery means having obtained all level 10 professions. This is optional, enabled by default, and obviously requires Prestige enabled.
    * Added the following translation keys:
        * `ui.pt.farmer_level`
        * `gmcm.extended_perfection_requirement.title`
        * `gmcm.extended_perfection_requirement.desc`

### Fixed

* Fixed issue when multiple stones are destroyed at once during a Prospector Hunt, introduced in 4.1.5.
* Fixed issue with using Magic Bait in Crab Pots.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.5

### Added

* Added localization for Treasure Hunt start/end broadcast messages (only releveant in multiplayer).
* Added the following translation keys:
    * `gmcm.prospector_detection_distance.title`
    * `gmcm.prospector_detection_distance.desc`
    * `gmcm.scavenger_detection_distance.title`
    * `gmcm.scavenger_detection_distance.desc`
    * `gmcm.use_legacy_prospector_hunt.title`
    * `gmcm.use_legacy_prospector_hunt.desc`

### Changed

* Changed the mechanics of Prospector Hunts. They are now unique instead of just a rehash of Scavenger Hunts.
    * Previously all you had to do was run around until you spotted the purple arrow. This has been removed by default.
    * Now, the hunt is based on auditory cues resembling a game of hot and cold. The player will have to smash rocks until they hear a 'ding' sound. The pitch of the ding indicates proximity to the treasure; the higher the pitch, the closer it is. This actually forces the player to mine in order to find the reward.
    * An alert will also be logged to the console for accessibility purposes. The old purple arrow mechanic also remains an option.
* Tuned down treasure hunt streak impact, which was applying twice and made it way too strong.

### Fixed

* Fixed multiplayer synchronization error caused by object projectiles fired by peers.
* Fixed issue of farmhands being unable to complete treasure hunts.

### Removed

* Removed the following translation keys:
    * `gmcm.treasure_detection_distance.title`
    * `gmcm.treasure_detection_distance.desc`

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.4

### Fixed

* Fixed typo in `ProspectorHunt.TryStart` which caused SO exception.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.3

### Added

* Added a buff to visually indicate Desperado's new quick-shot perk.
* Treasure Hunts now play quest complete sound on completion.

### Changed

* Improved and cleaned up Treasure Hunt start/end logic. Added MemberNotNull attributes. Changes will need to be reflected in the API.

### Fixed

* Fixed typo in i18n.
* Fixed an issue where Femur and Ossified Blade rewards from Prospector Hunt would spawn out-of-bounds.
* Not sure if prestiged Treasure Hunt time freeze was working in Multiplayer before this point, but it now should.
* Automated Artisan PFM machines now apply Artisan profession perks even if the output is not marked as an Artisan Good.
* PFM machines no longer apply Artisan perks twice when used manually.

### Removed

* Removed patches to Content Patcher.
    * If using SVE, certain NPC heart events will not trigger if your skill levels is greater than 10. To fix this, open up the following files, search for `SkillLevel`, and edit the field value to include the extended levels:
        * In `Stardew Valley Expanded\[CP] Stardew Valley Expanded\code\NPCs\Morgan.json`, there is 1 instance of `SkillLevel:Mining`. Edit so it looks like this:
        
        ```json
        "Action": "EditData",
        "Target": "data/events/wizardhouse",
        "When": {
            "SkillLevel:Mining": "7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20"
        },
        ```

        * In `Stardew Valley Expanded\[CP] Stardew Valley Expanded\code\NPCs\Willy.json`, there are 2 instances of `SkillLevel:Fishing`. Edit so they look like this:

        ```json
        "Action": "EditData",
        "Target": "data/events/farm",
        "When": {
            "SkillLevel:Fishing": "5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20"
        },
        ```

        ```json
        "Action": "EditData",
        "Target": "data/events/forest",
        "When": {
            "SkillLevel:Fishing": "8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20"
        },
        ```

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.2

### Fixed

* Fixed Null-Reference exception caused by `Monster.FindPlayer` patch.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.1.0

### Added

* Added compatibility for [More New Fish](https://www.nexusmods.com/stardewvalley/mods/3578).
    * Tui and La are now considered Legendary Fish for the Angler perk, and can be raised in Fish Ponds with Aquarist profession.
* Added `AnimalDerivedProducts` to config settings, so you can now add animal-derived goods from other mods to make them compatible with Producer perks (anything with Mayonnaise, Cheese, Butter, Yogurt, or Ice Cream in the name should work out-of-the-box).

### Changed

* Changed `CustomArtisanMachines` setting to simply `ArtisanMachines`. It now includes the vanilla machines in the list.

### Fixed

* Fixed Aquarist bug causing it to always consider the `FishPondCeiling` setting instead of the actual number of constructed Fish Ponds.
* Fixed an issue where extended levels (11-20) could cause CP skill level-based conditions not to trigger, depending on how they were written by the mod author. The "fix" has been to cap CP's skill level provider to level 10, so that CP thinks the player's level is 10 even if it is higher than that. This does have the downside of making it impossible for mod's to add specific conditions for extended levels, but I don't think anybody will ever do that anyway.
    * This also fixes the issue of certain SVE events not triggering (such as Morgan's 8 hearts).

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.0.1

### Changed

* Added new sound effect for the Desperado Limit Break to distinguish it from the slingshot special move.

### Fixed

* Improved performance of `MonsterFindPlayerPatcher` since Monster Musk implementation, which caused severe slowdowns in maps with ludicrous numbers of monsters (i.e., SVE, Ridgeside).

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 4.0.0

### Changed

* Major overhaul for most Combat professions:
    * **Brute:**
        * Bonus attack speed is now part of the base perk.
        * Prestige effect changed to double the duration of Rage stacks, making them easier to stack up to new heights.
    * **Poacher (Bushwhacker):**
        * Poaching no longer requires a critical strike. Items can now be poached on any hit. The odds are still dependent on critical strike chance, but now also factors in enemy resistance, jitteriness (jittery enemies are more unpredictable and thus harder to steal from) and your luck (**not** Daily Luck!).
    * **Rascal:**
        * Chance to recovery ammo when prestiged reduced from 70% to 55%. This chance is no longer reduced for pierced ammo.
        * New prestige effect: **Can equip Monster Musk to produce alluring ammo.**
            * Equip the Monster Musk like any other ammo. It will always slot to the bottom ammo slot. Only 1 Monster Musk will be slotted at a time, and it cannot be removed once slotted.
            * As long as a Monster Musk is equipped, projectiles will become "musked". A slotted Monster Musk will last for 10 shots.
            * If the musked projectile collides with anything, such as a wall or another monster, that thing will become musked for 15 seconds. During that time, other nearby enemies will flock towards the musked spot, allowing the player to completely evade combat.
            * Musked enemies cannot be damaged by other enemies.
            * Does not work with Explosive Ammo.
        * Removed the "trick shot" mechanic, which likely nobody even knew about.
    * **Desperado:**
        * Base perk changed: ~~Fire quicker at low HP~~ -> **Consecutive shots against different enemies within 1 second deal 50% more damage.**
            * After successfully hitting an enemy, you have 1 second to hit a different enemy in order to qualify for the bonus damage. This emphasizes quick-shooting and switching targets, as expected of a Desperado. *Firing speed bonuses are now very valuable to this profession. The Gatling enchantment from Combat module will also offer great synergy.*
        * Changes to Piercing mechanic from prestige Desperado:
            * Piercing chance now depends on enemy resistance (resistant enemies are harder to pierce). In exchange, a successful pierce ignores the enemy's resistance.
            * Ammo power and chance for susbsequent pierce now decreases by a factor based on the pierced enemie's resistance, intead of a flat 35%, which is much more immersive.
            * No longer capped at 2 pierces; shots can now pierce infinitely many times (although more than 2 is extremely unlikely).
    * **Slimed Piper:**
        * Health and energy recovery effects are now part of the base perk instead of prestige, since these are much more valuable early-game.
        * Non-prestige buffs are now limited to +Attack and +Defense and cap at 5 stacks each.
        * Prestige perk now adds all remaining buffs (+Luck, +Speed, and +Skill levels) and also doubles the base perk's effects (Attack and Defense cap increased to +10, and doubled recovery amounts).
    * Changed the following associated translation **values**:
        * `brute.desc` 
        * `brute.desc.prestiged`
        * `poacher.desc`
        * `poacher.desc.prestiged`
        * `desperado.desc`
        * `desperado.desc.prestiged`
        * `rascal.desc.prestiged`

### Fixed

* Fixed out-of-combat loss of Rage stacks, which probably was not working without Combat module.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.10

### Added

* Prospector Hunts can now trigger in Volcano Dungeon.

### Fixed

* Fixed Prospector Hunt incorrectly using Scavenger Hunt streak (wonder how many years this has been bugged).
* Fixed possible key exception in Prospector Hunt treasure calculation.
* Fixed a bug where Prospector Hunt would immediately finish successfully upon warping.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.8

### Changed

* Improved README.md UX using `<details>` tags and colored emojis.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.5

### Changed

* Spelunker ladder down chance changed from additive to multiplicative.
* Spelunker prestige recovery increased from +2.5% to +5% health and from +1% to +2.5% stamina per level.
* Ticker changed from PROFS to PRFS.

### Fixed

* Fixed an issue when attempting to stack ammo onto the second slot with an empty first slot.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.3

### Changed

* Changed the way Scavenger and Prospector treasures scale with the streak. Players should now see significantly more treasure if they manage to keep their streaks high.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.1.0

### Changed

* [CP] Fish Pond Data is now merged into the main mod, so no longer requires a separate download.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.0.2

### Changed

* Parallelized Luremaster logic to maybe improve performance on some systems.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 3.0.0

### Fixed

* SpaceCore skills should once again appear in the Generic Mod Config Menu for Exp. Multipliers.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.7

### Added

* Added to the `profs list` command the option to specify a skill name and get a list of professions in that skill. This can be helpful to find out the names of professions to use with the `profs add` command.

### Fixed

* Fix for Binning and Socializing skills not being loaded correctly.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.6

### Changed

* Reverted Aquarist to pre-version 2.5.0. Users pointe out that the change made it impossible to get pirate treasure.
    * Catching bar ~~increases more quickly~~ -> decreases more slowly.
* Custom skills are now loaded on GameLaunched. Previously all skills were loaded on SecondSecondUpdateTicked, because Love Of Cooking itself loads on FirstSecondUpdateTicked, and that needs to happen *first*. However, this was confusing and unecessarily complex for other developers. So now, LoC alone continues to load on SecondSecondUpdateTicked while everyone else loads at the reasonable stage.

### Fixed

* Fixed a small issue where the player could still keep the Limit Break associated with a removed profession. This probably doesn't concern 99.8% of players.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.5

### Added

* Added config to disable extended progression (level 20) while Prestige skill resetting is enabled.

### Changed

* Default value for ConservationistTaxDeductionCeiling changed to 100%.

### Fixed

* Fixed possible KeyNotFound exception is SLNGS module is not enabled.
* Explosive ammo now slays Mummies as expected.
* Fixed a possible KeyNotFound exception if placing a Crab Pot in an unusual map, like the Boat Tunnel.
* Fixed Prospector panning point tracking pointing off-screen.
* Fixed integration with Mushroom Propagator v2.2.0.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.1

### Fixed

* Fixed all xp gain being capped at 1.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.5.0

Major Overhaul for Fishing professions.

### Added

* Prospector Hunt streak now also buffs the chance of spawning panning points.
* Slingshot projectiles now leave a trail effect when overcharged.
* SpaceCore skill prestige choices can now also be respecced at the Statue of Prestige (untested).

### Changed

Major changes to the Fishing tree:
* **Fisher:**
    * Changed from what was essentially a "free permanent bait" effect, to "baits are twice as effective". The end result is more or less equal, but actually encourages using bait instead of trivializing it. Also makes Wild Bait (chance to double the catch) and Magnet (chance for pirate treasure) more appealing.
    * New prestige perk: baits are *three times* more effective. *Pretty obvious and natural progression. With Wild Bait the end result is effectively instant-nibble.*
* **Angler:**
    * Changed from "increased fish value by 1% per unique max-sized fish caught" to "increased fish value by 0.5% per unique fish caught"; i.e., scaling is reduced by half, but no longer requires max-size. Legendaries, likewise, are worth half as much, at 2.5% per.
    * Added: Fully-expending a tackle causes the fishing rod to "remember" the tackle's effects. Effectively, this doubles the duration of all tackles, but more importantly, it allows two tackles to be used at the same time. **Tackle effects cannot stack with themselves.** The currently "memorized" tackle appear in the rod's tooltip next to the equipped tackle, with transparency indicating the number of remaning uses.  
    * Removed "legendary fish recatch" prestige effect. New prestige perk: "increased fish vaue by *another* 0.5% for each unique max-sized fish caught". This makes up the other half of original Angler's scaling bonus.
* **Aquarist:**
    * Changed from "catching bar decreases more slowly" to "increases more quickly" for each unique fish species raised in a Fish Pond. *Actually helps to save time by reducing the mini-game duration, instead of just making the mini-game trivially easy.*
    * Ability to raise legendary fish in ponds is now available without prestige. *No longer will I have to answer folks who don't read the description why their Legend refuses to go in the pond. This perk was always more fun than gamechanging, so no reason to gate this until extremely late game.*
    * New prestige perk: pond capacity +4. *More useful and in-line with the Prestige theme.*
    * Default value for LegendaryPondPopulationCeiling lowered from 6 to 5.
* **Luremaster:**
    * Now also allows **baited** Crab Pots to produce 2x per day. Previous effects remain unchanged. *Huge and desperately needed buff. Also establishes a neat analogy to Producer.*
    * New prestige perk: produce 3x per day. *The natural progression.*
* **Conservationist:**
    * Village favor can now be gained without prestige. *This perk was pretty useless at extremely late game. It now gives further incentive to pick this profession early.*
    * New prestige effect: 50% faster trash build-up. *Not great, but the best I could come up with.*
    * Trash obtained from regular fishing also counts for tax deductions.

### Removed

* Removed LegendaryPondPopulationCeiling option. Max population of legendary ponds is now always half that of regular ponds (yes, Aquarist perk is also halved, for a total max of 7 with prestige).

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.4.0

### Added

* Brute Frenzy now causes Fear status on activation, if CMBT is enabled.
* Added compatibility for [Archery](http://www.nexusmods.com/stardewvalley/mods/16767).
    * Rascal's preservation effect will apply on top of the natural preservation chance of bows.
    * Bows can be overcharged by Desperado, and gain all the corresponding perks (damage bonus, knockback bonus, hitbox bonus and piercing chance). Note, however, that overcharging increases the odds of breaking arrows.
* Added a short delay before Desperado's overcharge begins. It should be barely noticeable while overcharging, but is *very* noticeable when you *don't* want to overcharge, so you don't hear the charging sound as often.

### Fixed

* Fixed Limit Break gauge still rendering with Limit Breaks disabled.
* Fixed a possible conflict with XPDisplay.
* Fixed an issue with Desperado's charging speed buff, which actualy reduced charging speed the player lost HP, instead of the opposite.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.3.0

### Added

* Added config settings to displace the Limit Gauge bar.

### Fixed

* ArtisanGoodsAlwaysInputQuality should now apply to PFM machines.
* Custom professions now show up in GMCM menu for BaseSkillExpMultiplier setting.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.8

### Changed

* Luremaster-owned Magnet-baited Crab Pots no longer produce trash, as stated in its description.

### Fixed

* LaxOwnershipRequirements should now apply to Rancher bonus.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.2

### Fixed

* Adjusted the position of prestige progression badges, which I guess were messed up by the latest SpaceCore update.
* Fixed ribbon-style prestige badges, which apparently were messed up by an update a while ago and nobody noticed, since nobody apparently uses it anymore.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.1

### Changed

* Refactored patchers in the Patchers.Common namespace, just for consistency.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.2.0

### Changed

* The intensity of the Slime ammo's slow effect has been reducing from 50% to 33.3%
* Desperado's overcharge bar now draw on RenderedWorld instead of RenderedHud, which makes it compatible with different zoom levels.

### Fixed

* Fixed possible bugs when a Rascal Slingshot is picked up by a non-Rascal.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.1.0

### Changed

* Special Abilities are now called Limit Breaks. The gauge has been changed accordingly.

### Fixed

* Fixed some possible bugs related to the number of attachments in a slingshot.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.5

### Fixed

* Config settings should now save correctly and not reset on every launch.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.4

### Fixed

* Fixed an ArgumentOutOfRange exception, which is, interestingly enough, caused by Vanilla when scrolling the toolbar.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 2.0.0

### Added

* Added a config to toggle the Crystalarium upgrade for Gemologists.
* Added a config to revert Artisan behavior to always preserve input quality.
* Added a config to cap the bonus from Aquarist.
* Conservationist now counts trash fished with a Fishing Rod.

### Changed

* Slingshot attachments upgrade for Rascals now occurs on equip rather than on profession gain. This should be more compatible and prevents a possible Null-Reference.

### Fixed

* Added several missing config options to the GMCM menu.
* Fixed a bug which allowed players to select both level 10 professions for custom skills during level-up.
* Fixed small typo in new Artisan description (removed "is").
* Fixed Poacher still gathering Special Ability charge with Special Abilities disabled.
* Fixed issue with Crab Pots ignoring farmhand professions in multiplayer games with LaxOwnershipRequirement set to `true` (thanks to [ncarigon](https://github.com/ncarigon)).
* Fixed ribbon display with new scrolling feature of SpaceCore.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.3.0

### Fixed

* Fixed a bug preventing explosive ammo from exploding without the Desperado profession.
* Fixed a possible Out-Of-Range Exception when hovering a Rascal Slingshot.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.2.1

### Changed

* Changes to Pacher Ambush -> *If an enemy is slain within ~~1.5s~~ 0.5s out of Ambush, immediately regain ~~20%~~ 25% special ability charge.*

### Fixed

* Fixed a possible throw when poaching from a monster that has no item to drop.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.2.0

### Changed

* Dramatically increased Blaster's chance of producing coal. Apparently the vanilla chance is so retardedly low that just doubling it doesn't make a noticeable difference.
* Piper Slimes no longer take on the properties of Hutch Slimes, because I decided that wasn't really immersive. Piper Slimes are not summoned from the Hutch; they are attracted due to the Piper's "sliminess", and therefore they should be native to the current location.

### Fixed

* The Prestige API should now be fully functional, so custom skill mod authors can now support prestige profession variants.
* Fixed the Agriculturist perk, which I straight up forgot to include in 1.1.0.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.1.1

### Changed

* Spelunker momentum is now gained on interaction with a ladder or sink hole, and not on player warped. This prevents exploiting other warp methods such as the elevator to rank up the buff for free.
* Prestige mechanic should now be fully functional for custom SpaceCore skills. Expect Binning and Socialization compatibility soon!

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.1.0

### Changed

* **Agriculturist:** The chance for Iridium Quality crops has been reduced to half of the chance with Deluxe Fertilizer.
* **Artisan:** Effect changed from "output quality **is at least as good** as input quality" to "output quality **can be as good** as input quality". I.e., instead of simply preserving the quality of the input, there is now a formula in place to choose a quality that is lower than or equal to that of the input. The 5% promotion chance still applies.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.2

### Added

* Prospector now also tracks Resource Clumps.
* Added Custom Resource Clumps integration.

### Fixed

* Fixed buggy nodes being spawned by Prospector.
* Fixed a typo in Brute's Rage buff.
* Fixed Custom Ore Nodes integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.1

### Changed

* Prospector ore spawning chance is no longer recursive, which should dramatically reduce the odds of extra node spawns.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 1.0.0

### Changed

* Reduced the spawn chance of extra mining nodes by Prospector.

### Fixed

* Fixed the range for triggering Demolitionist's Get Excited buff.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.9

### Fixed

* Fixed the display of Brute and Spelunker buffs.
* Brute buff now activates in a more clever way.
* Desperado Overcharge meter no longer renders in legacy firing mode.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.7

### Changed

* Custom skills are now loaded on 2nd SecondUpdateTicked instead of SaveLoaded.

### Fixed

* Fixed a typo causing a Null-Reference Exception when resetting a custom skill.
* Treasure hunts will no longer trigger during cutscenes.
* Fixed an incorrect patch target which should allow taunted monsters to reset their aggro when the taunter dies.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.6

### Added

* Added OnProfessionAdded and OnProfessionRemoved events. So far they only serve to invalidate the cache of profession icons, but will likely be useful in the future.

### Changed

* Lowered the log level of Ultimate setter from Warn to Info.

### Fixed

* The level 20 prestige choices should now accurately reflect the prestige branch chosen at level 15.
* The profession choice menu should now correctly display prestige profession icons.
* The skills page profession icons should now also correctly display the prestige choices made on the night previous.
* Replaced two more incorrect FieldGetters with the correct PropertyGetterGetter in Automate integration.
* Fixed a typo in "multiplyer" FieldGetter.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.4

### Fixed

* Fixed the order of displayed Prestige ribbons in Skills page.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.2

### Fixed

* Fixed Scavenger and Prospector tracking arrows not being displayed.
* Improved tracking arrow position for tracked bushes on-screen.
* Replaced an incorrect FieldGetter with the correct PropertyGetterGetter in Automate integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.1

### Fixed

* Fixed an issue with Luck Skill integration.

<sup><sup>[🔼 Back to top](#profs-changelog)</sup></sup>

## 0.9.0 (Initial release)

### Added

* When a Gemologist reaches a new quality threshold, all currently processing Crystalariums owned by that player will likewise receive a quality upgrade to reflect that.
* Scavenger profession now has a chance, proportional to the Treasure Hunt Streak, to spawn additional forage when entering a new map.
* Prospector profession, likewise, now has a chance, proportional to the Treasure Hunt Streak, to spawn additional ore nodes when entering a new mine level.
* Added golden versions of profession icons for Prestiged professions.
* Added config setting to disable Bee House being affected by Producer profession.
* Added API for custom Skill mods to register prestiged professions.

### Changed

* **Rascal** - ~~Slingshot damage +25%. 60% chance to recover spent ammo.~~ Gain one additional ammo slot. 35% chance to recover spent ammo.
    * The damage perk is gone. I always felt like Slingshot damage was overpowered anyway. In its place comes a second ammo slot; Rascals can now equip 2 different types of ammo (or the same ammo twice). Use the Mod Key (default LeftShift) to toggle between equipped ammos.
    * The ammo recovery perk has been nerfed to account for new enchantments provided by the Arsenal module. In exchange, the Prestige perk now doubles this value to a whopping 70%---a higher value than the original.
    * Ability to equip Slime ammo moved from Slimed Piper to Rascal. It will still cause a slow debuff, but will not heal ally Slimes unless the player *also* has the Piper profession.
* **Slimed Piper** - Summoned Slimes are now friendly (will not attack players, but will still cause damage if touched).
* **Desperado (Prestiged)** - ~~Overcharged shots become spreadshots.~~ Overcharged shots can pierce enemies.
    * The Spreadshot perk has been moved to a new enchantments provided by the Arsenal module. The ability to pierce enemies with overcharged shots is now the new prestige perk.
* The [API](../../API/IModularOverhaulApi.cs) has been slightly changed. Some mods may need to update the corresponding interfaces.

### Fixed

* Fixed an issue with Automated Junimo Chests being unable to decide their owner.
* Fixed a bug where Conservationist Trash Collected would not reset if no trash was collected during the season.
* Fixed a bug where Tapper perk would incorrectly apply to tapped Mushroom Trees in Winter, causing multiple progressively shortened harvests.
* Fixed a bug where the Piper Concerto super ability caused all Slimes in the Hutch to disappear.
* Crab Pots now correctly trap Seaweed instead of Green Algae in the Beach farm map.
* Fixed a bug where Special Abiltiy charge would still accumulate with Enable Specials config set to false.
* Fixed a possible NullReferenceException when shooting at Slimes.
* Fixed TrackerPointerScale and TrackerPointerBobbingRate config settings.
* Added a setter to the CustomArtisanMachines config, preventing it from being reset on game load.
* The print_fishdex console command now takes into account the value of AnglerMultiplierCap config.
* Fixed some translation errors.

### Removed

* Removed the SeaweedIsTrash config setting.
* Removed configs from the [IProfessions interface](../../API/IModularOverhaul.cs).

[🔼 Back to top](#profs-changelog)