<table align="center"><tr><td align="center" width="9999">

# Ligo :: Arsenal

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#intro">Intro</a></li>
    <li>
      <a href="#melee-weapons">Melee Weapons</a>
      <ul>
        <li><a href="#knockback">Knockback</a></li>
        <li><a href="#combos-swing-speed">Combos & Swing Speed</a></li>
        <li><a href="#critical-hits">Critical Hits</a></li>
        <li><a href="#stabbing-swords">Stabbing Swords</a></li>
        <li><a href="#enchantments-forged">Enchantments & Forges</a></li>
        <li><a href="#special-moves">Special Moves</a></li>
      </ul>
    </li>
    <li>
      <a href="#slingshots">Slingshots</a></li>
      <ul>
        <li><a href="#critical-hits">Critical Hits</a></li>
        <li><a href="#special-move">Special Move</a></li>
        <li><a href="#enchantments-forged">Enchantments & Forges</a></li>
        <li><a href="#damage-modifiers">Damage Modifiers</a></li>
        <li><a href="#travel-grace-period">Travel Grace Period</a></li>
        <li><a href="#snowballs">Snowballs</a></li>
      </ul>
    </li>
    <li>
      <a href="#general">General</a></li>
      <ul>
        <li><a href="#defense-overhaul">Defense Overhaul</a></li>
        <li><a href="#knockback-overhaul">Knocback Overhaul</a></li>
        <li><a href="#infinity-1-weapons">Infinity +1 Weapons</a></li>
        <li><a href="#woody-replaces-rusty">Woody Replaces Rusty</a></li>
        <li><a href="#facing-direction-slick-moves">Facing Direction & Slick Moves</a></li>
      </ul>
    </li>
    <li><a href="#recommended-mods">Compatibility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
  </ol>
</details>

</td></tr></table>

## Overview

What began as a simple weapon rebalance has become a huge overhaul of weapon mechanics, slingshot mechanics and combat itself. At last, the final module of the original Immersive Suite has arrived.

Please note that existing melee weapons may not be affected if this mod is enabled mid-save. Please use [CJB Item Spawner][mod:cjb-spawner] or similar to delete and respawn all owned melee weapons after enabling or disabling this module.

## Melee Weapons

One of the issues with melee weapons in vanilla is the supremacy of the sword, which combines the speed of the dagger with the knockback of the club. Several mods have attempted to balance this issue by increasing the knockback of the dagger, or increasing the speed of the club. But that solution is disingenuous in that all it does is make every weapon behave like a sword; the identity of each weapon type is lost in the process.

Another issue is the futility of most weapon stats; in vanilla, damage is king when choosing rings and forges, and the remaining five weapon stats (crit. chance, crit. power, knockback, speed and precision) are mostly, if not completely, useless. Crit. stats tend to be avoided due to the difficulty in building significant crit. chance; knockback and speed are ignored, again because the sword already offers enough of both, and precision does absolutely nothing.

This module seeks to fix all of these issues, rebalance all three weapon types while preserving their identities, and also add strategic value to all buildable weapon stats, allowing players to experiment a variety of different builds and playstyles. To achieve this, this module uses a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more challenging. Players who don't care for a challenge or are uninsterested in combat should probably keep this whole module disabled.

### Knockback

Knockback has been nerfed across the board. In vanilla, the default knockback for swords is the sweet spot, and the higher knockback offered by clubs is often more of a nuisance than a perk. Now, club knockback has become the sweet spot, and sword users will be forced to build knockback bonuses to achieve the same effect.

### Combos & Swing Speed

Rather than naively nerfing or buffing the swing speed of different weapons, we notice that the real issue lies deeper: in the spammy nature of weapons. Taking inspiration from the Haunted Chocolatier trailer, this module introduces **weapon combos**, which allow short bursts of swing speed followed by a longer cooldown period. Sword combos can perform up to 3 hits, maintaining the above-average speed of swords but preventing them from being spammed continously. Club combos can perform up to 2 hits only, which should give them an edge over their vanilla counterparts while remaining significantly slower than swords. Daggers cannot combo; they remain unchanged from vanilla.

Swing speed bonuses from emerald rings and forges have also been revamped. In vanilla, these bonuses would only reduce the duration of the final frame of the attack, which in fact consists of six frames in total. Now, every single frame will benefit from the speed stat, as well as the cooldown in between combos. This should make speed a significantly more valuable stat, making fast-hitting club builds a viable option.

Note that daggers, unlike the other two weapon types, have single-frame animations. As such, their attacks are near-instantaneous by default, and thus will gain little to no benefit from swing speed bonuses even with these changes.

### Critical Hits

Criticals are quite awkward in vanilla; the base crit. multiplier of all weapons is a whopping 300%, but the base chance to score a crit. is a lousy 2%. This module seeks to normalize this discrepancy. Each weapon type has unique and inversely proportional base crit. chance and crit. power:

| Weapon Type | Crit. Chance  | Crit. Power |
| ----------- | ------------- | ----------- |
| Sword       | 1/16 or 6.25% | 200%        |
| Dagger      | 1/8 or 12.5%  | 150%        |
| Club        | 1/32 or 3.12% | 300%        |

This makes clubs unreliable as crit. weapons, but capable of packing a punch if putting the effort to build-up the crit. chance. On the other hand, daggers are reliable crit. weapons, but require some extra crit. multipliers to reach the damage potential of the other weapon types. This is supported by the enchantment changes further below, as well as the [Rings](../Rings) module.

Note that daggers, as in vanilla, continue to gain slightly more out of each crit. point than other weapons.

### Stabbing Swords

In vanilla game code we can find traces of an abandoned fourth weapon type: **stabbing swords**. We can conclude that Concerned Ape did originally intend for some swords to *not* have the infamously hated and widely ignored defensive parry special move. This module thus reintroduces stabbing swords into the game, along with a brand new special move, which means that sword players can finally have an offensive special move: 
pressing the action button with a stabbing sword equipped will perform a forwards dash attack, which allows for quick repositioning while also damaging enemies along the way. The player is also invincible during the dash.

### Enchantments & Forges

Forges have been touched slightly. Analogous to its [Rings](../Rings) counterpart, the Jade enchantment has received a significant buff, from +10% to +50% crit. power. If the Rings module is enabled, a new forge will also be added for the Garnet gemstone, granting 10% cooldown reduction. The number of allowed forges for melee weapons now also depends on the weapon's level; basic weapons like the Wooden Blade can only receive 1 gemstone, where as higher leevel weapons continue receiving up to 3.

The real neat changes, though, are with the weapon enchantments, which have been almost entirely overhauled. Hopefully these enchantments will provide more interesting gameplay options.

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

### Special Moves

In order to make defense swords and the parry mechanic less useless (especially given the addition of stabbing swords). This module allows the player's parry damage to increase by 10% for every point of defense. This adds offensive value to the defense stat and a bit more viability for defensive builds.

There is also the option to make the club smash attack more immersive, after all a ground-shaking smash attack should do critical damage to all enemies underground, and should not do any damage at all to enemies in the air.



## Slingshots

Slingshots in vanilla are, surprisingly, way stronger than people give them credit for. But the lack of features surrounding slingshots lends to their lack of credibility as a main weapon. Together with the Rascal profession from the [Professions](../Professions) module, the Arsenal module seeks to change that poor perception and break slingshots into mainstream use by affording to them the same "ammenities" as melee weapons.

### Critical Hits

Slingshots can score critical hits! Think of them as headshots. It seems absurd that slingshots should not be able to score crit. hits. With this option enabled, all slingshots will benefit from crit. chance and crit. power bonuses.

### Special Move

Slingshots receive their own special move! This move will perform a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety. Press the action button to trigger this move.

### Enchantments & Forges

Slingshots are enchantable at the Forge! Like melee weapons, slingshots can be forged with gemstones and enchanted with Prismatic Shards. The number of allowed forges depends on the slingshot level (1 for basic Slingshot, 2 for Master Slingshot, and 3 for Galaxy Slingshot). All gemstones are compatible and apply the same effect as for melee weapons.

Melee weapon enchantments, on the other hand, are not compatible with slingshots. Instead, they gain especially-made brand-new ones:

| Name       | Effect |
| ---------- | -------|
| Engorging  | Doubles the size of fired projectiles. |
| Gatling    | Enables auto-fire.* |
| Preserving | 50% chance to not consume ammo. |
| Spreading  | Attacks fire 2 additional projectiles. Extra projectiles deal reduced (60%) damage and do not consume additional ammo.  |
| Quincy     | Attacks fire an energy projectile if no ammo is not equipped. Only works near enemies.** |

\* *Firing speed is lower compared to [Desperado](../Professions)'s Death Blossom. If the Professions module is enabled, auto-firing requires holding the Mod Key (default LeftShift).*

\** *The Quincy projectile cannot crit or knock back enemies, but is affected by damage modifiers. If the Professions module is enabled and the player has the Desperado profession, the Quincy projectile will also be affected by Overcharge, which will also increase the projectile's size.*

### Damage Modifiers

In order to accomodate all the aforementioned new mechanics without completely breaking slingshots, the damage modifiers have been nerfed. This is meant to encourage more strategic character building, instead of mindless one-shotting enemies.

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

In vanilla, you may have noticed that slingshot projectiles will travel right through and ignore enemies that get too close. This essentially makes the slingshot a useless weapon in close quarters. This module removes the grace period required before projectiles are allowed to deal damage, making slingshot a more reliable offensive option.

### Snowballs

This is purely a novelty, for-fun feature. When the player is standing on snowy ground, attempting to fire an empty slingshot will fire a snowball projectile. Now you can annoy the villagers and your friends!



## General

In addition to weapon and slingshot-specific changes already described, this module also alters certain mechanics of general combat to create a distinct combat experience.

### Defense Overhaul

Defense in vanilla is linearly subtracted from damage. There are several problems with this approach which make  makes the defense stat simultaneously useless and overpowered:

- While a single point of defense can easily mean a 50% damage reduction against early-game Green Slimes, that same point of defense is largely worthless against an end-game monster in the difficult Mines. Moreover, any damage increase applied to monsters by other mods likewise contribute to making defense less and less valuable.
- Though it can be difficult to build sufficient defense, if enough mods are installed which introduce new ways to stack defense (as with earlier releases of the [Rings](../Rings) module), it becomes possible to essentially negate all damage and trivialize combat.

It becomes clear that the linear, subtractive defense model from vanilla is unscalable and inadequate. This module fixes that by introducing an exponential and multiplicative defense model:
```
resistance = 10 / (10 + defense)
```
One point of defense will now reduce incoming damage by 10% regardless of the enemy's damage, making it a consistently valuable stat throughout the game. Subsequent points in defense, however, will have diminishing returns, such that 100% damage negation is no longer possible to achieve.

This change also has the notable side-effect of allowing the use of fractional defense points, whereas only full integers could be used in vanilla. This will be relevant if the player decides to enable the [Rings](../Rings) module and the associated Resonance mechanic.

Also note that the defense changes apply to monsters as well as players! This change is a lot more noticeable on enemies since the player's damage is more inflated; i.e., a few points of defense in vanilla will make little difference against the player's 100+ damage. With this setting, monsters will now be able to resist your a significant portion of damage. Crit. strikes have the added benefit of ignoring enemy defense, meaning that critical builds will counter certain enemies.

### Knockback Overhaul

Though it has been nerfed for melee weapons, the knockback stat overall has become much more useful: knocked-back enemies will now take damage porportional to the knockback stat when colliding with a wall or obstacle. This makes knockback a viable offensive stat in addition to its defensive value.

### Infinity +1 Weapons

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind ... , but its power is matched by how hard it is to acquire". If you were ever bothered by how easy it was to obtain the Galaxy and Infinity weapons in vanilla (and immediately trivialize all the rest), this module has got your back, by making these weapons truly legendary.

To you obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the three magic pillars. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of 10 Iridium Bars, which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required. In addition, the awarded weapon will not longer necessarily be a sword, but will be based on the player's most-used weapon type up to that point – this includes the Galaxy Slingshot. All subsequent weapon types continue to be sold by Marlon at the Adventurer's Guild.

/// explain Infinity part

In return for all that extra work, the Infinity weapons receive some extra perks:
    
1. +1 gemstone slot (4 total).
2. An exclusive enchantment: while at full health, weapon swings fire a beam of energy capable of hitting enemies at mid-range.

### Woody Replaces Rusty

The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with the Wooden Blade.

### Facing Direction & Slick Moves

This popular feature is built-in to this module; when playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor. Additionally, swinging a weapon or charging a slingshot while running will also cause the player to drift in the direction of movement while performing that action, instead of coming to an abrupt halt.

### Difficulty Sliders

Last but not least, this module offers three sliders to taylor monster difficulty to your liking:
- Monster health multiplier
- Monster damage multiplier
- Monster defense multiplier



## Compatibility

- **Not** compatible with other mods that overhaul weapons or slingshots, such as [Better Slingshots](https://www.nexusmods.com/stardewvalley/mods/2067) and [Enhanced Slingshots](https://www.nexusmods.com/stardewvalley/mods/12763).
- **Not** compatible with the likes of [Combat Controls - Fixed Mouse Click](https://www.nexusmods.com/stardewvalley/mods/2590) or [Combat Controls Ligo](https://www.nexusmods.com/stardewvalley/mods/10496), as those features are already included in this and other Ligo modules.
- Compatible with [Advanced Melee Framework](https://www.nexusmods.com/stardewvalley/mods/7886) and related content packs, but I do not recommend using both together.

## Installation

This module can be enabled on existing saves.

Before uninstalling Ligo or disabling this module, you **must disable BringBackStabbySword**, then load up your save and play through one day to pesist the changes. Failure to do so will cause your swords to become permanently unusable.
