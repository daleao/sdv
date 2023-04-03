# Weapons Change Log

## 2.0.0

* Initial release of Weapons module.

### Changed

* Reduced the minimum mine level required for new weapons to appear at Marlon's shop. The first batch of weapons will now be available instantly. The second will appear as soon as level 15. Some prices have also been lowered.
* Mine chests no longer drop seeds. Can now drop fertilizers. Increased the stack of several chest rewards. Improved the odds of hitting the jackpot (Quality Sprinklers).
* Increased the spawn chance of breakable containers in the mines. Implemented a handicap system to dynamically increase the chance of obtaining special items from breakable containers (including weapons).
* Implemented a handicap system to dynamically increase the chance of encountering monsters with special item drops. Weapons dropped from monsters are still rarer and stronger on average than those dropped by containers.
* The base knocback of swords and daggers has been cut in half.
* Some weapons' stats have been changed, including the Galaxy and Infinity weapons which received significant buffs.
* Weapon revalidation now occurs automatically upon first initialization of the module. Manually executing the `revalidate` command is no longer necessary. However, you **must** manually disable Arsenal module before uninstalling MARGO.

### Fixed

* The Dawn Blade will no longer be dismantled as long as it has forges to be removed.
* Generosity Trial now only checks for completion on the Master player, to avoid issues in multiplayer games.
* The description of the Jinxed debuff should now properly reflect the changes made by the Overhauled Defense setting.
* Fixed some typos causing Aquamarine, Jade and Emerald-resonances to not apply correctly to their correspondingly-forged weapons.
* Generosity Trial completion should now register on all farmhands.
* I'm not sure why, but Slick Moves now also works in the direction of movement.

---

*Changes below this point refer to the legacy Arsenal module.*

## 1.3.5

### Fixed

* Fixed a bad Null-Reference Exception introduced in 1.3.4.

## 1.3.4

### Fixed

* Fixed missing scythe swipe sound.

## 1.3.2

### Fixed

* Enemies defeated by knockback damage now properly count for game stats, quests and drop items. 

## 1.3.1

### Fixed

* Fixed start-up error with Neptune's Glaive patcher.

## 1.3.0

### Changed

* Drop chance of Lava Katana has been doubled.
* Drop chance of Neptune's Glaive has been halved.
* Added apostrophe to "Neptune's Glaive", to make it sound more unique.

### Fixed

* Fixed a bug preventing explosive ammo from exploding without the Desperado profession.
* Fixed a bug with Obsidian Edge and Lava Katana drops creating error daggers instead.

## 1.2.3

### Added

* Added some emote bubbles to spice up Blade of the Ruined Hero and Clint's Forge events.
* Added the ability to customize the auto-selection border color.

## Changed

* Tribute enchantment threshold now inreases with each kill, and resets on taking damage.

### Fixed

* Blade of the Ruined Hero introduction event is no longer skippable.

## 1.2.1

### Fixed

* Fixed auto-selection preventing other LeftShift actions in inventory menu.
* Fixed inverted swipe effect when swinging scythe.

## 1.2.0

### Added

* Added arsenal auto-selection.
* You can now specify individual weapon with the console command for getting blueprints (`ars get bp <weapon1> <weapon2> etc...`).

### Fixed

* Fixed a bug in the console command for getting blueprints (`ars get bp`).

## 1.1.0

### Fixed

* Fixed out-of-bounds tooltip elements for slingshots higher than basic.
* Fixed bullseye position while aiming.

## 1.0.4

### Added

* Added SwipeHold config. You can now hold the tool button to perform a continuous combo instead of spam-clicking.
* Added CustomStabbingSwords config. Users can now register swords from their modlist to be treated as Stabbing swords. **Do not enter vanilla swords here, or non-sword weapons.**
* Lava Katana swing effect is now orange-ish.

### Fixed

* Fixed shipping bin logic, which was also messed up by the Dark Sword.

## 1.0.3-Hotfix

Hotfix for Null-Reference Exception when opening Clint's shop menu.

## 1.0.2

### Added

* Defense overhaul setting now also reduces Jinxed debuff from -8 to -5 defense.

### Fixed

* Fixed the overlaping ammo stacks displaying on slingshots with double ammo.
* Fixed the shop selling logic which was messed up in 1.0.1.
* Fixed the tooltip of weapons enchanted with Diamond.
* Dark Sword should now be immune from accidental deposit by Better Chests' stack feature.
* Clint's menu should now support mod geodes like Kaya's.

## 1.0.1

### Added

* Added an option to change the display style of weapon tooltips between relative and absolute values.
* Added colorful swipe effect for Blade of Ruin and Blade of Dawn, as well as Infinity Gavel smash hit.
* Legendary weapons can no longer be sold.
* Enchanted weapons now sell for a premium (+1000g).

### Changed

* Minimal edits to Blade of Dawn description.

### Fixed

* Galaxy and Infinity weapons, Blade of Ruin and Blade of Dawn are now marked as Special and cannot be trashed.
* Fixed a bug with Garnet resonance.
* Improved the response of FaceMouseCursor feature in-between combo hits.
* Fixed the direction of the swipe effect on backhand sword swipes.
* No longer conflicts with Love Of Cooking's Frying Pan upgrades or drbirbdev's Panning and Ranching Tool Upgrades.
* No longer prevents players from receiving the Galaxy Sword mail flag.

## 1.0.0

### Added

* Added flavor text for each virtue at the Yoba altar.

### Changed

* Removed Gil requirement for advancing the BOTRK quest-line. Now, simply exhausting the Yoba altar's dialogue options will advance the quest.
* Improved the textures for Blade of Ruin and Blade of Dawn.

### Fixed

* Garnet Enchantment can now be applied to weapons correctly.
* Garnet Enchantment's effect will now apply correctly.
* Fixed an issue where holding anything besides a weapon or slingshot would cause the player to take 0 damage.
* Fixed a possible Null-Reference Exception when receiving a Galaxy weapon.
* Infinity weapons now have 4 forge slots as advertised under the rebalance.
* Fixed a bug with Ruby Enchantment applying only a single point of damage.
* Weapon and Slingshot Forges now unapply correctly.
* Hoe is not longer classified as Rare (color-coded Blue).

## 0.9.9

### Added

* Ruined Blade quest-line is now fully implemented.
* Added option to draw bulls-eye instead of mouse cursor while firing a slingshot.

### Changed

* Few translations changed.
* Reduced the aura on the Blade of Ruin texture.

### Fixed

* Small fix for Slick Moves to prevent the player from sliding during combo cooldown.
* Scythes can no longer perform the stabby special move.
* The item hold up animation now plays correctly when obtaining the Blade of Ruin.
* The Blade of Ruin is now a Stabbing Sword, as it should.
* No longer triggers stabbing special move when obtaining the Blade of Dawn.
* Fixed possible bug with Bloodthirsty enchantment.
* Fixed some incorrect translation keys.

## 0.9.8

### Added

* The natural damage and knockback modifiers of advanced slingshots now display in the tooltip.

### Changed

* Improvements to Slick Moves. Can now drift in the parallel direction (i.e., when swinging directly in front or behind you). Before, drifting only worked for perpendicular directions (i.e., when swinging to the sides). Also increased the drift velocity slightly. Finally, the drift now comes to a halt at the end of the animation, removing the unimmersive impression of buttery floor.
* Clint's follow-up quest now begins correctly.

## 0.9.7

### Changed

* SVE's Treasure Cave now rewards an Obsidian Edge instead of Galaxy Slingshot.
* Clint's forging quest is no longer random, and now takes less time the higher your friendship with him.
* Changed the knockback of some weapons:
    * Wooden Blade: +0.25
    * Rapier: -0.05
    * Steel Falchion: -0.05
    * Katana: -0.2
* The Dark Sword's curse has been changed. No longer affects stamina consumption. Now, as the curse strengthens the sword will gain a progressively higher chance to auto-equip itself, preventing you from using other weapons.
* Community Center is no longer a requirement for obtaining the Dark Sword.

### Fixed

* Fixed an issue which caused critical hits to deal absurdly low damage.
* Fixed an error that could be thrown when playing the hold-up-item animation.
* Clint's forging quest was still bugged. Now tested.
* Fixed a Null-Reference Exception when attempting to grab the Dwarvish Blueprint from a Scavenger Hunt chest.
* Fixed a Null-Reference Exception in pirate treasure menu when a Neptune's Glaive is produced.
* Fixed missing patch targets for allowing crits to ignore monster defense.
* Added config checks to logic that was missing it.

## 0.9.6

### Fixed

* The event which counts the days left until Clint finishes translating the blueprints should now trigger correctly.
* Fixed a compatibility issue with any modded recipes containing Dragon Tooth.
* Fixed a null-reference exception when farmer takes damage from bombs and maybe other sources too.

## 0.9.5

### Added

* Added Initialize command to apply all necessary configurations to weapons and farmers on existing saves.

### Fixed

* Club second combo hit now actually does damage.

### Removed

* Removed automatic initialization script from SaveLoaded event. This was inefficient and unreliable. Replaced with manual console command.

## 0.9.4

### Added

* If SVE is installed and Infinity +1 feature is enabled, Tempered Galaxy weapons will now be automatically removed from Alesia and Isaac's shops, so users no longer have to manually remove them from SVE's shop.json file. 

### Changed

* Slingshot special move cooldown is now a property of Farmer and not Slingshot, as it should be.
* Default values for difficulty sliders are now set to 1.
* The base damage of some starter weapons has been slightly increased.

### Fixed

* Fixed Slick Moves applying to Scythe.
* Fixed Brute prestige perk not applying with Arsenal module enabled.
* Fixed Galaxy Slingshot being instantiated as Melee Weapon.
* Fixed special item hold up message for first Galaxy weapon not being displayed.
* Fixed Mine chests changes not being applied as intended with the Weapon Rebalance setting.
* Players on existing save files that have already obtained the Galaxy Sword should now be able to obtain the remaining Galaxy weapons.
* Fixed a bug in the monster stat randomization logic, which was generating monsters with current HP higher than max HP.

## 0.9.3

### Fixed

* Fixed null-reference exception when opening Marlon's shop (forgot to pass `__result` by `ref`).
* Control settings now apply only to weapons, as they should.

## 0.9.2

### Changed

* Weapon tooltips now revert to vanilla when `RebalancedStats` option is disabled.

### Fixed

* Added even more robust null-checking for custom JA items to avoid issues.
* Fixed SlickMoves config setting in GMCM which was incorrectly mapped to FaceMouseCursor.

### Fixed

* The category of Dwarven Blueprint has been changed from Artifact to Junk. This avoids the error caused by the game attempting to spawn a blueprint when the player digs an artifact spot.
* Fixed player's facing direction changing during active menu.

## 0.9.0 (Initial release)

* Initial Version