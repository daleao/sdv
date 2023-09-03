<div align="center">

# MARGO - Modular Gameplay Overhaul

A complete and comprehensive rework of Stardew Valley gameplay mechanics, offering a much more engaging and immersive "Vanilla+" experience.

[![License][shield:license]](LICENSE) [![Nexus][shield:nexus]][url:nexus] [![Mod Drop][shield:moddrop]][url:moddrop]

</div>

## About this mod

This mod is a compilation of overhaul modules, each targeting a specific gameplay component or mechanic. Together, the modules complement each other to create a "Vanilla+" experience.

The modular nature of this mod allows users to cherry-pick features to their liking, while also preserving the deep native integration required between individual modules. This reduces the amount of code redundancy and improves code maintainability.

This mod is the culmination of over a year of work. Please take the time to read the descriptions before asking questions.

## Modules

The available modules are listed below. **Please read this page carefuly in its entirety.** Modules can be toggled on or off in the title screen via GMCM. Each module is itself highly configurable, and will be added to the GMCM menu if enabled. Some modules require specific enabling/disabling instructions you should pay attention to. These requirements will be mentioned below.

All modules should be fully multiplayer and splitscreen-ready so long as all players have it installed. Unless explicitly stated otherwise, none of the modules are Android-compatible. Please refer to each module's specific documentation page for further details and compatibility information.

- **[PROFS](Modules/Professions)** is the original module, formely known as Walk Of Life. It overhauls all the game's professions with the goal of supporting more diverse and interesting playstyles. It also introduces optional Prestige mechanics for very-late game save files and Limit Breaks for combat professions.

- **[CMBT](Modules/Combat)** ﻿is a huge overhaul of nearly all aspects of combat; from rebalanced stats, melee and ranged weapons, rings and enchantments, to entirely new mechanics like status effects, weapon combos, a new weapon type, Gemstone Music Theory, and much more. **This module adds new items via Json Assets, and thus may cause Json Shuffle on existing saves.** 

- **[PNDS](Modules/Ponds)** is a complement to the new Aquarist profession. It allows Fish Ponds to produce Roe with scaling quantities and qualities, spontaneously grow algae, and even enrich the nuclei of metals.

- **[TXS](Modules/Taxes)** is a complement to the new Conservationist profession. It introduces a realistic taxation system as an added challenge and end-game gold sink. Because surely a nation at war would be capitalizing on that juicy farm income.

- **[TOLS](Modules/Tools)** is a one-stop-shop for tool customization and quality-of-life. It enables resource-tool charging, farming-tool customization, intelligent tool auto-selection, and even adds Radioactive tool upgrades, among other things.

- **[TWX](Modules/Tweex)** is the final module, and serves as a repository for smaller tweaks and fixes to inconsistencies not large enough to merit a separate module.

Please note that only the Professions and Tweex modules are enabled by default.

All modules should be fully multiplayer and split-screen compatible **if and only if all players have it installed**. **This mod is not Android-compatible**, but an Android version of Chargeable Tools is available as an optional download.

## Installation & Update

1. Extract the downloaded archive file into your local mods folder.
2. Start the game once with SMAPI to generate a config file.
3. Use the Generic Mod Config Menu to enable the desired modules.
4. Restart the game for changes to take effect.

As with any mod, always **delete any previous installation completely** before updating. If you'd like to preserve your config settings you can delete everything except the configs.json file.

**The use of Vortex or other mod managers is not recommended for Stardew Valley.**

## For C# Developers

This mod offers an [API](./API/IModularOverhaulApi.cs) for C# developers wishing to add third-party compatibility.
To use it, copy both files in the API folder over to your project, and change the namespace to something appropriate.
Then [request SMAPI for a proxy](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#using-an-api).

Below are some usecases for the API:

- **[PROFS]**: Checking the current value of dynamic perks associated with certain professions;
- **[PROFS]**: Hooking custom logic into Scavenger and Prospector Treasure Hunts.
- **[PROFS]**: Hooking custom logic to several stages of [Limit Breaks](./Modules/Professions/README.md#limit-breaks).
- **[PROFS]**: Allowing SpaceCore skills to surpass level 10, and be [Prestiged](./Modules/Professions/README.md#prestige) at levels 15 and 20.
- **[CMBT]**: Checking the [Resonances](./Modules/Rings/README.md#chords) currently active on any given player.
- Checking the config settings of any given player (note that you must create your own interface for this).

## Credits & Special Thanks

All hail our Lord and Savior [Pathoschild][user:pathoschild], creator of [SMAPI][url:smapi], Content Patcher and the mod-verse, as well as our Father, **ConcernedApe**, creator of Stardew Valley, a benevolent God who continues to support the game for both players and modders.    

This mod borrows ideas and assets from [Ragnarok Online][url:ragnarok], [League of Legends][url:league] and early Pokemon games. Credit to those, respectively, goes to [Gravity][url:gravity], [Riot Games][url:riot] and [Game Freak][url:gamefreak]. This mod is completely free and open-source, provided under [Common Clause-extended MIT License](LICENSE).

Special thanks the translators who have contributed to this project:

* ![][flag:german][FoxDie1986](https://www.nexusmods.com/stardewvalley/users/1369870)
* ![][flag:chinese][xuzhi1977](https://www.nexusmods.com/users/136644498)
* ![][flag:korean][BrightEast99](https://www.nexusmods.com/users/158443518)
* ![][flag:japanese][sakusakusakuya](https://www.nexusmods.com/stardewvalley/users/155983153)
* ![][flag:russian][romario314](https://www.nexusmods.com/stardewvalley/users/68548986)

You have the right to upload your own translation of this project, but I reserve the right to add your translation directly to the project.

Lastly, a shout-out to [JetBrains][url:jetbrains] for providing a free open-source license to ReSharper and other tools, which provide an immense help to improve and maintain the quality of the code in this mod.

<img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"> <img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo.">



<!-- MARKDOWN LINKS & IMAGES -->
[shield:license]: https://img.shields.io/badge/License-Commons%20Clause%20(MIT)-brightgreen?style=for-the-badge
[shield:nexus]: https://img.shields.io/badge/Download-Nexus-yellow?style=for-the-badge
[url:nexus]: https://www.nexusmods.com/stardewvalley/mods/14470
[shield:moddrop]: https://img.shields.io/badge/Download-Mod%20Drop-blue?style=for-the-badge
[url:moddrop]: https://www.moddrop.com/stardew-valley/

[url:stardewvalley]: <https://www.stardewvalley.net/> "Stardew Valley"
[url:jetbrains]: <https://jb.gg/OpenSource> "JetBrains"
[url:smapi]: <https://smapi.io/> "SMAPI"
[url:gamefreak]: <https://www.gamefreak.co.jp/> "Game Freak"
[url:gravity]: <https://www.gravity.co.kr/> "Gravity"
[url:ragnarok]: <https://ro.gnjoy.com/index.asp> "Ragnarok Online"
[url:riot]: <https://www.riotgames.com/> "Riot Games"
[url:league]: <https://www.leagueoflegends.com/> "League of Legends"

[user:pathoschild]: <https://www.nexusmods.com/stardewvalley/users/1552317> "Pathoschild"

[flag:german]: <https://i.imgur.com/Rx3ITqh.png>
[flag:chinese]: <https://i.imgur.com/zuQC9Di.png>
[flag:korean]: <https://i.imgur.com/Jvsm5YJ.png>
[flag:japanese]: <https://i.imgur.com/BMA0w39.png>
[flag:russian]: <https://i.imgur.com/cXhDLc5.png>

[🔼 Back to top](#margo-modular-gameplay-overhaul)