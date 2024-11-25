<div align="center">

# Aquarism

</div>

## What This Is

This mod is a companion to [Walk Of Life](../Professions)'s Aquarist profession, but can be used without it. It makes Fish Ponds actually viable as income source and significantly more interesting as a mechanic and playstyle.


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


## Compatibility

- Compatible with Content Patcher mods which apply visual changes to Fish Ponds or which edit Fish Pond Data.


## Credits & Special Thanks

Credits to the following translators:
- ![](https://i.imgur.com/ezVo9Fb.png) [CaranudLapin](https://github.com/CaranudLapin) for French.
- ![](https://i.imgur.com/Jvsm5YJ.png) [whdms2008](https://next.nexusmods.com/profile/whdms2008/about-me?gameId=1303) for Korean.
- ![](https://i.imgur.com/zuQC9Di.png) [Awassakura](https://next.nexusmods.com/profile/Awassakura/about-me?gameId=1303) for Chinese.
