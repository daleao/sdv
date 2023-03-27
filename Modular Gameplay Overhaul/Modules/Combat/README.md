
<div align="center">

# Modular Overhaul :: Combat

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
  </ol>
</details>

## Overview

This simple module provides the following rebalancing options for general combat:

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

    They are compatible with any Content Patcher mods which affect monster stats.

4. **Varied Encounters:** Randomizes monster stats according to Daily Luck, reducing the monotony of dungeons.

5. **Stun Animation:** Gives a visual indication for when enemies are stunned by several actions.

All features are optional and can be toggled individually.

## Compatibility

- No known incompatibilities.
