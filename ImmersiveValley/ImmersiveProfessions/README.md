<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->
![](https://gitlab.com/theLion/smapi-mods/-/raw/main/ImmersiveProfessions/title_2.png)
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
    <li><a href="#ultimates">Ultimates</a></li>
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
- _[Optional]_ Lore-friendly multi-profession.
- _[Optional]_ Level 20 skills and prestiged professions.
- Gender-specific profession title.
- New icons for most professions, courtesy of [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) (please make sure to [endorse their original](https://www.nexusmods.com/stardewvalley/mods/4163) mod).
- New sound effects and visuals for certain professions.
- HUD elements compatible with SVE's Galdora and [Vintage Interface v2﻿](https://www.nexusmods.com/stardewvalley/mods/4697).

Skill-related features:
- Combat levels which reward a profession (multiples of 5) now also reward 5 HP. At level 10 the player will have 200 health instead of 190.

Other balancing features:
- Meads take after the honey type. Matching [BAGI](https://www.nexusmods.com/stardewvalley/mods/2080) ﻿icons are provided for vanilla flowers.
- _[Optional]_ Rebalanced Forges:
    - Jade: +10% -> +50% crit. power per level.
    - Topaz: +1 -> +5 defense per level.
    - Features intended to balance the new Brute and Poacher professions.
- _[Optional]_ Rebalanced Rings:
    - Jade: +10% -> +30% crit. power.
    - Topaz: nothing -> +3 defense, for consistency with enchantment.
    - Crab: +5 defense -> +8 defense, to distinguish from topaz.
    - Features intended to balance the new Brute and Poacher professions.

Integration is provided for Automate and several popular mods. See the [compatibility](#compatibility) section for details.

## Professions

### ![](https://i.imgur.com/p9QdB6L.png) Farming

- **Lv5 - Harvester** - 10% *(20%)* chance for extra yield from harvested crops.
    - Yields an equivalent 10% monetary bonus to vanilla on average, while also benefiting anybody who will not sell raw crops.
- **Lv10 - Agriculturist** - Crops grow 10% *(20%)* faster. Grow best-quality crops organically without fertilizer.
    - Allows harvesting of iridium-quality crops without any fertilizer. The chance is unchanged from vanilla, and is equal to half the chance of gold quality. Fertilizers will still massively increase that chance.
- **Lv10 - Artisan** - All artisan machines work 10% *(25%)* faster. Machine output quality is at least as good as input ingredient quality.
    - Essentially implements [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) (QAP), but exclusively for Artisans. Also adds 5% chance to promote the output quality by one level. In multiplayer, **the bonus production speed applies only to machines crafted by the player with this profession, and only when that player uses the machine**.
- **Lv5 - Rancher** - Befriend animals 2× *(3×)* quicker.
    - Gain double mood and friendship points from petting. Newborn animals are born with some starting friendship between 150 and 250 (out of 1000 maximum), chosen at random.
- **Lv10 - Breeder** - Incubation 2× *(3×)* faster and natural pregnancy 3× *(5×)* more likely. Increase value of animals at high friendship.
    - At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x. Animal Husbandry: gestation following insemination is also 2x *(3x)* faster.
- **Lv10 - Producer** - Happy animals produce 2× *(3×)* as frequently. Produce worth 5% more for every full barn or coop.
    - Note that happiness (mood) is **not** the same as friendship. Also note this will **not** allow certain animals (i.e. cows and chickens) to produce more than once per day. Bonus produce value also applies to artisan goods derived from animal products (i.e. cheeses, mayos and cloth), honey (bees are animals), and meat (Animal Husbandry or Fresh Meat). Only deluxe buildings can be considered full. **Note that honey is also considered an animal product.** There is no limit to the scaling. In multiplayer, **the bonus applies only to barns and coops owned by the player with this profession, and only when that player sells the produce**.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- **Lv5 - Forager** - 20% *(40%)* chance for double yield of foraged items.
    - _Unchanged effect from vanilla. Only the name is changed._
- **Lv10 - Ecologist** - Wild berries restore 50% *(100%)* more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same deterministic quality. This immediate gives inventory convenience. However the quality will start off at silver, and progress to iridium when enough items have been foraged. Applies to Mushroom Boxes, but only if the cave owner (i.e. the host player) has the profession.
- **Lv10 - Scavenger** - Location of foragable items revealed. Occasionally detect buried treasure. *Time freezes during Scavenger Hunts.*
    - On switching maps while outside you will occasionally detect hidden buried treasure. Find it and dig it up (with a hoe) within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items.
    - _Bonus: holding [ModKey](#configs) will highlight all foragables on-screen._
- **Lv5 - Lumberjack** - Felled trees yield 25% *(40%)* more wood.
    - _Unchanged effect from vanilla. Only the name is changed._
- **Lv10 - Arborist** - All trees grow faster. Normal trees can drop *(twice as much)* hardwood.
    - Bonus tree growth works as a global buff; i.e. in multiplayer, all trees will be affected as long as any player in the session has this profession, and the effect will stack for all additional online players that share this profession. _The hardwood bonus is unchanged from vanilla._
- **Lv10 - Tapper** - Tappers are cheaper to craft. Tapped trees give syrup 25% *(50%)* faster.
    - New regular recipe: x25 wood, x1 copper bar.
    - New Heavy recipe: x20 hardwood, x1 iridium bar, x1 radioactive ore.

### ![](https://i.imgur.com/TidtIw0.png) Mining 

- **Lv5 - Miner** - +1 *(+2)* ore per ore vein.
    - _Unchanged from vanilla._
- **Lv10 - Spelunker** - Chance to find ladders and shafts increases with every mine level. +1 speed every 10 levels. *Also recover some health and stamina with every mine level.*
    - Plus 0.5% ladder chance per level. Bonus ladder chance resets every time you leave the mines. **This includes taking the stairs back to the mine entrance.**
- **Lv10 - Prospector** - Location of ladders and mining nodes revealed. Occasionally detect rocks with valuable minerals. *Time freezes during Scavenger Hunts.*
    - Analogous to Scavenger. Tracks any mining nodes or mineral forages off-screen with a yellow pointer, and any ladders or shafts with a green pointer. On entering a new mine floor you will occasionally detect stones with prospective treasure. Find the stone and break it within the time limit to obtain a reward. The larger your win streak the better your odds of obtaining rare items. Completing a hunt automatically reveals a ladder.
    - _Bonus: holding [ModKey](#configs) will highlight all nodes and ladders on-screen._
- **Lv5 - Blaster** - Craft twice as many explosives. Exploded rocks yield 2× *(3×)* as much coal.
- **Lv10 - Demolitionist** - Bomb radius +1. Exploded rocks yield 20% *(40%)* more resources.
    - _Bonus: [Get excited!](https://www.youtube.com/watch?v=0nlJuwO0GDs) when hit by an explosion._
- **Lv10 - Gemologist** - Progressively identify gems and minerals of higher quality. Crystalariums work 25% *(50%)* faster.
    - Analogous to Ecologist. All gems and minerals mined from nodes have a fixed quality, starting at silver and increasing once enough minerals have been collected. Minerals collected from Crystalariums and Geode Crushers are counted for this total, **but not those from geodes broken at Clint's**. In multiplayer, **the bonus Crystalarium speed applies only to machines crafted by the player with this profession, and only when used by that player**.

### ![](https://i.imgur.com/XvdVsAn.png) Fishing

- **Lv5 - Fisher** - Fish bite faster *(instantly)*. Live bait reduces the chance to fish junk.
    - Here, "junk" includes algae and seaweed.
- **Lv10 - Angler** - Fish worth 1% more for every unique max-sized fish caught and 5% more for every legendary fish. *Can recatch legendary fish.*
    - "Legendary fish" includes the Extended Family Qi challenge varieties, counted only once.
- **Lv10 - Aquarist** - Fish pond max capacity +2. Catching bar decreases slower for every unique fish species raised in a fish pond. *Can raise legendary fish.*
    - The catching bar decays 5.5% slower per unique Fish Pond. Six ponds equal a permanent Trap Bobber. In multiplayer, **only counts Fish Ponds owned by the player with this profession**.
    - *Legendary fish and extended family can be raised in the same pond.*
- **Lv5 - Trapper** - Crab pots are cheaper to craft. Can trap higher-quality *(highest-quality)* haul.
    - All trapped fish can have quality up to gold. Chance depends on your fishing level (same formula as forage). _Recipe is unchanged from vanilla._
- **Lv10 - Luremaster** - Crab pots no longer produce junk. Use different baits to attract different catch. *60% chance to preserve bait.*
    - Each type bait will also apply it's regular fishing effects:
        - **Regular bait:** 25% chance to catch fish, subject to the same location and season limitations as regular fishing.
        - **Wild bait:** 25% chance to also double the haul.
        - **Magnet:** Repels all fish (as per its description), but attracts metal items such as resources, artifacts, treasure chests, rings and even weapons.
        - **Magic bait:** 25% chance to catch fish of any location or season. Also upgrades all catch to iridium-quality.
- **Lv10 - Conservationist** - Crab pots without bait can trap junk. Clean the Valley's waters to merit tax deductions. *Cleaning the Valley's waters also merits favor with the villagers.*
    - Every 100 (configurable) junk items collected will earn you a 1% tax deduction the following season (max 25%, also configurable), increasing the value of all shipped goods. You will receive a formal mail from the Ferngill Revenue Service each season informing your currrent tax bracket.

### ![](https://i.imgur.com/fUnZSTj.png) Combat

- **Lv5 - Fighter** - Damage +10% *(+20%)*. +15 HP.
    - _Unchanged from vanilla._
- **Lv10 - Brute** - Taking damage builds rage, improving combat prowess. +25 HP. *Rage also grants attack speed.*
    - You may want to consider building defense bonuses. In vanilla, defense has a soft cap, not allowing damage reduction beyond 50%. This mod removes that cap, making defense more effective.
    - **Rage:** Damage +1% per stack. Max 100 stacks. Lose 1 stack every 5 seconds after 30 seconds out of combat. *Attack speed +0.5% per stack*.
- **Lv10 - Bushwhacker** - Crit. chance +50%. Crit. strikes can poach items. *Successfully poaching an item refunds special move cooldown.*
    - Monsters can only be poached once.
- **Lv 5 - Rascal** - Slingshot damage +25% *(+50%)*. 60% chance to recover spent ammo.
    - In vanilla, slingshot projectiles have a short "grace period" during which it will ignore collision with enemies. This mod removes that grace period, allowing projectiles to hit enemies immediately in front of the Farmer. All professions are affected.
    -_Bonus: holding [ModKey](#configs) will fire a trick shot, which is weaker but can ricochet once. If prestiged, trick shots stun enemies for 5s._
- **Lv10 - Desperado** - Charge slingshot 50% faster. Can overcharge slingshots for more power, or quick-release for a double-shot. *Overcharged shots become spreadshots.*
    - **Overcharge:** Continue to hold the tool button to reveal the overcharge meter.
    - **Quick-shots:** Release the shot as soon as it completes charging to perform double-shots.
    - **Spreadhots:** Fire three projectiles in a cone. Requires overcharging at least half the bar. The higher the bar the narrower the cone. 
- **Lv10 - Slimed Piper** - Attract more Slimes in dangerous areas. Chance to gain a random buff when a Slime is defeated. *Chance to also recover some health and energy when a Slime is defeated.*
    - Each Slime raised in a hutch adds a chance to spawn an extra Slime in dungeons, up to the number of enemies on the map.
    - Buffs are the same as food/drink buffs (skill levels, attack, defense, speed, luck, max energy, magnetism). Lasts 3 minutes and stacks indefinitely, refreshing the duration each time.
    - _Bonus: immune to the Slimed debuff, even without a Slime Charmer ring._

## Prestige

The [Statue of Uncertainty](https://stardewvalleywiki.com/The_Sewers#Statue%20Of%20Uncertainty) has been replaced by the Statue of Prestige, which is capable of resetting level 10 skills, for a price. A skill reset preserves all related professions, and (optionally) forgets all related recipes. The farmer can use this to eventually acquire all 30 professions simultaneously.
Resetting a skill costs 10,000g the first time, 50,000g the second time, and 100,000g the last time (values are configurable). The ribbon in the skills page of the game menu reflects the number of professions acquired in each skill.

Once the ribbon has reached its fourth stage, signaling that all professions have been obtained, its level cap is raised to 20, allowing the farmer to continue developing tool proficiency or max health. Skill level also affects the odds of higher quality crops and fishes, the amount of berries foraged per bush, and the duration of Ultimate and related perks.
 
At levels 15 and 20, the farmer can choose a profession to prestige, improving one of its base perks, or, in some cases, granting entirely new ones. These choices can later be changed at the Statue of Prestige, for a cost.

All Prestige features are optional and may be disabled or customized in the configs. Non-vanilla skills can be reset, but their level cap cannot be raised.

## Ultimates

Each level 10 Combat profession is also granted an Ultimate ability. This ability must be charged by performing certain actions during combat. The current charge state is revealed by a new HUD bar next to health. The player can only have a single Ultimate ability registered at any time; i.e. acquiring all combat professions via skill resetting will not grant additional Ultimates beyond the first. The overnight level-up menu will prompt the player if they wish to replace their current Ultimate ability with the new professions'.

- **Brute / Amazon - Undying Frenzy** - Doubles rage accumulation for 15s. Immune to passing out. When the effect ends, recovers 5% health for every enemy slain while the buff was active.
    - Charged by taking damage or defeating enemies. Charges more quickly if wielding a blunt weapon.
- **Bushwhacker - Ambuscade** - Become invisible and untargetable for 30s. Effect ends prematurely if the player attacks an enemy. When the effect ends, gain a 2x crit. power buff that lasts for twice the leftover invisibility duration.
    - Charged by scoring critical hits. Consider wielding a dagger.
- **Desperado - Death Blossom** - Enable auto-reload for 15s. Fire in eight directions at once.
  - Journey of the Prairie King, "IRL".
  - Charged by hitting monsters with projectiles. Charges more quickly when low on health.
- **Slimed Piper / Slime Enchantress - Pandemonium** - Charm nearby Slimes for 30s. Charmed Slimes increase in size and power and will seek out other enemies. Enemies hit will aggro onto the Slime.
  - Slimes scale up factor is random, up to twice the original size. Slimes gain a proportional damage and health boost.
  - If defeated, engorged Slimes break up into smaller baby Slimes.
  - There is also a low chance to convert Slimes to a special variant. If "Prismatic Jelly" special order is active, low chance to convert the Slime to prismatic variant.
  - Nearby Big Slimes explode immediately.
  - Charged by being touched by Slimes, or by defeating Slimes and Big Slimes.


Once all 30 professions have been acquired, the Statue of Prestige may be used to switch Ultimates at will.

Ultimates can be disabled via a config setting if desired.

## Compatibility

The following mods are fully integrated:

- [Automate](https://www.nexusmods.com/stardewvalley/mods/1063) (for craftable machines, the machine's owner's professions will apply; for terrain features, i.e. berry bushes, only the session host's professions will apply).
- [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) and [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038) (same rules apply as above).
- [Animal Husbandry Mod](https://www.nexusmods.com/stardewvalley/mods/1538) (with benefits for Breeder and Producer professions).
- PPJA Packs: [Artisan Valley﻿](https://www.nexusmods.com/stardewvalley/mods/1926), [Artisanal Soda Makers](https://www.nexusmods.com/stardewvalley/mods/5173)﻿, [Fizzy Drinks](https://www.nexusmods.com/stardewvalley/mods/5342)﻿, [Shaved Ice & Frozen Treats](https://www.nexusmods.com/stardewvalley/mods/5388) will all work with Artisan profession.﻿ [Fresh Meat﻿](https://www.nexusmods.com/stardewvalley/mods/1721) as well, though meat crops will count as animal products and not as crops, and will thus benefit from Producer profession.
- [CJB Cheats Menu](https://www.nexusmods.com/stardewvalley/mods/4) (download the optional translation files to change profession names under skill cheats).
- [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866/) (the optional Recatchable Legendaries file is also compatible).
- [Custom Ore Nodes](https://www.nexusmods.com/stardewvalley/mods/5966) (will also be tracked by Prospector).﻿
- [Mushroom Propagator](https://www.nexusmods.com/stardewvalley/mods/4637) (applies Ecologist perks).
- [Vintage Interface v2](https://www.nexusmods.com/stardewvalley/mods/4697) (enable via configs).
- [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) (provides a compatible Galdoran-theme Ultimate bar if SVE is installed).﻿
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

The following mods are compatible without integration:
- [Multi Yield Crops](https://www.nexusmods.com/stardewvalley/mods/6069)
- [Craftable Mushroom Boxes](https://www.nexusmods.com/stardewvalley/mods/10296)
- [Better Beehouses](https://www.nexusmods.com/stardewvalley/mods/10996) (the mod author already handled compatibility).
- [Forage Fantasy](https://www.nexusmods.com/stardewvalley/mods/7554) (just make sure BerryBushQuality and MushroomBoxQuality are disabled to avoid interfering with Ecologist profession; if both mods are installed, those settings should be disabled by default).
- [Capstone Professions](https://www.nexusmods.com/stardewvalley/mods/7636) (though I don't recommend it if prestige and extended progression options are enabled).
- Custom SpaceCore skills (e.g. [Luck](https://www.nexusmods.com/stardewvalley/mods/521), [Magic](https://www.nexusmods.com/stardewvalley/mods/2007), [Love Of Cooking](https://www.nexusmods.com/stardewvalley/mods/6830)) can be reset at the Statue of Prestige, but cannot be leveled above 10.


The mods are **not** compatible:

- Any mods that change vanilla skills.
- [Better Crab Pots](https://www.nexusmods.com/stardewvalley/mods/3159), [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767) or any mod that affects Crab Pot behavior.
- [Better Slingshots](https://www.nexusmods.com/stardewvalley/mods/2067), [Ring Overhaul](https://www.nexusmods.com/stardewvalley/mods/10669), or any mod that affects Slingshot behavior.
- [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley), as they will be overriden by this mod's changes to the Artisan profession (use [Quality Of Life](https://www.nexusmods.com/stardewvalley/mods/11296) instead to get the flower meads feature). 
- [All Professions](https://www.nexusmods.com/stardewvalley/mods/174) (use included prestige or console commands instead).
- [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#) (already a mod feature).

Not compatible with Android version of the game.

## Installation

- You can install this mod on an existing save; all perks will be retroactively applied upon loading a saved game.
- To install simply drop the extracted folder onto your mods folder.
- To update **make sure to delete the old version** first and only then install the new version.
- There are no dependencies other than SMAPI.

## Configs

While the vast majority of professions bonuses are non-configurable, some of the more radical changes have been given configuration options to give the user some degree of control. As such the mod provides the following config options, which can be modified either in-game via Generic Mod Config Menu or by manually editing the configs.json file:

### General Configs
- **Modkey** (keybind) - The Prospector and Scavenger professions use this key to reveal the locations of key objects currently on the screen. If playing on a large screen with wide field of view, this can help locate foragables of mine nodes in large or busy maps. The default key is LeftShift for keyboards and LeftShoulder for controllers.
- **VintageInterface** (string) - Whether to enable vintage interface compatibility for custom menu elements. You generally want to keep this at "automatic".

### Profession Configs
- **ForagesNeededForBestQuality** (uint) - Determines the number of items foraged from the ground, bushes or mushroom boxes, required to reach permanent iridium-quality forage as an Ecologist. Default is 500.
- **MineralsNeededForBestQuality** (uint) - As above. Determines the number of minerals (gems or foraged minerals) mined or collected from geode crushers or crystalariums, required to reach permanent iridium-quality minerals as a Gemologist. Default is 500.
- **ChanceToStartTreasureHunt** (float) - The percent chance of triggering a treasure hunt when entering a new map as Prospector or Scavenger. Note that this only affects that chance the game will try to start a treasure hunt, and the actual chance is slightly lower as the game might fail to choose a valid treasure tile. Increase this value if you don't see enough treasure hunts, or decrease it if you find treasure hunts cumbersome and don't want to lose your streak. Default is 0.2 (20%).
- **AllowScavengerHuntsOnFarm** (bool) - Whether a Scavenger Hunt can trigger while entering a farm map.
- **ScavengerHuntHandicap** (float) - This number multiplies the Scavener Hunt time limit. Increase this number if you find that Scavenger hunts end too quickly.
- **ProspectorHuntHandicap** (float) - This number multiplies the Prospector Hunt time limit. Increase this number if you find that Prospector hunts end too quickly.
- **TreasureDetectionDistance** (float) - Represents the minimum number of adjacent tiles between the player and the treasure tile before the treasure tile will be revealed by a floating arrow. Increase this value is you find treasure hunts too difficult. Default is 3.
- **SpelunkerSpeedCap** (uint) - The maximum speed bonus a Spelunker can reach (values above 10 may cause problems).
- **EnableGetExcited** (bool) - Toggles the Get Excited buff when a Demolitionist is hit by an explosion.
- **SeaweedIsJunk** (bool) - Whether Seaweed and Algae are considered junk for fishing purposes.
- **AnglerMultiplierCeiling** (float) - The maximum fish price multiplier that can be accumulated by Angler.
- **TrashNeededPerTaxLevel** (uint) - Represents the number of trash items the Conservationist must collect in order to gain a 1% tax deduction the following season. Use this value to balance your game if you use or don't use Automate. Default is 100.
- **TrashNeededPerFriendshipPoint** (uint) - Represents the number of trash items the Prestiged Conservationist must collect in order to gain 1 point of friendship towards all villagers. Default is 100.
- **TaxDeductionCeiling** (float) - Represents the maximum allowed tax deduction by the Ferngill Revenue Service. Set this to a sensible value to avoid breaking your game. Default is 0.25 (25% bonus value on every item).

### Ultimate Configs
- **EnableUltimate** (bool) - Required to allow Ultimate activation.
- **UltimateKey** (keybind) - This is the key that activates Ultimate for 2nd-tier combat professions. By default this is the same key as Modkey, but can also be set to a different key.
- **HoldKeyToActivateUltimate** (bool) - If set to true, then Ultimate will be activated after holding the above key for a short amount of time. If set to false, then Ultimate will activate immediately upon pressing the key. Useful if you are running out of keys to bind, or just want to prevent accidental activation of Ultimate. Default value is true. 
- **UltimateActivationDelay** (float) - If HoldKeyToActivateUltimate is set to true, this represents the number of seconds between pressing UltimateKey and activating Ultimate. Set to a higher value if you use Prospector profession and find yourself accidentally wasting your Ultimate in the Mines.
- **UltimateGainFactor** (double) - Determines how quickly the Ultimate meter fills up.
- **UltimateDrainFactor** (double) - Determines how quickly the Ultimate meter drains while active. The base duration is 15 seconds. Lower numbers make Ultimate last longer.

### Prestige Configs
- **EnablePrestige** (bool) - Whether to apply prestige changes.
- **SkillResetCostMultiplier** (float) - Multiplies the base skill reset cost. Set to 0 to prestige for free.
- **ForgetRecipesOnSkillReset** (bool) - Wether resetting a skill also clears all associated recipes.
- **AllowPrestigeMultiplePerDay** (bool) - Whether the player can use the Statue of Prestige more than once per day.
- **BonusSkillExpPerReset** (float) - Cumulative bonus that multiplies a skill's experience gain after each respective skill reset..
- **RequiredExpPerExtendedLevel** (uint) - How much skill experience is required for each level up beyond 10.
- **PrestigeRespecCost** (uint) - Monetary cost of respecing prestige profession choices for a skill.
- **ChangeUltCost** (uint) - Monetary cost of changing the combat ultimate.

### Difficulty Configs:
- **BaseSkillExpMultiplier** (float array) - Multiplies all skill experience gained from the start of the game (in order: Farming, Fishing, Foraging, Mining, Combat).
- **MonsterHealthMultiplier** (float) - Increases the health of all monsters.
- **MonsterDamageMultiplier** (float) - Increases the damage of all monsters.
- **MonsterDefenseMultiplier** (float) - Increases the damage resistance of all monsters.

## Console Commands

The mod provides the following console commands, which you can enter in the SMAPI console for testing, checking or cheating:

- **player_skills** - List the player's current skill levels.
- **player_resetskills** - Reset the specified skills, or all skills if none are specified.
- **player_professions** - List the player's current professions.
- **player_addprofessions** - Add the specified professions to the local player.
- **player_resetprofessions** - Remove all professions for the specified skills, or all professions if none are specified. Does not affect skill levels.
- **player_readyult** - Max-out the Ultimate meter, or set it to the specified value (between 0 and 100).
- **player_changeult** - Change the Ultimate profession to the desired profession.
- **player_whichult** - Check the currently registered Ultimate profession.
- **player_maxanimalfriendship** - Max-out the friendship of all owned animals, which affects their sale value as Breeder.
- **player_maxanimalmood** - Max-out the mood of all owned animals, which affects production frequency as Producer.
- **player_fishingprogress** - Check your fishing progress and bonus fish value as Angler.
- **wol_data** - Check current value of all mod data fields (FEcologistItemsForaged, GemologistMineralsCollected, ProspectorHuntStreak, ScavengerHuntStreak, ConservationistTrashCollectedThisSeason, ConservationistActiveTaxBonusPct).
- **wol_setdata** - Set a new value for one of the mod data fields above.
- **wol_events** - List currently subscribed mod events (for debugging).
- **wol_resetthehunt** - Forcefully reset the currently active Treasure Hunt and choose a new target treasure tile.

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
