namespace DaLion.Stardew.Professions;

#region using directives

using Framework;
using Framework.TreasureHunts;
using Framework.Ultimates;
using StardewValley.Monsters;
using System.Collections.Generic;

#endregion using directives

internal class PlayerState
{
    private Ultimate? _registeredUltimate;

    internal Ultimate? RegisteredUltimate
    {
        get => _registeredUltimate;
        set
        {
            if (value is null) ModEntry.EventManager.UnhookStartingWith("Ultimate");
            _registeredUltimate = value;
        }
    }

    internal HudPointer Pointer { get; } = new();
    internal TreasureHunt ScavengerHunt { get; } = new ScavengerHunt();
    internal TreasureHunt ProspectorHunt { get; } = new ProspectorHunt();
    internal HashSet<GreenSlime> PipedSlimes { get; } = new();
    internal int[] AppliedPiperBuffs { get; } = new int[12];
    internal int BruteRageCounter { get; set; }
    internal int BruteKillCounter { get; set; }
    internal int SecondsOutOfCombat { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
    internal Queue<ISkill> SkillsToReset { get; } = new();
}