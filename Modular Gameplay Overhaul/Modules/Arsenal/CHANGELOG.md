# Arsenal Change Log

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