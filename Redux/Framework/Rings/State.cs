namespace DaLion.Redux.Framework.Rings;

#region using directives

using System.Collections.Generic;
using DaLion.Redux.Framework.Rings.Resonance;

#endregion using directives

/// <summary>Holds the runtime state variables of the Rings module.</summary>
internal sealed class State
{
    internal int SavageExcitedness { get; set; }

    internal int WarriorKillCount { get; set; }

    internal List<Chord> ResonatingChords { get; } = new();
}
