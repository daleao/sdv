using System.Security.Policy;

namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewValley.Monsters;

using Framework;
using Framework.TreasureHunt;
using Framework.Ultimate;

#endregion using directives

internal class PlayerState
{
    private IUltimate _registeredUltimate;

    internal IUltimate RegisteredUltimate
    {
        get => _registeredUltimate;
        set
        {
            if (value is null) _registeredUltimate?.Dispose();
            _registeredUltimate = value;
        }
    }

    internal ITreasureHunt ScavengerHunt { get; set; } = new ScavengerHunt();
    internal ITreasureHunt ProspectorHunt { get; set; } = new ProspectorHunt();
    internal HudPointer Pointer { get; set; } = new();
    internal Dictionary<int, float> OverchargedBullets { get; } = new();
    internal HashSet<int> BlossomBullets { get; } = new();
    internal HashSet<int> BouncedBullets { get; } = new();
    internal HashSet<int> PiercedBullets { get; } = new();
    internal HashSet<Monster> FearedMonsters { get; } = new();
    internal HashSet<GreenSlime> PipedSlimes { get; } = new();
    internal int[] AppliedPiperBuffs { get; } = new int[12];
    internal int BruteRageCounter { get; set; }
    internal int BruteKillCounter { get; set; }
    internal int SecondsSinceLastCombat { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
}