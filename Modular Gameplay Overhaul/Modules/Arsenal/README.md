
<div align="center">

# ![](https://stardewvalleywiki.com/mediawiki/images/d/da/Steel_Smallsword.png) Modular Overhaul :: Arsenal ![](https://stardewvalleywiki.com/mediawiki/images/6/63/Master_Slingshot.png)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li>
      <a href="#melee-weapon-changes">Melee Weapon Changes</a>
      <ul>
        <li><a href="#combos-swing-speed">Combos & Swing Speed</a></li>
	    <li><a href="#offensive-defensive-swords">Offensive & Defensive Swords</a></li>    
        <li><a href="#weapon-stat-rebalance">Weapon Stat Rebalance</a></li>
        <li><a href="#weapon-retextures">Weapon Retextures</a></li>
        <li><a href="#woody-replaces-rusty">Woody Replaces Rusty</a></li>
      </ul>
    </li>
    <li>
      <a href="#slingshot-changes">Slingshot Changes</a>
      <ul>
        <li><a href="#fire-speed">Fire Speed</a></li>
        <li><a href="#critical-hits">Critical Hits</a></li>
        <li><a href="#special-move">Special Move</a></li>
        <li><a href="#forge-mechanics-infinity-slingshot">Forges Mechanics & Infinity Slingshot</a></li>
        <li><a href="#damage-modifiers">Damage Modifiers</a></li>
        <li><a href="#travel-grace-period">Travel Grace Period</a></li>
        <li><a href="#snowballs">Snowballs</a></li>
      </ul>
    </li>
    <li>
      <a href="#general-combat-changes">General Combat Changes</a>
      <ul>
        <li><a href="#knockback-damage">Knockback Damage</a></li>
        <li><a href="#defense-overhaul">Defense Overhaul</a></li>
      </ul>
    </li>
    <li>
      <a href="#enchantments-forges">Enchantments & Forges</a>
      <ul>
        <li><a href="#gemstone-forges">Gemstone Forges</a></li>
        <li><a href="#weapon-enchantments">Weapon Enchantments</a></li>
        <li><a href="#slingshot-enchantments">Slingshot Enchantments</a></li>
      </ul>
    </li>
    <li><a href="#infinity-1">Infinity +1</a></li>
    <li>
      <a href="#other-features">Other Features</a>
      <ul>
	    <li><a href="#facing-direction-slick-moves">Facing Direction & Slick Moves</a></li>
        <li><a href="#difficulty-sliders">Difficulty Sliders</a></li>
      </ul>
    </li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#faq">F.A.Q.</a></li>
  </ol>
</details>

## Overview

What began as a simple weapon rebalance has become a huge overhaul of weapon mechanics, slingshot mechanics and the entire combat experience. At last, the final module of the original Immersive Suite has arrived.

This module has the following main objectives:
1. To fix the main issues of Melee Weapons and Slingshots as I perceive them, which include the following:
    - Rebalance the weapon types, creating new playstyles by emphasizing the strengths and identity of each type. In vanilla, the sword is ubiquitous because it combines the speed of the dagger with the knockback and range of the club.
    - Rebalance the offensive stats, adding strategic and offensive value to all gemstone archetypes. In vanilla, five out of six weapon stats (crit. chance, crit. power, knockback, speed and precision) are mostly, if not completely, useless: crit. stats tend to be avoided due to the difficulty in building significant crit. chance; knockback and speed are ignored, again because the sword already offers enough of both, and precision does absolutely nothing.
    - Reduce the spammy nature of weapons; in vanilla, the speed stat only affects the last of the six attack animation frames. In addition, the game allows attacks to animation-cancel each other, bypassing that last frame. The result is a mindless combat that consists of spam clicking to both attack and defend. This also adds to the futility of the sword's defensive special move.
    - Rebalance the weapons themselves, making each weapon feel distinct and at least somewhat useful, rather than simple inventory clutter. This includes not just rebalancing stats, but also how they are obtained; i.e., improving weapon progression and removing legendary or unique weapons from Mine chests and shops.
    - Bring slingshots up to par with melee weapons, including the ability to crit., perform special moves and receive Forge enchantments.
2. To increase the longevity of the game by introducing end-game questlines for obtaining legendary weapons which increase the player's sense of accomplishment and makes those weapons feel truly legendary.

This module tries to achieve all of this through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging. Players who don't care for a challenge or are uninsterested in combat should probably keep this whole module disabled.

## Melee Weapon Changes

### Combos & Swing Speed

Introducing weapon combos to replace weapon spamming. These are short bursts of swing speed followed by a slightly longer cooldown period. Each weapon type comes with a combo limit:
    - **Swords** can perform up to 4 horizontal swipes.
    - **Clubs** can perform up to 2 hits, being one horizontal swipe and one vertical swipe.
    - **Daggers** do not have a limit, and behave as in vanilla.

If combo hits are enabled, swing speed bonuses from emerald will affect every frame of the attack animation, as well as the cooldown in between combos. This makes speed a significantly more valuable stat.

### Offensive & Defensive Swords

In vanilla game code we can find traces of an abandoned fourth weapon type: **Stabbing Swords**. This module re-implements stabbing swords into the game.

Swords are now split between **offensive** and **defensive** archetypes. Defensive swwords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames.

To improve the defensive parry move, parry damage is increased by 10% for every defense point. This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during combo downtime).

Lastly, there is also the option to make the club smash attack more immersive, after all a ground-shaking smash attack should do critical damage to all enemies underground, and should not do any damage at all to enemies in the air.

### Weapon Stat Rebalance

Weapon stats have been rebalanced across the board:

 - **Clubs** are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
 - **Daggers** are the opposite of clubs, being the quick, precise, but short-range and generally-lower-damage potential weapons.
 - **Offensive Swords** are somewhere in the middle, with average damage, speed and precision.
 - **Defensive Swords** are marginally weaker and slower than their offensive counterpart, but are otherwise heavier, sturdier and wider. They are somewhere in-between Offensive Swords and Clubs. 

Weapons are categorized by diffrent tiers, [color-coded for your convenience][tropes:color-coded]:

![shield:common]
![shield:uncommon]
![shield:rare]
![shield:epic]
![shield:mythic]
![shield:masterwork]
![shield:legendary]

Weapons below the Mythic tier have randomized damage, determined the moment they are dropped and scaled to your mine progression. This way, players can always loot stronger weapons, and no specific weapon is ever trivialized. Higher-tier weapons will generally be stronger than lower-tiered ones, although that may not always be the case. These weapons can still be purchased from Marlon, but they will have fixed and significantly weaker stats.

Mythic weapons are unique or extremely rare. They are usually quest rewards or rare monster drops, and tend to carry special perks in addition to their regular stats.

Masterwork weapons have fixed stats. These are weapons created by the Dwarven race using special materials. To obtain them, you will have to uncover the lost Dwarvish Blueprints and take them to a skilled blacksmith, along with the required materials:
- **Elven** weapons, carved out of **Elderwood** obtained from [Scavenger Hunts](../Professions), are quick, nimble weapons enchanted with forest magic which grants them high reach and knockback.
- **Dwarven** weapons, forged from **Dwarven Scraps** obtained from Volcano chests, are large, bulky weapons. They grant high defense and knockback, but reduce speed.
- **Dragonbone** weapons, forged from **Dragon Teeth** obtained from Volcano chests and near dragon skeletons, are light and sharp, granting the highest raw damage out of any weapon in the game.

Last but not least, Legendary weapons, will require [considerable work to obtain](#infinity-one).

All weapons have been removed from Mine chests. They will still offer valuable loot, but weapons will require some work to obtain.

Weapon tooltips have also been improved for clarity, so you should always know exactly what each stat means.

### Weapon Retextures

Some weapons have been slightly retouched to look more realistic or to complement their new weapon type. These textures will always be overwritten by Content Patcher weapon retextures.

### Woody Replaces Rusty

The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with the Wooden Blade.

## Slingshot Changes

### Fire Speed

Fire speed is now affected by Emerald speed boosts. This also affects Overcharge if the [Professions](../Professions) module is enabled.

### Critical Hits

Slingshots can score critical hits! Think of them as headshots. With this option enabled, all slingshots will benefit from crit. chance and crit. power bonuses.

### Special Move

Slingshots receive their own special move! This move will perform a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety. Press the action button to trigger this move.

### Forge Mechanics & Infinity Slingshot

Slingshots can be enchanted with gemstone forges as well as Prismatic Shards! They receive their own unique enchantments, distinct from melee weapons. For more information keep reading onto the [Enchantments](#enchantments) section.

The Galaxy Slingshot can likewise be enchanted with Galaxy Soul, being upgradeable to Infinity Slinsghot after 3 enchantments.

### Damage Modifiers

In order to accomodate the new mechanics without completely breaking slingshots, the damage modifiers have been nerfed. This is meant to encourage more strategic character building, instead of mindless one-shotting enemies.
- Master Slingshot: Ammo damage x2 >> x1.5
- Galaxy Slingshot: Ammo damage x4 >> x2
- (NEW) Infinity Slingshot: x2.5

Some ammunitions have also been tweaked, either for immersion or balance:
- Coal: 15 damage >> 2 damage
    - *Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo anyway.*
- Explosive Ammo: 20 damage >> 5 damage
    - *Explosive ammo is meant to be used as a mining utility only, so it's daage has been reduced to reflect that. If you'd like to use slingshots for combat and mining simultaneously, consider taking up the [Rascal](../Professions) profession.*

Lastly, Radioactive Ore can now be used as ammo, dealing considerably more damage than Iridium Ore.

### Travel Grace Period

In vanilla, you may have noticed that slingshot projectiles will travel right through and ignore enemies that get too close. This is caused by the so-called "grace period", which prevents projectiles from colliding before 100ms, and essentially makes the slingshot a useless weapon in close quarters. This module removes the grace period required before projectiles are allowed to deal damage, making slingshots significantly more reliable.

### Snowballs

This is purely a novelty, for-fun feature. When the player is standing on snowy ground, attempting to fire an empty slingshot will fire a snowball projectile. Now you can annoy the villagers and your friends!

## General Combat Changes

### Knockback Damage

Knocked-back enemies will take damage porportional to the knockback stat when colliding with a wall or obstacle. This makes knockback a viable offensive stat in addition to its defensive value. It also makes positioning an important strategic element.

### Defense Overhaul

Defense in vanilla is linearly subtracted from damage. There are several problems with this approach which make the defense stat unscalable:
- While a single point of defense can easily mean a 50% damage reduction against early-game Green Slimes, that same point of defense is largely worthless against end-game monsters in the Volcano or difficult Mines.
- Though it can be difficult to build sufficient defense, if enough mods are installed which introduce new ways to stack defense (as with earlier releases of the [Rings](../Rings) module), it becomes possible to essentially negate all damage and trivialize combat.

This module introduces an exponential and multiplicative defense model:
```
resistance = 10 / (10 + defense)
damage = rawDamage * resistance
```
One point of defense will now reduce incoming damage by 10% regardless of the enemy's damage, making it a consistently valuable stat throughout the game. Subsequent points in defense, however, will have diminishing returns, such that 100% damage negation is no longer possible to achieve.

Note that this change applies to monsters as well as players! It is also significantly more noticeable on enemies, given the player's inflated damage versus the fact that most monsters have just a few points of defense. Now, those few points can easily cut your damage by half. Crit. strikes have the added benefit of ignoring enemy defense, meaning that critical builds will counter defensive enemies.

## Enchantments & Forges

### Gemstone Forges

Forges have been touched slightly. Analogous to its [Rings](../Rings) counterpart, the Jade enchantment has received a significant buff, from +10% to +50% crit. power. If the Rings module is enabled, a new forge will also be added for the Garnet gemstone, granting 10% cooldown reduction.

The number of allowed forges for melee weapons now also depends on the weapon's level (up to three). Likewise for slingshots, the number of allowed forges increases by one with each upgrade.

### Weapon Enchantments

Enchantments have been almost entirely overhauled. Hopefully these enchantments will provide more interesting gameplay options.

| Name      | Effect |
| --------- | -------|
| Haymaker  | *Unchanged from vanilla.* |
| Artful    | Improves the special move of each weapon.* |
| Carving   | Attacks on-hit reduce enemy defense, down to a minimum of -1. Armored enemies (i.e., Armored Bugs and shelled Rock Crabs) lose their armor upon hitting 0 defense. |
| Cleaving  | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates Energize stacks, up to 100. At maximum stacks, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Tribute | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive kill, resetting when you take damage. |
| Bloodthirsty | Attacks on-hit steal 5% of enemies' current health. Excess healing is converted into a shield for up to 20% of (the player's) max health, which slowly decays after not dealing or taking damage for 25s. |

\* **Offensive Swords:** Dash distance +20%. **Defensive Swords:** Successful parries stun enemies for 1s. **Daggers:** Quick stab hit count +2. **Clubs:** Smash AoE + 50%.

### Slingshot Enchantments

All enchantments below are entirely new and unique to slingshots.

| Name       | Effect |
| ---------- | -------|
| Engorging  | Doubles the size of fired projectiles. |
| Gatling    | Enables auto-fire.* |
| Preserving | 50% chance to not consume ammo. |
| Spreading  | Attacks fire 2 additional projectiles. Extra projectiles deal reduced (60%) damage and do not consume additional ammo.  |
| Quincy     | Attacks fire an energy projectile if no ammo is equipped. Only works near enemies.** |

\* *Firing speed is lower compared to [Desperado](../Professions)'s Death Blossom. If the Professions module is enabled, auto-firing requires holding the Mod Key (default LeftShift).*

\** *The Quincy projectile cannot crit or knock back enemies, but is affected by damage modifiers. If the Professions module is enabled and the player has the Desperado profession, the Quincy projectile will also be affected by Overcharge, which will also increase the projectile's size.*

## Infinity +1

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind ... , but its power is matched by how hard it is to acquire". If you were ever bothered by how easy it was to obtain the Galaxy and Infinity weapons in vanilla (and immediately trivialize all the rest), this module has got your back, by making these weapons truly legendary.

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired at the desert, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the top of the backpack.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the all-new Blade of Ruin...

In return for all that extra work, the Infinity weapons have extra perks:    
1. +1 gemstone slot (4 total). Keeping in mind that each gemstone can resonate with equipped [Infinity Bands](../Rings).
2. While at full health, every swing will fire a mid-range energy beam.

## Other Features

This section describes features not specific to weapons or slingshots. It includes novelty features in addition to control improvements and general difficulty sliders.

### Facing Direction & Slick Moves

This popular feature is built-in to this module; when playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor. Additionally, swinging a weapon or charging a slingshot while running will also cause the player to drift in the direction of movement while performing that action, instead of coming to an abrupt halt.

### Difficulty Sliders

Last but not least, this module offers three sliders to taylor monster difficulty to your liking:
- Monster health multiplier
- Monster damage multiplier
- Monster defense multiplier

## Compatibility

- **Not** compatible with other mods that introduce weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] or [Enhanced Slingshots][mod:enhanced-slingshots].
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using both together.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and prevent Tempered Galaxy Weapons from appearing in shops.
- Compatible with [Vanilla Tweaks][mod:vanilla-tweaks], and will use compatible textures if that mod is installed.

## F.A.Q.

**How do I unlock Clint's Forging mechanic?**

Have the Dwarvish Translation Guide and at least 6 hearts with Clint, then enter Clint's shop once you have found the first Dwarvish Blueprint.

**Where can I find the Dwarvish Blueprints?﻿**

They can be found along with their corresponding crafting materials, described above in the Weapons section.

**Where can I find the Blade of Ruin?**

At the end of the single-floor [Quarry Mine](https://stardewvalleywiki.com/Quarry_Mine) from the statue of the Grim Reaper.

**How do I lift the Ruined Blade's curse?**

Prove your Honor, Compassion and Wisdom by selecting certain responses during character heart events (you will have at least 8 chances to prove each of these virtues at least 3 times). Prove your Valor by completing at least 5 monster slayer quests. Finally, prove your Generosity by purchasing the house upgrade for Pam. When you are ready, approach Yoba's altar in Pierre's house.

**What are the IDs of heart events related the Blade of Ruin?**

The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the Event Repeater mod is installed.


Events where you may demonstrate Honor:
7 - Maru 4 hearts
16 - Pierre 6 hearts
27 - Sebastian 6 hearts
36 - Penny 6 hearts
46 - Sam 4 hearts
58 - Harvey 6 hearts
100 - Kent 3 hearts
288847 - Alex 8 hearts
2481135 - Alex 4 hearts
733330 - Sam 3 hearts

Events where you may demonstrate Compassion:
13 - Haley 6 hearts
27 - Sebastian 6 hearts
51 - Leah 4 hearts
100 - Kent 3 hearts
288847 - Alex 8 hearts
502969 - Linus 0 hearts
503180 - Pam 9 hearts
733330 - Sam 3 hearts
3910975 - Shane 6 hearts

Events where you may demonstrate Wisdom:
11 - Haley 2 hearts
21 - Alex 5 hearts
25 - Demetrius 3 hearts
27 - Sebastian 6 hearts
34 - Penny 2 hearts
50 - Leah 2 hearts
56 - Harvey 2 hearts
97 - Clint 3 hearts

**How do I obtain the Infinity weapons?**

Unforge the Blade of Dawn to obtain a Hero Soul, and then forge that into any Galaxy weapon after 3 Galaxy Souls.

**What other unique or mythic weapons can be found?**

You can still obtain the Neptune's Glaive and Broken Trident from Fishing Chests. The Lava Katana can be dropped from Magma Sprites, and the Obsidian Edge can be dropped from Shadow people in the dangerous Mines. Lastly, the Insect Head can potentially become your strongest weapon.

<!-- MARKDOWN LINKS & IMAGES -->
[shield:common]: <https://img.shields.io/badge/Common-white?style=flat>
[shield:uncommon]: <https://img.shields.io/badge/Uncommon-green?style=flat>
[shield:rare]: <https://img.shields.io/badge/Rare-blue?style=flat>
[shield:epic]: <https://img.shields.io/badge/Epic-purple?style=flat>
[shield:mythic]: <https://img.shields.io/badge/Mythic-red?style=flat>
[shield:masterwork]: <https://img.shields.io/badge/Masterwork-orange?style=flat>
[shield:legendary]: <https://img.shields.io/badge/Legendary-gold?style=flat>

[mod:cjb-spawner]: <https://www.nexusmods.com/stardewvalley/mods/93> "CJB Item Spawner"
[mod:angels-rebalance]: <https://www.nexusmods.com/stardewvalley/mods/6894> "Angel's Weapon Rebalance"
[mod:better-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/2067> "Better Slingshots"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:vanilla-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/10852> "Vanilla Tweaks"
[mod:sve]: <https://www.nexusmods.com/stardewvalley/mods/3753> "Stardew Valley Expanded"
[tropes:color-coded]: <https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience> "Color-Coded for Your Convenience"