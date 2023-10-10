<div align="center">

# ![](https://i.imgur.com/6sWaRit.png) Chargeable Resource Tools ![](https://i.imgur.com/4rYYYCD.png)

</div>

## Overview

This module is inspired by the tool progression system of old Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen.
It allows the Axe and Pickaxe to be charged, similarly to the Hoe and Watering Can. Charging will expend more stamina to affect all tiles in a radius around the target. The affected area is similar to a bomb, and by default the radius will grow by 1 tile with each charge level.

<div algin="center">
<figure align="center" width="9999" id="fig1">
  <img src="cover.gif" align="center" height="auto" width="80%" alt="Logo">
</figure>
</div>

## Configs

- **'RequiredUpgradeLevelForCharging':** This is the minimum upgrade level your tool must be at in order to enable charging. Accepted values are "Copper", "Steel", "Gold", "Iridium", "Radioactive" and "Mythicite" (the last two require Mood Misadventures).
- **'RadiusAtEachLevel':**  Allows you to specify the shockwave radius at each charging level. Note that your charging level is separate from your upgrade level. For instance, if 'RequiredUpgradeLevelForCharging' is set to Iridium, and 'RadiusAtEachLevel' set to [ 1, 2, 3, 4 ], then you will not be able to charge until the tool is Iridium level, but once it is, then your charging progression will be similar to the gif above (starting at 1, and increase by 1 until 4). If you wanted to skip charging up and instantly get the max radius, you could set all four values to the same number (and set 'ShowAffectedTiles' to false to avoid the overlay instantly appearing). Only accepts positive integers.
- **'RequireModKey':** Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
- **'ModKey':** If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check[here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings) for a list of available keybinds. You can set multiple comma-separated keys.
- **'HideAffectedTiles':** If enabled, will not display the green overlay showing the size of the shockwave.
- **'StaminaCostMultiplier':** By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.
- **'TicksBetweenWaves':** The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.

Other settings are self explanatory. Use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you need verbatim explanations.

## Compatibility

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patches Tool behavior might be incompatible.

- Compatible with [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612).
- Compatible with[Harvest Moon FoMT-like Watering Can And Hoe Area](https://www.nexusmods.com/stardewvalley/mods/7851).
