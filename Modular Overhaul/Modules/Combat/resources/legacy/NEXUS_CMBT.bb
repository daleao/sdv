[b][center][size=6][font=bebas_neuebook]MARGO :: Combat (CMBT)[/font][/size][/center][/b]
[size=6][font=bebas_neuebook]Overview[/font][/size]

This simple module provides the following rebalancing options for general combat:

[list=1]
1. [b]Knockback Damage:[/b] Knocked-back enemies will take damage porportional to the knockback stat when colliding with a wall or obstacle. This makes knockback a more viable offensive stat, in addition to its defensive value. It also makes positioning an important strategic element.

2. [b]Defense Overhaul:[/b] Replaces the linear subtraction formula from Vanilla with an exponential multiplicative formula, providing better scalability, and thus more value to the defense stat. One defense point now reduces damage by 10% regardless. Subsequent points have diminishing returns, such that 100% damage negation is not possible to achieve. [b]Note that this applies to monsters as well![/b] Crit. strikes will ignore enemy defense, allowing critical builds to counter defensive enemies.
[size=1][spoiler]
   [b] Old formula:[/b] damage = Min(rawDamage - defense, 1)
   [b] New formula:[/b] [size=1]damage = rawDamage * [/size]10 / (10 + defense)
[/spoiler][/size]

3. [b]Difficulty Sliders:[/b] 3 sliders are provided to tailor monster difficulty to your preference:
[list]
[*]Monster health multiplier
[*]Monster damage multiplier
[*]Monster defense multiplier
[/list]
Compatible with any Content Patcher mods which affect monster stats.

4. [b]Varied Encounters:[/b] Randomizes monster stats according to Daily Luck, reducing the monotony of dungeons.[/list]
All features are optional and can be toggled individually.


[size=6][font=bebas_neuebook]Status Conditions[/font][/size]

Taking inspiration from classic RPG or strategy games, this module adds a framework for causing various status conditions to enemies. They are:
[list]
[*][b]Bleeding:[/b] Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
[*][b]Burning:[/b] Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magma Sparkers).
[*][b]Chilled:[/b] Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
[*][b]Freeze:[/b] Cannot move or attack for 30s. The next hit during the duration deals triple damage and ends the effect.
[*][b]Poisoned:[/b] Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
[*][b]Slowed:[/b] Reduces movement speed by half for the duration.
[*][b]Stunned:[/b] Cannot move or attack for the duration.
[/list]

While it doesn't do anything on it's own, this opens up the possibility for other modules within MARGO to create more interesting overhauls. Each status condition is accompanied by a neat corresponding animation. Status conditions cannot be applied on the player.


[size=6][font=bebas_neuebook]Compatibility[/font][/size]

No known incompatibilities.