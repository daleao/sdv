# TAXES Changelog

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