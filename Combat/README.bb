[font=bebas_neuebook][size=6]What this is[/size][/font]

This mod seeks to change the feel of combat from a button mash-fest to a more strategic and dynamic experience allowing for some expression of player skill and buildcraft. It does this mainly by introducing a multi-hit combo system inspired by Haunted Chocolatier trailers, accompanied by tweaks to how statistics other than flat damage affect your overall performance in combat.

[b]This is not a casual mod.[/b] It will make combat an overall more difficult and more frustrating, but hopefully more rewarding, experience.


[font=bebas_neuebook][size=6]Combo Framework[/size][/font]

Weapon spamming is a real problem in vanilla;
[list]
[*]It means that more offense is always the best defense, which renders both the defense stat and defensive special move of the sword completely useless.[/*]
[*]It lessens the impact of attack speed bonuses, which in vanilla only affects a single 1 out of the 4 ~ 5 frames in the attack swipe animation for swords and clubs.[/*]
[*]It turns knockback into more of a hindrance, since you can't spam click an enemy that gets pushed outside your attack range.[/*]
[/list]
All of this contributes to the lack of variety in combat encounters and player builds, with flat damage becoming the only real viable option.

We propose a solution by implementing a [b]combo framework[/b] for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced "cooldown". Each weapon type has a configurable combo limit:
[list]
[*][b]Swords:[/b] up to 4 horizontal swipes, by default.[/*]
[*][b]Clubs:[/b] up to 2 hits, being one horizontal swipe and one downward smash, by default.[/*]
[*][b]Daggers:[/b] unchanged, effectively up to infinite hits.[/*]
[/list]

You now actually have a reason to weave special moves in between regular attacks. Attack speed bonuses from [b]Emerald [/b]rings and weapon forges will now serve to reduce both the cooldown between combos as well as the duration of every frame in the attack animation.

You can choose whether to require clicks for each swipe, or simply hold down the tool button to execute a full combo. If you use the latter, be careful not to waste your precious attacks.


[font=bebas_neuebook][size=6]Control Tweaks[/size][/font]

[font=bebas_neuebook][size=5]Slick Moves[/size][/font]

To improve your mobility during combos, your attacks while running will preserve momentum, allowing you to drift in the direction of movement while attacking. This gives the impression of a faster-paced and more fluid combat.

[font=bebas_neuebook][size=5]Face Mouse Cursor[/size][/font]

When playing with mouse and keyboard, enabling this feature will cause the farmer to swing their weapon in the direction of the mouse cursor. Combined with slick moves, this allows you to drift in a direction while attacking behind you. Mastering this mechanic will be essential to avoid hits during combat.

You can choose to enable this for all tools, only weapons, or nothing at all.


[font=bebas_neuebook][size=6]Statistic Tweaks[/size][/font]

[font=bebas_neuebook][size=5]Defense[/size][/font]

Even with the removal of weapon spamming, defense still sucks. The vanilla game uses a simple linear formula for mitigating damage, which simply subtracts the defender's defense from the raw damage value:

[code]damage -= defense[/code]

The problem is that damage continually scales as you encounter tougher enemies by mining deeper Mine levels, visiting different dungeons, or the hard mode of the Mines. Your defense, however, does not. This means that 1 point of defense can make a big difference at the very start of the game, but quickly falls off and becomes completely useless. To make thing worse, the game also soft caps your defense if it reaches or surpasses 50% of the raw damage value (as if you could even reach that high) by subtracting a random amount (between 1/10 and 1/3) of your defense before applying the mitigation formula.

This mod replaces the vanilla mitigation formula with a very simple hyperbolic formula:

[code]damage *= 10 / (10 + defense)[/code]

This causes every additional point of defense to mitigate exactly 10% more damage, guaranteeing that defense is always equally useful throughout every point in the game, albeit with diminishing returns.

Moreover, the defense stat will also increase reflected damage, which includes the sword's parry special move as well as the Ring of Thorns. Defensive builds focusing on [b]Topaz [/b]rings and weapon forges should now actually be viable, even offensively.

[b]A note about Ridgeside Village and Sword and Sorcery:[/b]
I do not recommend using this option with any mods that introduce high-defense weapons or armor. There shouldn't be any issues, but those mods are designed with the vanilla linear formula in mind (the fact that these mods feel the need to grant the player hundreds of defense points proves just how bad the vanilla formula is). This feature is designed with the vanilla defense maximum of around 20. If you use both together, it becomes very easy to completely nullify all damage.


[font=bebas_neuebook][size=5]Knockback[/size][/font]

In many games, when a character hits a wall or an object while being knocked back they will suffer environment damage. This mod introduces a similar mechanic. This creates an additional strategic layer to combat, where fighting enemies close to stones, walls or other objects can be more favorable than combat in open areas. The amount of environment damage suffered depends on the velocity of the character at the moment of collision, which of course is increased by the knockback stat from [b]Amethyst[/b] rings and weapon forges.

Now, knocking enemies away from your combo is no longer necessarily a bad thing.


[font=bebas_neuebook][size=5]Critical Strikes[/size][/font]

So far, we've introduced tweaks to attack speed, defense and knockback. To round off the roster, we introduce two tweaks to critical strikes:
[list=1]
[*][b]Critical strikes ignore defense: [/b]This adds a layer of counter-play against tough defensive enemies, creating a rock-paper-scissors dynamic between various stats.[/*]
[*][b]Critical back strikes: [/b]Successful strikes to enemies from behind benefit from double critical strike chance. While this is ordinarily impossible against most enemies in vanilla, the DaLion suite of mods introduces various forms of [b]crowd control[/b] that you may be able to abuse for this effect.[/*]
[/list]

[font=bebas_neuebook][size=5]Luck[/size][/font]

Because I'm a fan of classic Ragnarok Online and a lot of my mods take inspiration from it, I thought it'd be cool if the Luck stat also had a similar effect in combat. So I've also added an option for Luck to improve critical hit chance* as well as introduce a dodge chance, at 1% per Luck level for both.

* The vanilla game already has a tiny amount of luck effect on crit chance, but it's a self-dependent (multiplicative) effect:
[code]critChance *= (1 + luck / 40)[/code]

This mod makes it a flat increase:
[code]critChance += luck / 10[/code]

This has an actually noticeable impact while still being subtle — a few points of luck can take you from no crits to a few, whereas with the vanilla formula, luck won't help unless you already have a decent crit chance.


[font=bebas_neuebook][size=6]Enemy Tweaks[/size][/font]

Lastly, this mod provides optional sliders for tweaking enemy statistics including Health, Damage and Defense. There is also a toggle for randomizing encounters based on Daily Luck to create more varied dungeon experiences. If you feel combat is too easy after stacking 2 or more [url=https://www.nexusmods.com/stardewvalley/mods/29612]Infinity Bands[/url], use this to adjust the difficulty.


[font=bebas_neuebook][size=6]Credits & Special Thanks[/size][/font]

Credit to [url=https://next.nexusmods.com/profile/DjStln?gameId=1303]DjStln[/url] and [url=https://next.nexusmods.com/profile/NormanPCN?gameId=1303]NormanPCN[/url] for [url=https://www.nexusmods.com/stardewvalley/mods/2590]Combat Controls[/url] and [url=https://www.nexusmods.com/stardewvalley/mods/10496]Redux[/url], respectively, from which the [i]Slick Moves[/i] and [i]Face Mouse Cursor[/i] features are inspired.

Special thanks to [url=https://next.nexusmods.com/profile/EnaiSiaion?gameId=1704]Enai Siaion[/url] for [url=https://www.nexusmods.com/skyrimspecialedition/mods/1368]Wildcat[/url].


[size=6][font=bebas_neuebook]DaLion Mod Series[/font][/size]

[url=https://www.nexusmods.com/stardewvalley/mods/24355]Walk of Life[/url] - An extensive overhaul of profession trees and skill progression systems.
[url=https://www.nexusmods.com/stardewvalley/mods/24356]Aquarism[/url] - Make Fish Ponds actually useful. Created with the Aquarist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24357]Serfdom[/url] - Realistic income and property taxation mechanics. Created with the Conservationist profession in mind.
[url=https://www.nexusmods.com/stardewvalley/mods/24832]Springmyst[/url] - Some actually interesting enchantments added to the game.
[url=https://www.nexusmods.com/stardewvalley/mods/29612]Mineracoustics[/url] - Overly complex overhaul of gemstone ring mechanics based on real Music Theory.
[url=https://www.nexusmods.com/stardewvalley/mods/23048]Chargeable Resource Tools[/url] - Charge the Axe and Pickaxe to break debris within an area.
[b]Wildcat[/b] - Weapon combo framework, slick moves & other combat tweaks to improve the feel of combat.

[b][url=https://github.com/daleao/sdv/tree/main/Combat]Source code[/url][/b]