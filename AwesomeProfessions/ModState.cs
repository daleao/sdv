using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.SuperMode;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions;

internal class ModState
{
    internal SuperMode SuperMode { get; set; }
    internal TreasureHunt ScavengerHunt { get; set; } = new ScavengerHunt();
    internal TreasureHunt ProspectorHunt { get; set; } = new ProspectorHunt();
    internal Indicator Indicator { get; set; } = new();
    internal bool UsedDogStatueToday { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal Dictionary<int, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
    internal HashSet<int> MonstersStolenFrom { get; set; } = new();
    internal HashSet<int> AuxiliaryBullets { get; set; } = new();
    internal HashSet<int> BouncedBullets { get; set; } = new();
    internal HashSet<int> PiercedBullets { get; set; } = new();
    internal Dictionary<GreenSlime, float> PipedSlimeScales { get; set; } = new();
    internal ICursorPosition CursorPosition { get; set; }
}