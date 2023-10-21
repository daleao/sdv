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
	<li>
		<a href="#melee-weapon-changes">Melee Weapon Changes</a>
		<ol>
			<li><a href="#combo-framework">Combo Framework</a></li>
			<li><a href="#offensive-defensive-swords">Offensive & Defensive Swords</a></li>
			<li><a href="#rebalanced-types--tiers">Rebalanced Types & Tiers</a></li>
			<li><a href="#melee-enchantments">Melee Enchantments</a></li>
			<li><a href="#other-features">Other Features</a></li>
		</ol>
	</li>
	<li>
		<a href="#ranged-weapon-changes">Ranged Weapon Changes</a>
		<ol>
			<li><a href="#rebalanced-modifiers--ammo">Rebalanced Modifers & Ammo</a></li>
			<li><a href="#critical-hits">Critical Hits</a></li>
			<li><a href="#special-move">Special Move</a></li>
			<li><a href="#ranged-enchantments">Ranged Enchantments</a></li>
			<li><a href="#other-features-1">Other Features</a></li>
		</ol>
	</li>
	<li><a href="#ring-changes">Ring Changes</a>
		<ol>
			<li><a href="#rebalances">Rebalances</a></li>
            <li><a href="#craftable-gemstone-rings">Craftable Gemstone Rings</a></li>
			<li><a href="#infinity-band">Infinity Band</a></li>
		</ol>
	</li>
	<li><a href="#infinity-1">Infinity +1</a></li>
	<li><a href="#garnet--gemstone-resonance-theory">Garnet & Gemstone Resonance Theory</a></li>
	<li><a href="#enemies">Enemies</a></li>
	<li><a href="#controls--quality-of-life">Controls & Quality of Life</a></li>
	<li><a href="#other-features-2">Other Features</a></li>
	<li><a href="#compatibility">Compatibility</a></li>
	<li><a href="#faq">F.A.Q.</a></li>
</ol>
</details>

## Overview

This humongous module consolidates rebalances to melee weapons, ranged weapons and combat rings, together with entirely new mechanics which will overall make combat significantly more strategic and challenging.

## Rebalanced Stats

Certain stats are simply not worth investing in vanilla. While some of these can be attributed to unbalanced equipment bonuses (and will be addressed below), others, namely **Knockback** and **Defense** are fundamentally broken.

1. **Knockback** is too high by default; hitting enemies immediately throws them several tiles away. Further down we will address the default knockback of most weapons. But that alone doesn't make this as attractive a stat as flat damage. Hence, enemies will now take damage proportional to their momentum when thrown against a wall or object. The added offensive angle makes this a great stat for clubs in particular. It also makes positioning an important strategic element in combat.
2. **Defense** is rather useless in vanilla, where each point simply mitigates a single unit of damage. Consequently, small defense bonuses become useless in late-game. This mod implements a new, simple damage mitigation formula, which allows defense to scale into late-game without becoming overpowered.

    Old formula:
    ```math
    damage = Min(rawDamage - defense, 1)
    ```

    New formula:
    ```math
    resistance = 10 / (10 + defense)
    ```
    ```math
    damage = rawDamage * resistance
    ```

    One defense point now reduces damage by 10% regardless of incoming damage. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve. This applies to monsters! The **Topaz Ring** is also changed to increased defense by 1 point, like its corresponding weapon forge.
3. **Critical strikes** are fun, but need a little more oomph to compete with flat damage. As a counter-play to the previous change to defense, this mod makes critical strikes ignore enemy defense. **Attacks from behind will** also **have double the chance to critically strike.** But reaching the backs of your enemies will not be easy! You will have to rely on [status effects](#status-effects) to achieve that. The effects of **Jade** bonuses from rings and weapon forges are also significantly buffed.
    - **Jade:** *+10% -> +50% crit. power.* A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, only when the player has 100% crit. chance then they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to a Ruby Ring, which straight up grants a 10% boost to damage, *all the time*. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type given an appropriate build. Above that threshold, Jade Rings become even stronger.
    - Slingshots can now critically strike. Think of them as headshots.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Melee Weapon Changes

Vanilla weapons suffer from two major issues:
    1. Same-y-ness. Apart from their special moves, none of the weapon types feel particularly unique; club and dagger are simply "worse swords".
    2. Overabundance. Weapons quickly become inventory clutter. The game is also quick to gift its most powerful weapons, trivializing any weapons obtained as drops.

This mod tries to fix both through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Combo Framework

Weapon spamming is makes vanilla combat a boring click-fest. This mod implements a **combo framework** for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced cooldown. Each weapon type has a configurable combo limit:
    
- **Swords:** up to 4 horizontal swipes, by default.
- **Clubs:** up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
- **Daggers:** unchanged, effectively up to infinite hits.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/combo_sword_small.gif" alt="combo_sword.gif" width="33%">
<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/combo_club_small.gif" alt="combo_club.gif" width="33%">
</div>

If combo hits are enabled, swing speed bonuses from **emerald** effects will apply to every frame of the attack animation, as well as the cooldown in between combos, as opposed to only the final frame as in vanilla. This immediately makes speed a more valuable stat.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Offensive & Defensive Swords

In vanilla game code we can find traces of an abandoned fourth weapon type: **Stabbing Swords**. This module re-implements stabbing swords into the game.

Swords are now split between **offensive** and **defensive** archetypes. Defensive sw   ords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames. You can also change the direction mid-dash by inputting a directional command in a perpendicular direction.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/stabbing_special.gif" alt="stabbing_special.gif" width="67%">
</div>

To improve the defensive parry move, parry damage is increased by 10% for every defense point. This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during combo downtime).

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Rebalanced Types & Tiers

Weapon stats have been rebalanced across the board so as to provide a more clear distinction between the weapon types:

 - **Clubs** are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
 - **Daggers** are the opposite of clubs, being the quick, precise, but short-range and generally-lower-damage potential weapons. **Can cause Bleeding**.
 - **Offensive Swords** are somewhere in the middle, with average damage, speed and precision.
 - **Defensive Swords** are marginally weaker and slower than their offensive counterpart, but are otherwise heavier, sturdier and wider. They are somewhere in-between Offensive Swords and Clubs. 

Weapons are categorized by different tiers, [color-coded for your convenience][tropes:color-coded]:

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/tiers.png" alt="tiers">
</div>

<br>

Weapons below the Mythic tier all have randomized damage, determined the moment they are dropped and scaled to your Mine progression. This way, players can always loot stronger weapons; all weapons can be useful, and no specific weapon is ever trivialized. Higher-tier weapons will generally be stronger than lower-tiered ones, although that may not always be the case.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/color-coded.gif" alt="color-coded.gif" width="67%">
</div>

<font color="red"><b>Mythic Tier</b></font>

Mythic weapons are unique or extremely rare. They are usually quest rewards or very rare monster drops, and tend to carry intrinsic special abilities. In exchange, they cannot receive additional Prismatic Shard enchantments at the forge.

The known Mythic weapons are:

| Weapon | Type | Effects |
| ------ | ---- | ------- |
| ![](./resources/readme/yetitooth.png) Yeti Tooth | Defense Sword | Can cause Chilled status.* |
| ![](./resources/readme/neptuneglaive.png) Neptune Glaive | Defense Sword | Deals heavy knockback. |
| ![](./resources/readme/obsidianedge.png) Obsidian Edge | Stabbing Sword | Ignores enemy resistances. Can cause Bleeding.* |
| ![](./resources/readme/lavakatana.png) Lava Katana | Stabbing Sword | Can cause Burning.* |
| ![](./resources/readme/insecthead.png) Insect Head | Dagger | Damage depends on the number of slain insects. |
| ![](./resources/readme/iridiumneedle.png) Iridium Needle | Dagger | Always crits. |

\* *Status effects are explained [further below](#status-effects).*

<font color="orange"><b>Masterwork Tier</b></font>

Masterwork weapons are relics of the Dwarven race, crafted from special materials. They can only be created by uncovering the lost Dwarvish Blueprints, and taking them to a skilled blacksmith along with the corresponding material:

| Weapons | Description | Material | Source |
| ------- | ----------- | -------- | ------ |
| ![](./resources/readme/elven_set.png) Elven | Quick, nimble weapons enchanted with forest magic which grants them high reach and knockback. | ![](./resources/readme/elderwood.png) Elderwood | [Scavenger Hunts](../Professions) |
| ![](./resources/readme/dwarven_set.png) Dwarven | Large, bulky weapons. They grant high defense and knockback, but reduce speed. | ![](./resources/readme/dwarvenscrap.png) Dwarven Scrap Metal | Volcano Chests |
| ![](./resources/readme/dragontooth_set.png) Dragontooth | Light and sharp, granting the highest raw damage out of any weapon in the game. | ![](./resources/readme/dragontooth.png) Dragon Tooth | Volcano Chests, Dragon Skeletons |

Masterwork weapons can only be obtained if the Dwarven Legacy setting is enabled.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/dwarvish_crafting.gif" alt="dwarvish_crafting.gif" width="67%">
</div>

Note that **only vanilla weapons have been rebalanced**. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. **That's not on me.** Those weapons are broken by design. I just pulled the curtains. You're welcome.

<font color="gold"><b>Legendary Tier</b></font>

See [Infinity+1](#infinity-1).

**Retexture & FX**

In order to better reflect their new weapon types, as well as their mythical or legendary status, several weapons have received vanilla-friendly retextures. These textures will always be overwritten by any installed Content Patcher mods, so there is no concern for compatibility. Moreover, there are tons of small immersive details like visual and sound effects added to mythic and legendary weapons.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Melee Enchantments

Weapons enchantments have been almost entirely overhauled. These new enchantments should provide more interesting gameplay options:

| Name      | Effect |
| --------- | -------|
| Haymaker | *Unchanged from vanilla.* |
| Blasting | Accumulates and stores half of the damage from enemy hits (before mitigation). If enough damage is accumulated, the next special move releases that damage as an explosion. |
| Bloodthirsty | Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s. |
| Carving | Attacks on-hit reduce enemy defense by 1 (continuing below zero). Removes the armor from Armored Bugs and de-shells Rock Crabs. |
| Cleaving | Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target. |
| Energized | Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area. |
| Mammonite's | Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage. |
| Steadfast | Can no longer critically strike, but multiplies base damage by a factor of crit. power. |
| Wabbajack | Causes unpredictable effects.** |

\*
***Offensive Swords:** Can dash twice in succession. **Defensive Swords:** The next attack within 5s of a successful parry is guaranteed a critical strike. **Daggers:** Quick stab deals an additional hit. All hits also apply Bleed with 100% chance. **Clubs:** Smash area +50%. Enemies in range are stunned for 2s.*

\*\* *Examples: damage or heal the enemy; decrease or increase the enemie's stats; transfigure into a different enemy, creature or any random item (note: this can spawn illegal items).*

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Other Features

* **The Mines & Weapon Acquisition**

    In vanilla, the player is quick to amass large quantities of fodder weapons, both from Mine chests as well as barrel and crate drops. To support the intended experience of progression, all weapons are removed from Mine chests, which instead reward random goodies. In order to obtain new weapons, players will have to fight for monster drops or get lucky with breakable containers. Monster-dropped weapons are rare, but tend to be stronger. Lower-tier weapons can also be purchased directly from Marlon, but will have fixed and significantly weaker stats when obtained this way.

* **Tooltips**

    Vanilla weapon tooltips are confusing. Who the heck knows what "+1 Speed" means?
    
    This mod improves weapon tooltips for clarity, so you always know exactly what each stat means. You may configure the tooltips to show **absolute** or **relative** stats; the former displays straight-forward raw stats, while the latter displays percentages **based on the weapon type's base stats**. The new tooltips also display gemstone sockets instead of the generic vanilla "Forged x/3" text.

    Note that this is the only feature of this mod that affects third-party mod weapons. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. **That's not on me.** Those weapons are broken by design. I just pulled the curtains. You're welcome.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Ranged Weapon Changes

Ranged weapons are actually quite strong in vanilla, mainly because ammo's deal insane damage. They are also boring, however, since ranged combat is very unidimensional. This mod seeks to alleviate that by reducing base ammo damage while also introducing many of the same mechanics afforded to melee weapons.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Rebalanced Modifiers & Ammo

To make room for critical headshots and the new Infinity Slingshot the base damage and knockback modifiers of each slingshot are reduced to more reasonable values:
- Master Slingshot: Ammo damage x2 >> x1.5
- Galaxy Slingshot: Ammo damage x4 >> x2
- **Infinity Slingshot:** x2.5

The following ammos have been nerfed, either for immersion or balance:
- Coal: 15 damage >> 2 damage - *Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo anyway.*
- Explosive Ammo: 20 damage >> 2 damage ﻿- *Explosive ammo is meant to be used as a mining utility **only**, so it's damage has been reduced to reflect that. If you'd like to use slingshots for combat and mining simultaneously, consider taking up the [Rascal](../Professions)'s extra ammo slot.*

The following new ammos have been added:
- Radioactive Ore: 80 damage
- Gemstones (Ruby, Emerald, etc.): 50 damage
- Diamond: 120 damage
- Prismatic Shard: 200 damage

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Special Move

Pressing the action button will engage gatling mode, enabling auto-fire for 3 seconds, or as long as you hold the action key.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Ranged Enchantments

The following new enchantments can be applied to ranged weapons:

| Name       | Effect |
| ---------- | -------|
| Energized | Moving and shooting generates energy. When fully-energized, the next projectile carries an electric charge, which discharges dealing heavy damage when it hits an enemy. |
| Freljord | Progressively chill enemies on hit for 2 seconds, freezing after stacking 3 times. |
| Quincy | Attacks fire an energy projectile if no ammo is equipped. The projectile is stronger at lower health. Only works when enemies are nearby.* |
| Runaan | Summons two phantom copies of the fired projectile. The phantoms automatically aim at the nearest enemy after a short delay. Only works when enemies are nearby.** |

\* *Quincy projectile cannot crit nor knock back enemies, but is affected by damage modifiers. Below 2/3 max health, the projectile gains a 50% damage boost, increasing to 100% when below 1/3 (the projectile will change color to reflect these improvements). If [PRFS](../Professions) is enabled and the player has the Desperado profession, the Quincy projectile's size will be increased proportionally by overcharge **instead of** its velocity.*

\** *Additional projectiles inherit 40% of the main projectile's damage, but 100% of the crit. chance, crit. power, knockbak and overcharge.*

Gemstone forges cannot directly be applied to slingshots, but **gemstones can be equipped as ammo**, and will apply their analogous bonuses when equipped, and will also [resonate](#resonances) if applicable. With the [Rascal](../Professions) profession, you may slot up to two gemstones at a time to possibly achieve a level-2 forge.

* **Emeralds**, instead of attack speed, grant **firing speed** (idem for Emerald Rings), which also affects **overcharge** if the [Professions](../Professions) module's Desperado profession is used;

Lastly, the **Galaxy Soul** can be applied to the Galaxy Slingshot, as with other Galaxy weapons, to eventually create the **Infinity Slingshot.**

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Other Features

* **Removed Grace Period**

    Vanilla slingshots are unable to hit enemies in close-range of the player; these shots will fly straight through them. This mod removes this limitation, making ranged combat more reliable.

* **Snowballs**

    Standing in a snowy tile with an empty slingshot will allow the player to fire a snowball. The snowball projectile deals no significant damage; this is meant as a fun flavor feature for multiplayer.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Ring Changes

Most combat-oriented rings in vanilla are underwhelming and completely overlooked next to the Iridium Band, which provides a free 3-in-1 ring that can also be combined. This mod tries to make combat rings more interesting, and introduce all-new mechanics specific to the Iridium Band.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Rebalanced Combat Rings

This following rings have been changed:
- **Warrior Ring:** ~~Chance of giving the Warrior Energy~~ (attack +10 for 5s) ~~buff after slaying a monster.~~ -> Gain a progressively higher attack bonus as you slay enemies (every 3 enemies increases attack by 1), which falls off gradually after some time out of combat.
- **Ring of Yoba:** ~~Chance of giving the Yoba's Blessing~~ (invincible for 5s) ~~buff after taking damage.~~ -> Taking damage that would leave you below 30% health instead grants a shield that absorbs up to 50% of your maximum health for 30s. Cannot be used again until health is fully recovered.
- **Savage Ring:** ~~+2 Speed for 3s after slaying a monster.**~~ -> Gain a rapidly decaying Speed buff after slaying a monster.
- **Ring of Thorns:** Can cause Bleeding status (in addition to reflected damage).

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

### Craftable Gemstone Rings

All gemstone rings become craftable at various levels of the Combat skill, requiring the corresponding gemstone and a type of metal bar:

- **Amethyst & Topaz:** Combat level 2 (Copper Bar)
- **Aquamarine & Jade:** Combat level 4 (Iron Bar)
- **Ruby & Emerald:** Combat level 6 (Gold Bar)
- **Garnet:** Combat level 7 (Gold Bar)

This addition accompanies some visual changes to each ring to match the required metal bar.

### Infinity Band

The Iridium Band has been completely overhauled. Initially, a newly crafted Iridium Band will grant no effects at all. Only with access to the Forge will you be able to awaken its true form by infusing it with a Galaxy Soul to transform it into an **Infinity Band**.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/infinity_band.gif" alt="infinity_band.gif" width="67%">
</div>

The Infinity Band likewise does nothing on its own, but it serves as a vessel for up to **four** gemstones. To add a gemstone to the Infinity Band, you must fuse it with a corresponding gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones may lead to powerful [resonances](#garnet--gemstone-resonance-theory).

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Infinity +1

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/infinity.png" alt="cursed_event.png">
</div>

According to [TV Tropes Wiki](https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword), an Infinity +1 sword is "not only the most powerful of its kind [...] , but its power is matched by how hard it is to acquire". The vanilla Infinity weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired at the desert, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the top of the backpack.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the all-new Blade of Ruin...

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/cursed_event.png" alt="cursed_event.png" width="67%">
</div>

In return for all that extra work, the Infinity weapons have extra perks:    
1. +1 gemstone slot (4 total). *Keeping in mind that each gemstone has the potential to [resonate](#resonance).*
2. Small boost to the weapon's special move:
    * **Stabing Sword:** Increased dash distance.
    * **Defense Sword:** Parried enemies are dazed for 1 second.
    * **Dagger:** Quick-stab deal one additional hit.
    * **Club:** Smash AoE is 25% larger.
    * **Slingshot**: Auto-fire mode lasts for 1 additional second.
3. **Melee only:** while at full health, every swing fires a mid-range energy beam.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/infinity_beam.gif" alt="infinity_beam.gif" width="67%">
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Garnet & Gemstone Resonance Theory

To compensate for the [removal of vanilla Acrobat profession](../Professions), this mod introduces a seventh gemstone, the **Garnet**, which can be mined upwards of Mine level 80. Socketed to a ring or a weapon, it will grant 10% cooldown reduction to special moves. [As described above](#craftable-gesmtone-rings), the Garnet Ring is craftable at Combat level 7.

With the addition of Garnet, the seven gemstones together form a [Diatonic Scale](https://en.wikipedia.org/wiki/Diatonic_scale):

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/Modules/Combat/resources/readme/diatonic.png" alt="diatonic_gemstone_scale.png" width="67%">
 <figcaption><font size="2">The Diatonic Gemstone Scale. The dashed lines show the Tertian Tetrads rooted in Ruby (red) and Aquamarine (blue).</font></figcaption>
</div>

<br>

Beginning at the top, the scale progresses clockwise and is cyclic; i.e., after **Rb** comes **Aq**, **Am**, and so on until **Tp**, before again repeating **Rb**. In this example, **Rb** is called the **Tonic**, or **Root**.

Like strings in a guitar, the characteristic vibration of each gemstone causes [interference](https://en.wikipedia.org/wiki/Wave_interference) patterns. These interferences can be constructive or destructive, and they create complex [overtones](https://en.wikipedia.org/wiki/Overtone) that add richness to the resulting vibrations, known as [Harmonies](https://en.wikipedia.org/wiki/Harmony). In other words, certain gemstones will harmonize together, creating resonances that amplify their individual effects. At the same time, other gemstone pairs will lead to dissonances, which instead dampen those effects. The magnitude of a resonance is determined by the interval between gemstones. As a rule of thumb, stones that are positioned farthest from each other in the Diatonic Scale will resonate more strongly, and those positioned adjacent to each other will dissonate. One full rotation of the circle is called an [Octave](https://en.wikipedia.org/wiki/Octave), denoting the interval between a gemstone and itself. All possible intervals are given below:

| Interval | Resonance | Examples |
| -------- | --------- | -------- |
| Second | -12.5% | `Rb - Aq`, `Am - Ga`, `Ga - Em` |
| Third | 16.6% | `Rb - Am`, `Am - Em`, `Ga - Jd` |
| Fourth | 33.3% | `Rb - Ga`, `Am - Jd`, `Ga - Tp` |
| Fifth | 50% | `Rb - Em`, `Am - Tp`, `Ga - Rb` |
| Sixth | 16.6% | `Rb - Jd`, `Am - Rb`, `Ga - Aq` |
| Seventh | -12.5%* | `Rb - Tp`, `Am - Aq`, `Ga - Am` |
| Octave | - | `Rb - Rb`, `Am - Am`, `Ga - Ga` |

 The **Fifth** is the strongest resonating interval, followed by the **Fourth**, and the **Third**/**Sixth**. Notice that, because the scale is cyclic, complementary intervals are essentially equivalent; a **Sixth** (e.g., `Rb - Jd`) is a just a **Third** counted backwards (`Jd - Rb`), and therefore their resonances are the same. Likewise for **Second** and **Seventh**, the two dissonant intervals. The **Fourth** and **Fifth** are also a complementary pair, but their resonances are different (due to some simplifications from real life).

Multiple resonanating gemstones together give rise to [Chords](https://en.wikipedia.org/wiki/Chord_(music)). A chord represents the total of all superpositioned resonances between those gemstones. Up to 4 gemstones can be placed in any given Infinity Band to produce one chord.

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/monad.png" alt="monad.png" align="right" width="33%">

**Monad:** A 1-note chord is called a **Monad**. A Monad results from up to 4 repeated gemstones. As it only contains Octaves, this chord offers no resonances, but achieves the highest single-stat total from any other chord. E.g., `Rb - Rb - Rb - Rb` (show to the right).

<br>

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/dyad.png" alt="dyad.png" align="left" width="33%">
 

**Dyad:** A 2-note chord is called a **Dyad**. Given the intervals above, it should be clear that the best possible Dyad is a I - V configuration, like `Rb - Em`. This chord contains two intervals: 1 Fifth and 1 Fourth (from the inverse, `Em - Rb`), resulting in a +50% resonance for Rb and +33.3% for Em. A double `I - I - V - V` chord is called a [Power Chord](https://en.wikipedia.org/wiki/Power_chord); the simplest possible chord (and a staple of rock music). E.g., `Rb - Rb - Em - Em` (show to the left).

<br>

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/triad.png" alt="triad.png" align="right" width="33%">

**Triad:** A 3-note chord is called a **Triad**. There are many possible Triad combinations, but only one that avoids dissonances: the [Tertian](https://en.wikipedia.org/wiki/Tertian). The Tertian Triad is formed by any two sequential Third intervals, i.e., `I - III - V`. Notice that due to the cyclic nature of the scale, this is equivalent to a `I - IV - VI` by simply shifting all notes to the left. Take for instance:

* `Rb - Am - Em` is a `I - III - V` configuration.
* `Em - Rb - Am` is a `I - IV - VI` configuration. But if we shift all notes to the left, then the chord once again becomes `Rb - Am - Em`. Notice that shifting notes around does not change the chord, but allows us to see it from a different perspective.

E.g., `Rb - Rb - Am - Em` (show to the right).

<br>

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/tetrad.png" alt="tetrad.png" align="left" width="33%">
<figcaption></fg

**Tetrad:** A 4-note chord is called a **Tetrad**. Notice that it is impossible for any 4-note configuration to avoid any dissonances, but this is okay; by adding another Third interval onto the Tertian Triad we achieve a **Tertian Tetrad**, or `I - III - V - VII`. In this special case, the dissonant Seventh becomes resonant, adding +12.5% instead of subtracting it. The Tertian Tetrad achieves the highest possible total resonance, though it distributes this total among 4 different stats. E.g., `Rb - Am - Em - Tp` (show to the left).

<br>

The gemstone with the highest amplitude in a chord becomes the **Tonic**, or **Root**. All resonating chords emit light, the color and amplitude of which is determined by the Root. Chords have also an associated **Richness**, which measures the variety of overtones in the resulting vibrations. Richness is achieved by avoiding repeated notes (i.e., Triads and Tetrads). Sufficiently rich chords (i.e., Tertians) will additionally create **magnetism**.

If the player's currently held weapon contains forged gemstones, resonating chords from equipped Infinity Bands will also amplify all gemstone forges matching the chord's root note. Note that forged gemstones do not form chords themselves nor share any of the same resonance and dissonance mechanics from Infinity Bands described above.

It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Status Effects

Taking inspiration from classic RPG and strategy games, this module adds a framework for causing various status conditions to enemies, described below:

| Status | Effect | Sources |
| ------ | ------ | ------- |
| Bleeding | Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (i.e., Dwarven Sentry). | Daggers, Obsidian Edge, Ring of Thorns |
| Burning | Causes damage equal to 1/16th of max health every 3 seconds, and reduces attack by half. Also causes enemies to move about more randomly. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4x as quickly. Does not affect Magma Sprites or Lava Lurks | Lava Katana |
| Chilled | Reduces movement speed for the duration. If Chilled is inflicted again during this time, then applies Freeze for 5x the duration. Does not affect Ghosts or Skeleton Mage. | Yeti Tooth, Freljord Enchantment |
| Frozen | Cannot move or attack. The next hit during the duration deals triple damage and ends the effect. | - |
| Poisoned | Causes damage equal to 1/16 of max health every 3s, stacking up to 3x. Does not affect Ghosts.| *Unused* |
| Slowed | Reduces movement speed for the duration. | Slime Ammo |
| Dazed | Cannot move or attack for the duration. | Enhanced Parry |

Durations depend on the source. These status conditions are exclusively applied to monsters, with two exceptions; a few player-applied status conditions are also tweaked to be more interesting and/or more consistent:

| Status | Effects | Sources | Duration |
| ------ | ------- | ------- | -------- |
| Burnt | *Same as above.* | Magma Sparker | 15s |
| Frozen | *Same as above.* | Skeleton Mage | 5s |
| Jinxed | Defense -5. Prevents the use of special moves. | Shadow Shaman | 8s |
| ~~Weakness~~ Confusion | Causes unpredictable movement. | Blue Squid | 3s |

Most status conditions accompany neat new visual and/or sound effects.

<div align="center">

<img src="https://gitlab.com/daleao/modular-overhaul/-/raw/main/resources/screenshots/stun_animation.gif" alt="stun_animation.gif" width="67%">
</div>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Enemies

This mod can optionally randomize enemy stats to provide more dynamic encounters. Randomized stats are biased to the player's daily luck, introducing yet another layer to that mechanic. Visiting the Mines on unlucky days will now provide a truly brutal experience.

This mod also provides three sliders to tailor general combat difficulty. These sliders allow scaling monster health, attack and defense.

Finally, certain enemy hitboxes are also improved, and others have received small visual tweaks.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Controls & Quality of Life

This mod includes the following popular control-related features, often featured in other mods.

* **Face Mouse Cursor**
    
    When playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.

* **Slick Moves**

    Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced. 

* **Auto-Selection**

    If enemies are nearby, players can optionally choose a weapon, melee or ranged, to be equipped automatically.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Other Features

* **Woody Replaces Rusty**

    The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with a Wooden Blade.

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## Compatibility

<details>
<summary> 🟩 <b><font color="green">The following mods are compatible:</font></b> 🟩 </summary>

- Compatible with [Advanced Melee Framework][mod:amf] and related content packs, but I do not recommend using any with this module due to inconsistent or unbalanced special moves.
- Compatible with [Stardew Valley Expanded][mod:sve]﻿﻿ and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
- Compatible with [Better Rings][mod:better-rings], and will use compatible textures if that mod is installed. Credits to [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- Compatible with [Simple Weapons][mod:simple-weapons], and will use compatible textures if that mod is installed.
- Compatible with [Vanilla Tweaks][mod:vanilla-tweaks], and will use compatible weapon textures if that mod is installed.
- Compatible with [Archery][mod:archery] and the accompanying [Starter Pack][mod:archery-starter-pack]. Install the misc. Archery Rebalance file for the complete experience.
- Compatible with [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115).
- Compatible with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).
</details>

<details>
<summary> 🟥 <b><font color="red">The following mods are NOT compatible:</font></b> 🟥 </summary>

- While the Infinity Slingshot will appear in [CJB Item Spawner][mod:cjb-spawner], it will be incorrectly classified as a Melee Weapon and will be unusable if spawned in this way. This is due to CJB not recognizing non-vanilla slingshots. This likely will be fixed in game version 1.6.
- **Not** compatible with the likes of [Combat Controls][mod:combat-controls] or [Combat Controls Redux][mod:combat-controls-redux], as those features are already included in this and other modules.
- **Not** compatible with other mods that overhaul slingshots, such as [Better Slingshots][mod:better-slingshots] or [Enhanced Slingshots][mod:enhanced-slingshots].
- Weapon rebalance features are **not** compatible with other mods that introduce new weapon types or rebalance weapon stats, such as [Angel's Weapon Rebalance][mod:angels-rebalance].
- New enchantments are **not** compatible with other mods that introduce new enchantments, such as [Enhanced Slingshots][mod:enhanced-slingshots].
- Ring features are **not** compatible with other mods with similar scope, including [Combine Many Rings][mod:combine-many-rings], [Balanced Combine Many Rings][mod:balanced-many-rings] and, to an extent, [Ring Overhaul][mod:ring-overhaul]
- Other ring retextures will be lightly incompatible with the new Infinity Band, meaning there may be some visual glitches but otherwise no real issues.
</details>

<sup><sup>[🔼 Back to top](#margo--combat-cmbt)</sup></sup>

## F.A.Q.

<details>
<summary><b>How do I unlock the Dwarven relic weapons?</b></summary>

Have the Dwarvish Translation Guide and at least 6 hearts with Clint, then enter Clint's shop once you have found the first Dwarvish Blueprint. A short cutscene should play, and you will have to wait a random number of days (the higher your friendship points, the shorter the wait). Afterwards, speak to Clint again to unlock the **Forge** option.
</details>

<details>
<summary><b>Where can I find the Dwarvish Blueprints?﻿</b></summary>

They can be found in the exactly the same place as their corresponding weapons would be found in Vanilla (i.e., Volcano chests). The only exception are the Elven blueprints, which can only be obtained from Scavenger Hunts (requires the [Professions](../Professions) module). The corresponding crafting materials are also obtained in the exact same way, as described above in the Weapons section.
</details>

<details>
<summary><b>Where can I find the Blade of Ruin?</b></summary>

At the end of the single-floor [Quarry Mine](https://stardewvalleywiki.com/Quarry_Mine) from the statue of the Grim Reaper.
</details>

<details>
<summary><b>What is the Blade of Ruin's curse?</b></summary>

The Blade of Ruin will grow progressively stronger by cosuming enemies; every 5 enemies slain increases its attack power by 1 point. As it grows stronger, however, it will also begin to consume your own energy, dealing damage-over-time while held. At the same time, the Blade has a nasty habit of auto-equipping itself; the stronger the Blade, the more damage you will suffer, and the more often it will auto-equip itself.

Should you choose to ignore these side-effects and continue to strengthen the Blade, you will eventually become unable to use other weapons, and be forced to engage combat with 1 HP.

But fret not, for once a day you may pray to the Altar of Yoba to reduce the intensity of the curse.

</details>

<details>
<summary><b>How do I lift the Ruined Blade's curse?</b></summary>

To begin the quest, you must slay at least 50 enemies with the Blade equipped, prompting the Wizard to invite you over for a chat. To complete this initial quest, simply interact with the Yoba altar and exhaust all possible dialogue choices.
You will then be asked to prove your virtues:
- Prove your Honor, Compassion and Wisdom by selecting certain responses during character heart events.
    - Alternatively, prove your Honor by respectfully returning the Mayor's shorts.
- Prove your Valor by completing monster eradication goals or persevering through long digs in the Mines.
    - **You must speak with Gil to complete an eradication goal.** *This is vanilla guys. I don't know why everybody seems to forget this.*
    - Alternatively, prove your Valor by reaching SVE's [Treasure Cave](https://stardew-valley-expanded.fandom.com/wiki/Treasure_Cave).
- Prove your Generosity by gifting NPCs a certain amount of gold in gifts, or by purchasing Community Upgrades from Robin.
 
Exact completion criteria will depend on your difficulty settings (you can see them in-game in the your quest journal). When you are ready, approach Yoba's altar in Pierre's house with the Blade in hand.
</details>

<details>
<summary><b>What are the IDs of heart events related the Blade of Ruin?</b></summary>

The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the Event Repeater mod is installed.

| ID      | Description           | Virtue |
| ------- | --------------------- | ------ |
| 7       | Maru 4 hearts         | Honor  |
| 16      | Pierre 6 hearts       | Honor  |
| 36      | Penny 6 hearts        | Honor  |
| 46      | Sam 4 hearts          | Honor  |
| 58      | Harvey 6 hearts       | Honor  |
| 100     | Kent 3 hearts         | Honor  |
| 288847  | Alex 8 hearts         | Honor  |
| 2481135 | Alex 4 hearts         | Honor  |
| 733330  | Sam 3 hearts          | Honor  |
| 8185291 | Sophia 2 hearts (SVE) | Honor  |
| 13      | Haley 6 hearts               | Compassion |
| 51      | Leah 4 hearts                | Compassion |
| 100     | Kent 3 hearts                | Compassion |
| 288847  | Alex 8 hearts                | Compassion |
| 502969  | Linus 0 hearts               | Compassion |
| 503180  | Pam 9 hearts                 | Compassion |
| 733330  | Sam 3 hearts                 | Compassion |
| 3910975 | Shane 6 hearts               | Compassion |
| 1000005 | Sebastian Mature Event (SVE) | Compassion |
| 1000013 | Caroline Mature Event (SVE)  | Compassion |
| 11      | Haley 2 hearts          | Wisdom |
| 21      | Alex 5 hearts           | Wisdom |
| 25      | Demetrius 3 hearts      | Wisdom |
| 34      | Penny 2 hearts          | Wisdom |
| 50      | Leah 2 hearts           | Wisdom |
| 56      | Harvey 2 hearts         | Wisdom |
| 97      | Clint 3 hearts          | Wisdom |
| 1000018 | Jodi Mature Event (SVE) | Wisdom |
| 1000021 | Jas Mature Event (SVE)  | Wisdom |
| 27      | Sebastian 6 hearts | Honor, Compassion or Wisdom  |
| 3219871 | Claire 2 hearts (SVE)   | Generosity |
</details>

<details>
<summary><b>How do I obtain the Infinity weapons?</b></summary>

Unforge the Blade of Dawn to obtain a Hero Soul, and then forge it into any Galaxy weapon after 3 Galaxy Souls.
</details>

<details>
<summary><b>How do I obtain other mythic weapons?</b></summary>

- **Neptune's Glaive:** Fishing Chests, same as Vanilla.
- **Yeti Tooth:** Dropped by enemies or crates in the icy section of the Mines.
- **Obsidian Edge:** Dropped from Shadow people in the dangerous Mines.
- **Lava Katana:** Dropped from certain enemies in Volcano Dungeon. Alternatively, from the Treasure Cave in Crimson Badlands.
</details>

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

[🔼 Back to top](#margo--combat-cmbt)