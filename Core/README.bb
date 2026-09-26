[font=bebas_neuebook][size=6]What this is[/size][/font]

This is the core mod which provides shared functionality required by other DaLion mods.

It carries a few features of its own because I've no interest in managing a bunch of small miscellaneous mods. All are disabled by default.
[list]
[*][b]Colored Slime Balls:[/b] Causes Slime Balls to take on the color of the Slimes which produced them, and adds regular color-based Slime drops to Slime Ball loot tables. This is required by [url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk of Life[/url], so will auto-enable when that mod is installed.[/*]
[*][b]Two-Way Hoppers:[/b] Adds the ability for Hoppers to pull items back out from machines, allowing them to fully automate a single machine at a time and transforming them from completely useless into a more balanced version of [url=https://www.nexusmods.com/stardewvalley/mods/1063]Automate[/url].[/*]
[*][b]Witherable Crops:[/b] Crops may wither if left un-watered.[/*]
[*][b]Immersive Hay:[/b] Obtain Hay from harvesting pre-mature Wheat (stage 4). Mimics how hay is made IRL.[/*]
[*][b]Winter Wheat:[/b] Plant Wheat near the end of Fall to have it survive dormant through Winter. In Spring, it resumes growing, yielding twice as much harvest. Mimics how real wheat is planted IRL.[/*]
[*][b]Snowball Fights:[/b] Use an empty slingshot while standing over a snowy tile to fire a snowball projectile. Doesn't do damage; it's just for fun.[/*]
[/list]


[font=bebas_neuebook][size=6]Status Effects[/size][/font]

Taking inspiration from classic game tropes, this mod adds a framework for causing various status conditions to enemies. These effects will be used by the various DaLion mods, and can also be used by any C# mod which consumes the provided [url=https://github.com/daleao/sdv/blob/main/Core/ICoreApi.cs]API[/url].
[list]
[*][b]Bleeding:[/b] Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (i.e., Dwarven Sentry).[/*]
[*][b]Blinded:[/b] Causes enemies to lose track of their target and miss attacks. Does not affect Bats or Duggys.[/*]
[*][b]Burning:[/b] Causes damage equal to 1/16th of max health every 3 seconds, and reduces attack by half. Also causes enemies to move about more randomly. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers). Insects burn 4x as quickly.[/*]
[*][b]Chilled:[/b] Reduces speed for the duration. If Chilled is inflicted again during this time, the target may be Frozen for three times the duration. Does not affect Ghosts or Skeleton Mage.[/*]
[*][b]Frozen:[/b] Cannot move or attack. The next hit during the duration deals double damage and ends the effect.[/*]
[*][b]Poisoned:[/b] Causes damage equal to 1/16 of max health every 3s, stacking up to 3×. If enough stacks are applied the target may suffer instant death. Does not affect Ghosts or Skeletons.[/*]
[*][b]Slowed:[/b] Reduces speed (movement and action) for the duration.[/*]
[*][b]Stunned:[/b] Cannot move or attack for the duration.[/*]
[/list]
Durations will depend on the source. These status conditions are exclusively applied to monsters.

[font=bebas_neuebook][size=5]Consistent Farmer Debuffs[/size][/font]

This setting will optionally modify the Burnt and Frozen debuff on the player to be consistent with the player-inflicted debuffs above.
It also modifies Jinxed and Weakness to work better with the overall balance intended by DaLion mod series:
[list]
[*][b]Jinxed:[/b] Reduces player defense by 50% and prevents use of weapon special moves for the duration.[/*]
[*][b][s]Weakness[/s] Disoriented:[/b] Lose control of movement (movement directions are inverted).[/*]
[/list]

[font=bebas_neuebook][size=6]Credits & Special Thanks[/size][/font]

Credit to [url=https://next.nexusmods.com/profile/Roscid/about-me?gameId=1303]Roscid[/url] for [url=https://www.nexusmods.com/stardewvalley/mods/7634]Slime Produce[/url].


[size=6][font=bebas_neuebook]DaLion Mod Series[/font][/size]

[url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk of Life[/url] - An extensive overhaul of profession trees and skill progression systems.
[url=https://www.nexusmods.com/stardewvalley/mods/24356]Aquarism[/url] - Make Fish Ponds actually useful. Created with the Aquarist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24357]Serfdom[/url] - Realistic income and property taxation mechanics. Created with the Conservationist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24832]Springmyst[/url] - Some actually interesting enchantments added to the game.
[url=https://www.nexusmods.com/stardewvalley/mods/29612]Mineracoustics[/url] - Overly complex overhaul of gemstone ring mechanics based on real Music Theory.
[url=https://www.nexusmods.com/stardewvalley/mods/23048]Chargeable Resource Tools[/url] - Charge the Axe and Pickaxe to break debris within an area.
[url=https://www.nexusmods.com/stardewvalley/mods/29830]Wildcat[/url] - Weapon combo framework, slick moves & other combat tweaks to improve the feel of combat.

[b][url=https://github.com/daleao/sdv/tree/main/Core]Source code[/url][/b]