# PROFESSIONS Changelog

## 1.0.1

### Fixed

* Fixed a small display issue in the Skill Page menu which caused all level 10 professions to show as "Desperado".

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