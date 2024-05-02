namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Shared.Data;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="ModDataManager"/> class.</summary>
internal static class ModDataManagerExtensions
{
    /// <summary>Appends the specified item <paramref name="id"/> to the <paramref name="farmer"/>>'s list of foraged items.</summary>
    /// <param name="data">The <see cref="ModDataManager"/>.</param>
    /// <param name="id">The item's (non-qualified) ID.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void AppendToEcologistItemsForaged(this ModDataManager data, string id, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        var itemsForaged = data.Read(farmer, DataKeys.EcologistVarietiesForaged)
            .ParseList<string>()
            .ToHashSet();
        if (!itemsForaged.Contains(id))
        {
            data.Append(farmer, DataKeys.EcologistVarietiesForaged, id);
        }
    }

    /// <summary>Appends the specified item <paramref name="id"/> to the <paramref name="farmer"/>'s list of collected minerals.</summary>
    /// <param name="data">The <see cref="ModDataManager"/>.</param>
    /// <param name="id">The item's (non-qualified) ID.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void AppendToGemologistMineralsCollected(this ModDataManager data, string id, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        var mineralsCollected = data.Read(farmer, DataKeys.GemologistMineralsStudied)
            .ParseList<string>()
            .ToHashSet();
        if (!mineralsCollected.Contains(id))
        {
            data.Append(farmer, DataKeys.GemologistMineralsStudied, id);
        }
    }
}
