
<div align="center">

# Modular Overhaul :: Combat (CMBT)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#overview">Status Conditions</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
  </ol>
</details>

## Overview

This relatively simple module provides the following major rebalancing options for general combat:

1. **Knockback Damage:** Knocked-back enemies will take damage porportional to the knockback stat when colliding with a wall or obstacle. This makes knockback a more viable offensive stat, in addition to its defensive value. It also makes positioning an important strategic element.
2. **Defense Overhaul:** Replaces the linear subtraction formula from Vanilla with an exponential multiplicative formula, providing better scalability, and thus more value to the defense stat.

    Old formula:
    ```
    damage = Min(rawDamage - defense, 1)
    ```

    New formula:
    ```
    resistance = 10 / (10 + defense)
    damage = rawDamage * resistance
    ```

    One defense point now reduces damage by 10% regardless. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve.

    Note that this applies to monsters as well!
    
    Crit. strikes will ignore enemy defense, allowing critical builds to counter defensive enemies.

3. **Difficulty Sliders:** 3 sliders are provided to taylor monster difficulty to your preference:
    - Monster health multiplier
    - Monster damage multiplier
    - Monster defense multiplier

    Compatible with any Content Patcher mods which affect monster stats.

4. **Varied Encounters:** Randomizes monster stats according to Daily Luck, reducing the monotony of dungeons.

All features are optional and can be toggled individually.

## Status Conditions

Taking inspiration from classic RPG or strategy games, this module adds a framework for causing various status conditions to enemies. They are:

- **Bleeding:** Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
- **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magama Sparkers).
- **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
- **Freeze:** Cannot move or attack for 30s. The next hit during the duration deals triple damage and ends the effect.
- **Poisoned:** Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
- **Slowed:** Reduces movement speed by half for the duration.
- **Stunned:** Cannot move or attack for the duration.

While it doesn't do anything on it's own, this opens up the possibility for other modules within MARGO to create more interesting overhauls.Each status conditions is accompanied by a neat corresponding animation. Status conditions cannot be applied on the player.

## Compatibility

- No known incompatibilities.
