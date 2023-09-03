<div align="center">

# MARGO :: Combat (CMBT)

</div>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
    <li><a href="#status-effects">Status Effects</a></li>
    <li><a href="#rebalanced-stats">Rebalanced Stats</a></li>
    <li><a href="#melee-weapon-changes">Melee Weapon Changes</a></li>
    <ol>
        <li><a href="#combo-framework">Combo Framework</a></li>
        <li><a href="#offensive-defensive-swords">Offensive & Defensive Swords</a></li>
        <li><a href="#rebalanced-types-tiers">Rebalanced Types & Tiers</a></li>
        <li><a href="#melee-enchantments">Melee Enchantments</a></li>
        <li><a href="#acquiring-weapons">Acquiring Weapons</a></li>
        <li><a href="#tooltips">Tooltips</a></li>
    </ol>
    <li><a href="#ranged-weapon-changes">Ranged Weapon Changes</a></li>
    <ol>
        <li><a href="#rebalanced-modifiers">Rebalanced Modifers</a></li>
        <li><a href="#critical-hits">Critical Hits</a></li>
        <li><a href="#special-move">Special Move</a></li>
        <li><a href="#forges-gemstone-ammo">Forges & Gemstone Ammo</a></li>
        <li><a href="#ranged-enchantments">Ranged Enchantments</a></li>
        <li><a href="#infinity-slingshot">Infinity Slingshot</a></li>
        <li><a href="#removed-grace-period">Removed Grace Period</a></li>
        <li><a href="#snowballs">Snowballs</a></li>
    </ol>
    <li><a href="#ring-changes">Ring Changes</a></li>
     <ol>
        <li><a href="#rebalances">Rebalances</a></li>
        <li><a href="#infinity-band">Infinity Band</a></li>
    </ol>
    <li><a href="#infinity-1">Infinity +1</a></li>
    <li><a href="#garnet-gemstone-resonance-theory">Garnet & Gemstone Resonance Theory</a></li>
    <li><a href="#enemies">Enemies</a></li>
    <li><a href="#controls-quality-of-life">Controls & Quality of Life</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#faq">F.A.Q.</a></li>
  </ol>
</details>

## Overview

This humongous module consolidates rebalances to melee weapons, ranged weapons and combat rings, together with entirely new mechanics which will overall make combat significantly more strategic and challenging.

## Status Effects

Taking inspiration from classic RPG or strategy games, this module adds a framework for causing various status conditions to enemies. They are:

- **Bleeding:** Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
- **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4x as quickly.
- **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
- **Frozen:** Cannot move or attack for 30s. The next hit during the duration deals triple damage and ends the effect.
- **Poisoned:** Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
- **Slowed:** Reduces movement speed by half for the duration.
- **Stunned:** Cannot move or attack for the duration.

Each status condition is accompanied by a neat corresponding animation. Status conditions cannot be applied on the player.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Rebalanced Stats

Certain stats are simply not worth investing in vanilla. While some of these can be attributed to unbalanced equipment bonuses (and will be addressed below), others, namely **Knockback** and **Defense** are fundamentally broken.

1. **Knockback** is too high by default; hitting enemies immediately throws them several tiles away. Further down we will address the default knockback of most weapons. But that alone doesn't make this as attractive a stat as flat damage. Hence, enemies will now take damage proportional to their momentum when thrown against a wall or object. The added offensive angle makes this a great stat for clubs in particular. It also makes positioning an important strategic element in combat.
2. **Defense** is rather useless in vanilla, where each point simply mitigates a single unit of damage. Consequently, small defense bonuses become useless in late-game. This mod implements a new, simple damage mitigation formula, which allows defense to scale into late-game without becoming overpowered.

    Old formula:
    ```
    damage = Min(rawDamage - defense, 1)
    ```

    New formula:
    ```
    resistance = 10 / (10 + defense)
    damage = rawDamage * resistance
    ```

    One defense point now reduces damage by 10% regardless of incoming damage. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve. This applies to monsters! The **Topaz Ring** is also changed to increased defense by 1 point, like its corresponding weapon forge.
3. **Critical strikes** are fun, but need a little more oomph to compete with flat damage. As a counter-play to the previous change to defense, this mod makes critical strikes ignore enemy defense. Attacks from behind will also have double the chance to critically strike. But reaching the backs of your enemies will not be easy! You will have to rely on [status effects](#status-effects) to achieve that. The effects of **Jade** bonuses from rings and weapon forges are also significantly buffed.
    - **Jade:** *+10% -> +50% crit. power.* A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, only when the player has 100% crit. chance then they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to a Ruby Ring, which straight up grants a 10% boost to damage, *all the time*. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type given an appropriate build. Above that threshold, Jade Rings become even stronger.
    - Slingshots can now critically strike. Think of them as headshots.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Melee Weapon Changes

Vanilla weapons suffer from two major issues:
    1. Same-y-ness. Apart from their special moves, none of the weapon types feel particularly unique; club and dagger are simply "worse swords".
    2. Overabundance. Weapons quickly become inventory clutter. The game is also quick to gift its most powerful weapons, trivializing any weapons obtained as drops.

This mod tries to fix both through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Combo Framework

Weapon spamming is makes vanilla combat a boring click-fest. This mod implements a **combo framework** for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced cooldown. Each weapon type has a configurable combo limit:
    
- **Swords:** up to 4 horizontal swipes, by default.
- **Clubs:** up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
- **Daggers:** unchanged, effectively up to infinite hits.

If combo hits are enabled, swing speed bonuses from emerald effects will apply to every frame of the attack animation, as well as the cooldown in between combos, as opposed to only the final frame as in vanilla. This immediately makes speed a more valuable stat.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Offensive & Defensive Swords

In vanilla game code we can find traces of an abandoned fourth weapon type: **Stabbing Swords**. This module re-implements stabbing swords into the game.

Swords are now split between **offensive** and **defensive** archetypes. Defensive sw   ords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames. You can also change the direction mid-dash by inputting a directional command in a perpendicular direction.

To improve the defensive parry move, parry damage is increased by 10% for every defense point. This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during combo downtime).

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Rebalanced Types & Tiers

Weapon stats have been rebalanced across the board so as to provide a more clear distinction between the weapon types:

 - **Clubs** are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
 - **Daggers** are the opposite of clubs, being the quick, precise, but short-range and generally-lower-damage potential weapons. **Can cause Bleeding**.
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

**Mythic Tier**

Mythic weapons are unique or extremely rare. They are usually quest rewards or very rare monster drops, and tend to carry intrinsic special abilities. In exchange, they cannot receive additional Prismatic Shard enchantments at the forge.

The known Mythic weapons are:
- **Yeti Tooth**: Can cause Chilled effect.
- **Neptune Glaive**: *No effect, yet...*
- **Obsidian Edge**: Ignores enemy resistances. Can cause Bleeding.
- **Lava Katana**: Can cause Burning.
- **Insect Head**: Damage scales with the number of slain insects.
- **Iridium Needle**: Always crits.

**Masterwork Tier**

Masterwork weapons are relics of the Dwarven race, crafted from special materials. They can only be created by uncovering the lost Dwarvish Blueprints, and taking them to a skilled blacksmith along with the corresponding material:
- **Elven** weapons, carved out of **Elderwood** obtained from [Scavenger Hunts](../Professions), are quick, nimble weapons enchanted with forest magic which grants them high reach and knockback.
- **Dwarven** weapons, forged from **Dwarven Scraps** obtained from Volcano chests, are large, bulky weapons. They grant high defense and knockback, but reduce speed.
- **Dragonbone** weapons, forged from **Dragon Teeth** obtained from Volcano chests and near dragon skeletons, are light and sharp, granting the highest raw damage out of any weapon in the game.

Masterwork weapons can only be obtained if the Dwarven Legacy setting is enabled.

Note that **only vanilla weapons have been rebalanced**. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. **That's not on me.** Those weapons are broken by design. I just pulled the curtains. You're welcome.

**Legendary Tier**

See [Infinity+1](#infinity-1).

**Retexture & FX**

In order to better reflect their new weapon types, as well as their mythical or legendary status, several weapons have received vanilla-friendly retextures. These textures will always be overwritten by any installed Content Patcher mods, so there is no concern for compatibility. Moreover, there are tons of small immersive details like visual and sound effects added to mythic and legendary weapons.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Melee Enchantments

Weapons enchantments have been almost entirely overhauled. Hopefully these enchantments will provide more interesting gameplay options:

| Name      | Effect |
| --------- | -------|
| Haymaker  | *Unchanged from vanilla.* |
| Artful    | Improves the special move of each weapon.* |
| Blasting | Accumulates and stores half of the damage from enemy hits (before mitigation). If enough damage is accumulated, the next special move releases that damage as an explosion. |
| Bloodthirsty | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s. |
| Carving   | Attacks on-hit reduce enemy defense by 1 (continuing below zero). Removes the armor from Armored Bugs and de-shells Rock Crabs. |
| Cleaving  | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Mammonite's | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage. |
| Steadfast    | Converts critical strike chance into bonus raw damage (multiplied by critical power). |
| Wabbajack | Causes unpredictable effects.** |

\*
***Offensive Swords:** Can dash twice in succession. **Defensive Swords:** The next attack within 5s of a successful parry is guaranteed a critical strike. **Daggers:** Quick stab deals an additional hit. All hits also apply Bleed with 100% chance. **Clubs:** Smash area +50%. Enemies in range are stunned for 2s.*

\*\* *Examples: damage or heal the enemy; decrease or increase the enemie's stats; transfigure into a different enemy, creature or any random item (note: this can spawn illegal items).*

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Other Features

* **The Mines & Weapon Acquisition**
    In vanilla, the player is quick to amass large quantities of fodder weapons, both from Mine chests as well as barrel and crate drops. To support the intended experience of progression, all weapons are removed from Mine chests, which instead reward random goodies. In order to obtain new weapons, players will have to fight for monster dropped from monsters or breakable containers in Mines and other dungeons. Monster-dropped weapons are rare, but tend to be stronger. Lower-tier weapons can also be purchased directly from Marlon, but will have fixed and significantly weaker stats when obtained this way.

* **Tooltips**

    Vanilla weapon tooltips are confusing. Who the heck knows what "+1 Speed" means?
    
    This mod makes improves weapon tooltips for clarity, so you always know exactly what each stat means. You may configure the tooltips to show **absolute** or **relative** stats; the former displays straight-forward raw stats, while the latter displays percentages **based on the weapon type's base stats**.

    Note that this is the only feature of this mod that affects third-party mod weapons. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. **That's not on me.** Those weapons are broken by design. I just pulled the curtains. You're welcome.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Ranged Weapon Changes

Ranged weapons are actually quite strong in vanilla, mainly because ammo's deal insane damage. They are also boring, however, since ranged combat is very unidimensional. This mod seeks to alleviates that by reducing base ammo damage while also introducing many of the same mechanics afforded to melee weapons.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Rebalanced Modifiers & Ammo

To make room for critical headshots and the new Infinity Slingshot the base damage and knockback modifiers of each slingshot are reduced to more reasonable values:
- Master Slingshot: Ammo damage x2 >> x1.5
- Galaxy Slingshot: Ammo damage x4 >> x2
- **Infinity Slingshot:** x2.5

The following ammos have been nerfed, either for immersion or balance:
- Coal: 15 damage >> 2 damage - *Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo anyway.*
- Explosive Ammo: 20 damage >> 2 damage ﻿- *Explosive ammo is meant to be used as a mining utility **only**, so it's damage has been reduced to reflect that. If you'd like to use slingshots for combat and mining simultaneously, consider taking up the [Rascal](../Professions)'s extra ammo slot.*

The following new ammos have been added:
- Radioactive Ore: 80
- Gemstones (Ruby, Emerald, etc.): 50
- Diamond: 120
- Prismatic Shard: 200

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Special Move

Pressing the action button will trigger a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Ranged Enchantments

The following new enchantments have been created specifically for ranged weapons:

| Name       | Effect |
| ---------- | -------|
| Gatling    | Enables auto-fire.* |
| Magnum     | Fires enlarged projectiles. |
| Preserving | Does not consume ammo. |
| Quincy     | Attacks fire an energy projectile if no ammo is equipped. Only works near enemies.** |
| Spreading  | Consume one additional ammo to fire two additional angled projectiles. |

\* *Double-click/press and then **hold** the use-tool key to engage auto-fire.*

\** *Quincy projectile cannot crit nor knock back enemies, but is affected by damage modifiers. If [PROFS](../Professions) is enabled and the player has the Desperado profession, Quincy projectile will also be affected by Overcharge.*

Gemstone forges cannot directly be applied to slingshots, but **gemstones can be equipped as ammo**, and will apply their analogous bonuses when equipped, and will also [resonate](#resonances) if applicable. With the [Rascal](../Professions) profession, you may slot up to two gemstones at a time to possibly achieve a level-2 forge.

* **Emeralds**, instead of attack speed, grant **firing speed** (idem for Emerald Rings), which also affects **overcharge** if the [Professions](../Professions) module's Desperado profession is used;

Lastly, the **Galaxy Soul** can be applied to the Galaxy Slingshot, as with other Galaxy weapons, to eventually create the **Infinity Slingshot.**

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Other Features

* **Removed Grace Period**

    Vanilla slingshots are unable to hit enemies in close-range of the player; these shots will fly straight through them. This mod removes this limitation, making ranged combat more reliable.

* **Snowballs**

    Standing in a snowy tile with an empty slingshot will allow the player to fire a snowball. The snowball projectile deals no significant damage; this is meant as a fun flavor feature for multiplayer.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Ring Changes

Most combat-oriented rings in vanilla are underwhelming and completely overlooked next to the Iridium Band, which provides a free 3-in-1 ring that can also be combined. This mod tries to make combat rings more interesting, and introduce all-new mechanics specific to the Iridium Band.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Rebalanced Combat Rings

This following rings have been changed:
- **Warrior Ring:** ~~Chance of giving the Warrior Energy~~ (attack +10 for 5s) ~~buff after slaying a monster.~~ -> Gain a progressively higher attack bonus as you slay enemies (every 3 enemies increases attack by 1), which falls off gradually after some time out of combat.
- **Ring of Yoba:** ~~Chance of giving the Yoba's Blessing~~ (invincible for 5s) ~~buff after taking damage.~~ -> Taking damage that would leave you below 30% health instead grants a shield that absorbs up to 50% of your maximum health for 30s. Cannot be used again until health is fully recovered.
- **Savage Ring:** ~~+2 Speed for 3s after slaying a monster.**~~ -> Gain a rapidly decaying Speed buff after slaying a monster.
- **Immunity Ring:** ~~Immunity +4.~~ -> Gain 100% immunity.
- **Ring of Thorns:** Can cause Bleeding status (in addition to reflected damage).

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

### Infinity Band

The Iridium Band has been completely overhauled. Initially, a newly crafted Iridium Band will grant no effects at all. Only with access to the Forge will you be able to awaken its true form by infusing it with a Galaxy Soul to transform it into an **Infinity Band**.

The Infinity Band likewise does nothing on its own, but it serves as a vessel for up to **four** gemstones. To add a gemstone to the Infinity Band, you must fuse it with a corresponding gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones will potentially lead to powerful [resonances](#resonance).

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Infinity +1

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind [...] , but its power is matched by how hard it is to acquire". The vanilla Infinity weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired at the desert, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the top of the backpack.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the all-new Blade of Ruin...

In return for all that extra work, the Infinity weapons have extra perks:    
1. +1 gemstone slot (4 total). Keeping in mind that each gemstone has the potential to [resonate](#resonance).
2. **Melee only:** while at full health, every swing will fire a mid-range energy beam.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Garnet & Gemstone Resonance Theory

To compensate for the [removal of vanilla Acrobat profession](../Professions), this mod introduces a seventh gemstone.

The **garnet** can be mined upwards of Mine level 80. Socketed to a ring or a weapon, it will grant 10% cooldown reduction to special moves. Garnet Rings, along with all other gemstone rings become craftable at various levels of Combat skill.

With the addition of Garnet, the seven gemstones together form a [Diatonic Scale](https://en.wikipedia.org/wiki/Diatonic_scale):

<div align="center">

![shield:rb] -> ![shield:aq] -> ![shield:am] -> ![shield:ga] -> ![shield:em] -> ![shield:jd] -> ![shield:tp]

</div>

The scale is cyclic, so after Tp comes Rb again, and so on. The first note in the scale is called the **Tonic**, or **Root**. Above, Rb was used as an example, but the scale can be shifted, or *transposed*, to place any gemstone at the root. But regardless of the root note, the order is always the same.

Like strings in a guitar, the characteristic vibration of each gemstone causes interference patterns. These interferences can be constructive and/or destructive, and they create complex [overtones](https://en.wikipedia.org/wiki/Overtone) that add richness to the resulting vibrations, known as [Harmonies](https://en.wikipedia.org/wiki/Harmony). In other words, certain gemstones will harmonize together, creating resonances that amplify their individual effects. At the same time, other gemstone pairs will lead to dissonances, which instead dampen those effects. As a rule of thumb, Gemstones that are positioned farthest from each other in the Diatonic Scale will resonate more strongly, and those positioned adjacent to each other will dissonate. This means that the interval `I - V` (for example, `Rb - Em`, `Am - Tp`, `Ga - Rb` etc.) will lead to the strongest resonance, while the interval `I - II` will lead to a dissonance (for example, `Rb - Aq`, `Am - Ga`, `Tp - Rb`, etc.).

Gemstones placed together in an Infinity Band not only resonate, but can also make up [Chords](https://en.wikipedia.org/wiki/Chord_(music)). Chords have an associated **richness**, which measures the variety of overtones in the resulting vibrations. A sufficiently rich chord may give rise to entirely new effects. To maximize richness, try to maximize resonance while avoiding repeating Gemstones. 

If either the [Weapons](../Weapons) or [Slingshots](../Slingshots) modules are enabled and the player is currently holding a forged weapon or slingshot, respectively, resonating chords from equipped Infinity Bands will also amplify the corresponding gemstone forges.

It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Enemies

This mod can optionally randomize enemy stats to provide more dynamic encounters. Randomized stats are biased to the player's daily luck, introducing yet another layer to that mechanic. Visiting the Mines on unlucky days will now provide a truly brutal experience.

This mod also provides three sliders to tailor general combat difficulty. These sliders allow scaling monster health, attack and defense.

Finally, certain enemy hitboxes are also improved, and others have received small visual tweaks.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Controls & Quality of Life

This mod includes the following popular control-related features, often featured in other mods.

* **Face Mouse Cursor**
    
    When playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.

* **Slick Moves**

    Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced. 

* **Auto-Selection**

    If enemies are nearby, players can optionally choose a weapon, melee or ranged, to be equipped automatically.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Other Features

* **Woody Replaces Rusty**

    The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with a Wooden Blade.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

## Compatibility

- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using both together.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
- Compatible with [Better Rings][mod:better-rings], and will use compatible textures if that mod is installed. Credits to [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- Compatible with [Simple Weapons][mod:simple-weapons], and will use compatible textures if that mod is installed.
- Compatible with [Vanilla Tweaks][mod:vanilla-tweaks], and will use compatible weapon textures if that mod is installed.
- Compatible with [Archery][mod:archery] and the accompanying [Starter Pack][mod:archery-starter-pack]. Install the misc. Archery Rebalance file for the complete experience.
- Compatible with [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115).
- Compatible with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).

- While the Infinity Slingshot will appear in [CJB Item Spawner][mod:cjb-spawner], it will be incorrectly classified as a Melee Weapon and will be unusable if spawned in this way. This is due to CJB not recognizing non-vanilla slingshots. This likely will be fixed in game version 1.6.
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] or [Enhanced Slingshots][mod:enhanced-slingshots].
- Weapon rebalance features are **not** compatible with other mods that introduce new weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- New enchantments are **not** compatible with other mods that introduce new enchantments, such as [Enhanced Slingshots][mod:enhanced-slingshots].
- Ring features are **not** compatible with other mods with similar scope, including [Combine Many Rings][mod:combine-many-rings], [Balanced Combine Many Rings][mod:balanced-many-rings] and, to an extent, [Ring Overhaul][mod:ring-overhaul]
- Other ring retextures will be lightly incompatible with the new Infinity Band, meaning there may be some visual glitches but otherwise no real issues.

<sup><sub><sup>[🔼 Back to top](#margo-combat-cmbt)</sup></sub></sup>

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
[shield:rb]: https://img.shields.io/badge/Ruby%20(Rb)-e13939?style=flat
[shield:aq]: https://img.shields.io/badge/Aquamarine%20(Aq)-2390aa?style=flat
[shield:am]: https://img.shields.io/badge/Amethyst%20(Am)-6f3cc4?style=flat
[shield:ga]: https://img.shields.io/badge/Garnet%20(Ga)-981d2d?style=flat
[shield:em]: https://img.shields.io/badge/Emerald%20(Em)-048036?style=flat
[shield:jd]: https://img.shields.io/badge/Jade%20(Jd)-759663?style=flat
[shield:tp]: https://img.shields.io/badge/Topaz%20(Tp)-dc8f08?style=flat

[mod:cjb-spawner]: <https://www.nexusmods.com/stardewvalley/mods/93> "CJB Item Spawner"
[mod:sve]: <https://www.nexusmods.com/stardewvalley/mods/3753> "Stardew Valley Expanded"
[mod:angels-rebalance]: <https://www.nexusmods.com/stardewvalley/mods/6894> "Angel's Weapon Rebalance"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:vanilla-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/10852> "Vanilla Tweaks"
[mod:simple-tweaks]: <https://www.nexusmods.com/stardewvalley/mods/16491> "Simple Weapons"
[mod:better-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/2067> "Better Slingshots"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:combine-many-rings]: <https://www.nexusmods.com/stardewvalley/mods/8801> "Combine Many Rings"
[mod:balanced-many-rings]: <https://www.nexusmods.com/stardewvalley/mods/8981> "Balanced Combine Many Rings"
[mod:ring-overhaul]: <https://www.nexusmods.com/stardewvalley/mods/10669> "Ring Overhaul"
[mod:better-rings]: <https://www.nexusmods.com/stardewvalley/mods/8642> "Better Rings"
[mod:combat-controls]: <https://www.nexusmods.com/stardewvalley/mods/2590> "Combat Controls - Fixed Mouse Click"
[mod:combat-controls-redux]: <https://www.nexusmods.com/stardewvalley/mods/10496> "Combat Controls Redux"
[mod:amf]: <https://www.nexusmods.com/stardewvalley/mods/7886> "Advanced Melee Framework"
[mod:enhanced-slingshots]: <https://www.nexusmods.com/stardewvalley/mods/12763> "Enhanced Slingshots"
[mod:archery]: <https://www.nexusmods.com/stardewvalley/mods/16767> "Archery"
[mod:archery-starter-pack]: <https://www.nexusmods.com/stardewvalley/mods/16768> "Archery Starter Pack"

[tropes:color-coded]: <https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience> "Color-Coded for Your Convenience"

[🔼 Back to top](#margo-combat-cmbt)