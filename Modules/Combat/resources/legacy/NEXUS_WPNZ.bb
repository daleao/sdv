[b][center][size=6][font=bebas_neuebook]MARGO :: Weapons (WPNZ)[/font][/size][/center][/b]
[size=6][font=bebas_neuebook]Overview[/font][/size]

What began as a simple weapon rebalance has become a huge overhaul of all Melee Weapon.

This module has the following objectives:
1. To rebalance the weapon types, creating new playstyles by emphasizing the strengths and identity of each type, in order to scale-back the ubiquity of swords.
2. Reduce the spam-clicky nature of Vanilla Melee Weapons.
3. Rebalance the weapons themselves, making each weapon feel distinct and at least somewhat useful, rather than pure inventory clutter. This includes adding new interesting way to obtain special weapons as well as making the legendary weapons feel truly legendary.

This module tries to achieve all of this through a combination of nerfs, buffs and entirely new mechanics which will overall make combat significantly more strategic and challenging. Players who don't care for a challenge or are uninterested in combat should probably keep this module disabled.

[b][color=#ff7700]Before uninstalling MARGO, you must play through at least one day with this module disabled.[/color][/b]


[size=6][font=bebas_neuebook]Combos & Swing Speed[/font][/size]

Weapon spamming is replaced by combos. These are short bursts of continuous swings followed by a short cooldown. Each weapon type has a configurable combo limit:
[list]
[*][b]Swords[/b] up to 4 horizontal swipes, by default.
[*][b]Clubs[/b] up to 2 hits, being one horizontal swipe and one vertical swipe, by default.
[*][b]Daggers[/b] unchanged, effectively up to infinite hits.
[/list]

If combo hits are enabled, swing speed bonuses from emerald will affect every frame of the attack animation, as well as the cooldown in between combos. This makes speed a significantly more valuable stat than in Vanilla, where it only affected 1 out of 6 frames (and that frame could be animation-canceled).

[center][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/combo_sword_small.gif[/img]   ﻿[img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/combo_club_small.gif[/img]﻿﻿[/center]

[size=6][font=bebas_neuebook]Offensive & Defensive Swords[/font][/size]

In vanilla game code we can find traces of an abandoned fourth weapon type: [b]Stabbing Swords[/b]. This module re-implements stabbing swords into the game.

Swords are now split between [b]offensive[/b] and [b]defensive[/b] archetypes. Defensive swords retain the parry special move, while offensive swords gain a new stabbing thrust move. This move allows quick repositioning and also grants invincibility frames.

[center][img]https://gitlab.com/daleao/sdv-mods/-/raw/main/Modular%20Overhaul/resources/screenshots/stabbing_special.gif[/img]﻿[/center]
To improve the defensive parry move, parry damage is increased by 10% for every defense point (configurable). This adds offensive value to the defense stat itself and makes defensive builds more viable. Note that the introduction of weapon combos also adds to the value of a defensive move (you need to protect yourself during the combo downtime).


[size=6][font=bebas_neuebook]Weapon Tiers & Rebalance[/font][/size]

Weapon stats have been rebalanced across the board:
[list]
[*][b]Clubs[/b] are your general unreliable, imprecise and sluggish, but huge-damage-potential, heavy and larger-hit-box weapons.
[*][b]Daggers[/b] are the opposite of clubs, being the quick, precise, but short-range and generally lower-damage-potential weapons.
[*][b]Offensive Swords[/b] are somewhere in the middle, with average damage, speed and precision.
[*][b]Defensive Swords[/b] are marginally weaker and slower than their offensive counterpart, but are otherwise heavier, sturdier and wider. They are somewhere in-between Offensive Swords and Clubs.
[/list]

Weapons are categorized by different tiers, [url=https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience]color-coded for your convenience[/url]:

[center]﻿[b]Common [/b]< [b][color=#00ff00]Uncommon [/color][/b][color=#f4f4f4]<[/color] [b][color=#3d85c6]Rare [/color][/b]< [color=#9900ff][b]Epic [/b][/color][color=#f4f4f4]<[/color] [b][color=#ff0000]Mythic [/color][/b]< [b][color=#ff7700]Masterwork [/color][/b]< [b][color=#ffff00]Legendary[/color][/b][/center][b][color=#ffff00]
[/color][/b]
[center][img]https://staticdelivery.nexusmods.com/mods/1303/images/14470/14470-1670827097-402941320.gif[/img]﻿[/center]
Weapons below the [color=#ff0000][b]Mythic[/b][/color] tier have randomized damage, determined the moment they are dropped and scaled to your mine progression. This way, players can always loot stronger weapons, and no specific weapon is ever trivialized. Higher-tier weapons will generally be stronger than lower-tiered ones, although that may not always be the case. These weapons can still be purchased from Marlon, but they will have fixed and significantly weaker stats.

[color=#ff0000][b]Mythic[/b][/color] weapons themselves are unique or extremely rare. They are usually quest rewards or rare monster drops, and tend to carry special enchantments in addition to their regular stats.

[b][color=#ff7700]Masterwork[/color][/b] weapons have fixed stats. These are weapons created by the Dwarven race using special materials. They can ony be created by uncovering the lost Dwarvish Blueprints, and taking them to a skilled blacksmith along with the corresponding material:l
[list]
[*][b]Elven[/b] weapons, carved out of [b]Elderwood[/b] obtained from [b]Scavenger Hunts[/b] (requires [b][url=https://www.nexusmods.com/stardewvalley/articles/1261]PRFS[/url]﻿﻿[/b] module), are quick, nimble weapons enchanted with forest magic which grants them high reach and knockback.
[*][b]Dwarven[/b] weapons, forged from [b]Dwarven Scraps[/b] obtained from Volcano chests, are large, bulky weapons. They grant high defense and knockback, but reduce swing speed.
[*][b]Dragontooth[/b] weapons, forged from [b]Dragon Teeth[/b] obtained from Volcano chests and near dragon skeletons, are light and sharp, granting the highest raw damage out of any weapon in the game.
[/list]
[center][img]https://staticdelivery.nexusmods.com/mods/1303/images/14470/14470-1670910838-1333255876.gif[/img]﻿[/center]
Last but not least, the [color=#ffff00][b]Legendary[/b][/color] Galaxy and Infinity will be explained in the next section, but will require considerable work to obtain (see [b]Infinity +1[/b]).

If the Rebalance option is enabled, all weapons will be removed from Mine chests (replaced with valuable but random loot). New weapons will have to be dropped from monsters or breakable containers in Mines and other dungeons. Monster-dropped weapons are rare, but tend to be stronger. Lower-tier weapons can also be purchased directly from Marlon, but will have fixed and significantly weaker stats when obtained this way.

Weapon tooltips have also been improved for clarity, so you should always know exactly what each stat means.

[b]Note that only Vanilla weapons are affected by these features. If playing with expansion mods which add new weapons (e.g., Ridgeside Village), those weapons will remain unchanged and unbalanced. I strongly recommend ignoring these weapons, or even *deleting* them manually from the mod's files.[/b]


[size=6][font=bebas_neuebook]Infinity +1[/font][/size]

According to [url=https://tvtropes.org/pmwiki/pmwiki.php/Main/InfinityPlusOneSword]TV Tropes Wiki[/url], an Infinity +1 sword is "not only the most powerful of its kind ... , but its power is matched by how hard it is to acquire". The Vanilla Galaxy weapons do not quite fit that definition. Let's fix that, shall we?

To obtain your first Galaxy weapon, as in vanilla you must first unlock the desert, acquire a Prismatic Shard and offer it to the Three Sand Sisters. Unlike vanilla, however, the weapon will not materialize out of thin air, but will be shaped out of a configurable amount of Iridium Bars (10 by default), which must be in your inventory. This will prevent a lucky Prismatic Shard drop from the Mines or a Fishing Chest from instantly rewarding one of the strongest weapons in the game before the player has even set foot in the Skull Caverns. Now, some venturing into the Skull Caverns is required.

Subsequent Galaxy weapons will no longer be available for purchase at the Adventurer's Guild; one full set, including the slingshot, can now be acquired in the same way at the desert pillars, but each weapon will require a larger stack of Prismatic Shards. The order in which the weapons are obtained can be influenced by placing the desired weapon type at the top of the backpack (for example, place any club at the number 1 backpack position to receive the Galaxy Hammer). The Galaxy "riddle" at the Pelican Town graveyard has also been rewritten, go check it out! If SVE is installed, it's own copy of the Galaxy Slingshot will be replaced with an Obsidian Edge.

Upgrading to Infinity is now a much more involved task, requiring the player to prove they have a virtuous and heroic soul. Doing so will require completion of a new questline revolving around the all-new Blade of Ruin...

In return for all that extra work, the Infinity weapons have extra perks:
[list=1]
[*]   ﻿+1 gemstone enchantment socket (4 total, keeping in mind that each gemstone can resonate with equipped Infinity Bands, if [b][url=https://www.nexusmods.com/stardewvalley/articles/1263]RNGS[/url]﻿﻿[/b] module is enabled).
[*]   ﻿While at full health, every swing will fire a mid-range energy beam.
[/list]

[b]As the Tempered Galaxy weapons added by SVE do not fit the questline, this feature, if enabled, will prevent those weapons from being sold at Galdoran shops.[/b]


[size=6][font=bebas_neuebook]Other Features[/font][/size]

[list]
[*][b]Weapon Retexture:[/b] Available optionally, weapons can be retextured to better reflect their type or rarity. This is strongly recommended to visually distinguish Defensive and Offensive-oriented swords, and to make Mythic and above weapons look more unique and powerful. These textures will always be overwritten by Content Patcher weapon retextures if any is installed.
[*][b]Grounded Club Smash:[/b] Prevents gliders from being hit by the Club's smash attack, but guarantees a critical hit on under-ground Duggies. A controversial but immersive change.
[*][b]Woody Replaces Rusty:[/b] The vanilla game has too many weapons for its own good. A minor issue which results from this is the very awkward "upgrade" from the starting Rusty Sword to the Wooden Blade. Why would Marlon be mocking about with a rusty weapon anyway? This has always bothered me, and so, for a slight increase in immersion, this novelty feature will remove the Rusty Sword from the game and replace the starter weapon with a Wooden Blade.
[*][b]Face Mouse Cursor:[/b] This popular feature found in other mods is built-in to this module; when playing with mouse and keyboard the farmer will always swing their weapon in the direction of the mouse cursor.
[*][b]Slick Moves:[/b] Swinging a weapon while running will preserve the player's momentum, causing them to drift in the direction of movement. This increases the player's mobility and makes combat feel more fast-paced.
[*][b]Auto-Selection:[/b] If enemies are nearby, players can optionally choose a weapon to be equipped automatically.
[/list]

[font=bebas_neuebook][size=6]Compatibility[/size]

[/font][list]
[*][b]Not[/b] compatible with other mods that introduce weapon types or rebalance weapon stats, such as [url=https://www.nexusmods.com/stardewvalley/mods/6894]Angel's Weapon Rebalance[/url]﻿ or [url=https://www.nexusmods.com/stardewvalley/mods/12345]Durin's Rest[/url]﻿.
[*][b]Not[/b] compatible with the likes of [url=https://www.nexusmods.com/stardewvalley/mods/2590]Combat Controls[/url]﻿ or [url=https://www.nexusmods.com/stardewvalley/mods/10496]Combat Controls Redux[/url]﻿, as those features are already included in this and other modules.
[*][b]Not[/b] compatible with [url=https://www.nexusmods.com/stardewvalley/mods/10941]Don't Stop Me Now[/url]﻿.
[*][b]Not[/b] compatible with mod-added weapons. They will still work perfectly fine, but will not be affected by the Rebalance and will be extremely unbalanced as a result.
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/7886]Advanced Melee Framework[/url]﻿ and related content packs, but I do not recommend using both together.
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/3753]Stardew Valley Expanded[/url]﻿﻿ and will overwrite the changes to weapons stats from that mod, and also prevent Tempered Galaxy Weapons from appearing in shops. An optional FTM file is available to overwrite SVE's weapon spawns and prevent them from breaking this module's intended balance.
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/10852]Vanilla Tweaks[/url], and will use some compatible-textures if that mod is installed and it's weapon textures are enabled (thanks to [url=https://www.nexusmods.com/stardewvalley/users/92060238]Taiyokun[/url]﻿ for permission to include their textures).﻿
[/list][size=6][font=bebas_neuebook]
Frequently Asked Questions[/font][/size][b]

How do I unlock Clint's Forging mechanic?
[/b]   ﻿[spoiler]You must find one of the old Dwarvish blueprints, be able to understand it, and be close enough friends with a blacksmith so they'll be willing to help you out.[spoiler]Have the Dwarvish Translation Guide and at least 6 hearts with Clint.[/spoiler][/spoiler]

[b]Where can I find the Dwarvish Blueprints?
[/b]   ﻿[spoiler]They can be found along with their corresponding crafting materials, described above in the Weapons section.[/spoiler]

[b]Where can I find the Blade of Ruin?
[/b]   ﻿[spoiler]Think of the evilest places in Stardew Valley.[spoiler]Look behind a certain statue.[spoiler]At the end of the single-floor [url=https://stardewvalleywiki.com/Quarry_Mine]Quarry Mine[/url] from the statue of the Grim Reaper.[/spoiler][/spoiler][/spoiler]

[b]What is the Blade of Ruin's curse?
[/b]   ﻿[spoiler]The Blade of Ruin will grow progressively stronger by cosuming enemies; every 5 enemies slain increases its attack power by 1 point. As it grows stronger, however, it will also begin to consume your own energy, dealing damage-over-time while held. At the same time, the Blade has a nasty habit of auto-equipping itself; the stronger the Blade, the more damage you will suffer, and the more often it will auto-equip itself.
Should you choose to ignore these side-effects and continue to strengthen the Blade, you will eventually become unable to use other weapons, and be forced to engage combat with 1 HP.[/spoiler]

[b]How do I lift the Ruined Blade's curse?
[/b]   ﻿[spoiler]Gil at the Aventurer's Guild can give you a clue.[spoiler]You must prove the 5 heroic virtues throughout your playtime in order to receive Yoba's blessing.[spoiler]Prove your Honor, Compassion and Wisdom by selecting certain responses during character heart events (you will have at least 8 chances to prove each of these virtues at least 3 times). Prove your Valor by completing at least 5 monster slayer quests. Finally, prove your Generosity by purchasing the house upgrade for Pam. When you are ready, approach Yoba's altar in Pierre's house.[/spoiler][/spoiler][/spoiler]

[b]What are the IDs of heart events related to the Virtues?
[/b]   ﻿[spoiler]The following events provide chances to demonstrate your virtues. You can use these IDs in conjunction with the `debug ebi <id>` command to replay these events, provided that the [url=https://www.nexusmods.com/stardewvalley/mods/3642]Event Repeater[/url] mod is installed.[list]
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
[/list][list]Events where you may demonstrate Wisdom:[*][b]11 - [/b]Haley 2 hearts
[*][b]21 - [/b]Alex 5 hearts
[*][b]25 - [/b]Demetrius 3 hearts
[*][b]27 - [/b]Sebastian 6 hearts
[*][b]34 - [/b]Penny 2 hearts
[*][b]50 - [/b]Leah 2 hearts
[*][b]56 - [/b]Harvey 2 hearts
[*][b]97 - [/b]Clint 3 hearts
[/list]   ﻿[/spoiler]
  
[b]How do I obtain the Infinity weapons?[/b]
   ﻿[spoiler]The true power of a Galaxy weapon can only be unlocked by a virtuous hero.[spoiler]Find an item that contains the soul of a hero, and a way to extract it from that item.[spoiler]Unforge the Blade of Dawn to obtain a Hero Soul, and then forge that into any Galaxy weapon after 3 Galaxy Souls.[/spoiler][/spoiler][/spoiler]
  
[b]How do I obtain other mythic weapons?[/b]
   ﻿[spoiler][list]
[*][b]Neptune's Glaive:[/b] Fishing Chests, same as Vanilla.
[*][b]Yeti Tooth:[/b] Dropped by enemies or crates in the icy section of the Mines.
[*][b]Obsidian Edge:[/b] Dropped from Shadow people in the dangerous Mines.
[*][b]Lava Katana:[/b] Dropped from certain enemies in Volcano Dungeon.
[/list][/spoiler]
[b]Will you rebalance weapons from other mods, such as Ridgeside Village?[/b]
   ﻿No. Here's my reasoning:
   ﻿For RSV it's simply not possible. There's far too many weapons that are meant to be "unique" / "special", and not enough numbers to play with in terms of weapon stats to make them all [i]feel[/i] unique, while still being balanced. It was difficult enough to rebalance the unique vanilla weapons, and I'm not even entirely sure that I've succeeded.
   ﻿The RSV team have known for a long time that their weapons are broken, and they still haven't made any effort to fix that, and I imagine it's because they agree that it can't be done. So no, I will not even attempt to do it. And if I won't do it for a high-quality mod like RSV, you can be sure I won't do it for any other.