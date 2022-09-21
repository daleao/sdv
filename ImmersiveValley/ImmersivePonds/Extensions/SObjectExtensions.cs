namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using DaLion.Common.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is algae or seaweed.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgae(this SObject obj)
    {
        return obj.ParentSheetIndex is Constants.SeaweedIndex or Constants.GreenAlgaeIndex
            or Constants.WhiteAlgaeIndex;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a non-radioactive metallic ore.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is either copper, iron, gold or iridium ore, otherwise <see langword="false"/>.</returns>
    public static bool IsNonRadioactiveOre(this SObject obj)
    {
        return obj.ParentSheetIndex is 378 or 380 or 384 or 386;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a non-radioactive metal ingot.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is either copper, iron, gold or iridium ore, otherwise <see langword="false"/>.</returns>
    public static bool IsNonRadioactiveIngot(this SObject obj)
    {
        return obj.ParentSheetIndex is 334 or 335 or 336 or 337;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a radioactive fish.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    public static bool IsRadioactiveFish(this SObject obj)
    {
        return obj.Category == SObject.FishCategory && obj.Name.ContainsAnyOf("Mutant", "Radioactive");
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a legendary fish.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> has the legendary fish context tag, otherwise <see langword="false"/>.</returns>
    public static bool IsLegendary(this SObject obj)
    {
        return obj.HasContextTag("fish_legendary");
    }
}
