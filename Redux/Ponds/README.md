<table align="center"><tr><td align="center" width="9999">

<!-- LOGO, TITLE, DESCRIPTION -->

# Aquarism - Immersive Fish Ponds

<br/>

<!-- TABLE OF CONTENTS -->
<details open="open" align="left">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#features">Features</a></li>
    <li><a href="#compatibility">Compatbility</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#special-thanks">Special Thanks</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

</td></tr></table>

## Features

This mod makes Fish Ponds useful and immersive by implementing the following features:

1. Fish Ponds preserve the quality of fish placed inside. The quality of newly spawned fish is inherited from a randomly chosen parent. Fishing from a pond always removes the lowest-quality fish first.
2. Instead of choosing only one of the possible items specified in FishPondData, the pond will attempt to produce all possible items, such that each pond may produce several items per day. Interacting with a Fish Pond which holds multiple items opens a chest-like menu.
3. Roe and ink are handled separately: each fish species is given a roe/ink production chance based on it's value; the higher the value of the fish, the lower its chance to produce roe or ink. Every day, each fish in the pond will attempt to produce roe/ink based on that chance. Population size also boosts each individual fish's production chance. This results in roe/ink production that scales with population size, with common fish tending to produce a lot of low-value roe/ink, and rarer fish producing less but more valuable roe/ink, which should promote variability. Sturgeons have special conditions to produce bonus roe, and Coral produce algae or seaweed instead.
4. After 3 days, empty ponds will spontaneously grow algae/seaweed. New algae/seaweed spawn every 2 days, and produce a random amount of algae/seaweed depending on population. Note that seaweed, green algae and white algae will all grow simultaneously in the same pond. Algae/seaweed ponds have population gates and quests, but their population can only increase naturally; i.e. you cannot manually place more algae/seaweed in the pond.
5. Ponds containing Radioactive or Mutant fish species will enrich any ores or metal bars dropped inside, converting them to radioactive counterparts after some days.

This is a companion mod for [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111) and its Aquarist profession, but can be used without it.

## Compatibility

This mod makes heavy use of Harmony to patch the behavior of Fish Ponds and adjacent objects. Any SMAPI mods that also patch Fish Pond behavior might be incompatible. Content Patcher packs that edit Fish Ponds or FishPondData are compatible, however.

- Compatible with (and meant to be paired with) [Walk Of Life](https://www.nexusmods.com/stardewvalley/mods/8111).
- Compatible with [Pond Painter](https://www.nexusmods.com/stardewvalley/mods/4703).
- **Not** compatible with [Anything Pond](https://www.nexusmods.com/stardewvalley/mods/4702) or [Quality Fish Ponds](https://www.nexusmods.com/stardewvalley/mods/11021).
- **Not** compatible with [Smaller Fish Ponds](https://www.nexusmods.com/stardewvalley/mods/7651)﻿ due to a bug in that mod which prevents Fish Wells from persisting mod data.
- 
Should be fully compatible with multiplayer. Not compatible with Android.

## Installation

Install like any other mod, by extracting the content of the downloaded zip file to your mods folder and starting the game via SMAPI.

To update, first delete the old version and then install the new one. You can optionally keep your configs.json in case you have personalized settings.

Before uninstall, **fish out and clear all ponds** to remove rogue data. Then delete the mod from your mods folder.

## Special Thanks

- [MouseyPounds](https://www.nexusmods.com/stardewvalley/users/3604264), author of Anything Ponds, for the idea of spontaneous algae growth.
