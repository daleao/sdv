<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->
![](https://gitlab.com/theLion/smapi-mods/-/raw/main/AwesomeProfessions/title_2.png)
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
    <li><a href="#extended-progression">Extended Progression</a></li>
    <li><a href="#compatibility">Compatibility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#configs">Configs</a></li>
    <li><a href="#console-commands">Console Commands</a></li>
    <li><a href="#recommended-mods">Recommended Mods</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>


Ever wondered why there aren't any profession overhaul mods on the Nexus? Me too.


## Features

- Rebalanced and reworked [almost] every profession to be an equally unique and attractive choice.
- Each profession targets a specific style of gameplay, some which were not viable in vanilla (e.g. ranching).
- No more ~~boring~~ uninspiring flat value bonuses.
- Bomberman mining; Thief/Assassin combat; epic slingshots; command an army of giant Slimes.
- Scaling end-game objectives.
- Level 5 professions provide simple early-game buffs that benefit most styles of general gameplay.
- Level 10 professions are more specialized and engaging, providing two bonuses which change the way you play.
- Professions are more consistent across the board, with several analogous perks and synergies.
- Combat professions have unique [Ultimates](https://tvtropes.org/pmwiki/pmwiki.php/Main/SuperMode)
- Lore-friendly multi-profession.
- Level 20 skills and prestiged professions.
- Lightweight. This mod is built with a dynamic event handling system to avoid overhead.
- Gender-specific profession title.
- New icons for most professions, courtesy of [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) (please make sure to [endorse their original](https://www.nexusmods.com/stardewvalley/mods/4163) mod).
- New sound effects for certain professions.

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
- **Lv10 - Breeder** - Animals incubate 2× *(3×)* faster and breed 3× *(5×)* more frequently. Increase value of animals at high friendship.
    - Makes mammals three times as likely to give birth and oviparous (egg-laying) animals incubate twice as fast. At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x.
- **Lv10 - Producer** - Happy animals produce 2× *(3×)* as frequently. Produce worth 5% more for every full barn or coop.
    - Note that happiness (mood) is **not** the same as friendship. Also note this will **not** allow certain animals (i.e. cows and chickens) to produce more than once per day. Bonus produce value also applies to artisan goods derived from animal products (i.e. cheeses, mayos and cloth) and to honey (bees are animals). Only deluxe buildings can be considered full. **Note that honey is also considered an animal product.** There is no limit to the scaling. In multiplayer, **the bonus applies only to barns and coops owned by the player with this profession, and only when that player sells the produce**.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- **Lv5 - Forager** - 20% *(40%)* chance for double yield of foraged items.
    - _Unchanged effect from vanilla. Only the name is changed._
- **Lv10 - Ecologist** - Wild berries restore 50% *(100%)* more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same deterministic quality. This immediate gives inventory convenience. However the quality will start off at silver, and progress to iridium when enough items have been foraged. Applies to Mushroom Boxes, but only if the cave owner (i.e. the host player) has the profession.
- **Lv10 - Scavenger** - Location of forageable items revealed. Occasionally detect buried treasure. *Time freezes during Scavenger Hunts.*
    - On switching maps while outside you will occasionally detect hidden buried treasure. Find it and dig it up (with a hoe) within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items.
    - _Bonus: holding [ModKey](#configs) will highlight all forageables on-screen._
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
- **Lv10 - Spelunker** - Chance to find ladders and shafts increases with every mine level. +1 speed every 5 levels. *Also recover some health and stamina with every mine level.*
    - Plus 0.5% ladder chance per level. Bonus ladder chance resets every time you leave the mines. **This includes taking the stairs back to the mine entrance.**
- **Lv10 - Prospector** - Location of ladders and mining nodes revealed. Occasionally detect rocks with valuable minerals. *Time freezes during Scavenger Hunts.*
    - Analogous to Scavenger. Tracks any mining nodes or mineral forages off-screen with a yellow pointer, and any ladders or shafts with a green pointer. On entering a new mine floor you will occasionally detect stones with prospective treasure. Find the stone and break it within the time limit to obtain a reward. The larger your win streak the better your odds of obtaining rare items.
    - _Bonus: holding [ModKey](#configs) will highlight all nodes and ladders on-screen._
- **Lv5 - Blaster** - Bombs are cheaper to craft. Exploded rocks yield 2× *(3×)* as much coal.
    - New recipe: x2 ore (copper, iron, gold) and x1 coal.
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

The combat tree has received a much more extensive overhaul.  In addition to their primary fixed perks, level 10 combat professions each have a unique "stackable" secondary perk. Once the maximum stacks have been collected, these professions may sacrifice all stacks to activate a powerful [Super Mode](https://tvtropes.org/pmwiki/pmwiki.php/Main/SuperMode) for a short duration. A new bar has been added to the HUD to help you keep track of current stacks. Only a single profession's Super Mode may be registered at any time; if a player gains multiple combat professions from skill reseting, only base perks will apply, and not the stackable secondary perk or Super Mode. The player will prompted during level up whether they wish to replace their current Super Mode with the new professions'.

- **Lv5 - Fighter** - Damage +10% *(+20%)*. +15 HP.
    - _Unchanged from vanilla._
- **Lv10 - Brute** - Damage +15%. +25 HP. Build fury in combat, further increasing damage. *Non-profession-based damage bonuses also decrease special move cooldown.*
    - **Fury:**
        - Damage bonus caps at +40% at Combat level 10.
		- Fury builds faster if wielding a club.
    - Unleash all pent-up fury to enter an **Undying Rage:** Doubles all damage bonuses. Immune to passing out.
        - Doubled damage bonuses include all sources, such as professions, rings and enchantments.
- **Lv10 - Bushwhacker** - +10% crit. chance. Crit. strikes can poach items. Your blood cools in battle, increasing lethality of crit. strikes. *Non-profession-based damage bonuses also decrease special move cooldown.*
    - Monsters can only be poached once.
    - **Cold Blood:**
        - Crit. power bonus caps at ×3 at Combat level 10.
		- Cold blood builds faster if wielding a dagger. Steal chance is also higher with a dagger.
    - Your cold blood masks your presence, allowing you to mount an **Ambuscade:** Become invisible and untargetable. Back stabs are deadly.
		- Failing to assassinate an enemy will instantly reveal your position, canceling the effect.
- **Lv 5 - Rascal** - Slingshots deal up to 50% more damage from afar. 60% chance to recover spent ammo. *Trick shots stun enemies for 5s.*
    - In Vanilla, slingshots are unable to hit enemies that are near the Farmer. As a way to improve slingshot combat, this mod also removes the grace period before a shot is allowed to deal damage. All professions are affected.
    -_Bonus: holding [ModKey](#configs) will fire a trick shot, which is weaker but can ricochet once._
- **Lv10 - Desperado** - Fire 50% faster. Chance to fire a double shot *(spread shot)*, based on current HP. Built-up temerity increases projectile cross section and impact power.
    - **Temerity:**
        - Projectiles are fired with gradually increasing velocity. Higher velocity projectiles have several properties:
            - Increased knockback (up to 1.5x).
            - Chance to pierce through enemies (up to 25%).
            - Increased hitbox, by affecting the air around them, damaging nearby enemies even if they miss (up to 50% on either side).
    - Your temerity culminates in one final desperate **Death Blossom:** Enables auto-reload. Fire eight shots around you at once.
- **Lv10 - Slimed Piper** - Slimes damage other enemies and drop more items the more Slimes are raised on the farm. Gathered eubstance attracts more Slimes in dungeons. *Slimes also heal you on contact.*
    - Slimes cannot damage flying enemies.
    - Each Slime raised on the farm, either in a hutch or outside, increases the chance for Slimes to drop additional items.
    - Bonus: immune to the Slimed debuff.
    - **Eubstance:**
        - Every dungeon floor can spawn up to 11 additional Slimes.
    - Spread all gathered Eubstance to nearby Slimes, allowing them to enter a state of **Superfluidity:** Engorge and empower nearby Slimes.
        - Causes Slimes to grow up to twice their size and gain a proportional damage boost.
        - Large enough Slimes can hit flying enemies, and break into smaller Slimes if defeated.
        - Low chance to convert Slimes to a special variant. If "Prismatic Jelly" special order is active, low chance to convert Slimes to prismatic variant.

## Prestige

The [Statue of Uncertainty](https://stardewvalleywiki.com/The_Sewers#Statue%20Of%20Uncertainty) has been replaced by the Statue of Prestige, which is capable of reseting level 10 skills, for a price. A skill reset preserves all related professions, and (optionally) forgets all related recipes. The farmer can use this to eventually acquire all 30 professions simultaneously.
Reseting a skill costs 10,000g the first time, 50,000g the second time, and 100,000g the last time (values are configurable). The ribbon in the skills page of the game menu reflects the number of professions acquired in each skill.

Once the ribbon has reached its fourth stage, signaling that all professions have been obtained, its level cap is raised to 20, allowing the farmer to continue developing tool proficiency or max health. Skill level also affects the odds of higher quality crops and fishes, the amount of berries foraged per bush, and the duration of Super Mode and related perks. Non-vanilla skills at the moment are not supported by prestige.
 
At levels 15 and 20, the farmer can choose a profession to prestige, improving one of its base perks, or, in some cases, granting entirely new ones. These choices can later be changed at the Statue of Prestige, for a cost.

## Compatbility

The following mods are fully integrated:

- [Automate](https://www.nexusmods.com/stardewvalley/mods/1063) (for craftable machines, the machine's owner's professions will apply; for terrain features, i.e. berry bushes, only the session host's professions will apply).
- [ProducerFrameworkMod](https://www.nexusmods.com/stardewvalley/mods/4970) and [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038) (same rules apply as above).
- PPJA Packs: [Artisan Valley﻿](https://www.nexusmods.com/stardewvalley/mods/1926), [Artisanal Soda Makers](https://www.nexusmods.com/stardewvalley/mods/5173)﻿, [Fizzy Drinks](https://www.nexusmods.com/stardewvalley/mods/5342)﻿, [Fresh Meat﻿](https://www.nexusmods.com/stardewvalley/mods/1721), [Shaved Ice & Frozen Treats](https://www.nexusmods.com/stardewvalley/mods/5388) will all work with Artisan profession.﻿.
- [CJB Cheats Menu] (https://www.nexusmods.com/stardewvalley/mods/4) (download the optional translation files to change profession names under skill cheats).
- [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866/)
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)
- [Vintage Interface](https://www.nexusmods.com/stardewvalley/mods/6654)

The following mods are compatible without integration:
- [Multi Yield Crops](https://www.nexusmods.com/stardewvalley/mods/6069)
- [Capstone Professions](https://www.nexusmods.com/stardewvalley/mods/7636) (though I don't recommend it if prestige and extended progression options are enabled).
- Custom SpaceCore skills (e.g. [Luck](https://www.nexusmods.com/stardewvalley/mods/521), [Magic](https://www.nexusmods.com/stardewvalley/mods/2007), [Love Of Cooking](https://www.nexusmods.com/stardewvalley/mods/6830)) (note that these skills cannot be prestiged).


The mods are **not** compatible:

- Any mods that change vanilla skills.
- [Better Crab Pots](https://www.nexusmods.com/stardewvalley/mods/3159), [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767) or any mod that affects Crab Pot behavior.
- [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley) (won't break anything, but makes Artisan profession redundant; all features are already included). 
- [Forage Fantasy](https://www.nexusmods.com/stardewvalley/mods/7554) (mushroom box quality is already included; other features may cause bad interactions with foraging professions).
- [All Professions](https://www.nexusmods.com/stardewvalley/mods/174) (use included prestige or console commands instead).
- [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#) (now also included).

## Installation

- You can install this mod on an existing save; all perks will be retroactively applied upon loading a saved game.
- To install simply drop the extracted folder onto your mods folder.
- To update make sure to delete the old version first and only then install the new version.
- There are no dependencies outside of SMAPI.

## Configs

While the vast majority of professions bonuses are non-configurable, some of the more radical changes have been given configuration options to give the user some degree of control. As such the mod provides the following config options, which can be modified either in-game via Generic Mod Config Menu or by manually editing the configs.json file:

- **Modkey** (keybind) - The Prospector and Scavenger professions use this key to reveal the locations of key objects currently on the screen. If playing on a large screen with wide field of view, this can help locate forageables of mine nodes in large or busy maps. The default key is LeftShift for keyboards and LeftShoulder for controllers.
- **SuperModeKey** (keybind) - This is the key that activates Super Mode for level 10 combat professions. By default this is the same key as Modkey, but can also be set to a different key.
- **HoldKeyToActivateSuperMode** (boolean) - If set to true, then Super Mode will be activated after holding the above key for a short amount of time. If set to false, then Super Mode will activate immediately upon pressing the key. This is settings is useful if SuperModeKey is set to a key already bound to a different on-press action, such as if keeping the default keybind settings for Modkey and SuperModeKey, which will allowing tracking on-screen items without activating Super Mode. Default value is true. 
- **SuperModeActivationDelay** (decimal) - If HoldKeyToActivateSuperMode is set to true, this represents the number of seconds between pressing SuperModeKey and activating Super Mode. Set to a higher value if you use Prospector profession and find yourself accidentally wasting your Super Mode in the Mines.
- **SuperModeDrainFactor** (integer) - Determines how quickly the Super Mode resource bar drains during Super Mode. This number represents the amount of game update ticks between each tick of the Super Mode resource bar. The default value is 3, which means that 1 point is deduced every 3 / 60 = 0.05 seconds, giving a total Super Mode duration of 0.05 * 500 = 25 seconds. 
- **ForagesNeededForBestQuality** (integer) - Determines the number of items foraged from the ground, bushes or mushroom boxes, required to reach permanent iridium-quality forage as an Ecologist. Default is 500.
- **MineralsNeededForBestQuality** (integer) - As above. Determines the number of minerals (gems or foraged minerals) mined or collected from geode crushers or crystalariums, required to reach permanent iridium-quality minerals as a Gemologist. Default it 500.
- **ChanceToStartTreasureHunt** (decimal) - The percent chance of triggering a treasure hunt when entering a new map as Prospector or Scavenger. Note that this only affects that chance the game will try to start a treasure hunt, and the actual chance is slightly lower as the game might fail to choose a valid treasure tile. Increase this value if you don't see enough treasure hunts, or decrease it if you find treasure hunts cumbersome and don't want to lose your streak. Default is 0.2 (20%).
- **AllowScavengerHuntsOnFarm** (boolean) - Whether a Scavenger Hunt can trigger while entering a farm map.
- **ScavengerHuntHandicap** (decimal) - This number multiplies the Scavener Hunt time limit. Increase this number if you find that Scavenger hunts end too quickly.
- **ProspectorHuntHandicap** (decimal) - This number multiplies the Prospector Hunt time limit. Increase this number if you find that Prospector hunts end too quickly.
- **TreasureDetectionDistance** (decimal) - Represents the minimum number of adjacent tiles between the player and the treasure tile before the treasure tile will be revealed by a floating arrow. Increase this value is you find treasure hunts too difficult. Default is 3.
- **SpelunkerSpeedCap** (integer) - The maximum speed bonus a Spelunker can reach (values above 10 may cause problems).
- **EnableGetExcited** (bool) - Toggles the Get Excited buff when a Demolitionist is hit by an explosion.
- **TrashNeededPerTaxLevel** (integer) - Represents the number of trash items the Conservationist must collect in order to gain a 1% tax deduction the following season. Use this value to balance your game if you use or don't use Automate. Default is 100.
- **TrashNeededPerFriendshipPoint** (integer) - Represents the number of trash items the Prestiged Conservationist must collect in order to gain 1 point of friendship towards all villagers. Default is 100.
- **TaxDeductionCeiling** (decimal) - Represents the maximum allowed tax deduction by the Ferngill Revenue Service. Set this to a sensible value to avoid breaking your game. Default is 0.25 (25% bonus value on every item).
- **EnablePrestige** (boolean) - Whether to apply prestige changes.
- **SkillResetCostMultiplier** (decimal) - Multiplies the base skill reset cost. Set to 0 to prestige for free.
- **ForgetRecipesOnSkillReset** (boolean) - Wether reseting a skill also clears all associated recipes.
- **AllowPrestigeMultiplePerDay** (boolean) - Whether the player can use the Statue of Prestige more than once per day.
- **BaseSkillExpMultiplier** (decimal) - Multiplies all skill experience gained from the start of the game.
- **BonusSkillExpPerReset** (decimal) - Multiplies all skill experience gained after each respective skill reset.
- **RequiredExpPerExtendedLevel** (integer) - How much skill experience is required for each level up beyond 10.
- **PrestigeRespecCost** (integer) - Monetary cost of respecing prestige profession choices for a skill.
- **ChangeUltCost** (integer) - Monetary cost of changing the combat ultimate.
- **UseVintageSkillBars** (boolean) - Set to true if you use the Vintage Interface﻿ mod. Makes the skill bars above level 10 a light pink. You can use this with the brown version of Vintage Interface. If you want to use the pink version you will have to recolor the file `assets/menus/skillbars_vintage.png` by yourself.

## Console Commands

The mod provides the following console commands, which you can enter in the SMAPI console for testing, checking or cheating:

- **player_skills** - List the player's current skill levels.
- **player_resetskills** - Reset all player's skills.
- **player_professions** - List the player's current professions.
- **player_addprofessions** - Add the specified professions to the local player.
- **player_resetprofessions** - Reset all skills and professions for the local player.
- **player_setultvalue** - Set the Super Mode value to the desired number.
- **player_readyult** - Max-out the Super Mode meter.
- **player_setultindex** - Change the Super Mode profession to the desired index.
- **player_whichult** - Check the currently registered Super Mode profession.
- **player_maxanimalfriendship** - Max-out the friendship of all owned animals, which affects their sale value as Breeder.
- **player_maxanimalmood** - Max-out the mood of all owned animals, which affects production frequency as Producer.
- **player_fishingprogress** - Check your fishing progress and bonus fish value as Angler.
- **wol_data** - Check current value of all mod data fields (ItemsForaged, MineralsCollected, ProspectorStreak, ScavengerStreak, WaterTrashCollectedThisSeason, ActiveTaxBonusPercent).
- **wol_setdata** - Set a new value for one of the mod data fields above.
- **wol_events** - List currently subscribed mod events (for debugging).

## Recommended Mods

- [Advanced Casks](https://www.nexusmods.com/stardewvalley/mods/8413) (if you miss Oenologist profession perk).
- [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926) (add variety to Artisan products and Producer).
- [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634) (make Slime ranching more interesting and profitable).
- [Ostrich Mayo and Golden Mayo](https://www.nexusmods.com/stardewvalley/mods/7660) (better consistency between Ostrich and Golden eggs for Artisan profession).

## Special Thanks

- [Bpendragon](https://www.nexusmods.com/stardewvalley/users/20668164) for [Forage Pointers](https://www.nexusmods.com/stardewvalley/mods/7781).
- [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) for [Profession Icons Redone](https://www.nexusmods.com/stardewvalley/mods/4163).
- [HimeTarts](https://www.nexusmods.com/stardewvalley/users/108124018) for the title logo.
- [PiknikKey](https://forums.nexusmods.com/index.php?/user/97782533-piknikkey/) for Chinese translation.
- [lacerta143](https://www.nexusmods.com/stardewvalley/users/38094530) for Korean translation.
- [TehPers](https://www.nexusmods.com/stardewvalley/users/3994776) for TFO integration.
- [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317) for SMAPI support.
- **ConcernedApe** for Stardew Valley.
- [JetBrains](https://jb.gg/OpenSource) for providing a free license to their tools.

<table>
  <tr>
    <td><img width="64" src="https://smapi.io/Content/images/pufferchick.png" alt="Pufferchick"></td>
    <td><img width="80" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains logo."></td>
  </tr>
</table>



## License

Distributed under the MIT License. See [LICENSE](../LICENSE) for more information.
