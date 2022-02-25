namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

using Framework.SuperMode;
using Framework.TreasureHunt;

#endregion using directives

internal class PlayerState
{
    private ISuperMode _superMode;
    private TargetMode _pipeMode = TargetMode.Passive;

    internal ISuperMode SuperMode
    {
        get => _superMode;
        set
        {
            if (value is null) _superMode?.Dispose();
            _superMode = value;
        }
    }

    internal TargetMode PipeMode
    {
        get => _pipeMode;
        set
        {
            if (Context.IsMainPlayer)
            {
                if (value == TargetMode.Aggressive)
                    ModEntry.HostState.AggressivePipers.Add(Game1.player.UniqueMultiplayerID);
                else
                    ModEntry.HostState.AggressivePipers.Remove(Game1.player.UniqueMultiplayerID);
            }
            else
            {
                ModEntry.ModHelper.Multiplayer.SendMessage("Toggled" + value + "Targeting", "RequestHostState",
                    new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
            }

            _pipeMode = value;
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
    internal int KeyPressAccumulator { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
}