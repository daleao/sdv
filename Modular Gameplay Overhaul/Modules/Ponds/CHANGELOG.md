# Ponds Module Change Log

## 1.0.2

### Changed

* Decreased the volume of metal bars thrown into radioactive ponds from 5 to 4 and doubled the capacity to 40 volume units. Each radioactive pond can now hold 40 ores, 10 bars, or a combination of both, for enrichment.

### Fixed

* Fixed *again* positioning of fishes in Pond Query Menu.

## 1.0.0

### Added

* Added DaysUntilAlgaeSpawn setting to the GMCM menu.
* Added RoeAlwaysSameQualityAsFish setting.

### Fixed

* Fixed an issue which caused invalid mod data leftover after changing a pond's fish type.
* Adjustments to the positioning of fish in Pond Query Menu.

## 0.9.6

### Fixed

* "Fixed" (but not really) an error thrown during Fish Pond production logic. This is actually caused by vanilla's `Utility.consolidateStacks` method, which deletes non-colored Roe from the produce list for some reason. Because this just slightly nerfs Fish Pond production, and they're slightly too strong, I'm choosing to embrace the bug and just hide the error.

## 0.9.2

### Changed

* The quality of produced roe is now less than or equal to the quality of the fish which produced it, instead of always being equal to it.

## 0.9.0

### Added

* The [Aquarist](../Professions) profession now adds a chance to boost the quality of newborn fishlings, so you may eventually breed high-quality fish from low-quality ones.

### Fixed

* Fixed a bug where the counter for spawning Algae in an empty Fish Pond would reset when reloading the save.
* Fixed a typo preventing TFO integration from applying, which in turn caused a conflict with it installed.