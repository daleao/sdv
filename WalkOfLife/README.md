<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->
![](https://stardewcommunitywiki.com/mediawiki/images/8/82/Farming_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/2/2f/Mining_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/f/f1/Foraging_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/e/e7/Fishing_Skill_Icon.png)
![](https://stardewcommunitywiki.com/mediawiki/images/c/cf/Combat_Skill_Icon.png)

# Walk Of Life

**A Professions Overhaul**

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
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#recommended-mods">Recommended Mods</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>


Ever wondered why there aren't any profession overhaul mods on the Nexus? Me too.


## Features

- Rebalanced and reworked [almost] every profession to be an equally unique and attractive choice.
- Each profession targets a specific style of gameplay, some which were not viable in vanilla (i.e. ranching).
- No more ~~boring~~ uninspiring flat value bonuses.
- Bomberman mining; Assassin, Sniper and Slime-army combat.
- Scaling end-game objectives.
- Level 5 professions provide simple early-game buffs that benefit most styles of general gameplay.
- Level 10 professions are more specialized and engaging, providing two bonuses which change the way you play.
- Professions are more consistent across the board, with several analogous perks and synergies.
- Lightweight. This mod is built with a dynamic event handling system to avoid overhead.
- New icons for most professions, courtesy of [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) (please make sure to [endorse their original](https://www.nexusmods.com/stardewvalley/mods/4163) mod).


## Professions

### ![](https://i.imgur.com/p9QdB6L.png) Farming

- **Lv5 - Harvester** - 10% chance for extra yield from harvested crops.
    - This yields an equivalent 10% monetary bonus to vanilla, but also brings additional benefit to Artisans.
- **Lv10 - Agriculturist** - Crops grow 10% faster. Grow best-quality crops organically without fertilizer.
    - Allows harvesting iridium-qual)ity crops without any fertilizer. The chance is unchanged from vanilla, and is equal to half the chance of gold quality. Fertilizers will still massively increase that chance.
- **Lv10 - Artisan** - All artisan machines work 10% faster. Machine output quality matches input ingredient quality.
    - Essentially implements [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) (QAP), but exclusively for Artisans. Since this would conflict with QAP, this mod also implements the remaining features of that mod (ex. mead cares about honey type), essentially replacing it entirely. As such, do **not** use QAP together with this mod, as it'll override this mod's changes and make the Artisan profession redundant. This mod's implementation also does **not** require the Producer Framework Mod. In multiplayer, the **bonus production speed applies only to machines crafted by the player with this profession, and only when that player uses the machine**.
- **Lv5 - Rancher** - Befriend animals quicker.
    - Gain double mood and friendship points from petting. Newborn animals are born with some starting friendship between 0 and 200 (out of 1000 maximum), chosen at random.
- **Lv10 - Breeder** - Animals incubate faster and breed more frequently. Increase value of animals at high friendship.
    - Makes mammals three times as likely to give birth and oviparous (egg-laying) animals incubate twice as fast. At max friendship animals are worth 2.5x their base price, instead of vanilla 1.3x.
- **Lv10 - Producer** - Happy animals produce twice as frequently. Produce worth 5% more for every full barn or coop.
    - Note that happiness (mood) is **not** the same as friendship. Also note this will **not** allow certain animals (i.e. cows and chickens) to produce more than once per day. Bonus produce value also applies to artisan goods derived from animal products (i.e. cheeses, mayos and cloth). **Note that honey is also considered an animal product.** There is no limit to the scaling. In multiplayer, **the bonus applies only to barns and coops owned by the player with this profession, and only when that player sells the produce**.

### ![](https://i.imgur.com/jf88nPt.png) Foraging

- **Lv5 - Forager** - 20% chance for double yield of foraged items.
    - _Unchanged effect from vanilla. Only the name is changed._
- **Lv10 - Ecologist** - Wild berries restore 50% more health and energy. Progressively identify forage of higher quality.
    - All foraged items will have the same deterministic quality. This immediate gives inventory convenience. However the quality will start off at silver, and progress to iridium when enough items have been foraged.
- **Lv10 - Scavenger** - Location of forageable items revealed. Occasionally track buried treasure.
    - On switching maps while outside you will occasionally detect hidden buried treasure. Find it and dig it up (with a hoe) within the time limit to obtain your reward. The larger your win streak the better your odds of obtaining rare items. Bonus: hold LeftShift (or LeftShoulder on gamepad) to reveal all forageables on-screen.
- **Lv5 - Lumberjack** - Felled trees yield 25% more wood.
    - _Unchanged effect from vanilla. Only the name is changed._
- **Lv10 - Arborist** - All trees grow faster and can drop hardwood.
    - Bonus tree growth works as a global buff; i.e. in multiplayer, all trees will be affected as long as any player in the session has this profession, and the effect will stack for all additional online players that share this profession. The hardwood bonus is unchanged from vanilla.
- **Lv10 - Tapper** - Tappers are cheaper to craft. Tapped trees give syrup 25% faster.
    - New recipe: x25 wood and x1 copper bar.

### ![](https://i.imgur.com/TidtIw0.png) Mining 

- **Lv5 - Miner** - +1 ore per ore vein.
    - _Unchanged from vanilla._
- **Lv10 - Spelunker** - Chance to find ladder and shafts increases by 1% per mine level. +1 speed while mining.
    - Bonus ladder chance resets every time you leave the mines. **This includes taking the stairs back to the mine entrance.**
- **Lv10 - Prospector** - Location of ladders and mining nodes revealed. Occasionally track special rocks.
    - Analogous to Scavenger. Track any mining nodes or mineral forages off-screen with a yellow pointer, and any ladders or shafts with a green pointer. On entering a new mine floor you will occasionally detect stones with prospective treasure. Find the stone and break it within the time limit to obtain a reward. The larger your win streak the better your odds of obtaining rare items. Bonus: hold LeftShift to highlight nodes and ladders on-screen.
- **Lv5 - Blaster** - Bombs are cheaper to craft. Exploding rocks yields twice as much coal.
    - New recipe: x2 ore (copper, iron, gold) and x1 coal.
- **Lv10 - Demolitionist** - Bomb radius +1. Exploding rocks yields 20% more resources.
    - Bonus: [Get excited](https://www.youtube.com/watch?v=0nlJuwO0GDs) when hit by an explosion.
- **Lv10 - Gemologist** - Progressively identify gems and minerals of higher quality. Crystalariums work 25% faster.
    - Analogous to Ecologist. All gems and minerals mined from nodes have a fixed quality, starting at silver and increasing once enough minerals have been collected. Minerals collected from Crystalariums and Geode Crushers are counted for this total, **but not those from geodes broken at Clint's**. In multiplayer, **bonus Crystalarium speed applies only to machines crafted by the player with this profession, and only when used by that player**.

### ![](https://i.imgur.com/XvdVsAn.png) Fishing

- **Lv5 - Fisher** - Live bait reduce the chance to fish junk.
    - Here, "junk" includes algae and seaweed.
- **Lv10 - Angler** - Fish are worth 1% more for every unique max-sized fish caught and 5% more for every legendary fish.
    - "Legendary fish" includes the Extended Family Qi challenge varieties, counted only once.
- **Lv10 - Aquarist** - Fish pond max capacity +2. Fishing bar height increases for every fish pond at max capacity.
    - Gain 6 pixels per Fish Pond. Every four ponds equal a permanent cork bobber. In multiplayer, **only counts Fish Ponds owned by the player with this profession**.
- **Lv5 - Trapper** - Crab pots are cheaper to craft. Can trap higher-quality haul.
    - All trapped fish can have quality up to gold. Chance depends on your fishing level. Recipe is unchanged from vanilla.
- **Lv10 - Luremaster** - Crab pots no longer produce junk items. Use different baits to attract different catch.
    - Each type bait will attract different catch:
        - **Regular bait:** 10% chance to catch fish up to level 70. Trappable fish are subject to the same location and season limitations as fishing.
        - **Wild bait:** 10% chance to catch fish up to level 90 (i.e. anything but legendaries and octopus). 50% chance to double the haul.
        - **Magnet:** repels all fish (as per its description), but attracts metal items such as resources, artifacts, treasure chests, rings and even weapons.
        - **Magic bait:** 25% chance to catch fish above level 70, excluding legendaries. Removes all location and seasonal restrictions for catching fish. Makes all catch iridium-quality. _This is the only way to obtain iridium-quality fish from Tappers._
- **Lv10 - Conservationist** - Crab pots without bait will trap junk. Remove junk from waters to merit village favor and tax deductions.
    - Every 10 (configurable) junk items collected from Crab Pots increases friendship with all villagers by 1. Every 100 (configurable) junk items collected will earn you a 1% tax deduction the following season (max 25%, also configurable), increasing the value of all shipped goods. You will receive a formal mail from the Ferngill Revenue Service each season informing your currrent tax bracket.

### ![](https://i.imgur.com/fUnZSTj.png) Combat

The combat tree has received a much more extensive overhaul. Each level 10 profession introduces, in addition to a fixed primary effect, a secondary stackable attribute and a [Super Mode](https://tvtropes.org/pmwiki/pmwiki.php/Main/SuperMode) that may be activated by a hot key _only once the maximum number of stacks has been collected_. Activating Super Mode will consume **all** stacks, reseting the secondary attribute. A new bar added to the UI next to the health bar displays the current stack count. **If a player has multiple combat professions, only the first one will add a secondary attribute and Super Mode, and all the rest will be disabled.**

- **Lv5 - Fighter** - Deal 10% more damage. +15 HP.
    - _Unchanged from vanilla._
- **Lv10 - Brute** - Deal 15% more damage. +25 HP. Wielding a hammer in combat builds Fury, further increasing damage up to 40%. Unleash Fury to provoke Undying Rage.
    - **Undying Rage:** Doubles the damage bonus and gain immunity to passing out. If using a hammer, also negates the special move cooldown.
- **Lv10 - Duelist** - Higher Crit. Power at high HP. Higher Crit. Chance at low HP. Crit. strikes build Poise, granting up to 20% parry chance. Spend poise to enter Riposte Stance.
    - The maximum Crit. Power bonus is x2 at max HP, and the minimum is x1 at 10% HP. The maximum Crit. Chance bonus is +50% at 10% HP, and +0 at max HP. Poise essentially negates all damage.
    - **Riposte Stance:** Doubles the parry chance and gain the highest bonuses to Crit. Power and Crit. Chance simultaneously, regardless of the current HP. If using a dagger, counter parried hits with special move.
- **Lv 5 - Rascal** - Slingshots deal up to 50% more damage from afar. 60% chance to recover spent ammo.
- **Lv10 - Desperado** - Timed shots build Cold Blood, adding up to 50% double strafe chance. Use Cold Blood to trigger Death Blossom.
    - "Timed" means releasing the fire button as soon as it's charged. "Double strafe" means two shots for the price of one.
    - **Death Blossom:** Enables autofire. Shots fire in all directions at once. 
- **Lv10 - Slimed Piper** - Attract more Slimes in dungeons. Slimes damage other monsters. Build Eubstance on contact with slimes, increasing resistance up to 50%. Consume Eubstance to cause Pandemonium.
    - Every Slime raised on the farm, either in a hutch or outside, will increase the chance to spawn an extra Slime on every dungeon floor. Slimes will damage any grounded enemies that intersect their hitbox. Flying enemies are immune. Eubstance stacks are gained even while wearing a Slime Charmer ring.
    - **Pandemonium:**

## Compatbility

- Compatible with [Multi Yield Crops](https://www.nexusmods.com/stardewvalley/mods/6069).
- Compatible with [Capstone Professions](https://www.nexusmods.com/stardewvalley/mods/7636).
- Limited compatibility with [All Professions](https://www.nexusmods.com/stardewvalley/mods/174). (Profession perks will not be applied immediately, but the following morning).
- Limited compatibility with [Skill Prestige](https://www.nexusmods.com/stardewvalley/mods/569#). (Same as above. The Prestige menu also won't reflect modded profession names or descriptions).
- Compatible with any mod that adds SpaceCore custom skills (i.e. [Love Of Cooking](https://www.nexusmods.com/stardewvalley/mods/6830)).
        
- Not compatible with mods that change vanilla skills.
- Not compatible with [Quality Artisan Products](https://www.moddrop.com/stardew-valley/mods/707502-quality-artisan-products) and [Quality Artisan Products for Artisan Valley](https://www.moddrop.com/stardew-valley/mods/726947-quality-artisan-products-for-artisan-valley). (Makes Artisan profession redundant).
- Not compatible with [Crab Pot Loot Has Quality And Bait Effects](https://www.nexusmods.com/stardewvalley/mods/7767). (Makes Trapper and Luremaster professions redundant).
- Not compatible with [Forage Fantasy](https://www.nexusmods.com/stardewvalley/mods/7554). (May cause bad interactions with Foraging professions).

## Recommended Mods

- Recommended use with [Advanced Casks](https://www.nexusmods.com/stardewvalley/mods/8413). (If you miss Oenologist profession perk).
- Recommended use with [Artisan Valley](https://www.nexusmods.com/stardewvalley/mods/1926).
- Recommended use with [Slime Produce](https://www.nexusmods.com/stardewvalley/mods/7634).

## Installation

- You can install this mod on an existing save; all perks will be tetroactively applied upon loading a saved game.
- To install simply drop the extracted folder onto your mods folder.
- To update make sure to delete the old version first and only then install the new version.
- There are no dependencies outside of SMAPI.

## Special Thanks

- [Bpendragon](https://www.nexusmods.com/stardewvalley/users/20668164) for [Forage Pointers](https://www.nexusmods.com/stardewvalley/mods/7781).
- [IllogicalMoodSwing](https://forums.nexusmods.com/index.php?/user/38784845-illogicalmoodswing/) for [Profession Icons Redone](https://www.nexusmods.com/stardewvalley/mods/4163).
- [Pathoschild](https://www.nexusmods.com/stardewvalley/users/1552317) for SMAPI support.
- **ConcernedApe** for Stardew Valley.

## License

Distributed under the MIT License. See [LICENSE](../LICENSE) for more information.
