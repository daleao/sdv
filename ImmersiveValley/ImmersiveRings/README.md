<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

# Fellowship - Immersive Rings

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <ul>
        <li><a href="#glow-and-magnet-rings">Glow and Magnet Rings</a></li>
        <li><a href="#gemstone-rings">Gemstone Rings</a></li>
        <li><a href="#iridium-band">Iridium Band</a></li>
        <li><a href="#resonance">Resonance</a></li>
    </ul>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

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
    - **Topaz:** *literally nothing -> +3 defense.* Since the precision stat is unused in the Vanilla game, the Topaz Ring was completely useless. ConcernedApe realized this, which is why he made the Topaz Enchantment grant defense instead. This change makes the Topaz Ring consistent with the Topaz Enchantment.
        - To compensate for the rebalanced Topaz Ring, the **Crabshell Ring**'s effect is doubled, from +5 defense to +10 defense, in order to remain relevant in combined rings, but not as strong as a full-Topaz [Infinity Band](#iridium-band).

- Adds the Garnet gemstone and Garnet Ring. This ring grants 10% cooldown reduction to special moves, which compensates for the lack of Acrobat profession in [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111).
    - Corresponding Garnet mining nodes are also added via an included [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) content pack. If you don't install Custom Ore Nodes, Garnets will be unobtainable in-game.

### Iridium Band

Completely overhauls the Iridium Band. In Vanilla, this ring was a free 3-in-1 before any Forge ring combination. It also completely triviliazes the Glowstone Ring (which is funny because the latter was only added in patch 1.5). So to fix this, this mod offers two choices, which can changed by config settings.

#### Choice #1: Old Iridium Band, but better.

Choice #1 is the simpler choice; the Iridium Band retains the same stats as in Vanilla, but its crafting recipe is changed to reflect those stats. It now requires both a Glowstone Ring and a gemstone ring of your choice. This means you can also customize the added stat of the Iridium Band to something other than the flat damage of the Ruby Ring.

#### Choice #2: The new Infinity Band (recommended).

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

This mod makes use of Harmony to patch vanilla crafting behavior. As such there may be unexpected behavior with mods that change the crafting menu.
NEW: [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115) is supported since v1.0.3 of this mod.

This mod is my own take on a "balanced combine many rings". Obviously it is not compatible with other mods with similar scope, including [Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8801), [Balanced Combine Many Rings](https://www.nexusmods.com/stardewvalley/mods/8981) and, to an extent, [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669); because of it's highly modular nature, Ring Overhaul in particular can still be used with this mod, provided you know how to customize the configs to cherry-pick non-conflicting features.

NEW: [Better Rings](https://www.nexusmods.com/stardewvalley/mods/8642) is now supported! I'm not aware of any other ring retextures but they will not be compatible.

Should be compatible with Wear More Rings, although I haven't tested it.

This is a companion mod for [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111), but can be used independently.

Should be fully compatible with multiplayer. Not compatible with Android.

## Installation

Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.

To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.

To uninstall simply delete the mod from your mods folder. This mod is safe to uninstall at any point.

## Special Thanks

- [Goldenrevolver](https://www.nexusmods.com/stardewvalley/users/5347339) for the idea of progressive gemstone rings.
- [compare123](https://www.nexusmods.com/stardewvalley/users/13917800) for Better Rings-compatible textures.
- **ConcernedApe** for StardewValley.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>

## License

See [LICENSE](../LICENSE) for more information.
