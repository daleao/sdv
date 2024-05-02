[center][img]https://i.imgur.com/6sWaRit.png[/img]   [b][size=6][font=bebas_neuebook]MARGO :: Tools (TOLS)[/font][/size][/b]   [img]https://i.imgur.com/4rYYYCD.png[/img][/center]

This module is inspired by the tool progression system of old Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen.


[size=6][font=bebas_neuebook]Overview[/font][/size]

[center][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/Modules/Tools/resources/cover.gif[/img][/center]

This module provides the following features:
1. Allows the Axe and Pickaxe to be charged according to the tool's upgrade level, just like Hoe and Watering Can.
2. Allows customizing the area of effect of the Hoe and Watering Can.
3. Allows customizing the range of Scythe and Golden Scythe, and allows them to harvest crops.
4. Extends certain tool enchantments, allowing them to be applied to a greater variety of tools.
5. Intelligently auto-selects the best tool for a target action tile.
6. Automatically turn to face the mouse cursor before using a tool.

All features can be toggled on or off.


[size=6][font=bebas_neuebook]Resource Tools[/font][/size]

Charging up the Axe or Pickaxe will release a shockwave which spreads the tool's effect around an area. The shape of the shockwave is similar to a bomb explosion, but the radius can be configured for each upgrade level.

Up to [b]seven[/b] upgrade levels are supported, which includes the Reaching Enchantment and the two extra levels from [url=https://www.nexusmods.com/stardewvalley/mods/10612]Moon Misadventures[/url].

All radius values should be positive whole numbers (obviously). By default, the radius at each level is equal to the tool's upgrade level.

Like the Tractor Mod, what the shockwave actually does can also be configured. By default it is set to only clear debris (like stones and twigs), weeds, dead crops and resource clumps (like stumps, logs and boulders), as well as mining nodes. You can optionally choose to let it affect other objects or terrain features, such as trees, live crops, and flooring; anything their corresponding tools ordinarily can do.


[size=6][font=bebas_neuebook]Farming Tools[/font][/size]

The area of effect of Hoe and Watering Can may be customized by setting a length and radius for each upgrade level. Note that the radius adds to both side of the farmer, such that a radius of 1 yields an area 3 tiles wide.

The radius of the Scythe and Golden Scythe can also be configured. By default, a regular Scythe will have twice the range of a sword, and a Golden Scythe will have twice the range of a regular Scythe. Setting the ranges to zero reverts them back to vanilla status. Also includes the popular Harvest With Scythe feature, replacing other similar mods.


[size=6][font=bebas_neuebook]Enchantments[/font][/size]

All tool enchantments are compatible. The Reaching Enchantment will work on chargeable resource tools as it ordinarily does for farming tools, increasing the maximum charge level by [b]one[/b]. The Powerful Enchantment likewise continues to increase the power of resource tools, and that extends to every affected tile in the shockwave.

In addition, this module will allow the Swift Enchantment to be applied to the Watering Can, and the Master Enchantment to be applied to all tools, boosting the corresponding skill level by [b]one[/b].

Lastly, the Haymaker Enchantment can now be applied to the Scythe and Golden Scythe.


[size=6][font=bebas_neuebook]Forge Upgrading[/font][/size]

Gaining access to the Volcano Forge, in addition to it's regular features, will also open up a new way to upgrade your tools. You will still require the correct metal bars as well as some Cinder Shards to ignite the Forge, but this method bypasses both the middleman's gold fee and, more importantly, the 2 day waiting period. This is also the only way to obtain the ultimate upgrade level.


[size=6][font=bebas_neuebook]Radioactive Tools[/font][/size]

Clint is no fool to mock about with radioactive substances. But with the Forge's Dwarven technology you can now upgrade your tools to the Radioactive tier. If [url=https://www.nexusmods.com/stardewvalley/mods/10612]Moon Misadventures[/url] is installed, this will also be required for the Mythic tier.


[size=6][font=bebas_neuebook]Auto-Selection[/font][/size]

Hold the Mod Key (default LeftShift) and click on any tool in your toolbar or inventory to enable it for auto-selection. As long the current tool is itself enabled for auto-selection, pressing the tool-use button will cause the appropriate enabled tool to be intelligently selected for the target tile.


[size=6][font=bebas_neuebook]Other[/font][/size]

Optionally allows gaining farming experience by watering crops, and prevents refilling the Watering Can with salt or ocean water.


[size=6][font=bebas_neuebook]Configs[/font][/size]

[list]
[*][b]'RequiredUpgradeLevelForCharging':[/b] This is the minimum upgrade level your tool must be at in order to enable charging. Accepted values are "Copper", "Steel", "Gold", "Iridium", "Radioactive" and "Mythicite" (the last one requires [url=https://www.nexusmods.com/stardewvalley/mods/10612]Moon Misadventures[/url]).
[*][b]'RadiusAtEachLevel':[/b] Allows you to specify the shockwave radius at each [i]charging[/i] level[i]. [/i]Note that your charging level is separate from your upgrade level. For instance, if 'RequiredUpgradeLevelForCharging' is set to [b]Iridium[/b], and 'RadiusAtEachLevel' set to [b][ 1, 2, 3, 4 ][/b], then[b] [/b]you will not be able to charge until the tool is Iridium level, but once it is, then your charging progression will be similar to the gif above (starting at 1, and increase by 1 until 4). If you wanted to skip charging up and instantly get the max radius, you could set all four values to the same number (and set 'ShowAffectedTiles' to false to avoid the overlay instantly appearing). Only accepts positive integers.
[*][b]'RequireModKey':[/b] Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
[*][b]'ModKey': [/b]If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [url=https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings]here[/url] for a list of available keybinds. You can set multiple comma-separated keys.
[*][b]'HideAffectedTiles':[/b] If enabled, will not display the green overlay showing the size of the shockwave (also affects the Hoe and Watering Can).
[*][b]'StaminaCostMultiplier':[/b] By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost [i]of the shockwave only.[/i] Setting to zero will make the  make the shockwave free (you will still lose stamina equal to the base tool cost). Accepts any decimal number greater than zero.
[*]'[b]TicksBetweenWaves':[/b] The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.
[*]Other settings are self explanatory. Use [url=https://www.nexusmods.com/stardewvalley/mods/5098]Generic Mod Config Menu[/url] if you need verbatim explanations.
[/list]

[size=6][font=bebas_neuebook]Compatibility[/font][/size]

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.
[list]
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/10612]Moon Misadventures[/url] (use the latest version of this mod).
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/7851]Harvest Moon FoMT-like Watering Can And Hoe Area[/url] as long as you don't touch Hoe and Watering Can settings (although you can just set them to the same values used by that mod to achieve the same effect).
[*][b]Not [/b]compatible with [url=https://www.nexusmods.com/stardewvalley/mods/2428]Prismatic Tool[/url][url=https://www.nexusmods.com/stardewvalley/mods/2428]s[/url] and [url=https://www.nexusmods.com/stardewvalley/mods/7630]Radioactive To[/url][url=https://www.nexusmods.com/stardewvalley/mods/7630]ols[/url].
[*][b]Not[/b] compatible with other Harvest With Scythe mods, such as [url=https://www.nexusmods.com/stardewvalley/mods/2731]Yet Another Harvest With Scythe[/url].﻿
[/list]