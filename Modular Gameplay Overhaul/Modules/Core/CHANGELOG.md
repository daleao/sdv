# Modular Overhaul Core Change Log

## 1.0.2

* Added Chinese translations by [xuzhi1977](https://www.nexusmods.com/users/136644498).
* Added Korean translations by [BrightEast99](https://www.nexusmods.com/users/158443518).
* Added Spanish and French translations.
* Updated Portuguese translation.

## 1.0.0

### Changed

* Rebranded as MARGO.

### Fixed

* Fixed a possible memory leak in the shared Event Manager logic.

### Removed

* Removed Generic Mod Config Menu as a hard requirement.

## 0.9.7

### Changed

* Mod integrations now use the Singleton pattern.

### Added

* Added FirstSecondUpdateTickedEvent.

## 0.9.5

### Added

* Added Revert command, complementary to the Initialize command from Arsenal. This will undo the changes made by Arsenal to resolve possible issues after disabling the module.

## 0.9.4

### Added

* Updated portuguese translations by [Onemix](https://www.nexusmods.com/stardewvalley/users/39429640)

### Changed

* Reverted some Virtual Properties back to PerScreen Mod State where more appropriate.

### Fixed

* Fixed SecondsOutOfCombat not reseting on damaging monsters (incorrect parameter name in Harmony Postfix).

## 0.9.3

### Changed

* Renamed the mod folder to be less vague.

### Added

* Added asset invalidation when toggling a module.

## 0.9.0 (Initial release)

* Initial Version