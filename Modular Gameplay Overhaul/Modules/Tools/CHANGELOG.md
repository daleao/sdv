# Tools Module Change Log

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