namespace DaLion.Overhaul.Modules.Ponds.Extensions;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is a radioactive fish.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactiveFish(this SObject obj)
    {
        return obj.Category == SObject.FishCategory && obj.Name.ContainsAnyOf("Mutant", "Radioactive");
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a non-radioactive ore or ingot.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is either copper, iron, gold or iridium, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeEnriched(this SObject obj)
    {
        return obj.ParentSheetIndex is ObjectIds.CopperOre or ObjectIds.IronOre or ObjectIds.GoldOre or ObjectIds.IridiumOre
            or ObjectIds.CopperBar or ObjectIds.IronBar or ObjectIds.GoldBar or ObjectIds.IridiumBar;
    }
}
