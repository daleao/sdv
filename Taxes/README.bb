[size=6][font=bebas_neuebook]What this is[/font][/size]

This mod implements a "simple" taxation system. Includes the expected Income Tax, as well as a novel Property Tax.

This mod was conceived as an add-on for [url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk Of Life[/url]'s Conservationist profession, but can be used without it. It provides an extra layer of challenge while also enriching gameplay by establishing a strategic reason to prefer engaging in business with local vendors rather than the Shipping Bin.


[size=6][font=bebas_neuebook]How it works[/font][/size]

[font=bebas_neuebook][size=5]Income Tax[/size][/font]

The Ferngill Revenue Service (FRS) will calculate your due federal obligations [b]based on Shipping Bin income only[/b], which gives you the choice of practicality vs. optimality.

The FRS defines 7 tax brackets for the individual taxpayer, modeled after the real brackets of the US of A, with [b]37%[/b] as the highest bracket for income tax. But these brackets and their respective thresholds are all configurable.

Federal obligations are due on the first of the month, end-of-day, and will be deducted automatically from the farmer's balance overnight (i.e., on the morning of day 2). This means that the farmer has one day to reinvest the closing season's profits into new seeds or livestock for the current season, before [s]being robbed blind[/s] making their contribution to the Federal Government. Your amount owed will be informed by mail on the 1st of the season. The choice, again, is yours, on whether to pay off your dues, or reinvest, possibly taking on a temporary debt, in return for greater profits at the end of the season.

Any re-investments (e.g., tool upgrades, animal or seed purchases, building commissions, etc.) may also be deducted as business expenses, up to 100% under the Ferngill Revenue Code (and your config settings).

If not enough funds are present in the farmer's account, they will be fined and seized of all Shipping Bin income until the outstanding amount is settled. All debts also accrue interest daily (configurable, default 72% [i]per annum[/i], i.e. ~0.64% per day).

If used together with [url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk Of Life[/url] and the player has the Conservationist profession, the professions' tax deduction perk will change from a % value increase to all items, to a more immersive % deduction of taxable income. Environmentalist activities can be used to deduct taxable income up to 100%. This means that farmers can be tax-exempt by collecting enough trash from oceans or rivers.

You can check your upcoming income taxes by running the console command `txs do income`.


[font=bebas_neuebook][size=5]Property Tax[/size][/font]

In addition to federal obligations, the farmer is also obliged to contribute Property Taxes to their local government.

The total value of the farmer's property will be appraised twice every season based on a Use-Value Assessment (UVA) program, which basically means that farmers are more liable for unproductive land; in other words, the taxation rate applied to land which is actively used for agriculture, livestock or forestry is generally [b]less[/b] than that applied to land which is not actively used (or is filled with debris, so hurry up and clean up that farm).

During the assessment, the total value of the farm's agriculture activities, livestock and real-estate all will be weighted. At the start of each year, Mayor Lewis will collect due property taxes from the [b]host[/b] farmer, based on the average UVA value of the entire property throughout the previous year, except Winter, which excludes the agriculture component for obvious reasons.

Farming activities in Ginger Island and other properties will not be charged.

Property taxes are not eligible for deductions.

Lateness fines are generally higher for property taxes (configurable). But since all debts are purchased by the same bank (canonically the Bank of Stardew), interest rates are always the same.

You can check your upcoming property taxes by running the console command `txs do property`.


[font=bebas_neuebook][size=6]For Mod Authors[/font][/size]

C# mod authors may request the [url=https://github.com/daleao/sdv/blob/main/Taxes/ITaxesApi.cs]Mod API[/url] to calculate the player's current taxes. This can be useful for authors who wish to add in-game methods of checking your taxes, as [b]I personally will not be doing that[/b].


[size=6][font=bebas_neuebook]DaLion Mod Series[/font][/size]

[url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk of Life[/url] - An extensive overhaul of profession trees and skill progression systems.
[url=https://www.nexusmods.com/stardewvalley/mods/24356]Aquarism[/url] - Make Fish Ponds actually useful. Created with the Aquarist profession in mind.
[b]Serfdom[/b] - Realistic income and property taxation mechanics. Created with the Conservationist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24832]Springmyst[/url] - Some actually interesting enchantments added to the game.
[url=https://www.nexusmods.com/stardewvalley/mods/29612]Mineracoustics[/url] - Overly complex overhaul of gemstone ring mechanics based on real Music Theory.
[url=https://www.nexusmods.com/stardewvalley/mods/23048]Chargeable Resource Tools[/url] - Charge the Axe and Pickaxe to break debris within an area.
[url=https://www.nexusmods.com/stardewvalley/mods/29830]Wildcat[/url] - Weapon combo framework, slick moves & other combat tweaks to improve the feel of combat.

[b][url=https://github.com/daleao/sdv/tree/main/Taxes]Source code[/url][/b]