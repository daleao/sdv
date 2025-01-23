# PROFESSIONS Changelog

## 1.2.1

### Fixed

* Prestiged Angler fishing chain now resets upon leaving the map.

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
      * Gold Slime variant can be bred by aiming for RGB(255, 215, 0), also known as HTML Gold (i.e. try to Red and Green Slimes).
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
