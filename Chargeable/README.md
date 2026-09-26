<div align="center">

# ⛏️ Chargeable Resource Tools

***Charge the Axe and Pickaxe to clear debris across an area, like the Hoe and Watering Can.***

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Chargeable%20Resource%20Tools-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/23048)
[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6-8a5a2b?logo=stardewvalley&logoColor=white)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-cc4400)](https://smapi.io/)
[![License](https://img.shields.io/badge/license-see%20LICENSE-888)](../LICENSE)

</div>

## 📖 What This Is

This mod is inspired by the tool progression system of Harvest Moon: Friends of Mineral Town, where the Axe and Hammer tools were also chargeable, and their ultimate upgrades could destroy all debris on-screen. Simply put, this mod allows the Axe and Pickaxe to be charged, similarly to the Hoe and Watering Can.

## ⚙️ How It Works

<div align="center">
  <img src="resources/cover.gif" width="80%" alt="Charging a tool to clear debris across an area">
</div>

By default you must hold a mod key (default <kbd>Shift</kbd>) and *then* hold the use-tool key to begin charging. This is to avoid overriding the vanilla tool-spam behavior when you hold the use-tool key, which can also be pretty useful. You may choose to override this in the mod's settings to not require the mod key.

The radius of each charge level can also be configured. By default, each tool upgrade adds 1 to the maximum radius. The resulting area is similar to a bomb.

As you might expect, the larger the area the more stamina is consumed.

## 🔩 Configs

- **`RadiusAtEachLevel`:** Allows you to specify the shockwave radius at each charging level.
- **`RequireModKey`:** Set to false if you want charging behavior to be the default when holding down the tool button. Set to true if you prefer the vanilla tool spamming behavior.
- **`ModKey`:** If `RequireModKey` is true, you must hold this key in order to charge (default <kbd>Shift</kbd>). If you play with a gamepad controller you can set this to LeftTrigger or LeftShoulder. Check [here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings) for a list of available keybinds. You can set multiple comma-separated keys.
- **`StaminaCostMultiplier`:** By default, charging multiplies your tool's base stamina cost by the charging level. Use this multiplier to adjust the cost of the shockwave *only*. Set to zero to make it free (you will still lose stamina equal to the base tool cost). Accepts any real number greater than zero.
- **`TicksBetweenWaves`:** The number of game ticks before the shockwave grows by 1 tile. Higher numbers cause the shockwave to travel slower. Setting this to 0 replicates the original behavior from older versions.

Other settings are self-explanatory. Use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) if you need verbatim explanations.

## 🧩 Compatibility

This mod uses Harmony to patch the behavior of Axe and Pickaxe. Any mods that also directly patch Tool behavior might be incompatible.

---

<div align="center">

### 🌾 The DaLion.Stardew Series

[Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) &nbsp;·&nbsp; [Aquarism](https://www.nexusmods.com/stardewvalley/mods/24356) &nbsp;·&nbsp; [Serfdom](https://www.nexusmods.com/stardewvalley/mods/24357) &nbsp;·&nbsp; [Springmyst](https://www.nexusmods.com/stardewvalley/mods/24832) &nbsp;·&nbsp; [Mineracoustics](https://www.nexusmods.com/stardewvalley/mods/29612) &nbsp;·&nbsp; **Chargeable Resource Tools** &nbsp;·&nbsp; [Wildcat](https://www.nexusmods.com/stardewvalley/mods/29830)

<br>

<sub>Made with 🌱 for the Stardew Valley community.</sub>

</div>
