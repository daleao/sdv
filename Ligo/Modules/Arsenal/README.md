
<div align="center">

# Ligo :: Arsenal

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li>
      <a href="#melee-weapons-rework">Melee Weapons Rework</a>
      <ul>
        <li><a href="#combos-swing-speed">Combos & Swing Speed</a></li>
	      <li><a href="#knockback-mechanics">Knockback Mechanics</a></li>    
        <li><a href="#defense-mechanics">Defense Mechanics</a></li>
        <li><a href="#weapon-stats">Weapon Stats</a></li>
        <li><a href="#stabbing-swords-special-moves">Stabbing Swords & Special Moves</a></li>
      </ul>
    </li>
    <li>
      <a href="#slingshots">Slingshots Rework</a>
      <ul>
        <li><a href="#critical-hits">Critical Hits</a></li>
        <li><a href="#special-move">Special Move</a></li>
        <li><a href="#forge-mechanics">Forges Mechanics</a></li>
        <li><a href="#damage-modifiers">Damage Modifiers</a></li>
        <li><a href="#travel-grace-period">Travel Grace Period</a></li>
      </ul>
    </li>
    <li>
      <a href="#enchantments-forges">Enchantments & Forges</a>
      <ul>
        <li><a href="#defense-overhaul">Gemstone Forges</a></li>
        <li><a href="#weapon-enchantments">Weapon Enchantments</a></li>
        <li><a href="#slingshot-enchantments">Slingshot Enchantments</a></li>
      </ul>
    </li>
    <li>
      <a href="#other-features">Other Features</a>
      <ul>
	    <li><a href="#facing-direction-slick-moves">Facing Direction & Slick Moves</a></li>
        <li><a href="#infinity-1-weapons">Infinity +1 Weapons</a></li>
        <li><a href="#woody-replaces-rusty">Woody Replaces Rusty</a></li>
        <li><a href="#snowballs">Snowballs</a></li>
        <li><a href="#difficulty-sliders">Difficulty Sliders</a></li>
      </ul>
    </li>
    <li><a href="#recommended-mods">Compatibility</a></li>
  </ol>
</details>

## Overview

What began as a simple weapon rebalance has become a huge overhaul of weapon mechanics, slingshot mechanics and combat itself. At last, the final module of the original Immersive Suite has arrived.

Please note that existing weapon will not be immediately be affected by all changes provided by this module if it is enabled mid-save. Please use [CJB Item Spawner][mod:cjb-spawner] or similar to delete and respawn all owned melee weapons after enabling or disabling this module.

## Melee Weapons Rework

Vanilla weapons have several issues.

The first issue is the supremacy of the sword, which combines the speed of the dagger with the knockback of the club. Several mods have attempted to rebalance weapons by increasing the knockback of daggers or increasing the speed of clubs. But that solution is disingenuous, making every weapon behave like a sword; the identity of each weapon type is lost in the process.

The second issue is the futility of most weapon stats; damage is king when choosing rings and forges, and the remaining five weapon stats (crit. chance, crit. power, knockback, speed and precision) are mostly, if not completely, useless: crit. stats tend to be avoided due to the difficulty in building significant crit. chance; knockback and speed are ignored, again because the sword already offers enough of both, and precision does absolutely nothing.

The third issue is the spammy nature of weapons; the speed stat in vanilla is doubly useless, because it only affects the last frame of the attack animation (i.e., the delay between attacks, and not the swipe animation as a whole), but also the game allows attacks to animation-cancel each other, bypassing that last frame. The result is that you can just spam click to attack continuously without penalty. This also adds to the futility of the sword's defensive special move.

The fourth issue is the overabundance of weapons, especially swords, paired with how easy it is to obtain the best-in-slot weapons; nearly all of the unique and best weapons are freely given every 10 mine levels, not even including how easy it is to obtain the Galaxy Sword. This quickly trivializes any weapons obtained from drops, which end up serving as little more than inventory clutter. 

A weapon overhaul must fix, or at least touch upon, all of these issues; it needs to rebalance all weapon types while preserving their identities, and also add strategic value to all buildable weapon stats, allowing players to experiment a variety of different builds and playstyles. To achieve this, this module uses a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more challenging. Players who don't care for a challenge or are uninsterested in combat should probably keep this whole module disabled.

### Combos & Swing Speed

Taking inspiration from the Haunted Chocolatier trailer, this module introduces **weapon combos**. These are short bursts of swing speed followed by a longer cooldown period. Sword combos can perform up to 4 hits, maintaining the above-average speed of swords but preventing them from being spammed continously. Club combos can perform up to 2 hits only, which should give them an edge over their vanilla counterparts while remaining significantly slower than swords. Daggers cannot combo; they remain unchanged from vanilla.

Swing speed bonuses from emerald rings and forges now affect every single frame of the attack animation, as well as the cooldown in between combos. This should make speed a significantly more valuable stat.

Note that daggers, unlike the other two weapon types, have single-frame animations. As such, their attacks are near-instantaneous by default, and thus will gain little to no benefit from swing speed bonuses even with these changes.

### Knockback Mechanics

Knocked-back enemies will now take damage porportional to the knockback stat when colliding with a wall or obstacle. This makes knockback a viable offensive stat in addition to its defensive value. This also means that positioning is now an important strategic combat element. The excessive knockback of vanilla weapons has also been nerfed by 25% to as much as 33% across the board.

### Defense Mechanics

Defense in vanilla is linearly subtracted from damage. There are several problems with this approach which make the defense stat unscalable:
- While a single point of defense can easily mean a 50% damage reduction against early-game Green Slimes, that same point of defense is largely worthless against end-game monsters in the Volcano or difficult Mines.
- Though it can be difficult to build sufficient defense, if enough mods are installed which introduce new ways to stack defense (as with earlier releases of the [Rings](../Rings) module), it becomes possible to essentially negate all damage and trivialize combat.

This module introduces an exponential and multiplicative defense model:
```
resistance = 10 / (10 + defense)
```
One point of defense will now reduce incoming damage by 10% regardless of the enemy's damage, making it a consistently valuable stat throughout the game. Subsequent points in defense, however, will have diminishing returns, such that 100% damage negation is no longer possible to achieve.

Note that this change applies to monsters as well as players! It is also significantly more noticeable on enemies, given the player's inflated damage versus the fact that most monsters have just a few points of defense. Now, those few points can easily cut your damage by half. Crit. strikes have the added benefit of ignoring enemy defense, meaning that critical builds will counter defensive enemies.

### Weapon Stats

Every weapon is given adjusted stats to accomodate and better fit the aforementioned changes. This includes changes to damage, in order to generate a more fluid progression in the Mines and compensate for the removal of weapons from chests. It also includes overall reductions in knockback and swing speed already described above. Criticals, in particular, are quite awkward in vanilla; all weapons share the exact same default crit. stats, with a whopping 300% crit. power and a lousy 2% crit. chance. The reworked stats seek to normalize the large discrepancy between crit. chance and power and also grant some identity to each weapon type.

| Weapon Type | Crit. Chance  | Crit. Power |
| ----------- | ------------- | ----------- |
| Sword       | 1/16 or 6.25% | 200%        |
| Dagger      | 1/8 or 12.5%  | 150%        |
| Club        | 1/32 or 3.12% | 300%        |

Clubs are unreliable crit. weapons, but pack a punch if you putt in the effort to build-up crit. chance.  On the other hand, daggers are reliable crit. weapons, but require some extra crit. multipliers to reach the same damage potential. Note that daggers, as in vanilla, continue to gain slightly more out of each crit. point than other weapons.

### Stabbing Swords & Special Moves

In vanilla game code we can find traces of an abandoned fourth weapon type: **stabbing swords**. We can conclude that Concerned Ape did originally intend for some swords to *not* have the infamously hated and widely ignored defensive parry special move. This module thus reintroduces stabbing swords into the game, along with a brand new special move, which means that sword players can finally have an offensive special move: 
pressing the action button with a stabbing sword equipped will perform a forwards dash attack, which allows for quick repositioning while also damaging enemies along the way. The player is also invincible during the dash.

This is by no means intended to further triviliaze the parry mechanic. On that contrary, parry is made immediately more useful by the introduction of combos and reduced knockback. On top of that, the parry damage has been improved and will now increase by 10% for every point of defense, adding some offensive value to the defense stat and making defensive builds more viable.

Lastly, there is also the option to make the club smash attack more immersive, after all a ground-shaking smash attack should do critical damage to all enemies underground, and should not do any damage at all to enemies in the air.

## Slingshots Rework

Slingshots in vanilla are, surprisingly, way stronger than people give them credit for. But the lack of features surrounding slingshots lends to their lack of credibility as a main weapon. Together with the Rascal profession from the [Professions](../Professions) module, the Arsenal module seeks to change that poor perception and break slingshots into mainstream use by adding critical hit, special move and Forge mechanics.

### Critical Hits

Slingshots can score critical hits! Think of them as headshots. It seems absurd that slingshots should not be able to score crit. hits. With this option enabled, all slingshots will benefit from crit. chance and crit. power bonuses.

### Special Move

Slingshots receive their own special move! This move will perform a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety. Press the action button to trigger this move.

### Forge Mechanics

Slingshots can be enchanted with gemstone forges as well as Prismatic Shards! They receive their own unique enchantments, distinct from melee weapons. For more information keep reading onto the [Enchantments](#enchantments) section.

### Damage Modifiers

In order to accomodate the new mechanics without completely breaking slingshots, the damage modifiers have been nerfed. This is meant to encourage more strategic character building, instead of mindless one-shotting enemies.
- Master Slingshot: Ammo damage x2 >> x1.5
- Galaxy Slingshot: Ammo damage x4 >> x2

Some ammunitions have also been tweaked, either for immersion or balance:
- Coal: 15 damage >> 2 damage
    - *Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo aanyway.*
- Explosive Ammo: 20 damage >> 5 damage
    - *Explosive ammo is meant to be used as a mining utility only, so it's daage has been reduced to reflect that. If you'd like to use slingshots for combat and mining simultaneously, consider taking up the [Rascal](../Professions) profession.*

Lastly, Radioactive Ore can now be used as ammo, dealing considerably more damage than Iridium Ore:
- Radioactive Ore: 80 damage

### Travel Grace Period

In vanilla, you may have noticed that slingshot projectiles will travel right through and ignore enemies that get too close. This is caused by the so-called "grace period", which prevents projectiles from colliding before 100ms, and essentially makes the slingshot a useless weapon in close quarters. This module removes the grace period required before projectiles are allowed to deal damage, making slingshots significantly more reliable.

## Enchantments & Forges

### Gemstone Forges

Forges have been touched slightly. Analogous to its [Rings](../Rings) counterpart, the Jade enchantment has received a significant buff, from +10% to +50% crit. power. If the Rings module is enabled, a new forge will also be added for the Garnet gemstone, granting 10% cooldown reduction. The number of allowed forges for melee weapons now also depends on the weapon's level; basic weapons like the Wooden Blade can only receive 1 gemstone, where as higher level weapons continue receiving up to 3. For slingshots, the number of allowed forges likewise depends on the slingshot level (1 for basic Slingshot, 2 for Master Slingshot, and 3 for Galaxy Slingshot). All gemstones are compatible and apply the same effect as for melee weapons, with the exception of Emerald which increases the charge speed instead of animation speed. If the [Professions](../Professions] module is enabled and the player has the Desperado profession, the Emerald gemstone will also improve the rate of overcharge.

### Weapon Enchantments

Enchantments have been almost entirely overhauled. Hopefully these enchantments will provide more interesting gameplay options.

| Name      | Effect |
| --------- | -------|
| Haymaker  | *Unchanged from vanilla.* |
| Artful    | Improves the special move of each weapon.* |
| Carving   | Attacks on-hit reduce enemy defense, down to a minimum of -1. Armored enemies (i.e., Armored Bugs and shelled Rock Crabs) lose their armor upon hitting 0 defense. |
| Cleaving  | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates Energize stacks, up to 100. At maximum stacks, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Tribute | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. |
| Bloodthirsty | Attacks on-hit steal 5% of enemies' current health. Excess healing is converted into a shield for up to 20% of (the player's) max health, which slowly decays after not dealing or taking damage for 25s. |

\* **Stabbing swords:** Dash distance +20%. **Defense swwords:** Successful parries stun enemies for 1s. **Daggers:** Quick stab hit count +2. **Clubs:** Smash AoE + 50%.

### Slingshot Enchantments

All enchantments below are entirely new and unique to slingshots.

| Name       | Effect |
| ---------- | -------|
| Engorging  | Doubles the size of fired projectiles. |
| Gatling    | Enables auto-fire.* |
| Preserving | 50% chance to not consume ammo. |
| Spreading  | Attacks fire 2 additional projectiles. Extra projectiles deal reduced (60%) damage and do not consume additional ammo.  |
| Quincy     | Attacks fire an energy projectile if no ammo is not equipped. Only works near enemies.** |

\* *Firing speed is lower compared to [Desperado](../Professions)'s Death Blossom. If the Professions module is enabled, auto-firing requires holding the Mod Key (default LeftShift).*

\** *The Quincy projectile cannot crit or knock back enemies, but is affected by damage modifiers. If the Professions module is enabled and the player has the Desperado profession, the Quincy projectile will also be affected by Overcharge, which will also increase the projectile's size.*

## Other Features

This section describes features not specific to weapons or slingshots. It includes novelty features in addition to control improvements and general difficulty sliders.

### Facing Direction & Slick Moves

This popular feature is built-in to this module; when playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor. Additionally, swinging a weapon or charging a slingshot while running will also cause the player to drift in the direction of movement while performing that action, instead of coming to an abrupt halt.

### Infinity +1 Weapons

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind ... , but its power is matched by how hard it is to acquire". If you were ever bothered by how easy it was to obtain the Galaxy and Infinity weapons in vanilla (and immediately trivialize all the rest), this module has got your back, by making these weapons truly legendary.

To you obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the three magic pillars. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of 10 Iridium Bars, which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required. In addition, the awarded weapon will not longer necessarily be a sword, but will be based on the player's most-used weapon type up to that point – this includes the Galaxy Slingshot. All subsequent weapon types continue to be sold by Marlon at the Adventurer's Guild.

/// explain Infinity part

In return for all that extra work, the Infinity weapons receive some extra perks:
    
1. +1 gemstone slot (4 total).
2. While at full health, every swing of an Infinity weapon fires a mid-range energy beam.

### Woody Replaces Rusty

The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with the Wooden Blade.

### Snowballs

This is purely a novelty, for-fun feature. When the player is standing on snowy ground, attempting to fire an empty slingshot will fire a snowball projectile. Now you can annoy the villagers and your friends!

### Difficulty Sliders

Last but not least, this module offers three sliders to taylor monster difficulty to your liking:
- Monster health multiplier
- Monster damage multiplier
- Monster defense multiplier

## Compatibility

- **Not** compatible with other mods that introduce weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] and [Enhanced Slingshots][mod:enhanced-slingshots].
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other Ligo modules.
- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using both together.

<!-- MARKDOWN LINKS & IMAGES -->
[mod:cjb-spawner]: <https://www.nexusmods.com/stardewvalley/mods/93> "CJB Item Spawner"
[mod:angels-rebalance]: <https://www.nexusmods.com/stardewvalley/mods/6894> "Angel's Weapon Rebalance"
[mod:better-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/2067> "Better Slingshots"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
