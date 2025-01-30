namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    private static readonly HashSet<string> VanillaAnimalGoodIds =
    [
        QIDs.DinosaurEgg,
        QIDs.Mayonnaise,
        QIDs.DuckMayonnaise,
        QIDs.DinosaurMayonnaise,
        QIDs.VoidMayonnaise,
        QIDs.Cheese,
        QIDs.GoatCheese,
        QIDs.Cloth,
        QIDs.Wool,
    ];

    /// <summary>Determines whether <paramref name="object"/> is an artisan machine.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a machine that creates artisan goods, otherwise <see langword="false"/>.</returns>
    internal static bool IsArtisanMachine(this SObject @object)
    {
        return Lookups.ArtisanMachines.Contains(@object.QualifiedItemId);
    }

    /// <summary>Determines whether the <paramref name="object"/> is an animal produce or a derived artisan good.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is an animal produce or a derived artisan good, otherwise <see langword="false"/>.</returns>
    internal static bool IsAnimalOrDerivedGood(this SObject @object)
    {
        return @object.Category.IsAnyOf(SObject.EggCategory, SObject.MilkCategory, SObject.meatCategory, SObject.sellAtPierresAndMarnies) ||
            VanillaAnimalGoodIds.Contains(@object.QualifiedItemId) ||
            Lookups.AnimalDerivedGoods.Contains(@object.QualifiedItemId);
    }

    /// <summary>Determines whether <paramref name="object"/> is a resource node.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node containing precious resources, otherwise <see langword="false"/>.</returns>
    internal static bool IsResourceNode(this SObject @object)
    {
        return Lookups.ResourceNodeIds.Contains(@object.QualifiedItemId) || (ItemExtensionsIntegration.Instance?.IsLoaded == true && ItemExtensionsIntegration.Instance.ModApi.IsResource(@object.ItemId, out _, out _));
    }

    /// <summary>Determines whether the <paramref name="profession"/> should track <paramref name="object"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="profession">Either <see cref="Profession.Scavenger"/> or <see cref="Profession.Prospector"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> should be tracked by the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldBeTrackedBy(this SObject @object, Profession profession)
    {
        return (profession == Profession.Scavenger && ((@object.IsSpawnedObject && !@object.IsForagedMineral()) ||
                                                       @object.QualifiedItemId == QIDs.SpringOnion)) ||
               (profession == Profession.Prospector && ((@object.IsStone() && @object.IsResourceNode()) ||
                                                        @object.IsForagedMineral()));
    }

    /// <summary>Determines whether the owner of this <paramref name="object"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="object"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this SObject @object, IProfession profession, bool prestiged = false)
    {
        return @object.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of this <paramref name="object"/>---or any <see cref="Farmer"/> instance in the game session, if allowed by the module's settings---has the specified <paramref name="profession"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="object"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfessionOrLax(this SObject @object, IProfession profession, bool prestiged = false)
    {
        return @object.GetOwner().HasProfessionOrLax(profession, prestiged);
    }

    /// <summary>Checks whether the <paramref name="object"/> is owned by the specified <see cref="Farmer"/>, or if <see cref="ProfessionsConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/>'s owner value is equal to the unique ID of the specified <paramref name="farmer"/> or if <see cref="ProfessionsConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings, otherwise <see langword="false"/>.</returns>
    internal static bool IsOwnedByOrLax(this SObject @object, Farmer farmer)
    {
        return @object.IsOwnedBy(farmer) || Config.LaxOwnershipRequirements;
    }
}
