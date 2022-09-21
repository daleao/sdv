namespace DaLion.Stardew.Professions;

#region using directives

using System;
using System.Collections.Generic;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.TreasureHunts;

#endregion using directives

internal sealed class ModState
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
