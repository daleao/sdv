namespace DaLion.Redux.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Redux.Professions.TreasureHunts;

#endregion using directives

/// <summary>Holds the runtime state variables of the Professions module.</summary>
internal sealed class State
{
    internal Lazy<TreasureHunt> ScavengerHunt { get; } = new(() => new ScavengerHunt());

    internal Lazy<TreasureHunt> ProspectorHunt { get; } = new(() => new ProspectorHunt());

    internal int[] AppliedPiperBuffs { get; } = new int[12];

    internal int BruteRageCounter { get; set; }

    internal int BruteKillCounter { get; set; }

    internal int SecondsOutOfCombat { get; set; }

    internal int DemolitionistExcitedness { get; set; }

    internal int SpelunkerLadderStreak { get; set; }

    internal int SlimeContactTimer { get; set; }

    internal bool UsingPrimaryAmmo { get; set; }

    internal bool UsingSecondaryAmmo { get; set; }

    internal bool UsedDogStatueToday { get; set; }

    internal Queue<ISkill> SkillsToReset { get; } = new();
}
