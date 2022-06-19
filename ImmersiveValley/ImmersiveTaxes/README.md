<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

# Serfdom - Immersive Taxes

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>

## Features

This mod implements a simple taxation system to the game, because surely a nation at war would be on top of that juicy end-game income.

The Ferngill Revenue Service (FRS) will calculate your due Federal obligations based on your Shipping Bin income. This adds a new gameplay element and a reason to prefer doing business locally, and also affords the player a choice to become a tax evader.

The FRS defines seven tax brackets for the individual taxpayer, modeled after the United States Of America (if you are curious, google what they are). By default, the highest bracket for income tax is 37% (the latter value is configurable, although the number of brackets is not).

Federal obligations will be deducted automatically from the farmer's balance. However, the FRS is not unsympathetic to the local businessman, and so Federal obligations will not be charged until the 2nd of every season. This means the farmer has one day to reinvest the closing season's profits into new seeds for the current season, before ~~being robbed by~~ making their contribution to the Federal Government. If the farmer does not have enough funds at the end of the day to cover their due obligations, the FRS will seize the farmer's Shipping Bin income until the outstanding amount is settled, while also charging interest at a configurable rate (default 6% per annum).

If [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111) is installed and the player has the Conservationist profession, the professions' tax deduction perk will be changed, from a % value increase for all items, to a more immersive % deduction of taxable income. Environmentalist activities can be used to deduct taxable income up to 100%. This means that farmers can be tax-exempt by collect enough trash from oceans or rivers (it is recommended to decrease the default Walk Of Life setting for TrashNeededPerTaxBonusPct, as the TaxBonusCeiling setting will be overridden to 100%).

## Compatibility

This is a companion mod for [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111), but can be used independently. Should be compatible with anything (including [Ferngill Revenue Service](https://www.nexusmods.com/stardewvalley/mods/7566, but please don't try using both together). Multiplayer compatibility is untested; please report any issues. Not compatible with Android.

## Installation

Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.

To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.

To uninstall simply delete the mod from your mods folder. This mod is safe to uninstall at any point.

## Special Thanks

- **ConcernedApe** for StardewValley.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>

## License

See [LICENSE](../../LICENSE) for more information.
