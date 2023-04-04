# Rings Module Change Log

## 2.0.5

### Fixed

* Non-colorful ring glow setting should now work correctly.

## 2.0.4

### Fixed

* Fixed small Glow and Magnet ring recipes incorrectly creating their regular versions.

## 2.0.0

### Added

* Resonance text is now color-coded.
* Added crafting recipes for small Glow and Magnet rings.
* Added config option to remove the color from Infinity Band glow.

### Changed

* New chord harmonization algorithm now considers all note interactions, instead of only distinct notes. Some gemstone combinations will suffer a small rebalance due to this change.
* The richness of Tryads and Tetrads now yields a boost to the root note resonance (as mentioned in the description). This is now a much more appealing choice.
* "Craftable Glow and Magnet" setting, and "Immersive Glowstone" setting, as well as new small ring recipes, have been grouped under a single config setting: "Better Glowstone Progression".
* Level required to craft Glow and Magnet rings increased from 2 to 4, and for Glowstone Ring from 4 to 6.

### Fixed

* Fixed the Warrior Energy buff, which previously wasn't implemented at all.

## 1.2.3
    
### Fixed

* Fixed an issue with the overhuled Warrior Ring buff never ending.

## 1.2.2

### Added

* Added resonance type to combined Infinity Band tooltip.

### Fixed

* Fixed a bug in Chord harmonization logic.

## 1.1.1

### Fixed

* Added missing config checks to Better Crafting integration.

## 1.0.2

### Fixed

* Apparently there was still a possible Null-Reference Exception in SpaceCore's NewForgeMenu.

## 1.0.0

### Fixed

* Fixed an issue with the display of topaz bonuses in Infinity Band tooltip.
* Fixed an issue when unforging Infinity Band.
* Fixed an issue when rendering an unforged Infinity Band.

## 0.9.7

### Changed

* The ternary chord now gives higher amplitude than a double power-chord.

### Fixed

* Fixed a bug preventing the MonsterSlay event of various rings like Napalm from triggering.

## 0.9.6

### Fixed

* Fixed a display bug with Infinity Band gemstones.

## 0.9.4

### Fixed

* Added one last null-check that was missing.
* Fixed Better Rings and Vanilla Tweaks integrations not being loaded.

## 0.9.2

### Fixed

* Fixed a null-reference exception when unforging rings.
* Added even more robust null-checking for custom JA items to avoid issues.

## 0.9.0 (Initial release)

### Added

* Added the Infinity Band. The Iridium Band must now be converted to Infinity before accepting gemstones.
* Weapon and Slingshots can now resonate with Infinity Bands if the Arsenal module is enabled.
* Added compatibility with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).
* Added compatibility with [Vanilla Tweaks](https://www.nexusmods.com/stardewvalley/mods/10852).

### Changed

* Complete overhaul of the Resonance feature, now inspired by real Music Theory. This is now a major gameplay mechanic instead of a small flavor feature, and will hopefully add versatility to the new Infinity Band and encourage variability. See the [module description](README.md) for details.
