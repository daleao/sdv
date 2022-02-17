namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

using Framework.SuperMode;
using Framework.TreasureHunt;

#endregion using directives

internal class ModState
{
    private ISuperMode _superMode;

    internal ISuperMode SuperMode
    {
        get => _superMode;
        set
        {
            if (value is null) _superMode?.Dispose();
            _superMode = value;
        }
    }

    internal ITreasureHunt ScavengerHunt { get; set; } = new ScavengerHunt();
    internal ITreasureHunt ProspectorHunt { get; set; } = new ProspectorHunt();
    internal HudPointer Pointer { get; set; } = new();
    internal HashSet<int> AuxiliaryBullets { get; } = new();
    internal HashSet<int> BouncedBullets { get; } = new();
    internal HashSet<int> PiercedBullets { get; } = new();
    internal HashSet<GreenSlime> PipedSlimes { get; } = new();
    internal HashSet<SuperfluidSlime> SuperfluidSlimes { get; } = new();
    internal Dictionary<long, Farmer> FakeFarmers { get; } = new();
    internal Dictionary<SuperModeIndex, HashSet<long>> ActivePeerSuperModes { get; } = new();
    internal TargetMode PipeMode { get; set; }
    internal int KeyPressAccumulator { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
    internal ICursorPosition DebugCursorPosition { get; set; }
}