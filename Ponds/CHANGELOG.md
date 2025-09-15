# PONDS Changelog

## 2.2.3

### Fixed

* Hotfix for not being able to place any fish in ponds.

**Note:** I've just realized that More New Fish have "fish_pond_ignore" tag on every legendary fish. This will prevent them from being placed in ponds, regardless of this mod. If using More New Fish, I recommend opening its `Data/Objects.json` file, do a search and replace for `,"fish_pond_ignore"`(yes, with the comma), and replace all occurrences with empty strings. 

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.2.2

### Fixed

* Mod now correctly checks pond reward conditions.
* Mod now correctly checks `RandomItemId` when `ItemId` is null, fixing a possible NRE with certain mod-added `FishPondReward`s (e.g., VPP).

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.2.1

### Added

* Added a bonus to the roe production chance of legendary fish.
* Max population is now automatically increased to 2 when trying to place Tui in La pond or vice versa (no longer requires [Walk of Life](../Professions).

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.2.0

### Added

* Mixing of legendary and extended family fishes in a pond has been moved to this mod and removed from [Walk of Life](../Professions). The code for tracking legendary fish has been simplified.
* Added optional Legendary Fish Pond data.

### Fixed

* Tui and La can now be placed in the same pond.
* Fixed Squids producing roe instead of ink.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.1.2

### Fixed

* Adjusted the pond inventory logic to prevent empty slots when opening up the chum bucket.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.1.1

### Fixed

* Fixed a possible Null-Ref Exception.

<sup><sup>[🔼 Back to top](#ponds-changelog)</sup></sup>

## 2.1.0

### Added

* Added compatibility for Automate.

### Changed

* Tweaked roe production chance formula.
  * The original formula was made to result in roughly equal profitability for all fishes. But that resulted in nothing produced for weeks at a time. In short, it wasn't fun.
  * The new formula still pulls back on higher-value fish, but now yields significantly more roe, making them actually worth raising.
* Chance to produce general non-roe items received a small buff, allowing it, too, to scale with population.

### Fixed

* Added support for Fish Ponds built outside the Farm, which should have been done during the migration to 1.6. 

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
