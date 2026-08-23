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

    /// <summary>Reads from the <paramref name="machine"/>'s treatments data.</summary>
    /// <param name="data">The <see cref="ModDataManager"/>.</param>
    /// <param name="machine">The <see cref="SObject"/> machine instance.</param>
    /// <returns>A tuple containing overclock cycles, coating cycles, and the type of coating applied.</returns>
    internal static (int OverclockCycles, int CoatingCycles, MachineTreatmentCategory CoatingCategory) ReadAppliedMachineTreatments(this ModDataManager data, SObject machine)
    {
        var value = data.Read(machine, DataKeys.AppliedMachineTreatments);
        if (string.IsNullOrEmpty(value))
        {
            return (0, 0, MachineTreatmentCategory.None);
        }

        var split = value.Split(',');
        return (
            int.Parse(split[0]),
            int.Parse(split[1]),
            Enum.Parse<MachineTreatmentCategory>(split[2]));
    }

    /// <summary>Writes to the <paramref name="machine"/>'s treatments data the.</summary>
    /// <param name="data">The <see cref="ModDataManager"/>.</param>
    /// <param name="machine">The <see cref="SObject"/> machine instance.</param>
    /// <param name="value">The values to be written.</param>
    internal static void WriteAppliedMachineTreatments(this ModDataManager data, SObject machine, (int OverclockCycles, int CoatingCycles, MachineTreatmentCategory CoatingCategory) value)
    {
        data.Write(machine, DataKeys.AppliedMachineTreatments, $"{value.OverclockCycles},{value.CoatingCycles},{(int)value.CoatingCategory}");
    }
}
