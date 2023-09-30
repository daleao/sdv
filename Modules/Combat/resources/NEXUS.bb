[b][center][size=6][font=bebas_neuebook]MARGO :: Combat (CMBT)[/font][/size][/center][/b]
[size=6][font=bebas_neuebook]Overview[/font][/size]

This humongous module consolidates rebalances to melee weapons, ranged weapons and combat rings, together with entirely new mechanics which will overall make combat significantly more strategic and challenging.


[size=6][font=bebas_neuebook]Status Conditions[/font][/size]

Taking inspiration from classic RPG or strategy games, this module adds a framework for causing various status conditions to enemies. They are:
[list]
[*][b]Bleeding:[/b] Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
[*][b]Burning:[/b] Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers).
[*][b]Chilled:[/b] Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
[*][b]Frozen:[/b] Cannot move or attack for 30s. The next hit during the duration deals triple damage and ends the effect.
[*][b]Poisoned:[/b] Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
[*][b]Slowed:[/b] Reduces movement speed by half for the duration.
[*][b]Stunned:[/b] Cannot move or attack for the duration.
[/list]
Each status condition is accompanied by a neat corresponding animation. Status conditions cannot be applied on the player.


[size=6][font=bebas_neuebook]Rebalanced Stats[/font][/size]

Certain stats are simply not worth investing in vanilla. While some of these can be attributed to unbalanced equipment bonuses (and will be addressed below), others, namely [b]Knockback[/b] and [b]Defense[/b] are fundamentally broken.
[list=1]
1. [b]Knockback[/b] is too high by default; hitting enemies immediately throws them several tiles away. Further down we will address the default knockback of most weapons. But that alone doesn't make this as attractive a stat as flat damage. Hence, enemies will now take damage proportional to their momentum when thrown against a wall or object. The added offensive angle makes this a great stat for clubs in particular. It also makes positioning an important strategic element in combat.

2. [b]Defense[/b] is rather useless in vanilla, where each point simply mitigates a single unit of damage. Consequently, small defense bonuses become useless in late-game. This mod implements a new, simple damage mitigation formula, which allows defense to scale into late-game without becoming overpowered.
[size=1][spoiler]
   [b] Old formula:[/b] damage = Min(rawDamage - defense, 1)
   [b] New formula:[/b]damage = rawDamage * 10 / (10 + defense)
[/spoiler][/size]
 One defense point now reduces damage by 10% regardless of incoming damage. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve.
This also applies to monsters!
The [b]Topaz Ring[/b] is also changed to increased defense by 1 point, like its corresponding weapon forge.

3. [b]Critical strikes[/b] are fun, but need a little more oomph to compete with flat damage. To counter-play the previous change to defense, this mod makes critical strikes ignore enemy defense. Attacks from behind will also have double the chance to critically strike. But reaching the backs of your enemies will not be easy! You will have to rely on [b]status conditions[/b] (see above) to achieve that. The effects of [b]Jade[/b] bonuses from rings and weapon forges are also significantly buffed.
[list]
[*] [b]Jade:[/b] [i]+10% -> +50% crit. power.[/i][spoiler]A 10% boost to crit. power is a 10% damage boost that *only* applies to crits. To put that in perspective, only when the player has 100% crit. chance then they will receive an overall 10% boost to damage. It should be clear that this is complete garbage next to a Ruby Ring, which straight up grants a 10% boost to damage, *all the time*. At 50% crit. power, the Jade Ring becomes a better choice than the Ruby Ring if the player has at least 20% crit. chance, which should be attainable by any weapon type given an appropriate build. Above that threshold, Jade Rings become even stronger.[/spoiler]
[/list][/list]   Lastly, ranged weapons can now also score critical hits. Think of them as headshots.

[size=6][font=bebas_neuebook]Melee Weapon Changes[/font][/size]

Vanilla weapons suffer from two major issues:
[list=1]
    1. Same-y-ness. Apart from their special moves, none of the weapon types feel particularly unique; club and dagger are simply "worse swords".
    2. Overabundance. Weapons quickly become inventory clutter. The game is also quick to gift its most powerful weapons, trivializing any weapons obtained as drops.[/list]
This mod tries to address both issues through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging.


[size=5][font=bebas_neuebook]Combo Framework[/font][/size]

Weapon spammingmakes vanilla combat a boring click-fest. This mod implements a [b]combo framework[/b]for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced cooldown. Each weapon type has a configurable combo limit:

[list]
[*] [b]Swords:[/b] up to 4 horizontal swipes, by default.
[*] [b]Clubs:[/b] up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
[*] [b]Daggers:[/b] unchanged, effectively up to infinite hits.
[/list]
If combo hits are enabled, swing speed bonuses from [b]emerald [/b]effects will apply to every frame of the attack animation, as well as the cooldown in between combos, as opposed to only the final frame as in vanilla. This immediately makes speed a more valuable stat.

[center][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/combo_sword_small.gif[/img][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/combo_club_small.gif[/img][/center]

[size=5][font=bebas_neuebook]Offensive & Defensive Swords[/font][/size]

In vanilla game code we can find traces of an abandoned fourth weapon type: [b]Stabbing Swords[/b]. This module re-implements stabbing swords into the game.

Swords are now split between [b]offensive[/b] and [b]defensive[/b] archetypes. Defensive swords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames. You can also change the direction mid-dash by inputting a directional command in a perpendicular direction.

[center][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/stabbing_special.gif[/img][/center]
To improve the defensive parry move, parry damage is increased by 10% for every defense point. This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during combo downtime).


[size=5][font=bebas_neuebook]Rebalanced Types & Tiers[/font][/size]

Weapon stats have been rebalanced across the board so as to provide a more clear distinction between the weapon types:
[list]
 [*] [b]Clubs[/b] are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
 [*] [b]Daggers[/b] are the opposite of clubs, being the quick, precise, but short-range and generally-lower-damage potential weapons. [b]Can cause Bleeding[/b].
 [*] [b]Offensive Swords[/b] are somewhere in the middle, with average damage, speed and precision.
 [*] [b]Defensive Swords[/b] are marginally weaker and slower than their offensive counterpart, but are otherwise heavier, sturdier and wider. They are somewhere in-between Offensive Swords and Clubs.
[/list]
Weapons are categorized by different tiers, [url=https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience]color-coded for your convenience[/url]:

[center][b]Common [/b]<[b][color=#00ff00]Uncommon [/color][/b][color=#f4f4f4]<[/color][b][color=#3d85c6]Rare [/color][/b]< [color=#9900ff][b]Epic [/b][/color][color=#f4f4f4]<[/color][b][color=#ff0000]Mythic [/color][/b]< [b][color=#ff7700]Masterwork [/color][/b]< [b][color=#ffff00]Legendary[/color][/b][/center][b][color=#ffff00]
[/color][/b]
[center][img]https://staticdelivery.nexusmods.com/mods/1303/images/14470/14470-1670827097-402941320.gif[/img][/center]
Weapons below the [color=#ff0000][b]Mythic[/b][/color] tier have randomized damage, determined the moment they are dropped and scaled to your Mine progression. This way, players can always loot stronger weapons, and no specific weapon is ever trivialized. Higher-tier weapons will generally be stronger than lower-tiered ones, although that may not always be the case. These weapons can still be purchased from Marlon, but they will have fixed and significantly weaker stats.

[color=#ff0000][b]Mythic[/b][/color] weapons themselves are unique or extremely rare. They are usually quest rewards or rare monster drops, and tend to carry special enchantments in addition to their regular stats.
[spoiler]
[list]
[*][b][color=#6fa8dc]Neptune's Glaive:[/color][/b]Each swing feels like a crashing wave.Deals heavy knockback.
[*][b][color=#00ffff]Yeti Tooth:[/color][/b]Chance to cause Chill.
[*][b][color=#999999]Obsidian Edge:[/color][/b]Cuts through enemy defense. Chance to cause Bleed.
[*][b][color=#ff0000]Lava Katana:[/color][/b]Chance to cause Burn.
[/list][/spoiler]

[b][color=#ff7700]Masterwork[/color][/b] weapons have fixed stats. These are weapons created by the Dwarven race using special materials. They can ony be created by uncovering the lost Dwarvish Blueprints, and taking them to a skilled blacksmith along with the corresponding material:
[list]
[*][b]Elven[/b]weapons, carved out of [b]Elderwood[/b]obtained from [b]Scavenger Hunts[/b](requires [b][url=https://www.nexusmods.com/stardewvalley/articles/1261]PRFS[/url][/b] module), are quick, nimble weapons enchanted with forest magic which grants them high reach and knockback.
[*][b]Dwarven[/b]weapons, forged from [b]Dwarven Scraps[/b]obtained from Volcano chests, are large, bulky weapons. They grant high defense and knockback, but reduce swing speed.
[*][b]Dragontooth[/b]weapons, forged from [b]Dragon Teeth[/b]obtained from Volcano chests and near dragon skeletons, are light and sharp, granting the highest raw damage out of any weapon in the game.
[/list]
[center][img]https://staticdelivery.nexusmods.com/mods/1303/images/14470/14470-1670910838-1333255876.gif[/img][/center]
Last but not least, the [color=#ffff00][b]Legendary[/b][/color] Galaxy and Infinity weapons will be explained in a following section, but will require considerable work to obtain (see [b]Infinity +1[/b]).

Note that [b]only vanilla weapons have been rebalanced[/b]. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. [b]That's not on me.[/b] Those weapons are broken by design. I just pulled the curtains. You're welcome.

Lastly, in order to better reflect their new weapon types, as well as their mythical or legendary status, several weapons have received vanilla-friendly retextures. These textures will always be overwritten by any installed Content Patcher mods, so there is no concern for compatibility. Moreover, there are tons of small immersive details like visual and sound effects added to mythic and legendary weapons.


[size=5][font=bebas_neuebook]Melee Enchantments[/font][/size]

Enchantments have been almost entirely overhauled. These new enchantments should provide more interesting gameplay options:

[list]
[*][b]Haymaker:[/b] [i]Unchanged from vanilla.[/i]
[*][b]Artful:[/b] Improves the special move of each weapon.[spoiler][b]Offensive Swords:[/b] Can dash twice in succession.[b]Defensive Swords:[/b] The next attack within 5s of a successful parry is guaranteed a critical strike.[b]Daggers:[/b] Quick stab deals an additional hit, and all hits apply Bleed with 100% chance.[b]Clubs:[/b] Smash area +50%. Enemies in range are stunned for 2s.[/spoiler]
[*][b]Blasting:[/b]Accumulates and stores half of the damage from enemy hits (before mitigation). If enough damage is accumulated, the next special move releases that damage as an explosion.
[*][b]Bloodthirsty:[/b]Enemy takedowns recover some health proportional to the enemy's max health. Excess healing is converted into a shield for up to 20% of the player's max health, which slowly decays after not dealing or taking damage for 25s.
[*][b]Carving:[/b] Attacks on-hit reduce enemy defense by 1 (continuing below zero). Removes the armor from Armored Bugs and de-shells Rock Crabs.
[*][b]Cleaving:[/b] Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the target.
[*][b]Energized:[/b] Moving and attacking generates energy. When fully-energized, the next attack causes an electric discharge, dealing heavy damage in a large area.
[*][b]Mammonite's:[/b] Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold. This threshold increases by 1% with each consecutive takedown, resetting when you take damage.
[*][b]Steadfast:[/b]Converts critical strike chance into bonus raw damage (multiplied by critical power).
[*][b]Wabbajack:[/b] Causes unpredictable effects.[spoiler]Examples: damage or heal the enemy; decrease or increase the enemy's stats; transfigure into a different enemy, creature or any random item (note: this can spawn illegal items).[/spoiler]
[/list]

[size=5][font=bebas_neuebook]Other Features[/font][/size]

[list]
[*] [b]The Mines & Weapon Acquisition[/b]
In vanilla, the player is quick to amass large quantities of fodder weapons, both from Mine chests as well as barrel and crate drops. To support the intended experience of progression, all weapons are removed from Mine chests, which instead reward random goodies. In order to obtain new weapons, players will have to fight for monster drops or get lucky with breakable containers. Monster-dropped weapons are rare, but tend to be stronger. Lower-tier weapons can also be purchased directly from Marlon, but will have fixed and significantly weaker stats when obtained this way.[/list]
[list]
[*] [b]Tooltips[/b]
Vanilla weapon tooltips are confusing. Who the heck knows what "+1 Speed" means? This mod improves weapon tooltips for clarity, so you always know exactly what each stat means. You may configure the tooltips to show [b]absolute[/b] or [b]relative[/b] stats; the former displays straight-forward raw stats, while the latter displays percentages [b]based on the weapon type's base stats[/b]. Note that this is the only feature of this mod that affects third-party mod weapons. If you play with expansion mods which add new weapons, such as Ridgeside Village, you will likely notice absurdly high stats in those weapons' tooltips. [b]That's not on me.[/b] Those weapons are broken by design. I just pulled the curtains. You're welcome.[/list]

[size=6][font=bebas_neuebook]Ranged Weapon Changes[/font][/size]

Ranged weapons are actually quite strong in vanilla, mainly because ammo's deal insane damage. They are also boring, however, since ranged combat is very unidimensional. This mod seeks to alleviate that by reducing base ammo damage while also introducing many of the same mechanics afforded to melee weapons.


[size=5][font=bebas_neuebook]Rebalanced Modifiers & Ammo[/font][/size]

To make room for critical headshots and the new Infinity Slingshot the base damage and knockback modifiers of each slingshot are reduced to more reasonable values:
[list]
[*] Master Slingshot: Ammo damage x2 >> x1.5
[*] Galaxy Slingshot: Ammo damage x4 >> x2
[*] [b]Infinity Slingshot:[/b] x2.5
[/list]
The following ammos have been nerfed, either for immersion or balance:
[list]
[*] Coal: 15 damage >> 2 damage - [i]Have you ever held a piece of coal? That stuff is brittle, and weaker than raw wood, so the damage has been reduced accordingly. Not that anybody uses this as ammo anyway.[/i]
[*] Explosive Ammo: 20 damage >> 1 damage - [i]Explosive ammo is meant to be used as a mining utility [b]only[/b], so it's damage has been reduced to reflect that. If you'd like to use slingshots for combat and mining simultaneously, consider taking up the [url=https://www.nexusmods.com/stardewvalley/articles/1261]Rascal[/url]'s extra ammo slot.[/i]
[/list]
The following new ammos have been added:
[list]
[*]Radioactive Ore: 80 damage
[*]Gemstones (Ruby, Emerald, etc.): 60 damage
[*]Diamond: 120 damage
[*]Prismatic Shard: 200 damage
[/list]

[size=5][font=bebas_neuebook]Special Move[/font][/size]

Pressing the action button will trigger a melee smack attack that stuns enemies for 2 seconds. This allows the player to react to enemies in close range without switching weapons, and quickly reposition to continue firing from safety.


[size=5][font=bebas_neuebook]Ranged Enchantments[/font][/size]

The following new enchantments have been created specifically for ranged weapons:
[list]
[*][b]Gatling:[/b]Enables auto-fire.[spoiler]Double-click/press and then hold the use-tool key to engage auto-fire.[/spoiler]
[*][b]Magnum:[/b] Fires enlarged projectiles (larger hitbox).
[*][b]Preserving:[/b] Does not consume ammo.
[*][b]Quincy:[/b] Attacks fire an energy projectile if no ammo is equipped. Only works near enemies.[spoiler]Quincy projectile cannot crit. and cannot knock-back enemies, but is affected by damage modifiers. If the [url=https://www.nexusmods.com/stardewvalley/articles/1261]PRFS[/url]is enabled and the player has the Desperado profession, Quincy projectile will also be affected by Overcharge (built-in Magnum effect).[/spoiler]
[*][b]Spreading:[/b]Consumes one additional ammo to fire two additional projectiles in a cone.
[/list]
Gemstone forges cannot directly be applied to slingshots, but[b]gemstones can be equipped as ammo[/b], and will apply their analogous bonuses when equipped, and will also[b]resonate[/b](see below) if a applicable. With the [url=https://www.nexusmods.com/stardewvalley/articles/1261]Rascal[/url] profession, you may slot up to two gemstones at a time to possibly achieve a level-2 forge.

[list]
[*][b]Emeralds[/b], instead of attack speed, grant[b]firing speed[/b](idem for Emerald Rings), which also affects[b]overcharge[/b]if the PRFS module's Desperado profession is used;
[/list]
Finally, the [b]Galaxy Soul[/b] can be applied to the Galaxy Slingshot, as with other Galaxy weapons, eventually creating the [b]Infinity Slingshot[/b].


[size=5][font=bebas_neuebook]Other Features[/font][/size]

[list]
[*] [b]Remove Grace Period[/b]
Vanilla slingshots are unable to hit enemies in close-range of the player; these shots will fly straight through them. This mod removes this limitation, making ranged combat more reliable.[/list]
[list]
[*] [b]Snowballs[/b]
Standing in a snowy tile with an empty slingshot will allow the player to fire a snowball. The snowball projectile deals no significant damage; this is meant as a fun flavor feature for multiplayer.[/list]

[size=6][font=bebas_neuebook]Ring Changes[/font][/size]

Most combat-oriented rings in vanilla are underwhelming and completely overlooked next to the Iridium Band, which provides a free 3-in-1 ring that can also be combined. This mod tries to make combat rings more interesting, and introduce all-new mechanics specific to the Iridium Band.


[size=5][font=bebas_neuebook]Rebalanced Combat Rings[/font][/size]

This following rings have been changed:
[list]
[*] [b]Warrior Ring:[/b][s]Chance of giving the Warrior Energy[/s] (attack +10 for 5s) [s]buff after slaying a monster.[/s]-> Gain a progressively higher attack bonus as you slay enemies (every 3 enemies increases attack by 1), which falls off gradually after some time out of combat.
[*] [b]Ring of Yoba:[/b] [s]Chance of giving the Yoba's Blessing[/s] (invincible for 5s) [s]buff after taking damage.[/s] -> Taking damage that would leave you below 30% health instead grants a shield that absorbs up to 50% of your maximum health for 30s. Cannot be used again until health is fully recovered.
[*] [b]Savage Ring:[/b][s]+2 Speed for 3s after slaying a monster.[/s] -> Gain a rapidly decaying Speed buff after slaying a monster.
[*] [b]Immunity Ring:[/b][s]Immunity +4.[/s]-> Gain 100% immunity.
[*] [b]Ring of Thorns:[/b] Can cause Bleeding status (in addition to reflected damage).
[/list]

[size=5][font=bebas_neuebook]Infinity Band[/font][/size]

In vanilla, the Iridium Band is an instant 3-rings-in-1 which can also be forged to get a 4-rings-in-1. It completely triviliazes the Ruby Ring, Glow Ring, Magnet Ring and even the brand new Glowstone Ring, introduced in patch 1.5. This module aims to solve all of these issues while also alluding to everyone's favorite cinematic universe.

The Iridium Band has been completely overhauled. Initially, a newly crafted Iridium Band will grant no effects at all. Only with access to the Forge will you be able to awaken its true form by infusing it with a Galaxy Soul, transforming it into an [b]Infinity Band[/b].

The Infinity Band likewise does nothing on its own, but it serves as a vessel for up to [b]four[/b]gemstones. To add a gemstone to the Infinity Band, you must fuse it with a corresponding gemstone ring at the Forge. The same type of gemstone can be added more than once, compounding the effect. Alternatively, combining different gemstones will potentially lead to powerful [b]Resonances[/b].


[size=5][font=bebas_neuebook]Garnet & Resonance Theory[/font][/size]

To compensate for the removal of vanilla Acrobat profession, this mod introduces a seventh gemstone.

The [b]garnet[/b] can be mined upwards of Mine level 80. Socketed to a ring or a weapon, it will grant 10% cooldown reduction to special moves. Garnet Rings, along with all other gemstone rings become craftable at various levels of Combat skill.

With the addition of Garnet, the seven gemstones form a [url=https://en.wikipedia.org/wiki/Diatonic_scale]Diatonic Scale[/url]:

[center][b][color=#ff0000]Ruby (Rb)[/color][/b]➡[b][color=#00ffff]Aquamarine (Aq)[/color][/b]➡[b][color=#9900ff]Amethyst (Am)[/color][/b]➡[b][color=#a61c00]Garnet (Ga)[/color][/b]➡[b][color=#00ff00]Emerald (Em)[/color][/b]➡[b][color=#93c47d]Jade (Jd)[/color][/b]➡[b][color=#f1c232]Topaz (Tp)[/color][/b][/center]
The scale is cyclic, so after [b][color=#f1c232]Tp [/color][/b]comes [b][color=#ff0000]Rb [/color][/b]again, and so on. The first note in the scale is called the [b]Tonic[/b], or [b]Root[/b]. Above, [b][color=#ff0000]Rb [/color][/b]was used as an example, but the scale can be shifted, or [b]transposed[/b], to place any gemstone at the root. But regardless of the root note, the order is always the same.

Like strings in a guitar, the characteristic vibration of each gemstone causes interference patterns. These interferences can be constructive and/or destructive, and they create complex [url=https://en.wikipedia.org/wiki/Overtone]overtones[/url]that add richness to the resulting vibrations, known as [url=https://en.wikipedia.org/wiki/Harmony]Harmonies[/url]. In other words, certain gemstones will harmonize together, creating resonances that amplify their individual effects. At the same time, other gemstone pairs will lead to dissonances, which instead dampen those effects. As a rule of thumb, Gemstones that are positioned farthest from each other in the Diatonic Scale will resonate more strongly, and those positioned adjacent to each other will dissonate. This means that the interval `I - V` (for example, `[color=#ff0000]Rb [/color]- [color=#00ff00]Em[/color]`, `[color=#9900ff]Am [/color]- [color=#f1c232]Tp[/color]`, `[color=#980000]Ga [/color]- [color=#ff0000]Rb[/color]`, etc.) will lead to the strongest resonance, while the interval `I - II` will lead to a dissonance (for example, `[color=#ff0000]Rb [/color]- [color=#00ffff]Aq[/color]`, `[color=#9900ff]Am [/color]- [color=#980000]Ga[/color]`, `[color=#f1c232]Tp [/color]- [color=#ff0000]Rb[/color]`, etc.).

Gemstones placed together in an Infinity Band not only resonate, but can also make up [url=https://en.wikipedia.org/wiki/Chord_(music)]Chords[/url]. Chords have an associated [b]richness[/b], which measures the variety of overtones[url=https://en.wikipedia.org/wiki/Overtone][/url]in the resulting vibrations. A sufficiently rich chord may give rise to entirely new effects. To maximize richness, try to maximize resonance while avoiding repeating Gemstones.

If the player is currently holding a weapon or slingshot with gemstone forges / ammo, resonating chords from equipped Infinity Bands will also amplify the corresponding gemstone forges.

It is my hope that this mechanic will encourage experimentation, and also teach some basic Music Theory.


[size=6][font=bebas_neuebook]Infinity +1[/font][/size]

According to [url=https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword]TV Tropes Wiki[/url], an Infinity +1 sword is
[quote]not only the most powerful of its kind [...] , but its power is matched by how hard it is to acquire.[/quote]
The Vanilla Infinity weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in Vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike Vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired in the same way at the desert pillars, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the backpack (for example, place any club at the number 1 backpack position to receive the Galaxy Hammer). The Galaxy "riddle" at the Pelican Town graveyard has also been rewritten, go check it out! If SVE is installed, it's own copy of the Galaxy Slingshot will be replaced with an Obsidian Edge.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the cursed Blade of Ruin.

In return for all that extra work, the Infinity weapons have extra perks:
[list=1]
[*]+1 gemstone enchantment socket (4 total, keeping in mind that each gemstone can resonate with equipped Infinity Bands, if [b][url=https://www.nexusmods.com/stardewvalley/articles/1263]RNGS[/url][/b] module is enabled).
[*]While at full health, every swing will fire a mid-range energy beam.
[/list]
[b]As the Tempered Galaxy weapons added by SVE do not fit the questline, this feature, if enabled, will prevent those weapons from being sold at Galdoran shops.[/b]


[size=6][font=bebas_neuebook]Enemies[/font][/size]

This mod can optionally randomize enemy stats to provide more dynamic encounters. Randomized stats are biased to the player's daily luck, introducing yet another layer to that mechanic. Visiting the Mines on unlucky days will now provide a truly brutal experience.

This mod also provides three sliders to tailor general combat difficulty. These sliders allow scaling monster health, attack and defense.

Finally, certain enemy hitboxes are also improved, and others have received small visual tweaks.


[size=6][font=bebas_neuebook]Controls & Quality of Life[/font][/size]

This mod includes the following popular control-related features, often featured in other mods.
[list]
[*] [b]Face Mouse Cursor[/b]
When playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.[*] [b]Slick Moves[/b]
Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced. [*] [b]Auto-Selection[/b]
If enemies are nearby, players can optionally choose a weapon, melee or ranged, to be equipped automatically.[/list]

[size=6][font=bebas_neuebook]Other Features[/font][/size]

[list]
[*] [b]Woody Replaces Rusty[/b]
The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with a Wooden Blade.[/list]

[font=bebas_neuebook][size=6]Compatibility[/size][/font]

The follow mods are [b][color=#ff0000]not [/color][/b]compatible.
[list]
[*]While the Infinity Slingshot will appear in[url=https://www.nexusmods.com/stardewvalley/mods/93]CJB Item Spawner[/url], it will be incorrectly classified as a Melee Weapon and will be unusable if spawned in this way. This is due to CJB not recognizing non-vanilla slingshots. This likely will be fixed in game version 1.6.
[*][b]Not[/b]compatible with other mods that introduce weapon types or rebalance weapon stats, such as[url=https://www.nexusmods.com/stardewvalley/mods/6894]Angel's Weapon Rebalance[/url] or[url=https://www.nexusmods.com/stardewvalley/mods/12345]Durin's Rest[/url].
[*][b]Not[/b] compatible with the likes of [url=https://www.nexusmods.com/stardewvalley/mods/2590]Combat Controls[/url] or [url=https://www.nexusmods.com/stardewvalley/mods/10496]Combat Controls Redux[/url], as those features are already included in this and other modules.
[*][b]Not[/b] compatible with other mods that overhaul slingshots, such as [url=https://www.nexusmods.com/stardewvalley/mods/2067]Better Slingshots[/url] or [url=https://www.nexusmods.com/stardewvalley/mods/12763]Enhanced Slingshots[/url].
[*]Weapon rebalance features are [b]not[/b]compatible with mod-added weapons. They will still work perfectly fine, but will not be affected by the Rebalance and will be extremely unbalanced as a result.
[*]New enchantments are [b]not[/b][b] [/b]compatible with other mods that introduce new enchantments, such as [url=https://www.nexusmods.com/stardewvalley/mods/12763]Enhanced Slingshots[/url].
[*]Ring features are [b]not[/b]compatible with other ring mods with similar scope, including [url=https://www.nexusmods.com/stardewvalley/mods/8801]Combine Many Rings[/url], [url=https://www.nexusmods.com/stardewvalley/mods/8981]Balanced Combine Many Rings[/url] and, to an extent, [url=https://www.nexusmods.com/stardewvalley/mods/10669]Ring Overhaul[/url].
[*][b]Not[/b]compatible with[url=https://www.nexusmods.com/stardewvalley/mods/10941]Don't Stop Me Now[/url].
[/list]
The following mods [b][color=#00ff00]are [/color][/b]compatible.[list]
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/7886]Advanced Melee Framework[/url] and related content packs, but I do not recommend using both together.
[*]Compatible with[url=https://www.nexusmods.com/stardewvalley/mods/3753]Stardew Valley Expanded[/url] and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
[*]Compatible with[url=https://www.nexusmods.com/stardewvalley/mods/8642]Better Rings[/url], and will use compatible textures if that mod is installed (thanks to[url=https://www.nexusmods.com/stardewvalley/users/13917800]compare123[/url] for providing Better Rings-compatible textures, and to[url=https://www.nexusmods.com/stardewvalley/users/106000973]bahbahrah[/url], author of Better Rings).
[*]Compatible with[url=https://www.nexusmods.com/stardewvalley/mods/10852]Vanilla Tweaks[/url], and will use some compatible-textures if that mod is installed and it's weapon textures are enabled (thanks to [url=https://www.nexusmods.com/stardewvalley/users/92060238]Taiyokun[/url] for permission to include their textures).
[*]Compatible with[url=https://www.nexusmods.com/stardewvalley/mods/16491]Simple Weapons[/url], and will use some compatible-textures if that mod is installed (thanks to[url=https://www.nexusmods.com/stardewvalley/users/154033023]dendenhsmy[/url] for permission to include their textures).
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/16767]Archery[/url] and the accompanying [url=https://www.nexusmods.com/stardewvalley/mods/16768]Starter Pack[/url]. Install and the misc. Rebalance file for the complete experience.
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/11115]Better Crafting[/url].
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/3214]Wear More Rings[/url].
[/list]

[size=6][font=bebas_neuebook]Frequently Asked Questions[/font][/size]

[b]How do I unlock Clint's Forging mechanic?
[/b][spoiler]You must find one of the old Dwarvish blueprints, be able to understand it, and be close enough friends with a blacksmith so they'll be willing to help you out.[spoiler]Have the Dwarvish Translation Guide and at least 6 hearts with Clint.[/spoiler][/spoiler]

[b]Where can I find the Dwarvish Blueprints?
[/b][spoiler]They can be found along with their corresponding crafting materials, described above in the Weapons section.[/spoiler]

[b]Where can I find the Blade of Ruin?
[/b][spoiler]Think of the evilest places in Stardew Valley.[spoiler]Look behind a certain statue.[spoiler]At the end of the single-floor[url=https://stardewvalleywiki.com/Quarry_Mine]Quarry Mine[/url]from the statue of the Grim Reaper.[/spoiler][/spoiler][/spoiler]

[b]I found the Blade of Ruin but did not get a quest?
[/b][spoiler]The quest to lift the curse will begin once the blade has been used enough to actually manifest said curse.[spoiler]Slay 50 enemies with the blade and you will receive a letter from the Wizard.[/spoiler][/spoiler]

[b]What is the Blade of Ruin's curse?
[/b][spoiler]The Blade of Ruin will grow progressively stronger by cosuming enemies; every 5 enemies slain increases its attack power by 1 point. As it grows stronger, however, it will also begin to consume your own energy, dealing damage-over-time while held. At the same time, the Blade has a nasty habit of auto-equipping itself; the stronger the Blade, the more damage you will suffer, and the more often it will auto-equip itself.
Should you choose to ignore these side-effects and continue to strengthen the Blade, you will eventually become unable to use other weapons, and be forced to engage combat with 1 HP.[/spoiler]

[b]How do I lift the Ruined Blade's curse?
[/b][spoiler]Seek the divine.[spoiler]Read allthe inscriptions at the altar of Yoba.[/spoiler][/spoiler]

[b]What are the IDs of heart events related to the Virtues?
[/b][spoiler]The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the [url=https://www.nexusmods.com/stardewvalley/mods/3642]Event Repeater[/url] mod is installed.[list]
Events where you may demonstrate Honor:[*][b]7 - [/b]Maru 4 hearts
[*][b]16 - [/b]Pierre 6 hearts
[*][b]27 - [/b]Sebastian 6 hearts
[*][b]36 - [/b]Penny 6 hearts
[*][b]46 - [/b]Sam 4 hearts
[*][b]58 - [/b]Harvey 6 hearts
[*][b]100 - [/b]Kent 3 hearts
[*][b]288847 - [/b]Alex 8 hearts
[*][b]2481135 - [/b]Alex 4 hearts
[*][b]733330 - [/b]Sam 3 hearts
[/list][list]
Events where you may demonstrate Compassion:[*][b]13 - [/b]Haley 6 hearts
[*][b]27 - [/b]Sebastian 6 hearts
[*][b]51 - [/b]Leah 4 hearts
[*][b]100 - [/b]Kent 3 hearts
[*][b]288847 - [/b]Alex 8 hearts
[*][b]502969 - [/b]Linus 0 hearts
[*][b]503180 - [/b]Pam 9 hearts
[*][b]733330 - [/b]Sam 3 hearts
[*][b]3910975 - [/b]Shane 6 hearts
[/list]
[list]Events where you may demonstrate Wisdom:[*][b]11 - [/b]Haley 2 hearts
[*][b]21 - [/b]Alex 5 hearts
[*][b]25 - [/b]Demetrius 3 hearts
[*][b]27 - [/b]Sebastian 6 hearts
[*][b]34 - [/b]Penny 2 hearts
[*][b]50 - [/b]Leah 2 hearts
[*][b]56 - [/b]Harvey 2 hearts
[*][b]97 - [/b]Clint 3 hearts
[/list][/spoiler]

[b]How do I obtain the Infinity weapons?[/b]
[spoiler]The true power of a Galaxy weapon can only be unlocked by a virtuous hero.[spoiler]Find an item that contains the soul of a hero, and a way to extract it from that item.[spoiler]Unforge the Blade of Dawn to obtain a Hero Soul, and then forge that into any Galaxy weapon after 3 Galaxy Souls.[/spoiler][/spoiler][/spoiler]

[b]How do I obtain other mythic weapons?[/b]
[spoiler][list]
[*][b]Neptune's Glaive:[/b] Fishing Chests, same as Vanilla.
[*][b]Yeti Tooth:[/b] Dropped by enemies or crates in the icy section of the Mines.
[*][b]Obsidian Edge:[/b] Dropped from Shadow people in the dangerous Mines.
[*][b]Lava Katana:[/b] Dropped from certain enemies in Volcano Dungeon.
[/list][/spoiler]
[b]Will you rebalance weapons from other mods, such as Ridgeside Village?[/b]
No. Here's my reasoning:
For RSV it's simply not possible. There's far too many weapons that are meant to be "unique" / "special", and not enough numbers to play with in terms of weapon stats to make them all[i]feel[/i]unique, while still being balanced. It was difficult enough to rebalance the unique vanilla weapons, and I'm not even entirely sure that I've succeeded.
The RSV team have known for a long time that their weapons are broken, and they still haven't made any effort to fix that, and I imagine it's because they agree that it can't be done.So no, I will not even attempt to do it. And if I won't do it for a high-quality mod like RSV, you can be sure I won't do it for any other.