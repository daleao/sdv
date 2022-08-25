<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

# ![](https://i.imgur.com/6sWaRit.png) Tooth And Nail - Immersive Tools ![](https://i.imgur.com/4rYYYCD.png)


<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li><a href="#configs">Configs</a></li>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>

## Features

This mod is inspired by the tools progression system of old Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades destroyed all debris on-screen.

<figure align="center" width="9999" id="fig1">
  <img src="cover.gif" align="center" height="auto" width="80%" alt="Logo">
</figure>

This mod provides two features:
1. Allows the Axe and Pickaxe to be charged according to the tool's upgrade level.
2. Allows customizing the area of effect of the Hoe and Watering Can.

The mod is fully modular.

For Axe and Pickaxe, the shape of the shockwave is similar to a bomb explosion, but the radius can be configured for each upgrade level.
Supports up to seven upgrade levels, including the Reaching Enchantment and the two extra levels from [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612).
All radius values should be positive whole numbers (obviously). By default, the radius at each levels is equal to the tool's upgrade level.
Like the Tractor Mod, what the shockwave actually does can also be configured. By default it is set to only clear debris (like stones and twigs), weeds, dead crops and resource clumps (like stumps, logs and boulders), as well as mining nodes.
You can optionally choose to let them affect other objects or terrain features, such as trees, live crops, and flooring.

For Hoe and Watering Can, the area of effect may be customized by setting a length and radius for each upgrade level. Note that the radius adds to both side of the farmer, such that a radius of 1 yields an area 3 tiles wide.

All tool enchantments are compatible.

## Configs

This section describes some of the configurable settings provided in configs.json:

- **'RequiredUpgradeLevelForCharging':** This is the minimum upgrade level your tool must be at in order to enable charging. Accepted values are "Copper", "Steel", "Gold", "Iridium", "Radioactive" and "Mythicite" (the last two require Mood Misadventures).
- **'RadiusAtEachLevel':**  Allows you to specify the shockwave radius at each charging level. Note that your charging level is separate from your upgrade level. For instance, if 'RequiredUpgradeLevelForCharging' is set to Iridium, and 'RadiusAtEachLevel' set to [ 1, 2, 3, 4 ], then you will not be able to charge until the tool is Iridium level, but once it is, then your charging progression will be similar to the gif above (starting at 1, and increase by 1 until 4). If you wanted to skip charging up and instantly get the max radius, you could set all four values to the same number (and set 'ShowAffectedTiles' to false to avoid the overlay instantly appearing). Only accepts positive integers.
- **'RequireModKey':** Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
- **'ModKey':** If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings) for a list of available keybinds. You can set multiple comma-separated keys.
- **'HideAffectedTiles':** If enabled, will not display the green overlay showing the size of the shockwave.
- **'StaminaCostMultiplier':** By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.
- **'TicksBetweenWaves':** The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.
- Other settings are self explanatory. Use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you need verbatim explanations.

## Compatibility

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.

- Compatible with [Moon Misadventures](https://www.nexusmods.com/stardewvalley/mods/10612).
- Compatible with [Harvest Moon FoMT-like Watering Can And Hoe Area](https://www.nexusmods.com/stardewvalley/mods/7851) as long as you don't touch Hoe and Watering Can settings (although you can just set them to the same values used by that mod to achieve the same effect).
- Compatible with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).

## Installation

Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.

To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.

To uninstall simply delete the mod from your mods folder. This mod is safe to uninstall at any point.

## Special Thanks

- [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317) for their [TractorMod](https://www.nexusmods.com/stardewvalley/mods/1401), from which this mod ~~steals~~ borrows much of its code.
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
