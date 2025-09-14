# PROFESSIONS Changelog

## 1.4.0

### TL;DR

* Small buffs and QoL to Slimed Piper.
  * A small nod to Expedition 33, Prestiged Slimed Pipers can now "paint" over their Slimes.
* No more gate-keeping legendary Fish Ponds.
* Several bug fixes.

### Added

* Added population gates to legendary fish ponds (this doesn't mean they can reproduce).
* Slimed Piper now pacifies wild Slimes. They will act neutral towards all players while in the presence of a Piper.
* Slimed Piper can now hold the Mod Key to temporarily charm the nearest Slime. This can be used to add an extra unit to your Slime army, but the main use of this perk is to manually herd Slimes for breeding.
* Prestiged Slimed Piper can now craft Slime Paintbrushes. 5 new recipes will be added upon choosing this prestige (Green Brush, Blue Brush, Red Brush, Purple Brush and Prismatic Brush). Brushes other than Prismatic can be applied to raised Slimes to increase their RGB color values, bringing them closer to perfect white. The Prismatic Brush can only be applied to a perfect White Slime, and will convert it into a Prismatic Slime. Prismatic Slimes gain a significant boost to combat stats and also gain access to all special colored abilities, expect for Black and Gold). For the purpose of breeding, a Prismatic Slime behaves like a regular White Slime.
    * Slime Paintbrushes are a universal neutral gift, except for:
      * Loved by: Leah, Emily, Jas, Vincent, Leo and Krobus (he's fascinated by colorful things).
      * Liked by: Penny and Robin.
      * Disliked by: Haley (it's icky), Pam and Sebastian (hates colorful things).
* Prestiged Slime Piper can now "see" the color components of raised Slimes, either by hovering the cursor over the Slime (Mouse & KB only) or by holding the Mod Key.

### Changed

* Slimed Piper now restricts summons to Slimes inside a Hutch, instead of the entire farm. This gives a small degree of control over which Slimes can be summoned, while doing the gross of your breeding outdoors.
* In combination with the previous point, also changed the algorithm that determines Slimed Piper number of Slimes to summon;
  * Before: # of raised slimes / 10 -> this required 10 raised Slimes before any would be summoned at all, increasing to 2 at 20 and 3 at full capacity of 30.
  * After: (# of raised slimes - 1) / 10 + 1 -> this forces a Slime to be summoned if at least 1 Slime is found inside a Hutch, increased to 2 at 11 and 3 at 21.
  * Improved again the Slimed Piper description in the README.
* Tapper recipes from the Tapper profession can now be edited by other CP mods.
* Scavenger hunts no longer occur on rainy days, because the rain makes the dirt hints impossible to see.
* Changes to LegendaryFishPondData.
* Extended Family Legendary fish can no longer be plprfs add piperaced in ponds by themselves; they can only be placed in ponds with their parents (and only if [Aquarism](../Ponds) is installed).
* Updated Chinese localization, credit to [Ellillilliott](https://forums.nexusmods.com/profile/121177218-ellillilliott/).

### Fixed

* Fixed ammo being duplicated by Desperado's Death Blossom.
* Legendary fish Aquarist condition changed from a Prefix to Transpiler to avoid the inlining issue which caused it to not apply for some users.
* Artisan and Industrialist processing speed perks now apply correctly to casks.
* Fixed some possible null-reference exceptions in Angler tackle memorization.
* Fixed Breeder incubation time bonus being applied twice.
* Removed an incorrect water source tile data from Premium Barn map.
* Fixed an edge case where passing out during a Scavenger Hunt would trigger an error loop.
* Fixed an issue caused by SVE's dumbass CP code which resulted in Immersive Dairy Yield setting not applying to the Butter Churn.
  * If anyone from SVE team reads this, your Machine Data entries for Butter Churn regarding Large Milk and Large Egg do not apply, as the regular Milk and Egg entries take precedence. Instead of separate entries, I recommend setting the `StackModifier` field with condition `ITEM_CONTEXT_TAG Input large_milk_item` or likewise for Large Eggs. Also, please just use `CopyQuality` field instead of adding individual entries for each quality. This way you can reduce 16 entries down to just 2 ([Modding: Machines](https://stardewvalleywiki.com/Modding:Machines)).
  * Note that this "fix" also removes the copy quality property of SVE Butter Churn, which is intended for consistency.
* Fixed a possible issue where Piper events were not enabled on save load.
* Added failsafe to check completion status during Treasure Hunts (hopefully should fix issues related to using bombs, especially in Prospector Hunts).
* Fixed a possible issue with Chroma Balls in multiplayer.

### Removed

* Removed the Aquarist restriction on legendary fish ponds; the Aquarist profession is no longer required to place legendary fish in ponds, as per vanilla rules (max population of 1).
  * Aquarist profession will instead open up the population gate quests, which will allow increasing the max population to 5 before prestige.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.3.4

### Changed

* Cropwhisperer perks now only apply to the local player.

### Fixed

* Fixed an issue with regular Artisan not assigning quality correctly.
* Fixed an issue where machine would not receive the production speed bonus if the output quality is iridium. Can't believe nobody reported this.
* Fixed Crop Fairy chance promised in the previous version.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.3.3

### Added

* Silviculturist now also doubles the chance of a Crop Fairy event (that's just an extra 1%).

### Changed

* Final take at Cropwhisperer rework. Took a few tries, but I think we got there:
  * Removed: Crops grow 20% faster. *Since base Agriculturist can now use crop rotation to increase growth speed further, this is unnecessary.* 
  * Removed: ~~Crops survive up to one week out of season.~~
  * Added: **Use Retaining Soil to grow crops out of season.** _This preserves the out of season mechanic, while also resolving the complaint about that feature forcing manual disposal of crops at the start of a new season, improving the usefulness of a massively underused resource (Retaining Soil), and also dramatically expanding the realm of possibilities for the player at a reasonable cost, all in one fell swoop. See the updated [README](./README.md#-farming) for details on how this will work._
* Some balance tweaks to Prospector and Scavenger Hunt treasure tables to avoid ridiculous stacks of ores.

### Fixed

* Fixed a bug causing Prestiged Slimed Piper Chroma Balls to spawn despite the profession not being prestiged.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.3.2

### Changed

* Silviculturist green rain chance changed from 1.5% per green rain tree to a base 10% + 2% per green rain tree. 
* Rodmancer legendary encounter chance now increases by 2% on a perfect catch. The chance now also counts the initial 10 catches, instead of starting at 0% after those 10 initial catches. 
* Baby Slimes now always inherit the highest stats from each parent, instead of inheriting from a random parent.

### Fixed

* Fixed tracking profession markers not appearing.
* Fixed renamed method in [Combat](../Combat) API.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.3.1

### Changed

* New changes to Agriculturist:
  * It was not my intention to nerf the vanilla profession. So I changed it again to a compromise; now grants flat 10% faster growth, *plus* 5% for each new crop type grown in the same tile and in the same season (no longer limited to 3 previous crops -> no more micromanagement).
  * Cropwhisperer perk was a placeholder until I could come up with something better.
    * Removed: Growth speed boosts also apply to regrowth cycles of multi-harvest crops.
    * Restored: Additional 10% flat growth speed boost.
    * Added: Chance to harvest crops grown previously in the same tile and season. _I wanted it to build on the crop rotation mechanic, and I like the idea of hybrid crops. This is a simpler version of that idea that I think still feels interesting._

### Fixed

* Fixed out-of-range exception with Treasure Hunt in multiplayer.
* Fixed incorrect skill reset cost.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.3.0

### Added

* Slimed Piper summoned minions are now represented by individual HUD portraits and can be dismissed by clicking the respective portrait.
  * The Hat Slime does not show up in the HUD as it cannot fight, and thus cannot be dismissed without first unequipping said hat.
* Added swirly VFX to piped Slimes, to distinguish from regular enemy Slimes (thanks [bblueberry](https://next.nexusmods.com/profile/bblueberry?gameId=1303)).
* Add, Set and Print commands now also work in local co-op. Use the `-f` flag to specify a player number (i.e., "1" for the host player, "2" for player 2, etc.).

### Changed

* Renamed data file patterns. Artisan Machines data must now end with `.ArtisanMachines.json` and Animal Derived Goods data must now end with `.AnimalDerivedGoods.json`. If you had previously made your own data files for compatibility with other mods, make sure to rename them accordingly.
* Due to changes with how the mod tracks skill resets, **gold** is now the highest stacked star that will show up in the skills menu, indicating three skill resets, which is enough to obtain all four 10th-level professions. There is no longer an iridium star, so don't be alarmed!!
* Legendary Fish Pond Data now has its own data asset file, so users can easily open it and read what items are configured to be produced, and customize it if they wish.
* Agriculturist has been reworked. No longer grants a flat 10% growth speed bonus. Instead, implements new crop rotation mechanic:
  * Each tile remembers the last 3 crops grown. Planting different crop varieties increases soil fertility, granting +5% growth speed and quality per unique crop, up to +15%. Repeating the same crop twice in a row cancels the most recent bonus. Replanting a different crop already in memory provides no effect. _Encourages rotating between different crops instead of repeatedly growing just Hoppers / Blueberries / whatever, promotes experimentation and crop variety._
* Cropwhisperer is also changed. Since the base Agriculturist can already provide up to 20% growth speed boost, additional growth speed is redundant. Players who reach the very late-game stages required for level 20 would rather step away from micromanagement and towards more optimized and automated layout.
  * ~~Crops grow 20% faster~~ Growth speed boosts also apply to regrowth cycles of multi-harvest crops. _This would be broken in the early game, but should be sufficiently strong as a late game perk._
  * Crop survival out of season increased from 5 days to 1 week (i.e., they will die on the 8th).
* Prospector and Scavenger Hunts have been overhauled:
  * Changes to the mini-game:
    * The Scavenger Hunt has been overhauled as "hot-or-cold" mini-game. Chasing a tiny arrow was mindless and boring. Now, the player must dig and follow the cues to the right dig spot.
    * The Prospector Hunt has be overhauled as a "Simon-says" mini-game. The target rock isn't hidden; it will be prominently displayed. But the player must mine a sequence of 8 highlighted rocks within the time limit. Each rock is progressively harder to mine.
    * The legacy mini-games have been retired. Players can no longer opt into "Legacy Treasure Hunt" mode.
  * Changes to the treasure tables:
    * Treasure tables have been boosted significantly. Previously they shared a similar logic to fishing treasure chests, which are very stingy. The logic would choose 1 out of 6 possible loot tables and then roll the dice for the items within that table. This did not work well, because fishing chests can be triggered many times by fishing repeatedly, and therefore can afford to be stingy. Whereas treasure hunts are meant to be triggered randomly only once or twice per day. The player must also go out of their way to complete the treasure hunt, while a fishing chest is an additional reward to the player's current activity. There was also a chance to repeat the looting algorithm, based on the player's winning streak. The player effectively had to put up with dozens of rounds winning trash until they could build enough momentum to begin seeing decent rewards. Failing or ignoring a treasure hunt would break the streak. This made treasure hunts feels like an annoying chore, more so than an exciting reward opportunity.
    * New treasure logic rolls multiple dice for each loot table, all with substantially increased odds. Overall, every hunt is now so rewarding that there is a big opportunity cost to ignoring it.
    * Loot tables no longer depend on hunting streaks, but do depend on luck and daily luck.
  * Changes to hunting streak mechanic:
    * Streaks no longer affect loot tables, but still affect the prestiged Prospector and Scavenger's first perk (increased spawns of forage and minerals).
    * The mod now remembers your longest streak, and uses that for the bonus instead of the current streak. So the punishment for missing a hunt is far less significant.
  * QoL changes:
    * The time limits of each treasure hunt can now be set directly, instead of setting an indirect handicap multiplier.
    * Treasure hunts now apply a dummy buff so that players can keep an eye on the time left.
* Artisan quality perks now apply as a quality floor; if the machine would already produce a higher quality, then that quality will no longer be overriden.
* Respawned Piper minions will now spawn as a new randomly chosen Slime, instead of continuously respawning the same one.
* Slime IVs have been slightly simplified. The max IV is now 5 instead of 10, and the growth curve mean value has been shifted by +1, so it is significantly easier to maximize Slime stats.
* Removing a Slime's hat now forces it do drop all items.
* The default value of bonus skill experience after a skill reset has been reduced from 25% to 10%.
* Improved formatting for some console command outputs.

### Fixed

* Fixed issues with Cropwhisperer out of season crop survival.
* Fixed tracking arrows not bobbing as they should.
* Slimed Piper summoned minions are no longer damaged by explosions.
* Fixed tile position of water spots in the Slime Hutch.
* Fixed players being able to assign more than one Hat Slimes.
* Added missing "honorific" key to i18n.
* Fixed a bug with Rascal ammo recovery.
* Fixed a long-standing issue with how the mod counts skill resets. Previously, just acquiring a level 10 profession would count as a reset. This also means that the bonus exp after completing all resets would be applied 4 times instead of 3. The mod now persists data at the moment a skill is reset. Bonus experience only begins applying after an actual reset, and can only be stacked up to 3 times, once per reset.
  * This will also change how stacked stars are displayed in the skills page; a star now indicates a *reset* and not a 10th-level profession acquired. The highest star is now gold, instead of iridium. 
* Players can no longer change ExpPerPrestigeLevel setting *after* having gained a prestige level. Additionally, changing this setting will now correctly adjust the cached experience curve.
* Fixed Wild Bait not working without Luremaster profession (I guess this was added in 1.6).
* Fixed The Art O' Crabbing bonus not stacking with Wild Bait.
* Fixed Spelunker safe-room recovery incorrectly requiring prestige.

### Removed

* Removed global chat notifications when a player starts/ends a treasure hunt.
* Mixing of legendary and extended family fishes in a pond has been moved to [Ponds](../Ponds) and removed from this mod.
* Fixed and then removed the broken code for using Tree Fertilizer on Fruit Trees. Dunno why that was in here in the first place.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.11

### Fixed

* Fixed soft-lock during prestige level-up menu for farmhands in local-coop, caused by the single-profession selection menu being unresponsive to gamepad input.  

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.10

### Fixed

* Fixed the prices of Ostrich and Golden Mayo, and new Slime goods, which were switched for some reason. 
* Professions should now apply to SVE animals.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.9

### Added

* Forgot to add SVE's Yarn to Animal Derived Goods data.

### Fixed

* Fixed the Null-Ref exception caused by mods which don't specify a goddamn ID for their machine output rules.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.8

### Added

* Added compatibility files for SVE Artisan Machines and Animal Derived Goods.
* Added new Slime Mayo and Cheese items to further flesh out the Slime Rancher playstyle (although they're not very useful to be frank).
* Added NPC gift tastes for modded mayo and cheese items, coherent with vanilla tastes (basical.

### Changed

* ImmersiveDairyYield setting should now apply to all mod machine rules for large eggs and milk, provided the machine rule ID follows the vanilla convention by including the words "LargeEgg" or "LargeMilk".

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.7

### Added

* Added SVE-specific descriptions for Breeder and Producer to include Premium buildings.

### Changed

* Buffed Cropwhisperer: *[...]* crops can survive ~~the first day~~ up to 5 days out-of-season.
  * A single day out-of-season was nowhere near impactful enough for an end-game prestige perk, especially in comparison to the Industrialists' much stronger ability to preserve all ingredient quality. This buff should put it in a much better spot, ensuring a full extra regrowth cycle of any regrowth crop.
* Breeder now only applies its bonus animal value perk to animals bred after obtaining the profession. Purchased animals are unaffected. Progenitor already worked the same way.
* Breeder and Progenitor both now consider Lax Requirements.
* Barn and Coop max capacity change conditions are now slightly more robust. Shouldn't affect anything, but you never know.
* Demolitionists' Get Excited buff now requires the prestiged variant and Pyromania setting enabled.

### Fixed

* Minor corrections to localizations.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.6

### Fixed

* Blinded status now affects Piper Slimes (Blinded enemies miss against Piped Slimes).
* Decoupled Piped Slime invincibility timer from monster invincibility timer, so Slimes can do damage will being invincible and take damage while enemy is invincible.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.5

### Added

* Prismarch Black Slimes now cause Blinded status.

### Fixed

* Removed event which was resetting mod state each night. Dunno why that was there.
* Better resource clean-up at end-of-day.
* Added missing localization for BreederFriendlyAnimalMultiplier setting added in [1.2.4](#124).

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.4

### Added

* Added config option to change the price multiplier of Breeder animals. Use this if playing with third-party balancing mods which reduce overall profits. Otherwise, I suggest leaving the default value.

### Changed

* ArtisanMachines and AnimalDerivedGoods data now reads all JSON files in the "assets/data" folder, instead of only the hardcoded files of the same name. This should allow users to create their own modularized data files without worrying about overwriting them during an update.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.3

### Added

* Added actual music to Piper's Hamelin Concerto. Activating this Limit Break now plays one of three concertos at random.
  * The music files are a little big unfortunately.
* Added German translations by [OromisElf](https://github.com/OromisElf).

### Changed

* Poacher Ambush now immediately re-activates upon assassinating an enemy, instead of only recovering 50% charge. You can pop in and out of the shadows.
* Updated Chinese translations by [BlackRosePetals](https://github.com/BlackRosePetals).

### Fixed

* Poacher Limit Break now properly deactivates right before an enemy is attacked.
* Fixed some translation keys.
* Removed the patch that was resetting special move cooldowns.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.2

### Added

* Added FullHouse command.

### Fixed

* Fixed Producer not counting Premium buildings. Please note that buildings must still be *full*, even if capacity is higher.
* Fixed picked-up items not having quality.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.1

### Fixed

* Prestiged Angler fishing chain now (properly) resets upon leaving the map.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.2.0

### Added

* Added compatibility for SVE Premium Barn and Coop.
* Added a config option to lock skill Mastery before fully obtaining all professions.
* Added simple Slime breeding mechanic via Slime IVs and stat inheritance, to support reworked Slimed Piper (see below). In the vanilla game, a Slime always inherits the stats of the male parent (even though there is code for choosing from either parent, the game then immediately overrides that with the male parent's stat value). This is now removed and replaced with an IV system similar to Pokémon.
    * Every Slime is assigned an IV (Individual Value) between 0 and 10 for each of Attack, Defense and Health.
    * IVs grant a multiplicative bonus to the corresponding stat (x2 at 10).
    * First-generation Slimes (hatched from eggs) are born with a random IV between 0 and 2.
    * IVs can be increased by breeding; when a baby Slime is born, it inherits each base stat from a random parent, and IVs are picked from a normal distribution peaking at 1 value higher than the parent's. This guarantees that IVs will eventually converge to 10 after several generations.
    * Baby Slime stats are no longer determined by the species (Green Slime, Frost Jelly or Sludge) as determined by game data; stats are always inherited from parents. This allows any Slime color to be raised effectively to maximum stats.
    * Purple Slimes still have the highest base stats, so you'll want to breed them in somewhere to get the highest stats, before then breeding for a specific color.
    * The Slimed Piper profession will grant benefits for each variety of Slime. This new feature is therefore intended to support the Slimed Piper profession.
    * Added special Slime variants that can be obtained by breeding:
      * Gold Slime variant can be bred by aiming for RGB(255, 215, 0), also known as HTML Gold (i.e. try to mix Red and Green Slimes).
      * Prismatic Slime variant can be bred at a low chance when breeding a White Slime RGB(230+, 230+, 230+).
* Added config option to show/hide minion health.
* Added new Prestiged profession sprites using Silicon's and Kawaii's gold palettes, made to match their Mastered skill icons. These palettes will also apply to the Gold Slime. Posister's and my own Metallic gold palettes are retired to the "old" folder. You can still use them by replacing either Silicon's or Kawaii's spritesheet in the main sprite folder.

### Changed

* Reworked Cavewarden:
  * Removed: Increased chance to find treasure and safe rooms.
  * Changed: Chance to resurface unclaimed mining debris is now dynamic, starting at ~10% and increasing with each mine level until ~100% at Skull Caverns floor 200, instead of a constant 50%.
  * Added: Once per day, revive at the last-visited safe room.
* Reworked Demolitionist:
  * Manual bomb detonation is now a much more usable toggle (double tap Mod key), instead of requiring the Mod key be held.
* Reworked Rodmancer:
  * Re-catching Legendary fish now requires building up a chain of successful catches.
  * At least 10 successful catches are necessary for Legendary fish to begin appearing.
  * Every subsequent successful catch after the 10th will increase the encounter rate by 1%.
  * The chance resets back to zero if you fail a catch. But successfully catching the Legendary fish does not.
  * Finding trash does not interrupt the chain.
* Reworked Slimed Piper: now works more like a Necromancer than a Beast Tamer archetype, which means you command an army of disposable Slimes instead of micromanaging a single stronger Slime. 
  * No longer tames wild Slimes.
  * Ally Slimes are now spawned in based on the number of raised Slimes, at the rate of 1 ally Slime per 10 raised Slimes.
  * Ally Slimes are picked at random from amongst your pool of raised Slimes. Players are therefore encouraged to raise stronger Slimes (Purple Slimes, Tiger Slimes).
    * Added special Gold Slime variant. Piper players can breed Gold and Prismatic Slime variants under special conditions. 
  * When an ally Slime is defeated, it respawns automatically after a short while.
  * All ally Slimes are now immune to player-inflicted damage.
  * Ally Slimes no longer pick up items, but you can offer a hat to one of your Slimes to convert it into a pickup mule.
  * A hat-wearing Slime will not participate in combat and cannot be defeated.
  * You can interact with your hat-wearing Slime at any moment to open its inventory (as long as it has any items).
  * To dismiss a hat-wearing Slime, simply remove its hat. If dismissed while carrying items, those items will be permanently deleted.
  * Items picked up by hat-wearing Slimes will be sent to the player's inventory if they already have a stack of that same item. Only if this attempt fails, the item will be placed in the Slime's own inventory.
  * Slime's inventory capacity increased from 10 to 12 slots.
    * Ally Slimes will only follow you into dungeons. A hat-wearing Slime can follow you anywhere except indoors (effectively a backpack upgrade).
  * Slime pathfinding is now multithreaded and async by default, which should improve performance in most systems. Added config option to disable async in case of issues~~~~~~~~~~~~~~~~.
  * **Limit Break:**
    * Inflates all Slimes within reach, both ally and wild.
    * When the effect ends, ally Slimes deflate back to normal. Wild Slimes burst instead.
    * When activated, Big Slimes will now burst *first*, releasing baby Slimes that are immediately inflated as well.
    * Lasts 15s.
* Reworked Slime Conductor:
  * Removed: Tame one additional ally Slime.
  * Added: Ally Slimes gain one special ability based on color:
    * **Green:** Causes Slimed debuff.
    * **Blue:** Causes Chilled/Frozen debuff.
    * **Red/Purple:** Causes Burn debuff.
    * **Black:** Chance to transform enemies into Void Essence.
    * **White:** Grants an aura that heals a low amount of health over time.
    * **Gold:** Causes nearby enemies to drop gold (100g per kill).
    * **Prismatic:** Causes nearby enemies to drop Prismatic Shard.
  * Renamed to Prismarch / Prismatrice.
* Tapper recipe cost reduction increased to 50%.
* Changed Luremaster extra catch probability to a proper cumulative Gaussian distribution instead of whatever linear function was used before. 

### Fixed

* Fixed an issue where Angler's second tackle memory was incorrectly being set to a Leek.
* Fixed an issue where Angler's first tackle memory was consumed once while recording the second tackle memory.
* Fixed an issue where Angler's tackle memory would not be consumed if no tackle were equipped.
* Fixed Legendary fish having 0% catch rate with Rodmancer profession.
* Fixed gamepad support for the Mastery confirmation dialogue box.
* Fixed an issue with Mastery confirmation ignoring UI zoom level.
* Fixed an issue where disabling Prestige Levels did nothing.
* Fixed a Null-Reference Exception in Ecologist tree shake logic.
* Fixed an error with the Piper-modified Slime Hutch map.
* Fixed an issue preventing regular profession change at the Statue of Uncertainty when Skill Reset is disabled.
* Fixed invalid `set gemologist` and `set ecologist` console commands.
* Fixed foraged minerals not receiving Gemologist quality.
* Fixed a potential issue where inflated Slimes would not deflate properly.
* Fixed an issue preventing setting NewLevels field of SpaceCore skills.
* Fixed Casks incorrectly downgrading input quality. They no longer gain the Artisan's chance to increase quality immediately either. They will only gain the faster production. 

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.1.0

### Changed

* Changed the item ID of Ostrich and Golden Mayo, preemptibely resolving the same issues encountered in [Harmonics](../Harmonics/CHANGELOG.md#changed-2).
* AnimalDerivedGoods and ArtisanMachines are now provided as dictionary content assets instead of config settings. Mod authors can now target these dictionaries with Content Patcher to add compatibility for their own mods, instead of relying on users needing to do this manually. Savvy users can still do it manually by adding qualified IDs to the corresponding files within the `assets/data/` folder.

### Fixed

* Fixed a possible conflict with [Ponds](../Ponds).

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.0.1

### Added

* Legendary Fish Ponds no longer receive the Aquarist max. population buff. However, the prestiged version of Aquarist now lifts the halved population limit of legendary Fish Ponds.
    * **Regular Aquarist** -> Legendary population cap = 5 (down from 6).
    * **Prestiged Aquarist** -> Legendary population cap = 10 (up from 7).
* Added some clarification to the mod description to clarify that **legendary fish cannot be raised in Fish Ponds without the Aquarist profession**, despite this feature being added in game version 1.6.

### Changed

* Prestiged Spelunker item recovery chance increased from 20% to 50%.

### Fixed

* Fixed a small display issue in the Skill Page menu which caused all level 10 professions to show as "Desperado".
* Fixed an issue caused by changes in SpaceCore 1.27.0.
* Removed the legacy hard-coded check for Golden Eggs which prevented it from working correctly with Artisan.
* Fixed issue causing 100% coal chance from stones in the farm.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 1.0.0

Updated for game version 1.6.14.

### Changed

* Angler boss fish recatch chance increase from 1% to 5%.
* Due to vanilla changes in 1.6.9, the default behavior of automated Conservationist Crab Pots has been changed to consume bait if available.

### Fixed

* Fixed an issue with Prestiged Miner perk not working correctly.
* Fixed an issue with Gemologist perk counting items that are not minerals obtained from breaking geodes.
* Fixed a possible Null-Reference Exception in SpelunkerWarpedEvent.
* Fixed a possible Null-Reference Exception in RecordTackleMemory.
* Fixed an exploit where Desperado's Limit Break allowed multiplying ammo.
* Fixed an issue with level 5 combat incorrectly awarding 50 health points.
* Fixed an unconfirmed issue with AnimalDerivedGoods config, and added Cornucopia items by default.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

### Removed

* Removed the changes to Fish Smoker which caused it to no longer preserve quality; Fish Smoker is no longer affected by any Artisan profession perks.

## 0.4.0

Updated for DaLionheart 1.3.0.

### Changed

* Tweaked tracking arrow colors a bit. Artifacts spots are now always tracked with green, and are the only thing tracked with that color. Prospector mining nodes is now a darker orange for better contrast with Scavenger forage.
* Default tracking arrow size was reduced to x1 (default vanilla size), down from x1.2.

### Removed

* Removed FakeFarmer (moved to Core mod).

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.5

### Changed

* Players can no longer gain Mastery Exp if one or more vanilla skills are not at level 10 or above.
* Players can no longer enter the Mastery Cave if one ore more vanilla skills are not at level 10 or above.

### Fixed

* Fixed mod not allowing player to gain any Luck exp. **Luck Skill is still not supported.**
* Fixed Limit Gauge rendering during screenshots.
* Fixed an issue with multiplayer farmhands getting repeated level ups.
* Fixed an issue during prestige profession selection introduced in the previous version.
* Fixed professions stars not appearing after the first skill reset.
* Fixed minor issues in SpaceCore's SkillLevelUpMenu.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.4

### Fixed

* I fixed something. I forgot what it was.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.3

### Fixed

* Fixed possible issue in LevelUpMenu code.
* Added missing translation keys. Updated Chinese translation, thanks to Awassakura.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.2

### Fixed

* Add/remove Mastery commands now also correctly changes Mastery level.
    * You can either use `prfs set <skill> <mastered/unmastered>` to add or remove a mastery, **OR** you can use `prfs add mastery <skill>` / `prfs remove mastery <skill>`. Both commands do the same thing, except that `add` and `remove` commands can accept the parameter `all` instead of a skill name to process all 5 skills at once. Obviously do not use `<>` when actually running these commands.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.1

### Added

* All shaken fruits are now considered forage for the purpose of Ecologist perks.
* Added a warning dialogue before claiming a skill Mastery to inform players that skill resetting will become unavailable. No excuses now.
* Added back UiInfoSuite2 compatibility.

### Changed

* Angler bonus increased from 0.5% to 1% per unique max-size fish, and from 2% to 5% per Legendary fish.
* Can no longer enter the Mastery Cave unless all skills are level 10 (vanilla behavior). Otherwise this would allow players to claim Mastery of skills at any level, unless I messed around with vanilla code even more, which just isn't worth it.
* Skill Reset stars will no longer appear until the player has 2 or more professions in any skill. This is to avoid players getting stuck looking at bronze stars if they don't plan on resetting any time soon.

### Fixed

* Fixed Farmhands not being able to complete Scavenger Hunts.
* Fixed Delight and Shiny Mayo still being present in the game when disabled.
* Trash fished out of Fish Ponds no longer counts towards Conservationist perks.
* Fixed a possible issue in Tree.shake patcher.
* Updated incorrect description in README for Ecologist profession which stated that it affects Mushroom Boxes. It doesn't.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.3.0

### Added

* Updated few lines of Korean translation by [whdms2008](https://next.nexusmods.com/profile/whdms2008/about-me?gameId=1303).
* Added Russian translations by [darthalex2014](https://forums.nexusmods.com/profile/122957028-darthalex2014/).
* Added console command to set/unset mastered skills.
* Added compatibility for new [Enchantments](../Enchantments) mod.
* Fish Smoker no longer mentions that it preserves quality, since it doesn't with this mod.

### Changed

* Changed the colors of tracking pointers.
    * **Yellow:** Foraging stuff (Scavenger)
    * **Orange:** Mining stuff (Prospector)
    * **Blue:** Panning spots (Prospector), ladders/shafts (Prospector), artifact spots (both)
* Removed Golden Mayo from the list of required items for Perfection.
* Spelunker buff is now a stackable buff.
* Massive performance improvement (7x) in enemy AI code ~~by undoing a stupid "optimization" I had done a while back~~.
* Brute rage decay lowered to 15s from 20s.

### Fixed

* Hopefully fixed possible issue caused bad badly-coded fish mods that don't properly assign context tags to their fish.
* Fixed Ostrich and Golden Mayo items not being produced correctly.
* Fixed typo in file `skills_KawaiiRoseGold.png`.
* Fixed an issue causing incorrect level 10 profession choices to be offerred. Apparently this also fixes people being stuck at 5/6.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.10

### Added

* Added Angler + Ms. Angler mating in legendary ponds.
* Added initial compatibility for Item Extensions.

### Changed

* Removed cache invalidation when opening the Game Menu.
* Some changes to PondQueryMenuDraw.

### Fixed

* Fixed 1.6 level up message not showing in the HUD.
* Fixed issues related to PondQueryMenuDraw for legendary ponds.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.9

### Added

* Mastered skills now display a gold icon in the Skills Page menu. Thanks to [silicon](https://next.nexusmods.com/profile/siliconmodding/about-me?gameId=1303) and [KawaiiMuski](https://next.nexusmods.com/profile/KawaiiMuski/about-me) for the icons.
* `set fishdex` command now accepts a flag `-t` for trap fish.

### Changed

* Ecologist now only applies edibility changes to gathered forage items instead of all forage items added to inventory. But those changes are now permanent, instead of being lost when the Ecologist deposits the item.
* Angler sell price bonus now also applies to Smoked Fish.

### Fixed

* Fixed an issue causing incorrect level 20 prestige options to be offered.
* Fixed issue of items not stacking when the player has the Ecologist profession.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.8

### Fixed

* Hotfix for BobberBarCtorPatcher.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.7

### Added

* Fisher profession now also applies to the bonus bobber bar height granted by Deluxe Bait.

### Fixed

* Fixed an issue where farmhand slingshots would not update to the correct number of attachment slots. Added the console command `professions fix_slingshots` as a workaround for existing saves with this issue.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.6

### Changed

* Profession tooltips in the Skills Page now display and properly wrap the entire tooltip text, instead of truncating at 90 characters.

### Fixed

* Fixed a possible Out-Of-Range exception in SkillsPageCtorPatcher
* Added a failsafe that should prevent errors with Luck Skill. But note that Luck Skill is **not** officially supported, and will not be.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.5

### Changed

* Updated Chinese translations.

### Fixed

* Fixed issue preventing location interactions introduced in 0.2.3.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.4

### Fixed

* Fixed possible Null-Ref exception in MineShaftCheckStoneForItemsPatcher.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.3 

### Fixed

* Fixed max-sized fish not being counted correctly.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.2

### Added

* Added Chinese, French and Korean translations. Credits added to [README](../Professions).

### Changed

* Additional slingshot ammo slot now draws horizontally in the slingshot's tooltip instead of vertically, matching the style of the Advanced Iridium Rod and saving some vertical space.
* "Memorized" fishing rod tackle now draw correctly in the rod's tooltip.

### Fixed

* Fixed various issues with custom skills.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.1

### Fixed

* Fixed possible crash when selecting Prestiged Harvester profession.
* Fixed possible Out-Of-Range exception in FarmerCurrentToolSetterPatcher.
* Fixed possible Null-Ref exception in MonsterFindPlayerPatcher.

### Changed

* Changed Sewer Statue logic to be more compatible with different configurations of Skill Reset / Prestige / Limit Break.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.0 - Beta release for 1.6

* No changes.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.1.1

### Added

* Added Mastery Limit Select menu when mastering the Combat skill.

### Fixed

* Fixed player not gaining experience.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.1.0 - Alpha release for 1.6

### Changed

* Prestige levels no longer require having all professions in the skill. It is now a reward for Mastering each individual skill. The Statue of Prestige has been renamed to Statue of Transcendence to avoid confusion. 
* Skill reset feature is unchanged. But note that choosing to Master a skill will prevent it from being reset further, effectively locking you out of any unobtained profession. *This may change in the future, but for now you have been warned.*
* Similarly, the Limit Break is now a reward for Mastering the Combat skill.
* All prestiged profession variants, and a few base variants, have been reworked. Prestige professions are now much more impactful end-game rewards. Please review the section [Professions](README.md#professions) of the README to learn more.
* Treasure Hunts are now triggered on Time Change instead of Player Warped. The chance to start a Treasure Hunt now depends on how far you have traveled or how many rocks you have broken since the previous hunt.
* Misc. code improvements.

### Removed

* Removed custom support for Luck skill and Love of Cooking.
* Removed alost all third-party mod integrations since I don't know which of them were/will be updated. I might re-add Automate integration later, or I might not.


[🔼 Back to top](#professions-changelog)

[View the 1.5 Changelog](resources/CHANGELOG_old.md)
