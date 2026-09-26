[size=6][font=bebas_neuebook]What this is[/font][/size]

This mod is inspired by the tool progression system of Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen. Simply put, this mod allows the Axe and Pickaxe to be charged, similarly to the Hoe and Watering Can.


[size=6][font=bebas_neuebook]HOW IT WORKS[/font][/size]

[center]
[img]https://raw.githubusercontent.com/daleao/sdv/main/Chargeable/resources/cover.gif[/img][/center]

By default you must hold a mod key (Default LeftShift) and [i]then[/i] hold the use-tool key to begin charging. This is to avoid overriding the vanilla tool-spam behavior when you hold the use-tool key, which can also be pretty useful. You may choose to override this in the mod's settings to not require the mod key.

The radius of each charge level can also be configured. By default, each tool upgrade adds 1 to the maximum radius. The resulting area is similar to a bomb.

As you might expect, the larger the area the more stamina is consumed.


[size=6][font=bebas_neuebook]Configs[/font][/size]
[list]
[*][b]'RadiusAtEachLevel':[/b]  Allows you to specify the shockwave radius at each charging level.[/*]
[*][b]'RequireModKey':[/b] Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.[/*]
[*][b]'ModKey':[/b] If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [url=https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings]here[/url] for a list of available keybinds. You can set multiple comma-separated keys.[/*]
[*][b]'StaminaCostMultiplier':[/b] By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.[/*]
[*][b]'TicksBetweenWaves':[/b] The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.[/*]
[/list]
Other settings are self-explanatory. Use [url=https://www.nexusmods.com/stardewvalley/mods/5098]Generic Mod Config Menu[/url] if you need verbatim explanations.


[size=6][font=bebas_neuebook]Compatibility[/font][/size]

Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/7851]Harvest Moon FoMT-like Watering Can And Hoe Area[/url].


[size=6][font=bebas_neuebook]DaLion Mod Series[/font][/size]

[url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk of Life[/url] - An extensive overhaul of profession trees and skill progression systems.
[url=https://www.nexusmods.com/stardewvalley/mods/24356]Aquarism[/url] - Make Fish Ponds actually useful. Created with the Aquarist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24357]Serfdom[/url] - Realistic income and property taxation mechanics. Created with the Conservationist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24832]Springmyst[/url] - Some actually interesting enchantments added to the game.
[url=https://www.nexusmods.com/stardewvalley/mods/29612]Mineracoustics[/url] - Overly complex overhaul of gemstone ring mechanics based on real Music Theory.
[b]Chargeable Resource Tools[/b] - Charge the Axe and Pickaxe to break debris within an area.
[url=https://www.nexusmods.com/stardewvalley/mods/29830]Wildcat[/url] - Weapon combo framework, slick moves & other combat tweaks to improve the feel of combat.

[b][url=https://github.com/daleao/sdv/tree/main/Chargeable]Source code[/url][/b]