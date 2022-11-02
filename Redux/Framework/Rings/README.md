<table align="center"><tr><td align="center" width="9999">

# Redux :: Rings

</td></tr></table>

## Features

This mod brings several immersive changes to vanilla rings and major new ring mechanics.

All features can be toggled on or off.

### Glow and Magnet Rings

- Adds missing ring crafting recipes and rebalances vanilla recipes to establish a clear crafting progression.
    - Glow and Magnet rings are given new crafting recipes which consume their smaller counterparts.
    - The recipe for the Glowstone Ring is replaced to actually consume one Glow and one Magnet rings.

### Gemstone Rings

- Adds progressive crafting recipes with corresponding visuals.
    - **Amethyst and Topaz:** *combat level 2, uses copper bars.*
    - **Aquamarine and Jade:** *combat level 4, uses iron bars.*
    - **Emerald and Ruby:** *combat level 6, uses gold bars.*

- Rebalances the Jade and Topaz rings.
    - **Jade:** *+10% -> +50% crit. power.* A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, it means that only if the player has 100% crit. chance they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to the Ruby Ring, which straight up grants an overall 10% boost to damage. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type. Above that threshold, Jade Ring becomes significantly stronger.
    - **Topaz:** *literally nothing -> +3 defense.* Since the precision stat is unused in the Vanilla game, the Topaz Ring was completely useless. ConcernedApe realized this, which is why he made the Topaz Enchantment grant defense instead. This change also makes the Topaz Ring consistent with the Topaz Enchantment.

- Adds the Garnet gemstone and Garnet Ring. This ring grants 10% cooldown reduction to special moves, which compensates for the lack of Acrobat profession in [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111).
    - Corresponding Garnet mining nodes are also added via an included [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) content pack. If you don't install Custom Ore Nodes, Garnets will be unobtainable in-game.

### Iridium Band

Completely overhauls the Iridium Band. In Vanilla, this ring was a free 3-in-1 before any Forge ring combination. It also completely triviliazes the Glowstone Ring (which is funny because the latter was only added in patch 1.5). To fix this, this mod introduces the all new Infinity Band.

Choice #2 is a complete overhaul which makes the Iridium Band an extremely powerful late game item. 
Initially, a newly crafted Iridium Band will grant no effects at all. Only with access to the Forge will you be able to awaken its true form by fusing a Galaxy Soul, which will transform it into an **Infinity Band**.

The Infinity Band is a vessel for up to **four** gemstones. To add a gemstone to the Infinity Band, you must fuse it with a gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones will potentially lead to powerful **Resonances**.

### Resonance

Only available with Choice #2.

The seven gemstones form a [Diatonic Scale](https://en.wikipedia.org/wiki/Diatonic_scale):

    Rb (I) - Aq (II) - Am (III) - Ga (IV) - Em (V) - Jd (VI) - Tp (VII)

The scale is cyclic, so after Tp comes Rb, and so on. The scale can be transposed to place any gemstone at the root, but the order is always preserved.

Like musical notes, the characteristic vibration of each gemstone causes paired gemstones to interfere constructively and destructively, creating overtones that bring new richness to the resulting [Harmonies](https://en.wikipedia.org/wiki/Harmony). In other words, certain gemstones will harmonize together, creating resonances that amplify their individual effects. At the same time, other gemstones will lead to dissonances, which instead dampen those effects. As a rule of thumb, Gemstones that are positioned farthest from each other in the Diatonic Scale will resonate more strongly, and those positioned adjacent to each other will dissonate. This means that the interval `I - V` (for example, `Rb - Ga`, or `Am - Tp`) will lead to the strongest resonance, while the interval `I - II` will lead to a dissonance (for example, `Rb - Aq`, or `Am - Ga`).

Gemstones placed together in an Infinity Band not only resonate, but can also make up [Chords](https://en.wikipedia.org/wiki/Chord_(music)). Chords have an associated **richness**, which measures the amount of [Overtones](https://en.wikipedia.org/wiki/Overtone) in the resulting vibrations. To maximize richness, try to maximize resonance while avoiding repeating Gemstones. A sufficiently rich chord may give rise to entirely new effects.

It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.

## Compatibility

- Compatible with [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115).
- Compatible with [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214).
- Compatible with [Better Rings](https://www.nexusmods.com/stardewvalley/mods/8642), and will use compatible textures if that mod is installed.

- Generally incompatible with other mods with similar scope, including [Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8801), [Balanced Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8981) and, to an extent, [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669)
    - Because of it's highly modular nature, Ring Overhaul in particular can still be used with this module, provided you know how to customize the configs to cherry-pick non-conflicting features.
- Other ring retextures will be lightly incompatible with the new Infinity Band, meaning there may be some visual glitches but otherwise no real issues.

## Uninstallation

Safe to uninstall or disable without special measures.

## Special Thanks

- [Goldenrevolver](https://www.nexusmods.com/stardewvalley/users/5347339) for the idea of progressive gemstone rings.
- [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
