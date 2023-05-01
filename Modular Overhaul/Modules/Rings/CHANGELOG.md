# RNGS Change Log

## 2.2.0 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Added

* Thorns Ring can now cause Bleed. Renamed to "Ring of Thorns", because it just sounds better.
* Ring of Yoba no longer grants invincibility; now grants a shield for 50% max health when your health drops below 30%.
* Immunity Ring now grants 100% immunity, instead of vanilla 40%.

### Changed

* Warrior Ring now gains stacks on every kill (instead of 3 kills), but is capped at +20 attack.

## 2.0.7 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Added

* Added GetInfinityBand command.
* Added some resonance texture options.

### Fixed

* Fixed yellow-tint in colorless resonance glow.
* Fixed a bug where unequipping an Infinity Band could cause the resonance glow to stick around.

## 2.0.5 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Non-colorful ring glow setting should now work correctly.

## 2.0.4 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Fixed small Glow and Magnet ring recipes incorrectly creating their regular versions.

## 2.0.0 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

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

## 1.2.3 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>
    
### Fixed

* Fixed an issue with the overhuled Warrior Ring buff never ending.

## 1.2.2 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Added

* Added resonance type to combined Infinity Band tooltip.

### Fixed

* Fixed a bug in Chord harmonization logic.

## 1.1.1 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Added missing config checks to Better Crafting integration.

## 1.0.2 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Apparently there was still a possible Null-Reference Exception in SpaceCore's NewForgeMenu.

## 1.0.0 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Fixed an issue with the display of topaz bonuses in Infinity Band tooltip.
* Fixed an issue when unforging Infinity Band.
* Fixed an issue when rendering an unforged Infinity Band.

## 0.9.7 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Changed

* The ternary chord now gives higher amplitude than a double power-chord.

### Fixed

* Fixed a bug preventing the MonsterSlay event of various rings like Napalm from triggering.

## 0.9.6 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Fixed a display bug with Infinity Band gemstones.

## 0.9.4 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

### Fixed

* Added one last null-check that was missing.
* Fixed Better Rings and Vanilla Tweaks integrations not being loaded.

## 0.9.2 <sup><sub><sup>[🔼](#rngs-change-log)</sup></sub></sup>

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

[🔼 Back to top](#rngs-change-log)