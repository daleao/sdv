# Tools Module Change Log

## 1.3.1

### Added

* By request, added the option to limit crop harvesting to Golden Scythe.

### Fixed

* Fixed scythe tooltip patcher not applying due to bad namespace.

## 1.3.0

## Added

* Added crop harvesting with Scythe.

## 1.2.3

## Added

* Added auto-select compatibility for Dr. Birb's Upgradeable Ranching Tools and Upgradeable Pan.
* Added the ability to customize the auto-selection border color.

## Changed

* Auto-selectable cache now uses Dictionary instead of Hash Set for much better performance.

## Fixed

* Fixed a possible memory leak in tool auto-selection logic.

## 1.2.0

### Added

* Added tool auto-selection.

## 1.0.4

### Fixed

* The AllowMasterEnchantment config should now work correctly.

## 1.0.1

### Changed

* Affected tile settings for Hoe and Watering Can now use named tuple array instead of jagged array. This is more efficient and more legible.

### Fixed

* Added a failsafe for an Index Out Of Range exception that may occur with Moon Misadventures installed.

## 0.9.9

### Fixed

* No longer changes the stats of scythes (which means they no longer need to be revalidated).

## 0.9.7

### Fixed

* Fixed a bug causing player Stamina to get stuck at 1 and not continue below 0.

## 0.9.4

### Fixed

* Fixed a bug preventing weapons from destroying bushes and other location objects.
* Fixed a bug with Scythe ClearTreeSaplings setting.
* Scythe can now receive the Haymaker enchantment as intended.

## 0.9.3

### Fixed

* Control settings now apply only to weapons, as they should.

## 0.9.0 (Initial release)

### Added

* Added Scythe settings.
* Added stamina multiplier setting to each tool.
* Added Face Mouse Cursor setting to match Arsenal.