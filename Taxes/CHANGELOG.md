# TAXES Changelog

## 2.2.1

### Fixed

* Fixed error due to change in Walk Of Life API.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 2.2.0

### Fixed

* FRS no longer sends a notice when the player doesn't owe any taxes.
* Fixed incorrect season displayed on tax reports.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 2.1.2

### Fixed

* Some more translation fixes/addendums.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 2.a1.1

### Fixed

* Fixed FRS mail to reflect changes made in [2.1.0](#210).
* Fixed some translations.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 2.1.0

### Added

* Added a baseline Unused Tile Cost (configurable).
* Added Artisan Value estimation. Property tax is now calculated based on a weighted average of raw and artisan costs, where weights are determined by the number of placed artisan machines in the world; the more machine you have placed, the higher your Farm's valuation.

### Changed

* Default building tax rate increased from 4% to 10%.
* Trees are now considered ecological exemption, and are discounted from available usable tiles.

### Fixed

* Fixed Fruit Trees incorrectly causing negative agricultural value.
* Fixed players not receiving intro mails if mod is installed mid-year.
* Fixed some translations.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 2.0.0

Updated for game version 1.6.14.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 1.1.0

### Added

* Can now choose the days on which income and property taxes are debited. By default, income taxes are set to be debited on the (night of the) 5th, and property taxes on the (night of the) 20th (note that property taxes are only charged in Spring).

### Changed

* Taxation notice letters will now always be received on the 1st of the season, and will warn of the impending charges for the current month. Previously these letters were only sent after being charged, which was not at all useful.
    * This does mean that most letters had to be edited. Translators will need to update their translations.

### Fixed

* Taxes can no longer go to negative when lower than business expenses.
* Inadvertedly fixed some other bugs during the changes above.

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 1.0.1

### Added

* Added French translations by [CaranudLapin](https://github.com/CaranudLapin).
* Added Chinese translations by [Awassakura](https://next.nexusmods.com/profile/Awassakura/about-me?gameId=1303).
* Added Korean translation by [whdms2008](https://next.nexusmods.com/profile/whdms2008/about-me?gameId=1303).

<sup><sup>[🔼 Back to top](#taxes-changelog)</sup></sup>

## 1.0.0 - Initial 1.6 release

### Fixed

* Fixed tile counting logic used for Property Taxes, which before used pixel dimensions instead of number of tiles, leading to astronomically incorrect property tax quotes.


[🔼 Back to top](#taxes-changelog)

[View the 1.5 Changelog](resources/CHANGELOG_old.md)
