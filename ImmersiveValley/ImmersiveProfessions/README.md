<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->
![](title_2.png)
# A Professions Overhaul
![](https://stardewcommunitywiki.com/mediawiki/images/8/82/Farming_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/2/2f/Mining_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/f/f1/Foraging_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/e/e7/Fishing_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/c/cf/Combat_Skill_Icon.png)

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li>
      <a href="#professions">Professions</a>
      <ul>
        <li><a href="#farming">Farming</a></li>
        <li><a href="#foraging">Foraging</a></li>
        <li><a href="#mining">Mining</a></li>
        <li><a href="#fishing">Fishing</a></li>
        <li><a href="#combat">Combat</a></li>
      </ul>
    </li>
    <li><a href="#prestige">Prestige</a></li>
    <li><a href="#special-abilities">Special Abilities</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#configs">Configs</a></li>
    <li><a href="#console-commands">Console Commands</a></li>
    <li><a href="#api">API</a></li>
    <li><a href="#recommended-mods">Recommended Mods</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>

Ever wondered why there aren't any profession overhaul mods on the Nexus? Me too.

## Features

Profession-related features:
- Rebalanced and reworked [almost] every profession to be an equally unique and attractive choice.
- Each profession targets a specific style of gameplay, some which were not viable in vanilla (e.g. ranching).
- No more ~~boring~~ uninspiring +X% sell price bonuses.
- Bomberman mining; Thief/Assassin combat; epic slingshots; command an army of giant Slimes.
- Scaling end-game objectives.
- Level 5 professions provide simple early-game buffs that benefit most styles of general gameplay.
- Level 10 professions are more specialized and engaging, providing two bonuses which change the way you play.
- Professions are more consistent across the board, with several analogous perks and synergies.
- _[Optional]_ Combat professions have unique special abilities.
- _[Optional]_ Lore-friendly and balanced multi-profession.
- _[Optional]_ Level 20 skills and prestiged professions.
- Gender-specific profession titles.
- New icons for most professions, courtesy of [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) (please make sure to [endorse their original](https://www.nexusmods.com/stardewvalley/mods/4163) mod).
- New sound effects and visuals for certain professions.
- Automatcally detects SVE's Galdora and Vintage Interface (both version) and provides compatible HUD elements.

Other skill-related features:
- Combat levels which reward a profession (multiples of 5) now also reward 5 HP. At level 10 the player will have 200 health instead of 190.

Integration is provided for Automate and several popular mods. See the [compatibility](#compatibility) section for details.

## Professions

### ![](https://i.imgur.com/p9QdB6L.png) Farming

- ![](resources/assets/sprites/loose/harvester.png) **Harvester (Lv5)** - 10% *(20%)* chance for extra yield from harvested crops.
    - Yields an equivalent 10% monetary bonus to vanilla on average, while also benefiting anybody who will not sell raw crops.
- ![](resources/assets/sprites/loose/agriculturist.png) **Agriculturist (Lv10)** - Crops grow 10% *(20%)* faster. Grow best-quality crops organically without fertilizer.
    - Allows harvesting of iridium-quality crops without any fertilizer. The chance is unchanged from vanilla, and is equal to half the chance of gold quality. Fertilizers will still massively increase that chance.
- ![](resources/assets/sprites/loose/artisan.png) **Artisan (Lv10)** - All artisan machines work 10% *(25%)* faster. Machine output quality is at least as good as input ingredient quality.
    - Essentially implements [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) (QAP), but exclusively for Artisans. Also adds 5% chance to promote the output quality by one level. In multiplayer, **the bonus production speed applies only to machines crafted by the player with this profession, and only when that player uses the machine**.
- ![](resources/assets/sprites/loose/miner.png) **Rancher (Lv5)** - Befriend animals 2× *(3×)* quicker.
    - Gain double mood and friendship points from petting. Newborn animals are born with some starting friendship between 150 and 250 (out of 1000 maximum), chosen at random.
- ![](resources/assets/sprites/loose/breeder.png) **Breeder (Lv10)** - Incubation 2× *(3×)* faster and natural pregnancy 3× *(5×)* more likely. Increase value of animals at high friendship.
    - At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x. Animal Husbandry: gestation following insemination is also 2x *(3x)* faster.
- ![](resources/assets/sprites/loose/producer.png) **Producer (Lv10)** - Happy animals produce 2× *(3×)* as frequently. Produce worth 5% more for every full barn or coop.
    - Note that happiness (mood) is **not** the same as friendship. Also note this will **not** allow certain animals (i.e. cows and chickens) to produce more than once per day. Bonus produce value also applies to artisan goods derived from animal products (i.e. cheeses, mayos and cloth), honey (bees are animals), and meat (Animal Husbandry or Fresh Meat). Only deluxe buildings can be considered full. **Note that honey is also considered an animal product.** There is no limit to the scaling. In multiplayer, **the bonus applies only to barns and coops owned by the player with this profession, and only when that player sells the produce**.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- ![](resources/assets/sprites/loose/forager.png) **Lv5 - Forager** - 20% *(40%)* chance for double yield of foraged items.
    - _Unchanged effect from vanilla. Only the name is changed._
- ![](resources/assets/sprites/loose/ecologist.png) **Ecologist (Lv10)** - Wild berries restore 50% *(100%)* more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same deterministic quality. This immediate gives inventory convenience. However the quality will start off at silver, and progress to iridium when enough items have been foraged. Applies to Mushroom Boxes, but only if the cave owner (i.e. the host player) has the profession.
- ![](resources/assets/sprites/loose/scavenger.png) **Scavenger (Lv10)** - Location of foragable items revealed. Occasionally detect buried treasure. *Time freezes during Scavenger Hunts.*
    - On switching maps while outside you will occasionally detect hidden buried treasure. Find it and dig it up (with a hoe) within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items.
    - _Bonus: holding [ModKey](#configs) will highlight all foragables on-screen._
- ![](resources/assets/sprites/loose/lumberjack.png) **Lumberjack (Lv5)** - Felled trees yield 25% *(40%)* more wood.
    - _Unchanged effect from vanilla. Only the name is changed._
- ![](resources/assets/sprites/loose/arborist.png) **Arborist (Lv10)** - All trees grow faster. Normal trees can drop *(twice as much)* hardwood.
    - Bonus tree growth works as a global buff; i.e. in multiplayer, all trees will be affected as long as any player in the session has this profession, and the effect will stack for all additional online players that share this profession. _The hardwood bonus is unchanged from vanilla._
- ![](resources/assets/sprites/loose/tapper.png) **Tapper (Lv10)** - Tappers are cheaper to craft. Tapped trees give syrup 25% *(50%)* faster.
    - New regular recipe: x25 wood, x1 copper bar.
    - New Heavy recipe: x20 hardwood, x1 iridium bar, x1 radioactive ore.

### ![](https://i.imgur.com/TidtIw0.png) Mining 

- ![](resources/assets/sprites/loose/miner.png) **Miner (Lv5)** - +1 *(+2)* ore per ore vein.
    - _Unchanged from vanilla._
- ![](resources/assets/sprites/loose/spelunker.png) **Spelunker (Lv10)** - Chance to find ladders and shafts increases with every mine level. +1 speed every 10 levels. *Also recover some health and stamina with every mine level.*
    - Plus 0.5% ladder chance per level. Bonus ladder chance resets every time you leave the mines. **This includes taking the stairs back to the mine entrance.**
- ![](resources/assets/sprites/loose/prospector.png) **Prospector (Lv10)** - Location of ladders and mining nodes revealed. Occasionally detect rocks with valuable minerals. *Time freezes during Scavenger Hunts.*
    - Analogous to Scavenger. Tracks any mining nodes or mineral forages off-screen with a yellow pointer, and any ladders or shafts with a green pointer. On entering a new mine floor you will occasionally detect stones with prospective treasure. Find the stone and break it within the time limit to obtain a reward. The larger your win streak the better your odds of obtaining rare items. Completing a hunt automatically reveals a ladder.
    - _Bonus: holding [ModKey](#configs) will highlight all nodes and ladders on-screen._
- ![](resources/assets/sprites/loose/blaster.png) **Blaster (Lv5)** - Craft twice as many explosives. Exploded rocks yield 2× *(3×)* as much coal.
- ![](resources/assets/sprites/loose/demolitionist.png) **Demolitionist (Lv10)** - Bomb radius +1. Exploded rocks yield 20% *(40%)* more resources.
    - _Bonus: [Get excited!](https://www.youtube.com/watch?v=0nlJuwO0GDs) when hit by an explosion._
- ![](resources/assets/sprites/loose/gemologist.png) **Gemologist (Lv10)** - Progressively identify gems and minerals of higher quality. Crystalariums work 25% *(50%)* faster.
    - Analogous to Ecologist. All gems and minerals mined from nodes have a fixed quality, starting at silver and increasing once enough minerals have been collected. Minerals collected from Crystalariums and Geode Crushers are counted for this total, **but not those from geodes broken at Clint's**. In multiplayer, **the bonus Crystalarium speed applies only to machines crafted by the player with this profession, and only when used by that player**.

### ![](https://i.imgur.com/XvdVsAn.png) Fishing

- ![](resources/assets/sprites/loose/fisher.png) **Fisher (Lv5)** - Fish bite faster *(instantly)*. Live bait reduces the chance to fish junk.
    - Here, "junk" includes algae and seaweed.
- ![](resources/assets/sprites/loose/angler.png) **Angler (Lv10)** - Fish worth 1% more for every unique max-sized fish caught and 5% more for every legendary fish. *Can recatch legendary fish.*
    - "Legendary fish" includes the Extended Family Qi challenge varieties, counted only once.
- ![](resources/assets/sprites/loose/aquarist.png) **Aquarist (Lv10)** - Fish pond max capacity +2. Catching bar decreases slower for every unique fish species raised in a fish pond. *Can raise legendary fish.*
    - The catching bar decays 5.5% slower per unique Fish Pond. Six ponds equal a permanent Trap Bobber. In multiplayer, **only counts Fish Ponds owned by the player with this profession**.
    - *Legendary fish and extended family can be raised in the same pond.*
- ![](resources/assets/sprites/loose/trapper.png) **Trapper (Lv5)** - Crab pots are cheaper to craft. Can trap higher-quality *(highest-quality)* haul.
    - All trapped fish can have quality up to gold. Chance depends on your fishing level (same formula as forage). _Recipe is unchanged from vanilla._
- ![](resources/assets/sprites/loose/luremaster.png) **Luremaster (Lv10)** - Crab pots no longer produce junk. Use different baits to attract different catch. *60% chance to preserve bait.*
    - Each type bait will also apply it's regular fishing effects:
        - **Regular bait:** 25% chance to catch fish, subject to the same location and season limitations as regular fishing.
        - **Wild bait:** 25% chance to also double the haul.
        - **Magnet:** Repels all fish (as per its description), but attracts metal items such as resources, artifacts, treasure chests, rings and even weapons.
        - **Magic bait:** 25% chance to catch fish of any location or season. Also upgrades all catch to iridium-quality.
- ![](resources/assets/sprites/loose/conservationist.png) **Conservationist (Lv10)** - Crab pots without bait can trap junk. Clean the Valley's waters to merit tax deductions. *Cleaning the Valley's waters also merits favor with the villagers.*
    - Every 100 (configurable) junk items collected will earn you a 1% tax deduction the following season (max 25%, also configurable), increasing the value of all shipped goods. You will receive a formal mail from the Ferngill Revenue Service each season informing your currrent tax bracket.

### ![](https://i.imgur.com/fUnZSTj.png) Combat

- ![](resources/assets/sprites/loose/fighter.png) **Fighter (Lv5)** - Damage +10% *(+20%)*. +15 HP.
    - _Unchanged from vanilla._
- ![](resources/assets/sprites/loose/brute.png) **Brute (Lv10)** - Taking damage builds rage, improving combat prowess. +25 HP. *Rage also grants attack speed.*
    - You may want to consider building defense bonuses. In vanilla, defense has a soft cap, not allowing damage reduction beyond 50%. This mod removes that cap, making defense more effective.
    - **Rage:** Damage +1% per stack. Max 100 stacks. Lose 1 stack every 5 seconds after 30 seconds out of combat. *Attack speed +0.5% per stack*.
- ![](resources/assets/sprites/loose/poacher.png) **Bushwhacker (Lv10)** - Crit. chance +50%. Crit. strikes can poach items. *Successfully poaching an item refunds special move cooldown.*
    - Monsters can only be poached once.
- ![](resources/assets/sprites/loose/rascal.png) **Rascal (Lv5)** - Slingshot damage +25% *(+50%)*. 60% chance to recover spent ammo.
    - In vanilla, slingshot projectiles have a short "grace period" during which it will ignore collision with enemies. This mod removes that grace period, allowing projectiles to hit enemies immediately in front of the Farmer. All professions are affected.
    -_Bonus: holding [ModKey](#configs) will fire a trick shot, which is weaker but can ricochet once. If prestiged, trick shots stun enemies for 5s._
- ![](resources/assets/sprites/loose/desperado.png) **Desperado (Lv10)** - Charge slingshot 50% faster. Can overcharge slingshots for more power, or quick-release for a double-shot. *Overcharged shots become spreadshots.*
    - **Overcharge:** Continue to hold the tool button to reveal the overcharge meter.
    - **Quick-shots:** Release the shot as soon as it completes charging to perform double-shots.
    - **Spreadhots:** Fire three projectiles in a cone. Requires overcharging at least half the bar. The higher the bar the narrower the cone. 
- ![](resources/assets/sprites/loose/piper.png) **Slimed Piper (Lv10)** - Attract more Slimes in dangerous areas. Chance to gain a random buff when a Slime is defeated. *Chance to also recover some health and energy when a Slime is defeated.*
    - Each Slime raised in a hutch adds a chance to spawn an extra Slime in dungeons, up to the number of enemies on the map.
    - Buffs are the same as food/drink buffs (skill levels, attack, defense, speed, luck, max energy, magnetism). Lasts 3 minutes and stacks indefinitely, refreshing the duration each time.
    - _Bonus: immune to the Slimed debuff, even without a Slime Charmer ring._

## Prestige

The [Statue of Uncertainty](https://stardewvalleywiki.com/The_Sewers#Statue%20Of%20Uncertainty) has been replaced by the **Statue of Prestige**.

Like it says, instead of changing your profession choices, the Statue of Prestige will reset your level 10 skills back to level 0, for a price. After resetting a skill, you will have to level up again to choose a different profession, but you get to keep every profession you've ever acquired (yes, including level 5). You will also find that leveling becomes progressively easier after each skill reset. By default, reseting a skill will also cause you to forget all associated recipes, but this may be disabled in the configs.

For this incredible service, the Statue of Prestige will charge you 10,000g the first time, 50,000g the second time, and 100,000g the last time, although the cost can also be configured. After performing three skill resets, the farmer should have acquired all four level 10 professions simultaneously. Pay attention to the tooltips to avoid choosing the same profession twice; the statue will not give refunds.

Whenever a level 10 profession is acquired, your progress will be reflected on the skills page menu, either by a new colorful star, or by a growing ribbon, depending on your settings.

Once you have acquired four stars, or the ribbon has reached its fourth stage, the level cap for the corresponding skill is raised to 20, allowing the farmer to continue to develop their tool proficiency or max health. Other perks of higher level include better odds for higher quality crops and fishes, higher amount of berries foraged per bush, and longer-lasting special abilities (see below).

On top of that, at levels 15 and 20 you will be able to choose a profession to prestige. A prestiged profession grants improved perks or, in some cases, entirely new ones.

Only after all possible skills have had their level cap raised will the Statue of Prestige resume it's old behavior, allowing you to change your prestige choices, for a modest fee of 20,000g (which is also configurable).

All of these features are optional. You may disable them at any time in your config settings. Note that doing this mid-game will not cause you to lose any professions already acquired, nor will it have any immediate effects on your skill levels. However your skill levels will be reduced to the regular cap of 10 the next time you load the save.

All custom mod skills based on SpaceCore are compatible with the skill reset feature, but cannot have their level cap raised above the regular 10.

On top the regular perks listed above, every level 10 Combat profession also grants a unique special ability. This ability must be charged by performing certain actions during combat. You can track the current charge by a the HUD bar labeled "S" ("special", or "super").

## Special Abilities

On top the regular perks listed above, every level 10 Combat profession also grants a unique special ability. This ability must be charged by performing certain actions during combat. You can track the current charge by a the HUD bar labeled "S" ("special", or "super").

Note that only a single special ability can be registered at any time; if the Combat skill is reset, you will be asked the moment you choose your next profession whether you wish to keep or replace your current special ability. The four special abilities are:

- ![](resources/assets/sprites/loose/undyingrage.png) **Undying Frenzy (Brute / Amazon)** - Doubles rage accumulation for 15s. Immune to passing out. When the effect ends, recovers 5% health for every enemy slain while the buff was active.
    - Charged by taking damage or defeating enemies. Charges more quickly if wielding a blunt weapon.
- ![](resources/assets/sprites/loose/cloaking.png) **Ambuscade (Bushwhacker)** - Become invisible and untargetable for 30s. Effect ends prematurely if the player attacks an enemy. When the effect ends, gain a 2x crit. power buff that lasts for twice the leftover invisibility duration.
    - Charged by scoring critical hits. Consider wielding a dagger.
- ![](resources/assets/sprites/loose/bullettime.png) **Death Blossom (Desperado)** - Enable auto-reload for 15s. Fire in eight directions at once.
  - Journey of the Prairie King, "IRL".
  - Charged by hitting monsters with projectiles. Charges more quickly when low on health.
- ![](resources/assets/sprites/loose/superfluidity.png) **Enthrall (Slimed Piper / Slime Enchantress)** - Charm nearby Slimes for 30s. Charmed Slimes increase in size and power and will seek out other enemies. Enemies hit will aggro onto the Slime.
  - Slimes scale up factor is random, up to twice the original size. Slimes gain a proportional damage and health boost.
  - If defeated, engorged Slimes break up into smaller baby Slimes.
  - There is also a low chance to convert Slimes to a special variant. If "Prismatic Jelly" special order is active, low chance to convert the Slime to prismatic variant.
  - Nearby Big Slimes explode immediately.
  - Charged by being touched by Slimes, or by defeating Slimes and Big Slimes.

After all possible skills have had their level cap raised, the Statue of Prestige will let you switch between the four abilities for free, though a cost can be configured.

Special abilities, like the Prestige system, is also optional, and can be disabled entirely in the mod settings.

## Compatibility

The following mods are fully integrated:

- [Luck Skill](https://www.nexusmods.com/stardewvalley/mods/521) and custom skill mods based on SpaceCore, like [Magic](https://www.nexusmods.com/stardewvalley/mods/2007) and [Love Of Cooking](https://www.nexusmods.com/stardewvalley/mods/6830)) can also be reset at the Statue of Prestige. They will display corresponding stars/ribbons in the Skills page and are affected by this mod's console commands. However they will not have their level cap raised above 10 and therefore cannot be prestiged. (I may provide an API for other mod authors to provide their own prestige professions in the future.)
- [Automate](https://www.nexusmods.com/stardewvalley/mods/1063) will apply Artisan, Ecologist and Gemologist perks to all appropriate machines only if the machine's owner (the one who crafted the machine) has the profession, and only when the owner uses the machine; in the case of machines that are not crafted, such as the Farm Cave Mushroom Boxes  and terrain features like Berry Bushes and Fruit Trees, only the session host's professions will apply.
- [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) and [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038) respect the same rules as above, but not all machines are compatible (keep reading).﻿
- Machines from the following PPJA packs and addons are compatible with the Artisan profession:
    - [Artisan Valley﻿](https://www.nexusmods.com/stardewvalley/mods/1926)
    - [Artisanal Soda Makers](https://www.nexusmods.com/stardewvalley/mods/5173)﻿
    - [Fizzy Drinks](https://www.nexusmods.com/stardewvalley/mods/5342)﻿
    - [Shaved Ice & Frozen Treats](https://www.nexusmods.com/stardewvalley/mods/5388)
    If you play with other mods that add artisan machines you can add them to the CustomArtisanMachines list in the config.
- [Animal Husbandry Mod](https://www.nexusmods.com/stardewvalley/mods/1538) will be affected by Breeder and Producer professions; Breeder will reduce pregnancy time, and Producer's price bonus will be applied to meats.
- [Fresh Meat﻿](https://www.nexusmods.com/stardewvalley/mods/1721) crops are considered animals products and not actually crops, and therefore will also benefit from Producer profession.
- [Mushroom Propagator](https://www.nexusmods.com/stardewvalley/mods/4637) will be affected by the Ecologist quality perk.
- [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) will also be tracked by Prospector.﻿﻿
- [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866/) will respect fishing profession bonuses; the optional Recatchable Legendaries file is also compatible.
- [Vintage Interface v2](https://www.nexusmods.com/stardewvalley/mods/4697) will be automatically detected and the Special Ability charge meter will be changed accordingly to match the installed version (v1 and v2 are both compatible).
- [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) will also change the look of the Special Ability charge meter in Galdora maps to match the Galdoran UI theme.
- [CJB Cheats Menu](https://www.nexusmods.com/stardewvalley/mods/4), if you download the optional translation files, will display this mod's profession names under Skill Cheats menu.﻿
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

The following mods are compatible without integration:
- [Multi Yield Crops](https://www.nexusmods.com/stardewvalley/mods/6069)
- [Craftable Mushroom Boxes](https://www.nexusmods.com/stardewvalley/mods/10296)
- [Better Beehouses](https://www.nexusmods.com/stardewvalley/mods/10996)
- [Forage Fantasy](https://www.nexusmods.com/stardewvalley/mods/7554)
- [Capstone Professions](https://www.nexusmods.com/stardewvalley/mods/7636)

The mods are **not** compatible:

- Any mods that change vanilla skills.
- [Better Crab Pots](https://www.nexusmods.com/stardewvalley/mods/3159), [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767) or any mod that affects Crab Pot behavior.
- [Better Slingshots](https://www.nexusmods.com/stardewvalley/mods/2067), [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669), or any mod that affects Slingshot behavior.
- [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley), as they will be overriden by this mod's changes to the Artisan profession (use [Quality Of Life](https://www.nexusmods.com/stardewvalley/mods/11296) instead to get the flower meads feature). 
- [All Professions](https://www.nexusmods.com/stardewvalley/mods/174) and [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#), as they conflict with this mod's Prestige system. You could potentially use them if you disable this mod's Prestige system, but I will not provide support in case of any bugs.

Compatible with Multiplayer and Splitscreen only if all players have this mod installed. Not compatible with Android version.

## Installation

- You can install this mod on an existing save; all perks will be retroactively applied upon loading a saved game.
- To install simply drop the extracted folder onto your mods folder.
- To update **make sure to delete the old version** first and only then install the new version.
- There are no dependencies other than SMAPI.

## Configs

While the vast majority of professions bonuses are non-configurable, some of the more radical changes have been given configuration options to give the user some degree of control. As such the mod provides the following config options, which can be modified either in-game via Generic Mod Config Menu or by manually editing the configs.json file:

### General Configs
- `Modkey` _(keybind)_ - The Prospector and Scavenger professions use this key to reveal the locations of key objects currently on the screen. If playing on a large screen with wide field of view, this can help locate foragables of mine nodes in large or busy maps. The default key is LeftShift for keyboards and LeftShoulder for controllers.
- `CustomArtisanMachines` _(string array)_ - Add any modded Artisan machines to this list to make them compatible with the Artisan perk (not available in GMCM).
- `PrestigeProgressionStyle` _(string)_ - Either "StackedStars", "Gen3Ribbons" or "Gen4Ribbons". Determines the way your Prestige progression is displayed in the Skills page.

### Profession Configs

- `CustomArtisanMachines` _(string list)_ - List of mod-added Artisan machines, for compatibility with the Artisan profession.
- `ShouldJunimosInheritProfessions` _(bool)_ - Whether professions bonuses (namely Harvester) should apply to Junimo harvesters.
- `ForagesNeededForBestQuality` _(uint)_ - Determines the number of items foraged from the ground, bushes or mushroom boxes, required to reach permanent iridium-quality forage as an Ecologist. Default is 500.
- `MineralsNeededForBestQuality` _(uint)_ - As above. Determines the number of minerals (gems or foraged minerals) mined or collected from geode crushers or crystalariums, required to reach permanent iridium-quality minerals as a Gemologist. Default is 500.
- `ShouldCountAutomatedHarvests` _(bool)_ - Whether Automated machines should count toward Ecologist and Gemologist goals.
- `TrackPointerScale` _(float)_ - Changes the size of the pointer used to track objects by Prospector and Scavenger professions.
- `TrackPointerBobbingRate` _(float)_ - Changes the speed at which the tracking pointer bounces up and down (higher is faster).
- `DisableAlwaysTrack` _(bool)_ - If enabled, Prospector and Scavenger will only track off-screen object while ModKey is held.
- `ChanceToStartTreasureHunt` _(float)_ - The percent chance of triggering a treasure hunt when entering a new map as Prospector or Scavenger. Note that this only affects that chance the game will try to start a treasure hunt, and the actual chance is slightly lower as the game might fail to choose a valid treasure tile. Increase this value if you don't see enough treasure hunts, or decrease it if you find treasure hunts cumbersome and don't want to lose your streak. Default is 0.2 (20%).
- `AllowScavengerHuntsOnFarm` _(bool)_ - Whether a Scavenger Hunt can trigger while entering a farm map.
- `ScavengerHuntHandicap` _(float)_ - This number multiplies the Scavener Hunt time limit. Increase this number if you find that Scavenger hunts end too quickly.
- `ProspectorHuntHandicap` _(float)_ - This number multiplies the Prospector Hunt time limit. Increase this number if you find that Prospector hunts end too quickly.
- `TreasureDetectionDistance` _(float)_ - Represents the minimum number of adjacent tiles between the player and the treasure tile before the treasure tile will be revealed by a floating arrow. Increase this value is you find treasure hunts too difficult. Default is 3.
- `SpelunkerSpeedCap` _(uint)_ - The maximum speed bonus a Spelunker can reach (values above 10 may cause problems).
- `EnableGetExcited` _(bool)_ - Toggles the Get Excited buff when a Demolitionist is hit by an explosion.
- `SeaweedIsJunk` _(bool)_ - Whether Seaweed and Algae are considered junk for fishing purposes.
- `AnglerMultiplierCap` _(float)_ - The maximum fish price multiplier that can be accumulated by Angler.
- `TrashNeededPerBonusPct` _(uint)_ - Represents the number of trash items the Conservationist must collect in order to gain a 1% tax deduction the following season. Use this value to balance your game if you use or don't use Automate. Default is 100.
- `TrashNeededPerFriendshipPoint` _(uint)_ - Represents the number of trash items the Prestiged Conservationist must collect in order to gain 1 point of friendship towards all villagers. Default is 100.
- `ConservationistTaxBonusCeiling` _(float)_ - Represents the maximum allowed tax deduction by the Ferngill Revenue Service. Set this to a sensible value to avoid breaking your game. Default is 0.37 (37% bonus value on every item).
- `PiperBuffCap` _(int)_ - The maximum stacks that can be gained for each buff stat.

### Special Ability Configs
- `EnableSpecials` _(bool)_ - Required to allow use of Special Abilities.
- `SpecialActivationKey` _(keybind)_ - This is the key that activates Special Ability for 2nd-tier combat professions. By default this is the same as Modkey, but can also be set to a different key.
- `HoldKeyToActivateSpecial` _(bool)_ -  If set to true, then the Special Ability will be activated after holding the above key for a short amount of time. If set to false, then the Special Ability will activate immediately upon pressing the key. Useful if you are running out of keys to bind, or just want to prevent accidental activation of the Special Ability. Default value is true.
- `SpecialActivationDelay` _(float)_ - If HoldKeyToActivateUltimate is set to true, this represents the number of seconds between pressing SpecialActivationKey and actually activating the Special Ability. Set to a higher value if you use Prospector profession and find yourself accidentally wasting your Special Ability in the Mines.
- `SpecialGainFactor` _(double)_ - Determines how quickly the Special Ability charges up.
- `SpecialDrainFactor` _(double)_ - Determines how quickly the Special Ability drains while active. The base duration is 15 seconds. Lower numbers make the Special Ability last longer.

### Prestige Configs
- `EnablePrestige` _(bool)_ - Whether to apply prestige changes.
- `SkillResetCostMultiplier` _(float)_ - Multiplies the base skill reset cost. Set to 0 to prestige for free.
- `ForgetRecipesOnSkillReset` _(bool)_ - Wether resetting a skill also clears all associated recipes.
- `AllowPrestigeMultiplePerDay` _(bool)_ - Whether the player can use the Statue of Prestige more than once per day.
- `BonusSkillExpPerReset` _(float)_ - Cumulative bonus that multiplies a skill's experience gain after each respective skill reset..
- `RequiredExpPerExtendedLevel` _(uint)_ - How much skill experience is required for each level up beyond 10.
- `PrestigeRespecCost` _(uint)_ - Monetary cost of respecing prestige profession choices for a skill.
- `ChangeUltCost` _(uint)_ - Monetary cost of regretting your choice of Special Ability.

### Difficulty Configs:
- `BaseSkillExpMultiplier` _(float array)_ - Multiplies all skill experience gained from the start of the game (in order: Farming, Fishing, Foraging, Mining, Combat).
- `MonsterHealthMultiplier` _(float)_ - Increases the health of all monsters.
- `MonsterDamageMultiplier` _(float)_ - Increases the damage of all monsters.
- `MonsterDefenseMultiplier` _(float)_ - Increases the damage resistance of all monsters.

## Console Commands

The mod provides several commands under the main entry point `wol`. You can type `wol help` to see a list of available commands. If the command requires any parameters, using the empty command will provide usage information.

- `levels` - List the player's current skill levels and experience.
- `set_levels` - Set the level of the specified skills to the specified values (only 0 - 10) allowed. (For DEBUG only !! This will not add recipes and other profession perks correctly).
- `reset_levels` - Reset the specified skills, or all skills if none are specified.
- `professions` - List the player's current professions.
- `add_professions` - Add the specified professions to the local player.
- `remove_professions` - Remove the specified professions from the local player, "all", or "unknown" to only remove leftover professions from uninstalled mods.
- `which_ult` - Check the currently registered Special Ability.
- `ready_ult` - Leave argument blank to max-out the Special Ability charge meter, or specify a number between 0 and 100 to set it to the specified percentage.
- `set_ult` - Change the currently registered Special Ability to that of a different combat profession.
- `max_animal_friendship` - Max-out the friendship of all owned animals, which affects their sale value as Breeder.
- `max_animal_mood` - Max-out the mood of all owned animals, which affects production frequency as Producer.
- `fishingdex` - Check your fishing progress and bonus fish value as Angler.
- `fishingdex_complete` - Max out your fishing progress and bonus fish value as Angler.
- `data` - Check current value of all mod data fields (FEcologistItemsForaged, GemologistMineralsCollected, ProspectorHuntStreak, ScavengerHuntStreak, ConservationistTrashCollectedThisSeason, ConservationistActiveTaxBonusPct).
- `set_data` - Set a new value for one of the mod data fields above.
- `events` - List currently subscribed mod events (for debugging).
- `hunt_reset` - Forcefully reset the currently active Treasure Hunt and choose a new target treasure tile.

## API

The mod exposes an API to facilitate integration with other mods. Currently exposed endpoints include:

- Checking the current quality of Ecologist forage or Gemologist minerals.
- Checking the current tax deduction bracket for Conservationist.
- Forcing new Treasure Hunt events, or interrupting active Treasure Hunts.
- Subscribing callbacks to Treasure Hunt started or ended events.
- Checking a player's registered Ultimate ability.
- Subscribeing callbacks to Ultimate charging, activation and deactivation events.
- Checking whether the Ultimate meter is currently being displayed (useful for UI mods to decide whether to reposition their own HUD elements).
- Checking the player's config settings for this mod.

To consume the API, copy both interfaces from the [Common.Integrations](../Common/Integrations/ImmersiveProfessions) namespace to your project and [ask SMAPI for a proxy](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations).

## Recommended Mods

- NEW: [Quality Of Life](https://www.nexusmods.com/stardewvalley/mods/11296), which now hosts all balancing features previously from this mod, plus new ones.
- NEW: [Aquarism](https://www.nexusmods.com/stardewvalley/mods/11288), which now hosts the old Fish Pond changes.﻿﻿
- NEW: [Fellowship](https://www.nexusmods.com/stardewvalley/mods/11380), which hosts the ring rebalance﻿ changes suited for this mod's combat professions.
- TBD: Arsenal, which will host rebalanced enchantments and weapons suited for this mod's combat professions.
- NEW: [Surfdom](https://www.nexusmods.com/stardewvalley/mods/12547)﻿, which adds a simple tax mechanic compatible with the Conservationist perk.
- [Advanced Casks](https://www.nexusmods.com/stardewvalley/mods/8413) (if you miss Oenologist profession perk).
- [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926) (add variety to Artisan products and Producer).
- [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634) (make Slime ranching more interesting and profitable).
- [Ostrich Mayo and Golden Mayo](https://www.nexusmods.com/stardewvalley/mods/7660) (better consistency for Ostrich and Golden eggs for Artisan profession).

## Special Thanks

- [Bpendragon](https://www.nexusmods.com/stardewvalley/users/20668164) for [Forage Pointers](https://www.nexusmods.com/stardewvalley/mods/7781).
- [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) for [Profession Icons Redone](https://www.nexusmods.com/stardewvalley/mods/4163).
- [HimeTarts](https://www.nexusmods.com/stardewvalley/users/108124018) for the title logo.
- [PiknikKey](https://forums.nexusmods.com/index.php?/user/97782533-piknikkey/) for Chinese translation.
- [lacerta143](https://www.nexusmods.com/stardewvalley/users/38094530) for Korean translation.
- [TehPers](https://www.nexusmods.com/stardewvalley/users/3994776) for TFO integration.
- [Goldenrevolver](https://www.nexusmods.com/stardewvalley/users/5347339) for ForageFantasy integration and troubleshooting support.﻿
- [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317) for SMAPI support.
- **ConcernedApe** for Stardew Valley.
- Game Freak, Gravity and Riot for ~~stolen~~ borrowed assets.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>


## License

See [LICENSE](../../LICENSE) for more information.
