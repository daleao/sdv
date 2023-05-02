[b][center][size=6][font=bebas_neuebook]MARGO :: Ponds (PNDS)[/font][/size][/center][/b]
[size=6][font=bebas_neuebook]Overview[/font][/size]

This module makes Fish Ponds useful and immersive by implementing the following features:

[list=1]
[*]Fish Ponds preserve the quality of fish placed inside. The quality of newly spawned fish is inherited from a randomly chosen parent. Fishing from a pond always removes the lowest-quality fish first.
[*]Instead of choosing only one of the possible items specified in FishPondData, the pond will attempt to produce all possible items, such that each pond may produce several items per day. Interacting with a Fish Pond which holds multiple items opens a chest-like menu.
[*]Roe and ink are handled separately: each fish species is given a roe/ink production chance based on it's value; the higher the value of the fish, the lower its chance to produce roe or ink. Every day, each fish in the pond will attempt to produce roe/ink based on that chance. Population size also boosts each individual fish's production chance. This results in roe/ink production that scales with population size, with common fish tending to produce a lot of low-value roe/ink, and rarer fish producing less but more valuable roe/ink, which should promote variability. Sturgeons have special conditions to produce bonus roe, and Coral produce algae or seaweed instead.
[*]After 3 days, empty ponds will spontaneously grow algae/seaweed. New algae/seaweed spawn every 2 days, and produce a random amount of algae/seaweed depending on population. Note that seaweed, green algae and white algae will all grow simultaneously in the same pond. Algae/seaweed ponds have population gates and quests, but their population can only increase naturally; i.e. you cannot manually place more algae/seaweed in the pond.
[*]Ponds containing Radioactive or Mutant fish species will enrich any ores or metal bars dropped inside, converting them to radioactive counterparts after some days.
[/list]

This module is intended to complement [b][url=https://www.nexusmods.com/stardewvalley/articles/1261]PROFS[/url][/b] module and its Aquarist profession, but can be used without it.

Before disabling this module, please [b]manually fish out and then clear every Fish Pond[/b] instance to reset mod data in order to prevent issues.


[size=6][font=bebas_neuebook]Compatibility[/font][/size]

[list]
[*]Compatible with Content Patcher packs which apply visual changes to Fish Ponds or which edit Fish Pond Data.
[*]Compatible with [url=https://www.nexusmods.com/stardewvalley/mods/4703]Pond Painter[/url]﻿.
[*][b]Not[/b] compatible with [url=https://www.nexusmods.com/stardewvalley/mods/4702]Anything Pond[/url]﻿ or [url=https://www.nexusmods.com/stardewvalley/mods/11021]Quality Fish Ponds[/url]﻿.
[*][b]Not[/b] compatible with [url=https://www.nexusmods.com/stardewvalley/mods/7651]Smaller Fish Ponds[/url]﻿﻿ due to a [b]bug [/b]in that mod which prevents Fish Wells from persisting mod data. [b]Don't ask me for compatibility. I've already sent the author a fix. There's nothing else I can do if they don't come back to update their mod.[/b]
[/list]