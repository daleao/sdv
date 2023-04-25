
<div align="center">

# Modular Overhaul :: Enchantments (ENCH)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#gemstone-forges">Gemstone Forges</a></li>
    <li><a href="#melee-enchantments">Melee Enchantments</a></li>
    <li><a href="#ranged-enchantments">Ranged Enchantments</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
  </ol>
</details>

## Overview

This module was born as an extension to the [Rings](../Rings) module's rebalance. It brings analogous changes to gemstone enchantments. But much more than that, it completely overhauls all Prismatic Shard enchantments for Melee Weapons and introduces all-new enchantments for Slingshots.

All features are optional and can be toggled individually.

### Gemstone Forges

- **Jade Enchantment:** Buffed significantly, from +10% to +50% crit. power.
- **Garnet Enchantment:** Introduced, provided that the Garnet stone is installed via the optional file, granting 10% cooldown reduction.

### Melee Enchantments

Enchantments have been almost entirely overhauled. Hopefully these enchantments will provide more interesting gameplay options.

| Name      | Effect |
| --------- | -------|
| Haymaker  | *Unchanged from vanilla.* |
| Artful    | Improves the special move of each weapon.* |
| Carving   | Attacks on-hit reduce enemy defense by 1. Removes the armor from Armored Bugs and de-shells Rock Crabs. |
| Cleaving  | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Blasting | Accumulates and stores half of the damage from enemy hits (before mitigation). If enough damage is accumulated, the next special move releases that damage as an explosion. |
| Steadfast    | Converts critical strike chance into bonus raw damage (multiplied by critical power). |
| Bloodthirsty | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s. |
| Mammonite's | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage. |
| Wabbajack | Causes unpredictable effects. *Examples: damage or heal the enemy; decrease or increase the enemie's stats; transfigure into a different enemy, creature or any random item (note: this can spawn illegal items).* |

\*
    - **Offensive Swords:** Can dash twice in succession.
    - **Defensive Swords:** The next attack within 5s of a successful parry is guaranteed a critical strike.
    - **Daggers:** Quick stab deals an additional hit. If [WPNZ](../Weapons) is enabled with rebalance option, all hits apply Bleed with 100% chance.
    - **Clubs:** Smash area +50%. Enemmies in range are stunned for 2s.

### Ranged Enchantments

The enchantments below are entirely new and unique to slingshots.

| Name       | Effect |
| ---------- | -------|
| Magnum     | Greatly increases the size of fired projectiles. |
| Gatling    | Enables auto-fire.* |
| Preserving | Does not consume ammo. |
| Spreading  | Consume one additional ammo to fire two additional angled projectiles. |
| Quincy     | Attacks fire an energy projectile if no ammo is equipped. Only works near enemies.** |

\* *Double-click/press and then **hold** the use-tool key to engage auto-fire.*

\** *Quincy projectile cannot crit nor knock back enemies, but is affected by damage modifiers. If the Professions module is enabled and the player has the Desperado profession, Quincy projectile will also be affected by Overcharge.*

## Compatibility

- **Not** compatible with other mods that introduce new enchantments, such as [Enhanced Slingshots][mod:enhanced-slingshots].

<!-- MARKDOWN LINKS & IMAGES -->

[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
