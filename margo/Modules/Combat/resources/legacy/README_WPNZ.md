<div align="center">

# MARGO :: Weapons (WPNZ)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#combos-swing-speed">Combos & Swing Speed</a></li>
	<li><a href="#offensive-defensive-swords">Offensive & Defensive Swords</a></li>    
    <li><a href="#weapon-tiers-rebalance">Weapon Tiers & Rebalance</a></li>
    <ol>
        <li><a href="#mythic-tier">Mythic Tier</a></li>
        <li><a href="#masterwork-tier">Masterwork Tier</a></li>
        <li><a href="#legendary-tier">Legendary Tier</a></li>
        <li><a href="#mines">Mines</a></li>
        <li><a href="#tooltips">Tooltips</a></li>
    </ol>
    <li><a href="#infinity-1">Infinity +1</a></li>
    <li><a href="#other-features">Other Features</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#faq">F.A.Q.</a></li>
  </ol>
</details>

## Overview

What began as a simple weapon rebalance has become a huge overhaul of all Melee Weapon.

This module has the following objectives:
1. To rebalance the weapon types, creating new playstyles by emphasizing the strengths and identity of each type, in order to scale-back the ubiquity of swirds.
2. Reduce the spam-clicky nature of Vanilla Melee Weapons.
3. Rebalance the weapons themselves, making each weapon feel distinct and at least somewhat useful, rather than pure inventory clutter. This includes adding new interesting way to obtain special weapons as well as making the legendary weapons feel truly legendary.

This module tries to achieve all of this through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging. Players who don't care for a challenge or are uninterested in combat should probably keep this module disabled.

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Combos & Swing Speed

Weapon spamming is replaced by combos. These are short bursts of continuous swings followed by a short cooldown. Each weapon type has a configurable combo limit:
    
- **Swords:** up to 4 horizontal swipes, by default.
- **Clubs:** up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
- **Daggers:** unchanged, effectively up to infinite hits.

If combo hits are enabled, swing speed bonuses from emerald will affect every frame of the attack animation, as well as the cooldown in between combos. This makes speed a significantly more valuable stat than in Vanilla, where it only affected 1 out of 6 frames (and that frame could be animation-canceled).

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Offensive & Defensive Swords

In vanilla game code we can find traces of an abandoned fourth weapon type: **Stabbing Swords**. This module re-implements stabbing swords into the game.

Swords are now split between **offensive** and **defensive** archetypes. Defensive swwords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames. You can also change the direction mid-dash by inputing a directional command in a perpendicular direction.

To improve the defensive parry move, parry damage is increased by 10% for every defense point. This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during combo downtime).

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Weapon Tiers & Rebalance

Weapon stats have been rebalanced across the board:

 - **Clubs** are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
 - **Daggers** are the opposite of clubs, being the quick, precise, but short-range and generally-lower-damage potential weapons. **Can cause Bleeding** (requires [CMBT](../Combat/README.md#status-conditions) module).
 - **Offensive Swords** are somewhere in the middle, with average damage, speed and precision.
 - **Defensive Swords** are marginally weaker and slower than their offensive counterpart, but are otherwise heavier, sturdier and wider. They are somewhere in-between Offensive Swords and Clubs. 

Weapons are categorized by different tiers, [color-coded for your convenience][tropes:color-coded]:

![shield:common]
![shield:uncommon]
![shield:rare]
![shield:epic]
![shield:mythic]
![shield:masterwork]
![shield:legendary]

Weapons below the Mythic tier all have randomized damage, determined the moment they are dropped and scaled to your Mine progression. This way, players can always loot stronger weapons; all weapons can be useful, and no specific weapon is ever trivialized. Higher-tier weapons will generally be stronger than lower-tiered ones, although that may not always be the case.

### Mythic Tier

Mythic weapons are unique or extremely rare. They are usually quest rewards or very rare monster drops, and tend to carry intrinsic special abilities. In exchange, they cannot receive additional Prismatic Shard enchantments at the forge.

The known Mythic weapons are:
- **Yeti Tooth**: Can cause Chilled effect.
- **Neptune Glaive**: *No effect, yet...*
- **Obsidian Edge**: Ignores enemy resistances. Can cause Bleeding.
- **Lava Katana**: Can cause Burning.
- **Insect Head**: Damage scales with the number of slain insects.
- **Iridium Needle**: Always crits.

Note that all status conditions require that the [CMBT](../Combat/README.md#status-conditions) module be enabled.

### Masterwork Tier

Masterwork weapons are relics of the Dwarven race, crafted from special materials. They can only be created by uncovering the lost Dwarvish Blueprints, and taking them to a skilled blacksmith along with the corresponding material:
- **Elven** weapons, carved out of **Elderwood** obtained from [Scavenger Hunts](../Professions), are quick, nimble weapons enchanted with forest magic which grants them high reach and knockback.
- **Dwarven** weapons, forged from **Dwarven Scraps** obtained from Volcano chests, are large, bulky weapons. They grant high defense and knockback, but reduce speed.
- **Dragonbone** weapons, forged from **Dragon Teeth** obtained from Volcano chests and near dragon skeletons, are light and sharp, granting the highest raw damage out of any weapon in the game.

Masterwork weapons can only be obtained if the Dwarven Legacy setting is enabled.

### Legendary Tier

See [Infinity+1](#infinity-1).

### Mines

If this option is enabled, all weapons will be removed from Mine chests (replaced with valuable but random loot). New weapons will have to be dropped from monsters or breakable containers in Mines and other dungeons. Monster-dropped weapons are rare, but tend to be stronger. Lower-tier weapons can also be purchased directly from Marlon, but will have fixed and significantly weaker stats when obtained this way.

### Tooltips

Weapon tooltips have also been improved for clarity, so you should always know exactly what each stat means.
This is the only part of the whole Rebalance that will affect non-Vanilla weapons. Therefore, if you play with expansion mods which add new weapons (e.g., Ridgeside Village), you may see unusually large numbers in their tooltips. **This is not a bug**. Those weapons have not been changed in any way. This mod is simply revealing how broken those weapons have always been. For this reason, I **strongly** recommend ignoring these weapons, or even *deleting* them manually from the mod's files.**

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Infinity +1

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind [...] , but its power is matched by how hard it is to acquire". The vanilla Infinity weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired at the desert, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the top of the backpack.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the all-new Blade of Ruin...

In return for all that extra work, the Infinity weapons have extra perks:    
1. +1 gemstone slot (4 total). Keeping in mind that each gemstone can resonate with equipped [Infinity Bands](../Rings).
2. While at full health, every swing will fire a mid-range energy beam.

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Other Features

* **Weapons Retexture:** Available optionally, weapons can be retextured to better reflect their type or rarity. This is strongly recommended to visually distinguish Defensive and Offensive-oriented swords, and to make Mythic and above weapons look more unique and powerful. These textures will always be overwritten by Content Patcher weapon retextures if any is installed.

* **Grounded Club Smash:** Prevents gliders from being hit by the Club's smash attack, but guarantees a critical hit on under-ground Duggies. A controversial but immersive change.

* **Woody Replaces Rusty:** The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with a Wooden Blade.

* **Face Mouse Cursor:** This popular feature found in other mods is built-in to this module; when playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.

* **Slick Moves:** Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced. 

* **Auto-Selection:** If enemies are nearby, players can optionally choose a weapon to be equipped automatically.

* **Novelty Special Effects:** This module additionally adds tons of immersive details to weapon sound and visual effects, like changing the Lava Katana swipe to a fiery effect, and adding colors and particles to Galaxy and Infinity weapons.

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## Compatibility

- **Not** compatible with other mods that introduce new weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using both together.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
- Compatible with [Vanilla Tweaks][mod:vanilla-tweaks], and will use compatible weapon textures if that mod is installed.

<sup><sup>[🔼 Back to top](#margo-weapons-wpnz)</sup></sup>

## F.A.Q.

**How do I unlock the Dwarven relic weapons?**

Have the Dwarvish Translation Guide and at least 6 hearts with Clint, then enter Clint's shop once you have found the first Dwarvish Blueprint. A short cutscene should play, and you will have to wait a random number of days (the higher your friendship points, the shorter the wait). Afterwards, speak to Clint again to unlock the **Forge** option.

**Where can I find the Dwarvish Blueprints?﻿**

They can be found in the exactly the same place as their corresponding weapons would be found in Vanilla (i.e., Volcano chests). The only exception are the Elven blueprints, which can only be obtained from Scavenger Hunts (requires the [Professions](../Professions) module). The corresponding crafting materials are also obtained in the exact same way, as described above in the Weapons section.

**Where can I find the Blade of Ruin?**

At the end of the single-floor [Quarry Mine](https://stardewvalleywiki.com/Quarry_Mine) from the statue of the Grim Reaper.

**What is the Blade of Ruin's curse?**

The Blade of Ruin will grow progressively stronger by cosuming enemies; every 5 enemies slain increases its attack power by 1 point. As it grows stronger, however, it will also begin to consume your own energy, dealing damage-over-time while held. At the same time, the Blade has a nasty habit of auto-equipping itself; the stronger the Blade, the more damage you will suffer, and the more often it will auto-equip itself.
Should you choose to ignore these side-effects and continue to strengthen the Blade, you will eventually become unable to use other weapons, and be forced to engage combat with 1 HP.

**How do I lift the Ruined Blade's curse?**

To begin the quest, you must slay at least 50 enemies with the Blade equipped, prompting the Wizard to invite you over for a chat. To complete this initial quest, simply interact with the Yoba altar and exhaust all possible dialogue choices.
You will then be asked to prove your virtues:
- Prove your Honor, Compassion and Wisdom by selecting certain responses during character heart events (you will have at least 8 chances to prove each of these virtues a number of times).
- Prove your Valor by completing at least monster slayer quests.
- Prove your Generosity by gifting NPCs a certain amount of gold in gifts, or by purchasing the house upgrade for Pam. Exact values will depend on your difficulty settings (you can see them in-game in the your quest journal).

When you are ready, approach Yoba's altar in Pierre's house with the Blade in hand  .

**What are the IDs of heart events related the Blade of Ruin?**

The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the Event Repeater mod is installed.


Events where you may demonstrate Honor:
* 7 - Maru 4 hearts
* 16 - Pierre 6 hearts
* 27 - Sebastian 6 hearts
* 36 - Penny 6 hearts
* 46 - Sam 4 hearts
* 58 - Harvey 6 hearts
* 100 - Kent 3 hearts
* 288847 - Alex 8 hearts
* 2481135 - Alex 4 hearts
* 733330 - Sam 3 hearts

Events where you may demonstrate Compassion:
* 13 - Haley 6 hearts
* 27 - Sebastian 6 hearts
* 51 - Leah 4 hearts
* 100 - Kent 3 hearts
* 288847 - Alex 8 hearts
* 502969 - Linus 0 hearts
* 503180 - Pam 9 hearts
* 733330 - Sam 3 hearts
* 3910975 - Shane 6 hearts

Events where you may demonstrate Wisdom:
* 11 - Haley 2 hearts
* 21 - Alex 5 hearts
* 25 - Demetrius 3 hearts
* 27 - Sebastian 6 hearts
* 34 - Penny 2 hearts
* 50 - Leah 2 hearts
* 56 - Harvey 2 hearts
* 97 - Clint 3 hearts

**How do I obtain the Infinity weapons?**

Unforge the Blade of Dawn to obtain a Hero Soul, and then forge it into any Galaxy weapon after 3 Galaxy Souls.

**How do I obtain other mythic weapons?**

- **Neptune's Glaive:** Fishing Chests, same as Vanilla.
- **Yeti Tooth:** Dropped by enemies or crates in the icy section of the Mines.
- **Obsidian Edge:** Dropped from Shadow people in the dangerous Mines.
- **Lava Katana:** Dropped from certain enemies in Volcano Dungeon.

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
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:vanilla-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/10852> "Vanilla Tweaks"
[mod:sve]: <https://www.nexusmods.com/stardewvalley/mods/3753> "Stardew Valley Expanded"
[tropes:color-coded]: <https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience> "Color-Coded for Your Convenience"

[🔼 Back to top](#margo-weapons-wpnz)