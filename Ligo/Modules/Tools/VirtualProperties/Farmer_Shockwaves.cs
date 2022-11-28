namespace DaLion.Ligo.Modules.Tools.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_Shockwaves
{
    internal static ConditionalWeakTable<Farmer, List<Shockwave>> Values { get; } = new();

    internal static List<Shockwave> Get_Shockwaves(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    internal static bool Get_HasShockwave(this Farmer farmer)
    {
        return Values.TryGetValue(farmer, out var shockwaves) && shockwaves.Count > 0;
    }
}
