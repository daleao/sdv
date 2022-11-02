# What is this mod?

This mod is a compilation of modules that each overhaul different aspects of vanilla game mechanics.

Currently included modules:
  - **[Professions](./Framework/Professions/README.md)** is the main module, and the only one enabled by default. This will overhaul all the game's professions with the goal of supporting more diverse and interesting playstyles.
  - **[Arsenal](./Framework/Arsenal/README.md)** is the second largest module. It overhauls various aspects of melee weapons and slingshots so as to diversify combat and provide viable alternatives to the all-powerful sword.
  - **[Rings](./Framework/Rings/README.md)** is a huge overhaul of the Iridium Band, introducing new forge mechanics and Gemstone Music Theory. It also rebalances other underwhelming rings and unimmersive crafting recipes.
  - **[Ponds](./Framework/Ponds/README.md)** is a complement to the new Aquarist profession. It overhauls Fish Ponds to merit a spot on any farm.
  - **[Taxes](./Framework/Taxes/README.md)** is a complement to the new Conservationist profession. It introduces a simple yet realistic taxations system. Because surely a nation at war would be on top of that juicy farm income.
  - **[Tools](./Framework/Tools/README.md)** is a simple mod allowing chargeable resource tools, customizable farming tools and some extended tool enchantments.
  - **[Tweaks](./Framework/Tweex/README.md)** is the final module, and serves as a repository for smaller tweaks and fixes to inconsistencies in vanilla.
                     
The modules can be individually enabled or disabled, and are fully customizable themselves. For more information, visit the READMEs for each individual module.

# Why does this exist?

This mod aggregates the "Immersive" suite of mods, all of which spawned from the original, Walk Of Life, Immersive Professions. It also includes the previously unreleased Immersive Arsenal. It was always the intention of all mods in the Immersive suite following Walk Of Life to be complements for the latter. Consequently, all of these mods were tightly integrated with Walk Of Life, but had otherwise little to no interaction with each other, which could reasonably be achieved by Walk Of Life's provided API.

This changed with the addition of Arsenal, which needed to communicate with several of the modules in addition to Walk Of Life. At this point, it became increasingly difficult to establish the required level of integration using APIs alone, not to mention the unecessary code clutter.

Another issue was the amount of redundancy and increased memory consumption caused by compiling separate mods all of which require the same underlying framework.

A single combined mod allows all components to communicate natively all the while sharing a single framework.

# Compatibility

All modules should be fully multiplayer-ready as long as all players have it installed.
Not Android-compatible.

# Installation and Update

1. Delete any previous installations of this or Immersive suite mods.
2. Extract the zip file into the mods folder.
3. Start the game once with SMAPI to generate a config file.
4. Use the Generic Mod Config Menu to enable the desired modules.
5. Restart the game for changes to take effect.

# Uninstallation

Be mindful that certain module have special uninstall instructions. Failure to comply with these instructions may lead to save file corruption.

After disabling a module in the Generic Mod Config Menu, you must restart the game for the changes to take effect.

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
