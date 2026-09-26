<div align="center">

# 🦁 Lionheart - Core Framework

***The core library that provides shared functionality for all DaLion mods.***

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Lionheart-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/24332)
[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6-8a5a2b?logo=stardewvalley&logoColor=white)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-cc4400)](https://smapi.io/)
[![License](https://img.shields.io/badge/license-see%20LICENSE-888)](../LICENSE)

</div>

## 📖 What This Is

This is the core mod which provides shared functionality required by other DaLion mods.

It carries a few features of its own because I've no interest in managing a bunch of small miscellaneous mods. All are disabled by default.

- **Colored Slime Balls:** Causes Slime Balls to take on the color of the Slimes which produced them, and adds regular color-based Slime drops to Slime Ball loot tables. This is required by [Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355), so will auto-enable when that mod is installed.
- **Two-Way Hoppers:** Adds the ability for Hoppers to pull items back out from machines, allowing them to fully automate a single machine at a time and transforming them from completely useless into a more balanced version of [Automate](https://www.nexusmods.com/stardewvalley/mods/1063).
- **Witherable Crops:** Crops may wither if left un-watered.
- **Immersive Hay:** Obtain Hay from harvesting pre-mature Wheat (stage 4). Mimics how hay is made IRL.
- **Winter Wheat:** Plant Wheat near the end of Fall to have it survive dormant through Winter. In Spring, it resumes growing, yielding twice as much harvest. Mimics how real wheat is planted IRL.
- **Snowball Fights:** Use an empty slingshot while standing over a snowy tile to fire a snowball projectile. Doesn't do damage; it's just for fun.

## ⚔️ Status Effects

Taking inspiration from classic game tropes, this mod adds a framework for causing various status conditions to enemies. These effects will be used by the various DaLion mods, and can also be used by any C# mod that wishes to consume the [API](https://github.com/daleao/sdv/blob/main/Core/ICoreApi.cs).

<div align="center">

| Status | Effect |
| :-- | :-- |
| **Bleeding** | Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5×. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (i.e., Dwarven Sentry). |
| **Blinded** | Causes enemies to lose track of their target and miss attacks. Does not affect Bats or Duggys. |
| **Burning** | Causes damage equal to 1/16th of max health every 3 seconds, and reduces attack by half. Also causes enemies to move about more randomly. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4× as quickly. |
| **Chilled** | Reduces speed for the duration. If Chilled is inflicted again during this time, the target may be Frozen for three times the duration. Does not affect Ghosts or Skeleton Mage. |
| **Frozen** | Cannot move or attack. The next hit during the duration deals double damage and ends the effect. |
| **Poisoned** | Causes damage equal to 1/16 of max health every 3s, stacking up to 3×. If enough stacks are applied the target may suffer instant death. Does not affect Ghosts or Skeletons. |
| **Slowed** | Reduces speed for the duration. |
| **Stunned** | Cannot move or attack for the duration. |

</div>

Durations depend on the source. These status conditions are exclusively applied to monsters.

### Consistent Farmer Debuffs

This setting will optionally modify the Burnt and Frozen debuff on the player to be consistent with the player-inflicted debuffs above. It also modifies Jinxed and Weakness to work better with the overall balance intended by the DaLion mod series:

- **Jinxed:** Reduces player defense by 50% and prevents use of weapon special moves for the duration.
- **~~Weakness~~ Disoriented:** Lose control of movement (movement directions are inverted).

## 🧩 Compatibility

N/A.

## 💖 Credits & Special Thanks

Credit to [Roscid](https://next.nexusmods.com/profile/Roscid/about-me?gameId=1303) for [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634).

---

<div align="center">

### 🌾 The DaLion.Stardew Series

[Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) &nbsp;·&nbsp; [Aquarism](https://www.nexusmods.com/stardewvalley/mods/24356) &nbsp;·&nbsp; [Serfdom](https://www.nexusmods.com/stardewvalley/mods/24357) &nbsp;·&nbsp; [Springmyst](https://www.nexusmods.com/stardewvalley/mods/24832) &nbsp;·&nbsp; [Mineracoustics](https://www.nexusmods.com/stardewvalley/mods/29612) &nbsp;·&nbsp; [Chargeable Resource Tools](https://www.nexusmods.com/stardewvalley/mods/23048) &nbsp;·&nbsp; [Wildcat](https://www.nexusmods.com/stardewvalley/mods/29830)

<br>

<sub>Made with 🌱 for the Stardew Valley community.</sub>

</div>
