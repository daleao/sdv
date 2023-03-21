<div align="center">

# Modular Overhaul :: Taxes

</div>

## Overview

### Income Tax

This mod implements a "simple" (*citation needed*) taxation system to the game, because surely a nation at war would be on top of that juicy end-game income.

The Ferngill Revenue Service (FRS) will calculate your due Federal obligations based on your Shipping Bin income. This adds a new gameplay element and a reason to prefer doing business locally, and also affords the player a choice to become a tax evader.

The FRS defines seven tax brackets for the individual taxpayer, modeled after the United States Of America (if you are curious, google what they are). By default, the highest bracket for income tax is 37% (the latter value is configurable, although the number of brackets is not).

Federal obligations are due on the first of the month, and will be deducted automatically from the farmer's balance overnight (i.e., on the morning of the day 2). This means that the farmer has one day to reinvest the closing season's profits into new seeds for the current season, before ~~being robbed by~~ making their contribution to the Federal Government. Reinvestments (e.g., tool upgrades, animal or seed purchases, building comissions, etc.) may count as business expenses, which are deductible up to 100% under the Ferngill Revenue Code. However, if the farmer does not have enough funds remaining at the end of the day to cover their due obligations, the FRS will seize the farmer's Shipping Bin income until the outstanding amount is settled, while also charging a lateness fine + daily interest (configurable, default 6% **per annum**).

If the [Professions](../Professions) module is enabled and the player has the Conservationist profession, the professions' tax deduction perk will change from a % value increase to all items, to a more immersive % deduction of taxable income. Environmentalist activities can be used to deduct taxable income up to 100%. This means that farmers can be tax-exempt by collecting enough trash from oceans or rivers (it is recommended to decrease the default Professions setting for TrashNeededPerTaxBonusPct, as the TaxBonusCeiling setting will be overridden to 100%).

### (NEW) Property Tax

In addition to federal obligations, the farmer is also obliged to contribute Property Tax to their local government.

The total value of your property will be appraised twice every season based on a Use-Value Assessment (UVA) program, which basically means that farmers are more liable for unproductive land; in other words, the taxation rate applied to land which is actively used for agriculture, livestock or forestry is generally **less** than that applied to land which is not actively used.

During appraisal, the total value of the farm's agriculture activities, livestock and real-estate will be weighted. At the start of each year, Mayor Lewis will collect due property taxes from the **host** farmer, based on the average UVA value of their property throughout the previous year.

Farming activities in Ginger Island and other properties will not be charged.

Property taxes are not eligible for deductions.

Lateness fines are generally higher for property taxes, but interest rates are the same, as all farmer debts are purchased by the same bank (canonically the Bank of Stardew).

## Compatibility

Should be compatible with anything (including [Ferngill Revenue Service](https://www.nexusmods.com/stardewvalley/mods/7566, but please don't try using both together).
