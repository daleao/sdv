<div align="center">

# 💰 Serfdom — Realistic Taxes

***A "simple" taxation system for Stardew Valley: Income Tax, plus a novel Property Tax.***

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Serfdom-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/24357)
[![Nexus downloads](https://img.shields.io/badge/downloads-on%20Nexus-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/24357)
[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6-8a5a2b?logo=stardewvalley&logoColor=white)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-cc4400)](https://smapi.io/)
[![License](https://img.shields.io/badge/license-see%20LICENSE-888)](https://github.com/daleao/sdv/blob/main/LICENSE)
[![Stars](https://img.shields.io/github/stars/daleao/sdv?logo=github&style=flat&color=e0b040)](https://github.com/daleao/sdv/stargazers)

</div>

## 📖 What This Is

This mod implements a "simple" taxation system. It includes the expected Income Tax, as well as a novel Property Tax.

This mod was conceived as an add-on for [Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355)'s Conservationist profession, but can be used without it. It provides an extra layer of challenge while also enriching gameplay by establishing a strategic reason to prefer engaging in business with local vendors rather than the Shipping Bin.

## ⚙️ How It Works

### Income Tax

The Ferngill Revenue Service (FRS) will calculate your due federal obligations **based on Shipping Bin income only**, which gives you the choice of practicality vs. optimality.

The FRS defines 7 tax brackets for the individual taxpayer, modeled after the real brackets of the US of A, with 37% as the highest bracket for income tax. But these brackets and their respective thresholds are all configurable.

Federal obligations are due on the first of the month, end-of-day, and will be deducted automatically from the farmer's balance overnight (i.e., on the morning of day 2). This means that the farmer has one day to reinvest the closing season's profits into new seeds or livestock for the current season, before ~~being robbed blind~~ making their contribution to the Federal Government. Your amount owed will be informed by mail on the 1st of the season. The choice, again, is yours: whether to pay off your dues, or reinvest, possibly taking on a temporary debt, in return for greater profits at the end of the season.

Any reinvestments (e.g., tool upgrades, animal or seed purchases, building commissions, etc.) may also be deducted as business expenses, up to 100% under the Ferngill Revenue Code (and your config settings).

If used together with [Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) and the player has the Conservationist profession, the profession's tax deduction perk will change from a % value increase to all items, to a more immersive % deduction of taxable income. Environmentalist activities can be used to deduct taxable income up to 100%. This means that farmers can be tax-exempt by collecting enough trash from oceans or rivers.

> [!WARNING]
> If not enough funds are present in the farmer's account, they will be fined and seized of all Shipping Bin income until the outstanding amount is settled. All debts also accrue interest daily (configurable, default 72% **per annum**, i.e. ~0.64% per day).

> [!TIP]
> Check your upcoming income taxes at any time by running the console command `txs do income`.

### Property Tax

In addition to federal obligations, the farmer is also obliged to contribute Property Taxes to their local government.

The total value of the farmer's property will be appraised twice every season based on a Use-Value Assessment (UVA) program, which basically means that farmers are more liable for unproductive land; in other words, the taxation rate applied to land which is actively used for agriculture, livestock or forestry is generally **less** than that applied to land which is not actively used (or is filled with debris, so hurry up and clean up that farm).

During the assessment, the total value of the farm's agriculture activities, livestock and real-estate will all be weighted. At the start of each year, Mayor Lewis will collect due property taxes from the **host** farmer, based on the average UVA value of the entire property throughout the previous year, except Winter, which excludes the agriculture component for obvious reasons.

Farming activities in Ginger Island and other properties will not be charged.

Property taxes are not eligible for deductions.

Lateness fines are generally higher for property taxes (configurable). But since all debts are purchased by the same bank (canonically the Bank of Stardew), interest rates are always the same.

> [!TIP]
> Check your upcoming property taxes at any time by running the console command `txs do property`.

## 🛠️ For Mod Authors

C# mod authors may request the [Mod API](https://github.com/daleao/sdv/blob/main/Taxes/ITaxesApi.cs) to calculate the player's current taxes. This can be useful for authors who wish to add in-game methods of checking your taxes, as **I personally will not be doing that**.

---

<div align="center">

### 🌾 The DaLion.Stardew Series

[Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) &nbsp;·&nbsp; [Aquarism](https://www.nexusmods.com/stardewvalley/mods/24356) &nbsp;·&nbsp; **Serfdom** &nbsp;·&nbsp; [Springmyst](https://www.nexusmods.com/stardewvalley/mods/24832) &nbsp;·&nbsp; [Mineracoustics](https://www.nexusmods.com/stardewvalley/mods/29612) &nbsp;·&nbsp; [Chargeable Resource Tools](https://www.nexusmods.com/stardewvalley/mods/23048) &nbsp;·&nbsp; [Wildcat](https://www.nexusmods.com/stardewvalley/mods/29830)

<br>

<sub>Made with 🌱 for the Stardew Valley community.</sub>

</div>
