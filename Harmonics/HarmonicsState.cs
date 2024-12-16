namespace DaLion.Harmonics;

#region using directives

using System.Collections.Generic;
using DaLion.Harmonics.Framework;
using StardewValley.Tools;

#endregion using directives

internal sealed class HarmonicsState
{
    internal Dictionary<string, Chord> ResonantChords { get; } = [];

    internal HashSet<MeleeWeapon> ResonantBlades { get; } = [];
}
