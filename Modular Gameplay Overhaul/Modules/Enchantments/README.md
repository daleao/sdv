
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
| Carving   | Attacks on-hit reduce enemy defense by 1. Removes the armor of Armored Bugs and de-shells Rock Crabs. |
| Cleaving  | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Blasting | Accumulates and stores half of the damage from enemy hits (before mitigation). After accumulating enough damage, the next special move releases that damage as an explosion. |
| Mammonite's | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage. |
| Bloodthirsty | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s. |

\* **Offensive Swords:** Dash distance +50% and can change direction mid-dash. **Defensive Swords:** Successful parries stun enemies for 1s. **Daggers:** Quick stab hit count +2. **Clubs:** Smash AoE + 50%.

### Ranged Enchantments

The enchantments below are entirely new and unique to slingshots.

| Name       | Effect |
| ---------- | -------|
| Magnum     | Doubles the size of fired projectiles. |
| Gatling    | Enables auto-fire.* |
| Preserving | 50% chance to not consume ammo. |
| Spreading  | Attacks fire 2 additional projectiles. Extra projectiles deal reduced (60%) damage and do not consume additional ammo.  |
| Quincy     | Attacks fire an energy projectile if no ammo is equipped. Only works near enemies.** |

\* *Firing speed is lower compared to [Desperado](../Professions)'s Death Blossom. If the Professions module is enabled, auto-firing requires holding the Mod Key (default LeftShift).*

\** *The Quincy projectile cannot crit nor knock back enemies, but is affected by damage modifiers. If the Professions module is enabled and the player has the Desperado profession, the Quincy projectile will also be affected by Overcharge, which will also increase the projectile's size.*

## Compatibility

- **Not** compatible with other mods that introduce new enchantments, such as [Enhanced Slingshots][mod:enhanced-slingshots].

<!-- MARKDOWN LINKS & IMAGES -->

[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
