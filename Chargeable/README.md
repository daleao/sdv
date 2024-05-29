<div align="center">

# ![](https://i.imgur.com/6sWaRit.png) Chargeable Resource Tools ![](https://i.imgur.com/4rYYYCD.png)

</div>

## What this is

This mod is inspired by the tool progression system of Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen. It allows the Axe and Pickaxe to be charged, similarly to the Hoe and Watering Can.


## How it works

<div algin="center">
<figure align="center" width="9999" id="fig1">
  <img src="resources/cover.gif" align="center" height="auto" width="80%" alt="Logo">
</figure>
</div>

By default you must hold a mod key (Default LeftShift) and *then* hold the use-tool key to begin charging. This is to avoid overriding the vanilla tool-spam behavior when you hold the use-tool key, which can also be pretty useful. You may choose to override this in the mod's settings to not require the mod key.

The radius of each charge level can also be configured. By default, each tool upgrade adds 1 to the maximum radius. The resulting area is similar to a bomb.

As you might expect, the larger the area the more stamina is consumed.


## Configs

- **'RadiusAtEachLevel':**  Allows you to specify the shockwave radius at each charging level.
- **'RequireModKey':** Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
- **'ModKey':** If 'RequireModKey' is true, you must hold this key in order to charge (default LeftShift). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings) for a list of available keybinds. You can set multiple comma-separated keys.
- **'StaminaCostMultiplier':** By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost).
- **'TicksBetweenWaves':** The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.

Other settings are self-explanatory. Use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you need verbatim explanations.


## Compatibility

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.


## Credits & Special Thanks

Credits to the following translators:
- ![](https://i.imgur.com/ezVo9Fb.png) [CaranudLapin](https://github.com/CaranudLapin) for French.
