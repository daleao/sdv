<div align="center">

# Ligo :: Rings

</div>

## Overview

This module brings several immersive changes to vanilla rings and major new ring mechanics.
All features can be toggled on or off.

    - After disabling this module, please use [CJB Item Spawner][mod:cjb-spawner] or similar to delete and respawn all owned Iridium Bands to avoid issues.
    - Please note that this module introduces new items via Json Assets. Therefore enabling or disabling on existing saves **will cause a Json Shuffle**. You can avoid the shuffle by deleting the 'assets/json-assets' folder. This will allow you to use the crafting recipe features, but all Infinity Band features will become unusable, and should be kept disabled.

## Ring Crafting Progression

The vanilla game does not allow rings to be used as crafting ingredients. This module bypasses that limitation, allowing, in particular, Glow and Magnet rings to be crafted from their smaller counterparts, and the Glowstone Ring to be crafted from the former two. This is not only more immersive, but also provides a more natural progression, making the hoards of small ring drops actually useful.

In addition, new progressive crafting recipes are added for each of the gemstone rings, with corresponding visual changes.
    - **Amethyst and Topaz:** *combat level 2, requires Copper Bars.*
    - **Aquamarine and Jade:** *combat level 4, requires Iron Bars.*
    - **Emerald and Ruby:** *combat level 6, requires Gold Bars.*

## Infinity Band

In vanilla, the Iridium Band is an instant 3-rings-in-1 which can also be forged to get a 4-rings-in-1. It completely triviliazes Glow Rings, Magnet Ring and even the brand new Glowstone Ring, introduced in patch 1.5. This module aims to solve all of these issues while also alluding to everyone's favorite cinematic universe.

The iridium Band is completely overhauled. Initially, a newly crafted Iridium Band will grant no effects at all. Only with access to the Forge will you be able to awaken its true form, infusing a Galaxy Soul to transform it into an **Infinity Band**.

The Infinity Band likewise does nothing on its own, but it serves as a vessel for up to **four** gemstones. To add a gemstone to the Infinity Band, you must fuse it with a gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones will potentially lead to powerful **Resonances**.

### Gemstone Rebalance

There are two problematic gemstones in vanilla: Jade and Topaz. This modules rebalances their effects to be on-par with the ubiquitous Ruby.
    - **Jade:** *+10% -> +50% crit. power.* A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, if the player has 100% crit. chance then they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to a Ruby Ring, which straight up grants a 10% boost to damage, but *all the time*. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type given an appropriate build. Above that threshold, Jade Ring becomes significantly stronger.
    - **Topaz:** *literally nothing -> +1 defense.* Since the precision stat is unused in the Vanilla game, the Topaz Ring was completely useless. ConcernedApe realized this, which is why he made the Topaz Enchantment grant defense instead. This change makes the Topaz Ring consistent with the Topaz Enchantment. If the [Arsenal](../Arsenal) module's Defense Overhaul is enabled, this will be converted to 10% damage reduction.

To achieve the intended balance for all gemstones, make sure to enable the [Arsenal](../Arsenal) module.

Finally, this module also introduces the Garnet Ring. This ring adds cooldown reduction as the seventh combat stat, compensating for the removed Acrobat profession by the [Professions](../Professions) module and completing the 7-note Diatonic Gemstone Scale (see below). Garnet Rings must be crafted from mined Garnet gemstones, added via an included [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) content pack. If you don't install Custom Ore Nodes, Garnets will be unobtainable in-game.

### Resonance

The seven gemstones form a [Diatonic Scale](https://en.wikipedia.org/wiki/Diatonic_scale):

<div align="center">

![shield:rb]  ![shield:aq]  ![shield:am]  ![shield:ga]  ![shield:em]  ![shield:jd]  ![shield:tp]

</div>

The scale is cyclic, so after Tp comes Rb again, and so on. The first note in the scale is called the **Root**. Above, Rb was used as an example, but the scale can be shifted, or *transposed*, to place any gemstone at the root. But regardless of the root note, the order is always preserved.

Like strings in a guitar, the characteristic vibration of each gemstone causes interference patterns. These interferences can be constructive and/or destructive, and they create complex overtones that add richness to the resulting vibrations, known as [Harmonies](https://en.wikipedia.org/wiki/Harmony). In other words, certain gemstones will harmonize together, creating resonances that amplify their individual effects. At the same time, other gemstone pairs will lead to dissonances, which instead dampen those effects. As a rule of thumb, Gemstones that are positioned farthest from each other in the Diatonic Scale will resonate more strongly, and those positioned adjacent to each other will dissonate. This means that the interval `I - V` (for example, `Rb - Em`, `Am - Tp`, `Ga - Rb` etc.) will lead to the strongest resonance, while the interval `I - II` will lead to a dissonance (for example, `Rb - Aq`, `Am - Ga`, `Tp - Rb`, etc.).

Gemstones placed together in an Infinity Band not only resonate, but can also make up [Chords](https://en.wikipedia.org/wiki/Chord_(music)). Chords have an associated **richness**, which measures the variety of [Overtones](https://en.wikipedia.org/wiki/Overtone) in the resulting vibrations. A sufficiently rich chord may give rise to entirely new effects. To maximize richness, try to maximize resonance while avoiding repeating Gemstones. 

If the [Arsenal](../Arsenal) module is enabled and the player is currently holding a forged weapon or slingshot, resonating chords from equipped Infinity Bands will also amplify the corresponding gemstone forges.

It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.

## Compatibility

- Compatible with [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115).
- Compatible with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).
- Compatible with [Better Rings](https://www.nexusmods.com/stardewvalley/mods/8642), and will use compatible textures if that mod is installed. Credits to [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- (NEW) compatible with [Vanilla Tweaks](https://www.nexusmods.com/stardewvalley/mods/10852), and will use compatible textures if that mod is installed.

- Generally incompatible with other mods with similar scope, including [Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8801), [Balanced Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8981) and, to an extent, [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669)
    - Because of it's highly modular nature, Ring Overhaul in particular can still be used with this module, provided you know how to customize the configs to cherry-pick non-conflicting features.
- Other ring retextures will be lightly incompatible with the new Infinity Band, meaning there may be some visual glitches but otherwise no real issues.


[shield:rb]: https://img.shields.io/badge/Ruby%20(Rb)-e13939?style=flat
[shield:aq]: https://img.shields.io/badge/Aquamarine%20(Aq)-2390aa?style=flat
[shield:am]: https://img.shields.io/badge/Amethyst%20(Am)-6f3cc4?style=flat
[shield:ga]: https://img.shields.io/badge/Garnet%20(Ga)-981d2d?style=flat
[shield:em]: https://img.shields.io/badge/Emerald%20(Em)-048036?style=flat
[shield:jd]: https://img.shields.io/badge/Jade%20(Jd)-759663?style=flat
[shield:tp]: https://img.shields.io/badge/Topaz%20(Tp)-dc8f08?style=flat
