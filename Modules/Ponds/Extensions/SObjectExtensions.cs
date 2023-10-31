namespace DaLion.Overhaul.Modules.Ponds.Extensions;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="object"/> is a legendary fish.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a legendary fish, otherwise <see langword="false"/>.</returns>
    public static bool IsLegendaryFish(this SObject @object)
    {
        return Lookups.LegendaryFishes.Contains(@object.Name) || @object.HasContextTag("fish_legendary");
    }

    /// <summary>Determines whether the <paramref name="object"/> is a radioactive fish.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactiveFish(this SObject @object)
    {
        return @object.Category == SObject.FishCategory && @object.Name.ContainsAnyOf("Mutant", "Radioactive");
    }

    /// <summary>Determines whether the <paramref name="object"/> is a non-radioactive ore or ingot.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is either copper, iron, gold or iridium, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeEnriched(this SObject @object)
    {
        return @object.ParentSheetIndex is ObjectIds.CopperOre or ObjectIds.IronOre or ObjectIds.GoldOre or ObjectIds.IridiumOre
            or ObjectIds.CopperBar or ObjectIds.IronBar or ObjectIds.GoldBar or ObjectIds.IridiumBar;
    }
}
