﻿namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CombinedRing_BufferedStats
{
    internal static ConditionalWeakTable<CombinedRing, StatBuffer> Values { get; } = new();

    internal static StatBuffer Get_StatBuffer(this CombinedRing combined)
    {
        return Values.GetValue(combined, Create);
    }

    private static StatBuffer Create(CombinedRing combined)
    {
        var buffer = new StatBuffer();
        combined.combinedRings.ForEach(r => Gemstone.FromRing(r.QualifiedItemId).Buffer(buffer));
        combined.Get_Chord()?.Buffer(buffer);
        return buffer;
    }
}
