namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The runtime state variables for PRFS.</summary>
internal sealed class State
{
    private int _rageCounter;
    private Monster? _lastDesperadoTarget;

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

    internal Monster? LastDesperadoTarget
    {
        get => this._lastDesperadoTarget;
        set
        {
            this._lastDesperadoTarget = value;
            if (value is not null)
            {
                EventManager.Enable<LastDesperadoTargetUpdateTickedEvent>();
            }
        }
    }

    internal Queue<ISkill> SkillsToReset { get; } = new();

    internal bool UsedStatueToday { get; set; }
}
