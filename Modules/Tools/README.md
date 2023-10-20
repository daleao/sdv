<div align="center">

# ![](https://i.imgur.com/6sWaRit.png) MARGO :: Tools (TOLS) ![](https://i.imgur.com/4rYYYCD.png)

</div>

## Overview

This module is inspired by the tool progression system of old Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen.

<div align="center">
  <img src="resources/cover.gif" width="67%" alt="cover.gif">
</div>

This module provides the following features:
1. Allows the Axe and Pickaxe to be charged according to the tool's upgrade level, just like Hoe and Watering Can.
2. Allows customizing the area of effect of the Hoe and Watering Can.
3. Allows customizing the range of Scythe and Golden Scythe, and allows them to harvest crops.
4. Extends certain tool enchantments, allowing them to be applied to a greater variety of tools.
5. Intelligently auto-selects the best tool for a target action tile.
6. Automatically faces the mouse cursor before using a tool.

All features can be toggled on or off.

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Resource Tools

Charging up the Axe or Pickaxe will release a shockwave which spreads the tool's effect around an area. The shape of the shockwave is similar to a bomb explosion, but the radius can be configured for each upgrade level.

Up to **seven** upgrade levels are supported, which includes the Reaching Enchantment and the two extra levels from [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612).
All radius values should be positive whole numbers (obviously). By default, the radius at each level is equal to the tool's upgrade level.

Like the Tractor Mod, what the shockwave actually does can also be configured. By default it is set to only clear debris (like stones and twigs), weeds, dead crops and resource clumps (like stumps, logs and boulders), as well as mining nodes. You can optionally choose to let it affect other objects or terrain features, such as trees, live crops, and flooring; anything their corresponding tools ordinarily can do.

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Farming Tools

The area of effect of Hoe and Watering Can may be customized by setting a length and radius for each upgrade level. Note that the radius adds to both side of the farmer, such that a radius of 1 yields an area 3 tiles wide.

The radius of the Scythe and Golden Scythe can also be configured. By default, a regular Scythe will have twice the range of a sword, and a Golden Scythe will have twice the range of a regular Scythe. Setting the ranges to zero reverts them back to vanilla status. Also includes the popular Harvest With Scythe feature, replacing other similar mods.

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Enchantments

All tool enchantments are compatible. The Reaching Enchantment will work on chargeable resource tools as it ordinarily does for farming tools, increasing the maximum charge level by **one**. The Powerful Enchantment likewise continues to increase the power of resource tools, and that extends to every affected tile in the shockwave.

In addition, this module will allow the Swift Enchantment to be applied to the Watering Can, and the Master Enchantment to be applied to all tools, boosting the corresponding skill level by **one**.

Lastly, the Haymaker Enchantment can now be applied to the Scythe and Golden Scythe.

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Forge Upgrading

Gaining access to the Volcano Forge, in addition to it's regular features, will also open up a new way to upgrade your tools. You will still require the correct metal bars as well as some Cinder Shards to ignite the Forge, but this method bypasses both the middleman's gold fee and, more importantly, the 2 day waiting period. This is also the only way to obtain the ultimate upgrade level.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/forge_upgrading.gif" alt="forge_upgrading.gif" width="67%">
</div>

Clint is no fool to mock about with radioactive substances. But with the Forge's Dwarven technology you can now upgrade your tools to the Radioactive tier. If [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612) is installed, this will also be required for the Mythic tier.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Tools/resources/radioactive.png" alt="radioactive.png">
</div>

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Auto-Selection

Hold the Mod Key (default LeftShift) and click on any tool in your toolbar or inventory to enable it for auto-selection. As long the current tool is itself enabled for auto-selection, pressing the tool-use button will cause the appropriate enabled tool to be intelligently selected for the target tile.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/auto_select.gif" alt="auto_select.gif" width="67%">
</div>

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Configs

This section describes some of the configurable settings provided in configs.json:

- **'RequiredUpgradeLevelForCharging':** This is the minimum upgrade level your tool must be at in order to enable charging. Accepted values are "Copper", "Steel", "Gold", "Iridium", "Radioactive" and "Mythicite" (the last two require Mood Misadventures).
- **'RadiusAtEachLevel':**  Allows you to specify the shockwave radius at each charging level. Note that your charging level is separate from your upgrade level. For instance, if 'RequiredUpgradeLevelForCharging' is set to Iridium, and 'RadiusAtEachLevel' set to [ 1, 2, 3, 4 ], then you will not be able to charge until the tool is Iridium level, but once it is, then your charging progression will be similar to the gif above (starting at 1, and increase by 1 until 4). If you wanted to skip charging up and instantly get the max radius, you could set all four values to the same number (and set 'ShowAffectedTiles' to false to avoid the overlay instantly appearing). Only accepts positive integers.
- **'RequireModKey':** Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
- **'ModKey':** If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings) for a list of available keybinds. You can set multiple comma-separated keys.
- **'HideAffectedTiles':** If enabled, will not display the green overlay showing the size of the shockwave.
- **'StaminaCostMultiplier':** By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.
- **'TicksBetweenWaves':** The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.

Other settings are self explanatory. Use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you need verbatim explanations.

<sup><sup>[🔼 Back to top](#margo--tools-tols)</sup></sup>

## Compatibility

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.

- Compatible with [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612).
- Compatible with [Grandpa's Tools](https://www.nexusmods.com/stardewvalley/mods/8835) and will use corresponding textures for Radioactive and Mythicite weapons.
- Compatible with [Harvest Moon FoMT-like Watering Can And Hoe Area](https://www.nexusmods.com/stardewvalley/mods/7851) as long as you don't touch Hoe and Watering Can settings (although you can just set them to the same values used by that mod to achieve the same effect).
- **Not** compatible with the likes of [Combat Controls - Fixed Mouse Click](https://www.nexusmods.com/stardewvalley/mods/2590) or [Combat Controls Redux](https://www.nexusmods.com/stardewvalley/mods/10496), as those features are already included in this and others modules.
- **Not** compatible with other Harvest With Scythe mods, such as [Yet Another Harvest With Scythe](https://www.nexusmods.com/stardewvalley/mods/2731).

[🔼 Back to top](#margo--tools-tols)