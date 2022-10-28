namespace DaLion.Stardew.Rings;

#region using directives

using System.Collections.Generic;
using DaLion.Stardew.Rings.Framework.Resonance;

#endregion using directives

internal sealed class ModState
{
    internal int SavageExcitedness { get; set; }

    internal List<Chord> ResonatingChords { get; } = new();
}
