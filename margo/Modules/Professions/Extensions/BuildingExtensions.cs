namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
internal static class BuildingExtensions
{
    /// <summary>Determines whether the owner of the <paramref name="building"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="building"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this Building building, IProfession profession, bool prestiged = false)
    {
        return building.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of the <paramref name="building"/> has the <see cref="VanillaProfession"/> corresponding to <paramref name="index"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="index">A valid profession index.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the owner of <paramref name="building"/> the <see cref="VanillaProfession"/> with the specified <paramref name="index"/>, otherwise <see langword="false"/>.</returns>
    /// <remarks>This overload exists only to be called by emitted ILCode. Only accepts <see cref="VanillaProfession"/> indices.</remarks>
    internal static bool DoesOwnerHaveProfession(this Building building, int index, bool prestiged = false)
    {
        return Profession.TryFromValue(index, out var profession) &&
               building.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Checks whether the <paramref name="building"/> is owned by the specified <see cref="Farmer"/>, or if <see cref="ProfessionConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="building"/>'s owner value is equal to the unique ID of the specified <paramref name="farmer"/> or if <see cref="ProfessionConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings, otherwise <see langword="false"/>.</returns>
    internal static bool IsOwnedByOrLax(this Building building, Farmer farmer)
    {
        return building.IsOwnedBy(farmer) || ProfessionsModule.Config.LaxOwnershipRequirements;
    }
}
