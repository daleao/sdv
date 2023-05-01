# TOLS Change Log

## 2.2.3 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* Revised enable condition for ButtonPressedEvent, which should fix issues with FaceMouseCursor, SlickMoves and AutoSelection working if any is disabled.

## 2.0.0 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Changed

* The stamina cost of the charged Axe and Pickaxe can now be configured separately.

### Fixed

* Added missing base stamina cost mulitpliers for each tool to GMCM.

## 1.3.3 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* Fixed object harvesting not limited to forage (woops).
* Fixed out of bounds experience gain.

### Remved

* Removed `HarvestSpringOnions` option. This is now considered forage.

## 1.3.2 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Added

* Added the ability to harvest forage with scythe.

### Changed

* Harvest with scythe functionality will no-longer apply while Yet Another Harvest With Scythe mod is installed.

## 1.3.1 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Added

* By request, added the option to limit crop harvesting to Golden Scythe.

### Fixed

* Fixed scythe tooltip patcher not applying due to bad namespace.

## 1.3.0 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

## Added

* Added crop harvesting with Scythe.

## 1.2.3 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

## Added

* Added auto-select compatibility for Dr. Birb's Upgradeable Ranching Tools and Upgradeable Pan.
* Added the ability to customize the auto-selection border color.

## Changed

* Auto-selectable cache now uses Dictionary instead of Hash Set for much better performance.

## Fixed

* Fixed a possible memory leak in tool auto-selection logic.

## 1.2.0 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Added

* Added tool auto-selection.

## 1.0.4 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* The AllowMasterEnchantment config should now work correctly.

## 1.0.1 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Changed

* Affected tile settings for Hoe and Watering Can now use named tuple array instead of jagged array. This is more efficient and more legible.

### Fixed

* Added a failsafe for an Index Out Of Range exception that may occur with Moon Misadventures installed.

## 0.9.9 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* No longer changes the stats of scythes (which means they no longer need to be revalidated).

## 0.9.7 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* Fixed a bug causing player Stamina to get stuck at 1 and not continue below 0.

## 0.9.4 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* Fixed a bug preventing weapons from destroying bushes and other location objects.
* Fixed a bug with Scythe ClearTreeSaplings setting.
* Scythe can now receive the Haymaker enchantment as intended.

## 0.9.3 <sup><sub><sup>[🔼](#tols-change-log)</sup></sub></sup>

### Fixed

* Control settings now apply only to weapons, as they should.

## 0.9.0 (Initial release)

### Added

* Added Scythe settings.
* Added stamina multiplier setting to each tool.
* Added Face Mouse Cursor setting to match Arsenal.

[🔼 Back to top](#tols-change-log)