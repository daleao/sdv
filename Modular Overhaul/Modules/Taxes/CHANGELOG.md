# TXS Change Log

## 2.5.5 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Changed

* You can now choose what percentage of each business expense is tax-deductible.
  YOU MUST EITHER DELETE YOUR CONFIGS, OR MANUALLY CHANGE THE FOLLOWING VALUES FROM "true/false" to "1.0/0.0":
  * DeductibleAnimalExpenses
  * DeductibleBuildingExpenses
  * DeductibleSeedExpenses
  * DeductibleToolExpenses
* Now actually uses ConservationistTaxBonusCeiling setting from PROFS, instead of setting it to 100%.

## 2.5.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixes

* Fixed an issue caused by ephemeral caching of overnight tax calculation between 1st and 2nd day of the season. If the player reset the game during the 2nd day of the season, calculations would be lost. These calculations are now persisted in mod data to prevent this issue.

## 2.4.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixes

* Fixed translation keys on every language.

## 2.3.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Added

* Added income and property tax information to the API.

## 2.2.6 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Agriculture and UVA totals are no longer weighed down by the winter season (as advertised).

## 2.2.3 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Now counts stacks of non-SObject items.

## 2.2.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Some correction to SetModData command.
* Fixed long-standing issues with debt calculation. Looks like daily income was being counted twice, leading to lower debts than expected.

## 2.1.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Fixed a typo in the default i18n key for outstanding debt.

## 2.0.5 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Fixed agriculture value calculation, so no longer should properties be charged billions.

## 2.0.3 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Fixed

* Fixed a typo in the config verification.

## 2.0.0 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Added

* Added new property taxes.
* The bodies of most, if not all, tax-related letters have been altered. Translations will need to be revised (sorry, translatrs).

### Changed

* You can now customize all tax brackets and tax rates for each bracket, including changing the number of brackets.

### Fixed

* Improved taxation in multiplayer when using shared wallets (thanks to [ncarigon](https://github.com/ncarigon)).

## 1.0.2 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Added

* In addition to interest, outstanding debt now suffers additional penalties at each season close. The penalty amount is the lesser of 100g or 5% of the outstanding amount. 

### Changed

* The default value for annual interest has been increased from 6% to 11% (the base corporate tax interest rate in the state of New York).

### Fixed

* Fixed the taxable amount displayed in the logs (should have no effect on gameplay).
* Taxes now apply to items sold via Mini-Shipping Bin.

## 0.9.7 <sup><sub><sup>[🔼](#txs-change-log)</sup></sub></sup>

### Added

* Added season name to console outputs.
* Added SetExpenses command.

### Fixed

* Business expenses will now correctly reset at the close of the season.
* Prevent business expenses from being greater than due taxes.

## 0.9.0 (Initial release)

### Added

* Added the option to deduct business expenses from taxable income. Business expenses can include building constructions, tool-related costs, animal-related costs and seed purchases. You may also specify individual objects by name that should be tax-deductible.
* Added new console commands for directly setting season income and outstanding debt.

### Changed

* Tax brackets now work progressively, as in real life; i.e., instead of applying only the highest applicable bracket, income is actually split into each applicable bracket. The result is less taxes paid overall.

### Fixed

* Fixed an issue where debited amount would not actually reduce the player's money when they didn't have any before sleeping. The debit now happens when the day starts, *after* the previous day's earnings have been added to the player's balance.
* Fixed a typo in introduction letter (english).
* Fixed a translation objects not being converted to strings.

[🔼 Back to top](#txs-change-log)