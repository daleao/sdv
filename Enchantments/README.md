<div align="center">

# 💎 Springmyst — Enchantments of Stardew

***An entirely new collection of weapon, slingshot and tool enchantments.***

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Springmyst-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/24832)
[![Nexus downloads](https://img.shields.io/badge/downloads-on%20Nexus-da8e35?logo=nexusmods&logoColor=white)](https://www.nexusmods.com/stardewvalley/mods/24832)
[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6-8a5a2b?logo=stardewvalley&logoColor=white)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-cc4400)](https://smapi.io/)
[![License](https://img.shields.io/badge/license-see%20LICENSE-888)](https://github.com/daleao/sdv/blob/main/LICENSE)
[![Stars](https://img.shields.io/github/stars/daleao/sdv?logo=github&style=flat&color=e0b040)](https://github.com/daleao/sdv/stargazers)

</div>

## 📖 What This Is

This mod replaces vanilla weapon enchantments with an entirely new and significantly more interesting collection. It also adds a few slingshot-exclusive enchantments and tweaks to tool enchantments.

In addition, this mod optionally replaces the generic vanilla "Forged" text in weapon tooltips with actual configurable gemstone sockets.

## ⚔️ Melee Weapon Enchantments

<div align="center">

| Enchantment | Effect |
| :-- | :-- |
| **Carving** | Attacks on-hit reduce enemy defense by 1 point (continuing below zero). Armored enemies (i.e., Armored Bugs and shelled Rock Crabs) lose their armor when their defense is reduced to zero. |
| **Cleaving** | Attacks on-hit spread 60% – 20% (based on distance) of the damage to other enemies around the target. |
| **Explosive** | Absorbs and stores the damage from enemy hits (before mitigation). The next special move releases twice the accumulated damage as an explosion. |
| **Energized** | Moving and attacking generates energy, up to 100 stacks. At maximum stacks, the next attack causes a powerful electric discharge which deals heavy damage in a large area. |
| **Greedy** | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. Each consecutive takedown increases this threshold by 1%, resetting when you take damage.<sup>1</sup> |
| **Radiant** | Deal 50% more damage to shadow and undead monsters and prevent them from resurrecting. Weapon swings create a mid-range sunlight projectile which blinds enemies upon collision. |
| **Reckless** | Replaces the defensive parry special move with a stabbing lunge attack. You can perform two special moves in succession. **Sword only**. |
| **Sanguine** | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 15s. |
| **Wabbajack** | Causes unpredictable effects.<sup>2</sup> |

</div>

<sub><sup>1</sup> Hard caps at 1000 HP. To prevent cheesing boss monsters from expansion mods, this is implemented as a percentage chance per hit, with the chance being near-zero close to the 1000 HP hard cap and near 100% for regular monsters.</sub>

<sub><sup>2</sup> Example effects: execute or fully-heal the enemy; apply a random [debuff](https://www.nexusmods.com/stardewvalley/mods/24332); grow or shrink the enemy; split the enemy in two; transform the enemy into a random animal; transform the enemy into a random amount of cheese.</sub>

## 🏹 Slingshot Enchantments

<div align="center">

| Enchantment | Effect |
| :-- | :-- |
| **Chilling** | Progressively chills enemies on hit for 2 seconds, freezing after stacking 3 times. |
| **Echoing** | Summons two "echoes" of the fired projectile, that auto-aim at the nearest enemy after a short delay. Only works when enemies are nearby.<sup>1</sup> |
| **Energized** | Moving and shooting generates energy, up to 100 stacks. At maximum stacks, the next shot carries an electric charge, which discharges dealing heavy area damage when it hits an enemy. |
| **Quincy** | Attacks fire an energy projectile if no ammo is equipped. The projectile is stronger at lower health. Only works when enemies are nearby.<sup>2</sup> |

</div>

<sub><sup>1</sup> Echo projectiles inherit 60% of the main projectile's damage.</sub>

<sub><sup>2</sup> Quincy projectiles have no knockback. Damage increases by 50% below 2/3 max health, and again by 100% when below 1/3 (the projectile will change color to reflect this). If [Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) is installed and the player has the Rascal profession, Quincy projectiles can be fired even if a different ammo is equipped in the second ammo slot. If the player also has the Desperado profession, the Quincy projectile's size will be increased proportionally by overcharge **instead of** its velocity and knockback.</sub>

## 🛠️ Tool Enchantments

<div align="center">

| Enchantment | Effect |
| :-- | :-- |
| **Master** | ~~Fishing Level +1~~ Increases the corresponding skill level by 20%. Can now be applied to Axe, Pickaxe, Hoe and Watering Can.<sup>1</sup> |
| **Swift** | Can now be applied to Watering Can. |
| **Reaching** | Can now be applied to Iridium Scythe (increases the area of effect instead of adding a charge level). |
| **Haymaker** | Can now be applied to Iridium Scythe **only**. |
| **Sharp** | Can be applied to Iridium Scythe and Pickaxe. Grants increased damage versus enemies.<sup>2</sup> |

</div>

<sub><sup>1</sup> Axe → Foraging Level, Pickaxe → Mining Level, Hoe & Watering Can → Farming Level.</sub>

<sub><sup>2</sup> Iridium Scythe gains damage equivalent to a Galaxy Sword. Iridium Pickaxe gains damage equivalent to a Galaxy Hammer.</sub>

## 🟢 Jade Rebalance

The Jade enchantment is useless in vanilla; it provides the exact same damage increase as the Ruby enchantment, but only to critical strikes. So to make it viable we give it a small buff:

> **Jade Enchantment:** Critical hit damage ~~+10%~~ **+50%**.

With this, the Jade enchantment becomes stronger than the Ruby enchantment at around 20% critical hit chance.

## ⚠️ Uninstalling

> [!WARNING]
> Before uninstalling this mod, **you must remove all enchantments from your existing weapons** and save the game. **Failure to do so may brick your save file.**

## 🧩 Compatibility

N/A.

## 💖 Credits & Special Thanks

Credits and inspiration:
- [Bethesda Game Studios](https://www.bethesdagamestudios.com/) for [Skyrim](https://elderscrolls.bethesda.net/en)
- [Gravity](https://ro.gnjoy.com/index.asp) for **Ragnarok Online**
- [Riot Games](https://www.riotgames.com/en) for [League Of Legends](https://www.leagueoflegends.com/en-us/)
- [Tite Kubo](https://en.wikipedia.org/wiki/Tite_Kubo) for [Bleach](https://www.crunchyroll.com/series/G63VGG2NY/bleach)

Special thanks to [Enai Siaion](https://next.nexusmods.com/profile/EnaiSiaion?gameId=1704) for [Summermyst](https://www.nexusmods.com/skyrimspecialedition/mods/6285) and [Wintermyst](https://www.nexusmods.com/skyrimspecialedition/mods/18603).

---

<div align="center">

### 🌾 The DaLion.Stardew Series

[Walk of Life](https://www.nexusmods.com/stardewvalley/mods/24355) &nbsp;·&nbsp; [Aquarism](https://www.nexusmods.com/stardewvalley/mods/24356) &nbsp;·&nbsp; [Serfdom](https://www.nexusmods.com/stardewvalley/mods/24357) &nbsp;·&nbsp; **Springmyst** &nbsp;·&nbsp; [Mineracoustics](https://www.nexusmods.com/stardewvalley/mods/29612) &nbsp;·&nbsp; [Chargeable Resource Tools](https://www.nexusmods.com/stardewvalley/mods/23048) &nbsp;·&nbsp; [Wildcat](https://www.nexusmods.com/stardewvalley/mods/29830)

<br>

<sub>Made with 🌱 for the Stardew Valley community.</sub>

</div>
