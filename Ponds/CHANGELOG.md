# PONDS Changelog

## 2.1.0

### Added

* Added compatibility for Automate.

### Changed

* Tweaked roe production chance formula.
  * The original formula was made to result in roughly equal profitability for all fishes. But that resulted in nothing produced for weeks at a time.
  * The new formula is still conservative for higher-value fish, but yields enough roe to make them worth raising.
* Chance to produce general non-roe items received a small buff to scale with population.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.0.1

### Fixed

* Fixed a possible Null-Reference Exception in Fish Pond production logic.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.0.0

Updated for game version 1.6.14.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.6

### Changed

* Chum bucket capacity reduced to 12 down from 36. This is to prevent players from leaving their ponds unattended for too long. If the number of distinct produced items exceeds this capacity, always the lowest-valued item will be discarded.
* Golden Animal Cracker now doubles every produced item, instead of just the most valuable one.

### Fixed

* Fixed Golden Animal Cracker doubling the output item cumulatively every day when uncollected.
* Other bug fixes.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.5

### Fixed

* Don't remember. But I fixed something.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.4

### Fixed

* Fixed a parsing error related to metal enrichment.
* Fixed cleared pond fishes not having quality.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.3

### Changed

* Some changes to PondQueryMenuDraw.

### Removed

* Removed Angler + Ms. Angler mating. Added to Professions mod.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.2

### Added

* Added French translations by [CaranudLapin](https://github.com/CaranudLapin).
* Added Chinese translations by [Awassakura](https://next.nexusmods.com/profile/Awassakura/about-me?gameId=1303).
* Added Korean translation by [whdms2008](https://next.nexusmods.com/profile/whdms2008/about-me?gameId=1303).

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.1

### Fixed

* Fixed some possible Null-Ref exceptions if pond is empty.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 1.0.0 - Initial 1.6 release

### Changed

* Individual data keys have been consolidated into a single, more robust data model (no change from user perspective). 


[🔼 Back to top](#ponds-changelog)

[View the 1.5 Changelog](resources/CHANGELOG_old.md)
