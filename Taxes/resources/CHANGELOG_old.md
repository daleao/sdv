# TXS Changelog

## 4.1.6

### Fixed

* Fixed incorrect subtraction of outstanding debt (thanks [DeinKoenig](https://github.com/DeinKoenig)).

### Removed

* Removed Extra Deductibles from GMCM.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 4.1.3

### Added

* Added Chinese translation.

### Fixed

* Fixed some translation parameters.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 3.1.4

### Changed

* Default annual interest increased to 72% (was previously 12%). The original value was chosen based on common IRL values. However, IRL, years are >3× longer, and debts are payed over several years, which allows compound interest to take effect. In-game, the 12% was too low to cause any noticeable impact. So I multiplied 12 by 3, and then again by 2 to get 72%. That should give a larger incentive to clear your debts.
    * The change will not affect player's who keep their old configs. I suggest players increase this config setting to taste.

### Fixed

* Fixed debt not being collected when reloading.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.5.5

### Changed

* You can now choose what percentage of each business expense is tax-deductible.
  YOU MUST EITHER DELETE YOUR CONFIGS, OR MANUALLY CHANGE THE FOLLOWING VALUES FROM "true/false" to "1.0/0.0":
  * DeductibleAnimalExpenses
  * DeductibleBuildingExpenses
  * DeductibleSeedExpenses
  * DeductibleToolExpenses
* Now actually uses ConservationistTaxBonusCeiling setting from PRFS, instead of setting it to 100%.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.5.0

### Fixes

* Fixed an issue caused by ephemeral caching of overnight tax calculation between 1st and 2nd day of the season. If the player reset the game during the 2nd day of the season, calculations would be lost. These calculations are now persisted in mod data to prevent this issue.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.4.0

### Fixes

* Fixed translation keys on every language.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.3.0

### Added

* Added income and property tax information to the API.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.2.6

### Fixed

* Agriculture and UVA totals are no longer weighed down by the winter season (as advertised).

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.2.3

### Fixed

* Now counts stacks of non-SObject items.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.2.0

### Fixed

* Some correction to SetModData command.
* Fixed long-standing issues with debt calculation. Looks like daily income was being counted twice, leading to lower debts than expected.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.1.0

### Fixed

* Fixed a typo in the default i18n key for outstanding debt.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.0.5

### Fixed

* Fixed agriculture value calculation, so no longer should properties be charged billions.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.0.3

### Fixed

* Fixed a typo in the config verification.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 2.0.0

### Added

* Added new property taxes.
* The bodies of most, if not all, tax-related letters have been altered. Translations will need to be revised (sorry, translatrs).

### Changed

* You can now customize all tax brackets and tax rates for each bracket, including changing the number of brackets.

### Fixed

* Improved taxation in multiplayer when using shared wallets (thanks to [ncarigon](https://github.com/ncarigon)).

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 1.0.2

### Added

* In addition to interest, outstanding debt now suffers additional penalties at each season close. The penalty amount is the lesser of 100g or 5% of the outstanding amount. 

### Changed

* The default value for annual interest has been increased from 6% to 11% (the base corporate tax interest rate in the state of New York).

### Fixed

* Fixed the taxable amount displayed in the logs (should have no effect on gameplay).
* Taxes now apply to items sold via Mini-Shipping Bin.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

## 0.9.7

### Added

* Added season name to console outputs.
* Added SetExpenses command.

### Fixed

* Business expenses will now correctly reset at the close of the season.
* Prevent business expenses from being greater than due taxes.

<sup><sup>[🔼 Back to top](#txs-changelog)</sup></sup>

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

[🔼 Back to top](#txs-changelog)