namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The ephemeral runtime state for Professions.</summary>
internal sealed class State
{
    private int _rageCounter;

    internal int BruteRageCounter
    {
        get => this._rageCounter;
        set
        {
            this._rageCounter = value switch
            {
                >= 100 => 100,
                <= 0 => 0,
                _ => value,
            };
        }
    }

    internal int SpelunkerLadderStreak { get; set; }

    internal int DemolitionistExcitedness { get; set; }

    internal int[] PiperBuffs { get; } = new int[12];

    internal Queue<ISkill> SkillsToReset { get; } = new();

    internal bool UsedStatueToday { get; set; }
}
