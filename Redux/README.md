# What is this mod?

This mod is a compilation of the "Immersive" suite of mods, all of which spawned from the original professions overhaul, i.e., Walk Of Life.
Mods included in this compilation include:
  - Immersive Professions (a.k.a. Walk Of Life)
  - Immersive Ponds (a.k.a. Aquarism)
  - Immersive Rings (a.k.a. Fellowship)
  - Immersive Taxes (a.k.a. Serfdom)
  - Immersive Tools (a.k.a. Tooth And Nail)
  - Immersive Tweaks (a.k.a. Quality Of Life)

It also includes the previously unreleased Immersive Arsenal, which overhauls combat, weapons and slingshots.

# Why does this exist?

It was always the intention of all mods in the Immersive following Walk Of Life to be complements for the latter; i.e., Aquarism was a complement for the Aquarist profession, Serfdom for the Conservationist profession, and Fellowship for all the new combat skill professions. Consequently, all of these mods were tightly integrated with Walk Of Life, but had little to no interaction with each other, which could reasonably be achieved by Walk Of Life's provided API.

This changed with the addition of the new Arsenal mod, which needed to communicate with several other mods in addition to Walk Of Life. At this point, it became increasingly difficult to establish the required level of integration using APIs, not to mention the unecessary code clutter.

Another issue was the amount of redundancy and increased memory consumption caused by compiling separate mods requiring the same shared framework.

A single combined mod allows all components to communicate natively and share a single framework.

# Installation and Update

1. Delete any previous installations of this or Immersive suite mods.
2. Extract the zip file into the mods folder.
3. Start the game once with SMAPI to generate a config file.
4. Use the Generic Mod Config Menu to enable the desired modules.
5. Restart the game for changes to take effect.

# Uninstallation

Be mindful that certain module have special uninstall instructions. Failure to comply with these instructions may lead to save file corruption.

# Special Thanks

- **ConcernedApe** for Stardew Valley.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>

# License

See [LICENSE](LICENSE) for more information.
