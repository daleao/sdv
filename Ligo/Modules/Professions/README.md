<!-- Improved compatibility of back to top link. -->
<a name="readme-top"></a>

<!-- PROJECT SHIELDS -->
[![MIT License][license-shield]][license-url]

<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

![](https://stardewcommunitywiki.com/mediawiki/images/8/82/Farming_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/2/2f/Mining_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/f/f1/Foraging_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/e/e7/Fishing_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/c/cf/Combat_Skill_Icon.png)

# Ligo :: Professions



<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#overview">Overview</a></li>
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
    <li><a href="#installation">Installation</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#recommended-mods">Recommended Mods</a></li>
    <li><a href="#for-skill-mod-developers">For Skill Mod Developers</a></li>
  </ol>
</details>

</td></tr></table>

## Features

This module is an extensive overhaul of vanilla professions and skills, and makes up the core of Ligo. Almost every profession has been reworked to be an equally unique and attractive choice targetting a specific style of gameplay, many of which were simply not viable in vanilla (i.e., ranching). And while it is not guaranteed that an "optimal" path does not exist, the main goal is to create opportunities for diversified or themed strategies as well as engaging new gameplay mechanics. Gone are the ~~boring~~ uninspiring +X% sell price bonuses, and in their stead we introduce bomberman mining, thief/assassin combat, truly epic sharpshooting and even Slime taming. The new professions are meant to scale with player, provide new end-game objectives and steer the player towards new playstyles.

By popular demand this module also introduces an immersive (and completely optional) **Prestige** system, which will eventually allow the player to obtain *all* professions and open up skill progression to level 20. Also added are unique special abilities for each combat profession, with accompanying new visual and sound effects.

This module was inspired by Enai Siaion's excellent [Ordinator](https://www.nexusmods.com/skyrimspecialedition/mods/1137) for Skyrim, and borrows many ideas (and a few assets) from the likes of League of Legends](https://www.leagueoflegends.com/), [Diablo](https://diablo2.blizzard.com/) and the classic [Ragnarok Online](http://iro.ragnarokonline.com/).

Integrations are provided for several popular mods. See the [compatibility](#compatibility) section for details.

## Professions

The perks in *italics* refer to Prestiged variants. Written in *(parenthesis)*, these perks **replace**, and do not stack with, their regular counterpart.

### ![](https://i.imgur.com/p9QdB6L.png) Farming

- ![](resources/assets/sprites/loose/harvester.png) **Harvester (Lv5)** - 10% *(20%)* chance for extra yield from harvested crops.
    - Yields an equivalent 10% monetary bonus to vanilla on average, while also benefiting those who will not sell raw crops.
- ![](resources/assets/sprites/loose/agriculturist.png) **Agriculturist (Lv10)** - Crops grow 10% *(20%)* faster. Grow best-quality crops organically without fertilizer.
    - Allows harvesting iridium-quality crops, normally only possible with Deluxe Fertilizer. The chance is unchanged from vanilla, and is equal to half the chance of gold quality. This means that fertilizers are **not** obsolete, as they will still massively increase that chance.
- ![](resources/assets/sprites/loose/artisan.png) **Artisan (Lv10)** - All artisan machines work 10% *(25%)* faster. Machine output quality is at least as good as input ingredient quality.
    - Essentially implements [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) exclusively for Artisans. Also added is 5% chance to promote the output quality by one level. Note that the quality preservation perk is tied to the Artisan player, while the quality promotion perk is tied to the machine itself; in other words, only Artisans themselves can preserve the quality of ingredients, but they can do so on any machine, whereas any non-Artisan player can benefit from a quality upgrade, so long as they use a machine crafted by an Artisan player (ownerhsip requirements can be turned off in the configs).
- ![](resources/assets/sprites/loose/miner.png) **Rancher (Lv5)** - Befriend animals 2× *(3×)* quicker.
    - Gain double mood *and* friendship points from petting. Newborn animals are also born with a non-zero, random initial friendship.
- ![](resources/assets/sprites/loose/breeder.png) **Breeder (Lv10)** - Incubation 2× *(3×)* faster and natural pregnancy 3× *(5×)* more likely. Increase value of animals at high friendship.
    - At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x. If Animal Husbandry is installed, gestation following insemination is also 2x *(3x)* faster.
- ![](resources/assets/sprites/loose/producer.png) **Producer (Lv10)** - Happy animals produce 2× *(3×)* as frequently. Produce is worth 5% more for every full barn or coop.
    - Note that happiness, or mood, is **not** the same as friendship. Also note that this will **not** allow certain animals (i.e., cows and chickens) to produce more than once per day. Bonus produce value also applies to artisan goods derived from animal products (such as cheeses, mayos and cloth), honey (bees are animals too), and meat from Animal Husbandry or PPJA Fresh Meat. Also also note that only deluxe buildings can be considered full, and only barns and coops owned by the Producer will be considered (ownership requirements can be turned off in the configs). There is no limit to this bonus value.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- ![](resources/assets/sprites/loose/forager.png) **Lv5 - Forager** - 20% *(40%)* chance for double yield of foraged items.
    - Other than the name, this profession is unchanged compared to vanilla.
- ![](resources/assets/sprites/loose/ecologist.png) **Ecologist (Lv10)** - Wild berries restore 50% *(100%)* more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same deterministic quality, providing immediate inventory convenience. However, that quality will initially start out at silver, and progress to iridium as you gather foraged items. Unlike vanilla this perk will also apply to hoed forage (such as Winter Root, Snow Yams and Ginger), Coconuts shaken off palm trees, and mushrooms produced by Mushroom Boxes, but only if the cave owner (i.e., the host player) has the profession. It will likewise apply to crafted Mushroom Boxes and Mushroom Propagators, if either of those mods is installed.
- ![](resources/assets/sprites/loose/scavenger.png) **Scavenger (Lv10)** - Location of foragable items revealed. Occasionally detect buried treasure. *Time freezes during Scavenger Hunts.*
    - Whenever you are outside there is a chance to trigger a short Scavenger Hunt for hidden buried treasure. Follow the purple HUD arrow to find the treasure and dig it up (with a hoe) within the time limit to obtain a reward. The larger your win streak the better your odds of obtaining rare items. You can optionally configure the HUD arrows to only appear when holding a key (LeftShift by default). This will also highlight forageable currently on-screen for your convenience.
- ![](resources/assets/sprites/loose/lumberjack.png) **Lumberjack (Lv5)** - Felled trees yield 25% *(40%)* more wood.
    - Other than the name, this profession is unchanged compared to vanilla.
- ![](resources/assets/sprites/loose/arborist.png) **Arborist (Lv10)** - All trees grow faster. Normal trees can drop *(twice as much)* hardwood.
    - Bonus tree growth works as a global buff; i.e., all trees in the world are affected as long as any player in the session has this profession, and the effect will stack for all additional online players that share this profession. The hardwood bonus is inherited and uchanged from vanilla.
- ![](resources/assets/sprites/loose/tapper.png) **Tapper (Lv10)** - Tappers are cheaper to craft. Tapped trees produce syrup 25% *(50%)* faster.
    - New regular recipe: x25 wood, x1 copper bar.
    - New Heavy recipe: x18 hardwood, x1 radioactive bar.

### ![](https://i.imgur.com/TidtIw0.png) Mining 

- ![](resources/assets/sprites/loose/miner.png) **Miner (Lv5)** - +1 *(+2)* ore per ore vein.
    - Completely unchanged compared to vanilla.
- ![](resources/assets/sprites/loose/spelunker.png) **Spelunker (Lv10)** - Chance to find ladders and shafts increases with every mine level. +1 speed every 10 levels. *Also recover some health and stamina with every mine level.*
    - Plus 0.5% ladder chance per level. Bonus ladder chance resets each time you leave the mines. **This includes taking the stairs back to the mine entrance.**
- ![](resources/assets/sprites/loose/prospector.png) **Prospector (Lv10)** - Location of ladders and mining nodes revealed. Occasionally detect rocks with valuable minerals. *Time freezes during Prospector Hunts.*
    - Analogous to Scavenger. Tracks all mining nodes and mineral forages off-screen with a yellow pointer, ladders, shafts and panning spots (when outside) with a green pointer. Whenever you are in the mines there is a chance to trigger a short Propsector Hunt for hidden stone treasure. Follow the purple HUD arrow to find the correct stone within the time limit and break it up to obtain a reward. The larger your win streak the better your odds of obtaining rare minerals or artifacts. Succesful completion of a hunt automatically reveals a ladder. You can optionally configure the HUD arrows to only appear when holding a key (LeftShift by default). This will also highlight mineral nodes and other tiles of interest currently on-screen for your convenience.
- ![](resources/assets/sprites/loose/blaster.png) **Blaster (Lv5)** - Craft twice as many explosives. Exploded rocks yield 2× *(3×)* as much coal.
    - This aims to provide a new style of mining while attempting to compensate for the lack of coal without the vanilla Prospector profession.
- ![](resources/assets/sprites/loose/demolitionist.png) **Demolitionist (Lv10)** - Bomb radius +1. Exploded rocks yield 20% *(40%)* more resources.
    - This aims to improve the bomberman style of mining while attempting to compensate for the lack of Geologist and Gemologist professions from vanilla. As a configurable bonus, the pyromaniac in your will [get excited](https://www.youtube.com/watch?v=0nlJuwO0GDs) when hit by an explosion.
- ![](resources/assets/sprites/loose/gemologist.png) **Gemologist (Lv10)** - Progressively identify gems and minerals of higher quality. Crystalariums work 25% *(50%)* faster.
    - Analogous to Ecologist. All gems and minerals mined from nodes have a fixed quality, starting at silver and increasing to iridium as you mine. Please note that this bonus will only apply to gems or minerals that have been either physically mined, or produced by Geode Crushers owned by the Gemologist. Crystalariums and geodes opened by Clint will **not** receive quality upgrades. The exception to this is Crystalariums already in production, which will all receive an equivalent quality upgrade whenever the owner reaches a quality milestone. The production time bonus for Crystalarium is likewise tied to the owner, and therefore only Crystalariums crafted by the Gemologist will receive that perk (ownerhsip requirements can be turned off in the configs).

### ![](https://i.imgur.com/XvdVsAn.png) Fishing

- ![](resources/assets/sprites/loose/fisher.png) **Fisher (Lv5)** - Fish bite faster *(instantly)*. Live bait reduces the chance to fish junk.
    - Here, "junk" includes algae and seaweed.
- ![](resources/assets/sprites/loose/angler.png) **Angler (Lv10)** - Fish worth 1% more for every unique max-sized fish caught and 5% more for every legendary fish. *Can recatch legendary fish.*
    - "Legendary fish" includes the Extended Family Qi challenge varieties, counted only once.
- ![](resources/assets/sprites/loose/aquarist.png) **Aquarist (Lv10)** - Fish pond max capacity +2. Catching bar decreases slower for every unique fish species raised in a fish pond. *Can raise legendary fish.*
    - The catching bar decays 5.5% slower per unique Fish Pond. Six ponds are equivalent to a permanent Trap Bobber. In multiplayer, **only counts Fish Ponds owned by the player with this profession**.
    - *Legendary fish and extended family can be raised in the same pond.*
- ![](resources/assets/sprites/loose/trapper.png) **Trapper (Lv5)** - Crab pots are cheaper to craft. Can trap higher-quality *(highest-quality)* haul.
    - All trapped fish can have quality up to gold. Chance depends on fishing level (same formula as forage). The Crab Pot recipe is unchanged from vanilla.
- ![](resources/assets/sprites/loose/luremaster.png) **Luremaster (Lv10)** - Crab pots no longer produce junk. Use different baits to attract different catch. *60% chance to preserve bait.*
    - Each type bait will also apply it's regular fishing effects:
        - **Regular bait:** 25% chance to catch fish, subject to the same location and season limitations as regular fishing.
        - **Wild bait:** 25% chance to also double the haul.
        - **Magnet:** Repels all fish (as per its description), but attracts metal items such as resources, artifacts, treasure chests, rings and even some weapons (treasure table is similar to fishing treasure chests).
        - **Magic bait:** 25% chance to catch fish of any location or season. Also upgrades all catch to iridium-quality.
- ![](resources/assets/sprites/loose/conservationist.png) **Conservationist (Lv10)** - Crab pots without bait can trap junk. Clean the Valley's waters to merit tax deductions. *Cleaning the Valley's waters also merits favor with the villagers.*
    - Every 100 (configurable) junk items collected will earn you a 1% tax deduction the following season (max 25%, also configurable). What a "tax deduction" means depends on whether the [Taxes](../Taxes/README.md) module is enabled; if it is enabled, a tax deduction works as you would expect, reducing your overall amount due. If this module is not enabled, then a tax deduction works as a % value increasing to all items shipped in the bin. If you quality for a deduction you will receive a formal mail from the Ferngill Revenue Service on the first of the season informing your currrent deduction rights.

### ![](https://i.imgur.com/fUnZSTj.png) Combat

- ![](resources/assets/sprites/loose/fighter.png) **Fighter (Lv5)** - Damage +10% *(+15%)*. +15 HP.
    - Completely unchanged compared to vanilla.
- ![](resources/assets/sprites/loose/brute.png) **Brute / Amazon (Lv10)** - Taking damage builds rage, improving damage. +25 HP. *Rage also grants attack speed.*
    - **Rage:** Damage +1% per stack. Max 100 stacks. Rage slowly decays after not dealing or taking damage for 25s. *Attack speed +0.5% per stack*.
- ![](resources/assets/sprites/loose/poacher.png) **Bushwhacker (Lv10)** - Crit. chance +50%. Crit. strikes can poach items. *Successfully poaching an item refunds special move cooldown.*
    - **Poaching:** Each monsters can only be poached once. Any item from the monster's available loot table can be obtained by this method.
- ![](resources/assets/sprites/loose/rascal.png) **Rascal (Lv5)** - Gain one additional ammo slot. 35% *(70%)* chance to recover spent ammo.
    - Press the Mod Key (default LeftShift) to cycle between equipped ammos.
    - Can equip Slime as ammo, which causes a slow debuff to enemies.
    - Squishy ammo (i.e., fish, fruits, veggies, and slime) and explosive ammo canot be recovered.
- ![](resources/assets/sprites/loose/desperado.png) **Desperado (Lv10)** - Firing speed is higher at lower HP. Can overcharge slingshots to increase ammo power and cross-section. *Overcharged shots can also pierce enemies.*
    - **Overcharge:** Continue to hold the tool button to reveal the overcharge meter. Overcharged shots have increased velocity, damage, knockback and cross-section (i.e., hitbox).
    - **Pierce:** Piercing chance begins at 50% and increases to 100% at full overcharge. Each pierced enemy decreases ammo power and subsequent pierce chances by 25%. Squishy projectiles (i.e., fish, fruits, veggies, and slime) cannot pierce regardless of overcharge.
- ![](resources/assets/sprites/loose/piper.png) **Slimed Piper / Enchantress (Lv10)** - Attract ally Slimes when near enemies. Chance to gain a random buff when a Slime is defeated. *Chance to also recover some health and energy when a Slime is defeated.*
    - Each Slime raised in a hutch adds a chance to spawn an extra Slime in dungeons and dangeorus areas, up to the number of enemies on the map.
    - Obtainable buffs are the same as food/drink buffs (skill levels, attack, defense, speed, luck, max energy, magnetism). A buff lasts real-life 3 minutes and stack indefinitely, refreshing the duration each time.
    - Immune to the Slimed debuff, even without a Slime Charmer ring (but **not** immune to Slime damage).
    - Slime ammo deals twice as much damage, and can be used to heal ally Slimes.

## Prestige

If this feature is enabled, the [Statue of Uncertainty](https://stardewvalleywiki.com/The_Sewers#Statue%20Of%20Uncertainty) is replaced by the **Statue of Prestige**.

Instead of changing your profession choices, the Statue of Prestige will reset your level 10 skills back to level 0, for a price. After resetting a skill, you will have to level up again to choose a different profession, but you get to keep every profession you've ever acquired (yes, including level 5). You will also find that leveling becomes progressively easier after each skill reset (or harder, depending on your config settings). By default, reseting a skill will also cause you to forget all associated recipes, but can also be turned off. For this incredible service, the Statue of Prestige will charge you 10,000g the first time, 50,000g the second, and 100,000g the third and last time, although the cost can also be configured. After performing three skill resets, you should have acquired all four level 10 professions simultaneously. As you reset and acquire new professions your progress will be reflected on the skills page menu, either by a new colorful star, or by a growing ribbon, depending on your settings.

Once you have acquired four stars, or the ribbon has reached its fourth stage, the level cap for the corresponding skill is raised to 20, allowing you to continue to develop your tool proficiency or max health. Other perks of higher levels include better odds of higher-quality crops, fishes and forage, a larger fishing bar, more berries foraged per bush, and longer-lasting special abilities (see below). On top of that, at levels 15 and 20 you will be able to choose a profession to **prestige**. A prestiged profession grants improved perks or, in some cases, entirely new ones.

Only after all possible skills have had their level cap raised will the Statue of Prestige resume it's old behavior, by allowing you to change your prestige choices, for a modest fee of 20,000g (also configurable).

The entire Prestige system is optional. It may be turned off at any time, but keep in mind that doing so mid-game will not cause you to lose any already acquired professions, nor will it have any immediate effects on your skill levels. However your skill levels will be reduced to the regular cap of 10 the next time the save is loaded.

All custom mod skills based on SpaceCore are compatible with the skill reset feature, but cannot have their level cap raised above the regular 10. For skill mod developers, if you wish to provide prestiged professions you can do so by register your skill via the provided [API](../../../Shared/Integrations/Ligo/ILigoApi.cs).

## Special Abilities

In addition to their regular perks listed above, every level 10 profession in the Combat skill tree also grants a unique **special ability**. This ability must be charged by performing certain actions during combat. You can track the current charge by the HUD bar labeled "S" (for "special", or "super").

Note that, though all combat professions can be acquired via skill reset, only one special ability can be registered at any time; if the Combat skill is reset, you will be asked the moment you choose your next profession whether you wish to keep or replace your current special ability. The four special abilities are:

- ![](resources/assets/sprites/loose/undyingrage.png) **Undying Frenzy (Brute / Amazon)** - Doubles rage accumulation for 15s. Immune to passing out. When the effect ends, recovers 5% health for every enemy slain while the buff was active.
    - Charged by taking damage or defeating enemies. Charges more quickly if enemies are defeated using a blunt weapon.
- ![](resources/assets/sprites/loose/cloaking.png) **Ambuscade (Bushwhacker)** - Become invisible and untargetable for 30s. Effect ends prematurely if the player attacks an enemy. When the effect ends, gain a 2x crit. power buff that lasts for twice the leftover invisibility duration. If an enemy is slain within 1.5s out of Ambush, immediately regain 20% special ability charge.
    - Charged by scoring critical hits, and the charge amount is proportional to crit. power.
- ![](resources/assets/sprites/loose/bullettime.png) **Death Blossom (Desperado)** - For 15s enable auto-fire in eight directions at once.
  - Journey of the Prairie King, "IRL".
  - Charged by hitting monsters with projectiles. Charges more quickly when low on health, or by scoring trick shots.
- ![](resources/assets/sprites/loose/superfluidity.png) **Hamelin Concerto (Slimed Piper / Enchantress)** - Charm nearby Slimes for 30s. Charmed Slimes increase in size and power and seek out other enemies, tanking and stealing their aggro.
  - The amount by which each Slime will grow is random, up to twice the original size. Slimes gain a proportional damage and health boost.
  - If defeated, engorged Slimes break up into smaller baby Slimes.
  - There is also a low chance to convert Slimes to a special variant. If "Prismatic Jelly" special order is active, low chance to convert the Slime to the prismatic variant.
  - Nearby Big Slimes explode immediately.
  - Charged by being touched by Slimes, or by defeating Slimes and Big Slimes.

After all possible skills have had their level cap raised, the Statue of Prestige will let you switch between the four abilities for free (though a cost can be configured).
Switching special abilities is not possible if the Prestige system is disabled. Like the Prestige system, special abilities can also be turned off in the configs.

## Compatibility

The following mods are fully integrated:

- All [SpaceCore][spacecore-url] custom skills are fully supported by the skill reset systems, including but not limited to: [Luck Skill][luck-skill-url], [Cooking][cooking-url], [Magic][magic-url], [Love Of Cooking][love-of-cooking-url], [Binning][binning-url] and [Socializing][socializing-url]. However, *only skills which explicitly provide prestiged professions will be supported by the Prestige system*. An API is provided for mod authors to register their custom skills for prestige. Authors are responsible for providing the prestige perks and functionality of their own professions,as well as corresponding prestige icons and descriptions.
- [Automate][automate-url] machines will apply Artisan, Ecologist and Gemologist perks to all appropriate machines which meet the ownership requirements (if that setting is enabled). In the case of machines that are not crafted, such as the Farm Cave Mushroom Boxes  and terrain features like Berry Bushes and Fruit Trees, only the session host's professions will apply. In cases which consider the professions of the user, and not of the owner, then the owner of the closest chest in the automation group is used.
- [Producer Modules Mod](https://www.nexusmods.com/stardewvalley/mods/4970) and [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038) respect the same rules as above, but not all machines are compatible by default. Machines from the following PPJA packs and addons are compatible out-of-the-box:
    - [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926)
    - [Artisanal Soda Makers](https://www.nexusmods.com/stardewvalley/mods/5173)
    - [Fizzy Drinks](https://www.nexusmods.com/stardewvalley/mods/5342)
    - [Shaved Ice & Frozen Treats](https://www.nexusmods.com/stardewvalley/mods/5388)
    If you play with other mods that add artisan machines you can add them to the CustomArtisanMachines list in the config. Note that this list cannot be configured in-game via GMCM; it must be configured manually in the config.json file.
- [Animal Husbandry Mod](https://www.nexusmods.com/stardewvalley/mods/1538) will be affected by Breeder and Producer professions; Breeder will reduce pregnancy time, and Producer's price bonus will be applied to meats.
- [Fresh Meat](https://www.nexusmods.com/stardewvalley/mods/1721) crops are considered animals products and not actually crops, and therefore will also benefit from Producer profession.
- [Mushroom Propagator](https://www.nexusmods.com/stardewvalley/mods/4637) will be affected by the Ecologist quality perk.
- [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) will also be tracked by Prospector.
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

The following mods are **not** compatible:

- Any mods that change vanilla skills.
- [Better Crab Pots](https://www.nexusmods.com/stardewvalley/mods/3159), [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767) or any mod that affects Crab Pot behavior.
- [Better Slingshots](https://www.nexusmods.com/stardewvalley/mods/2067), [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669), or any mod that affects Slingshot behavior.
- [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley), as they will be overriden by this mod's changes to the Artisan profession (use [Quality Of Life](https://www.nexusmods.com/stardewvalley/mods/11296) instead to get the flower meads feature). 
- [All Professions](https://www.nexusmods.com/stardewvalley/mods/174) and [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#), as they conflict with this mod's Prestige system. You could potentially use them if you disable this mod's Prestige system, but I will not provide support in case of any bugs.

## Installation

This module can be installed and enabled in existing saves and all perks will be retroactively applied upon loading a saved game. The only exceptions are Ecologist and Gemologist perks, which will have to be leveled up from scratch.

It is also safe to uninstall or disable without special measures.

## API

The module exposes an API to facilitate integration with other mods. Currently exposed endpoints include:

- Checking the current quality of Ecologist forage or Gemologist minerals.
- Checking the current tax deduction bracket for Conservationist.
- Forcing new Treasure Hunt events, or interrupting active Treasure Hunts.
- Subscribing callbacks to Treasure Hunt started or ended events.
- Checking a player's registered Ultimate ability.
- Subscribeing callbacks to Ultimate charging, activation and deactivation events.
- Checking whether the Ultimate meter is currently being displayed (useful for UI mods to decide whether to reposition their own HUD elements).
- Checking the player's config settings for this mod.
- Registering custom skills for Prestige.

To consume the API, copy both interfaces from the [Common.Integrations](../../../Shared/Integrations/Ligo) folder to your project and [ask SMAPI for a proxy](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations).

## Recommended Mods

- [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926) (adds variety to Artisan products and Producer).
- [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634) (makes Slime ranching more interesting and profitable).
- [Ostrich Mayo and Golden Mayo](https://www.nexusmods.com/stardewvalley/mods/7660) (better consistency for Ostrich and Golden eggs in combination with Artisan profession).



<!-- MARKDOWN LINKS & IMAGES -->
[license-shield]: https://img.shields.io/badge/license-Commons%20Clause%20(MIT)-orange
[license-url]: ./LICENSE

<!-- MOD LINKS -->
[mod:spacecore]: <https://www.nexusmods.com/stardewvalley/mods/1348> "SpaceCore"
[mod:luck-skill]: <https://www.nexusmods.com/stardewvalley/mods/521> "Luck Skill"
[mod:cooking-skill]: <https://www.nexusmods.com/stardewvalley/mods/522> "Cooking Skill"
[mod:magic]: <https://www.nexusmods.com/stardewvalley/mods/2007> "Magic"
[mod:love-of-cooking]: <https://www.nexusmods.com/stardewvalley/mods/6830> "Love Of Cooking"
[mod:binning-skill]: <https://www.nexusmods.com/stardewvalley/mods/14073> "Binning Skill"
[mod:socializing-skill]: <https://www.nexusmods.com/stardewvalley/mods/14142> "Socializing Skill"
[mod:forage-pointers]: <https://www.nexusmods.com/stardewvalley/mods/7781> "Forage Pointers"

<!-- USER LINKS -->
[user:pathoschild]: <https://www.nexusmods.com/stardewvalley/users/1552317> "Pathoschild"
[user:tehpers]: <https://www.nexusmods.com/stardewvalley/users/3994776> "TehPers"
[user:goldenrevolver]: <https://www.nexusmods.com/stardewvalley/users/5347339> "Goldenrevolver"
[user:bpendragon]: <https://www.nexusmods.com/stardewvalley/users/20668164> "Bpendragon"
[user:illogicalmoodswing]: <https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/> "IllogicalMoodSwing"
[user:himetarts]: <https://www.nexusmods.com/stardewvalley/users/108124018> "HimeTarts"
[user:piknikkey]: <https://forums.nexusmods.com/index.php?/user/97782533-piknikkey/> "PiknikKey"
[user:lacerta143]: <https://www.nexusmods.com/stardewvalley/users/38094530> "lacerta143"

<!-- OTHER LINKS -->
[co:gamefreak]: <https://www.gamefreak.co.jp/> "GameFreak"
[co:gravity]: <https://www.gravity.co.kr/> "Gravity"
[co:riot]: <https://www.riotgames.com/> "Riot Games"