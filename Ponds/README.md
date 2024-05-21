<div align="center">

# Aquarism

</div>

## What this is

This mod is a companion to the Aquarist profession from Ars Magisteri, but can be used without it. It makes Fish Ponds more useful and immersive.


## What it does

1. **Persistent quality:**
    - Fish Ponds preserve the quality of fish placed inside it.
    - New fishlings also inherit their quality from a randomly chosen parent.
    - Fishing from a pond always removes the lowest-quality fish first.
2. **Expanded item production:**
    - Rather than choose only one of the possible items from the fish's loot table, Fish Ponds attempt to produce all possible items.
    - A pond may thus produce multiple items per day.
    - Produced items are not replaced each morning, and instead are stored safely and indefinitely within the chum bucket's inventory.
3. **Scaling roe production:**
    - Roe is handled separately from other items. Roe entries in FishPondData are ignored, and instead each fish species is given a roe production chance proportional to the inverse of its value; i.e., the higher the value of the fish, the **lower** its chance to produce roe or ink.
    - Roe production chance is checked daily **for each fish in the pond**; i.e., roe and ink production scales with the population size of the pond.
    - Common fish will tend to produce a lot of low-value roe, and rarer fish will produce less but more valuable roe/ink. This setup promotes variability
    - Sturgeons have special conditions to produce bonus roe.
    - Squids instead produce ink, and Coral instead produce algae or seaweed.
4. **Spontaneously growing algae:**
    - After 3 days empty, a ponds will spontaneously begin to grow algae or seaweed at random. A new algae/seaweed will then spawn every 2 days after that.
    - A random  amount of algae/seaweed (also depending on population) will be added to the chum bucket daily.
    - Note that seaweed, green algae and white algae will all grow simultaneously in the same pond.
    - Algae/seaweed ponds have population gates and quests, but their population can only increase naturally; i.e. you cannot manually place more algae/seaweed in the pond.
5. **Radioactive enrichment:**
    - Ponds containing Radioactive or Mutant fish species are able to enrich any ores dropped inside, turning them into radioactive ore after a few days.
    - Enrichment time is longer for cheaper ore, and is also reduced the higher the pond's population.
6. **Family sharing:**
    - If the player has Ars Magisteri's Aquarist profession, which allows Legendary Fish Ponds, then extended family pairs can be raised together (in the same pond) with their legendary counterparts. An Angler (Angler + Ms. Angler) couple will be able to mate under these conditions.
    - If [More New Fish](https://www.nexusmods.com/stardewvalley/mods/3578) is installed, Tui and La count as a family pair. They produce essences instead of roe, and, if placed together in the same pond, may produce a Galaxy Soul instead.


## Compatibility

- Compatible with Content Patcher mods which apply visual changes to Fish Ponds or which edit Fish Pond Data.

[🔼 Back to top](#margo--ponds-pnds)