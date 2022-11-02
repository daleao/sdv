namespace DaLion.Redux.Framework.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether <paramref name="obj"/> is an artisan machine.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a machine that creates artisan goods, otherwise <see langword="false"/>.</returns>
    internal static bool IsArtisanMachine(this SObject obj)
    {
        return Collections.ArtisanMachines.Contains(obj.name);
    }

    /// <summary>Determines whether <paramref name="object"/> is a resource node.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node containing precious resources, otherwise <see langword="false"/>.</returns>
    internal static bool IsResourceNode(this SObject @object)
    {
        return Collections.ResourceNodeIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Determines whether the <paramref name="profession"/> should track <paramref name="obj"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="profession">Either <see cref="Profession.Scavenger"/> or <see cref="Profession.Prospector"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> should be tracked by the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldBeTrackedBy(this SObject obj, Profession profession)
    {
        return (profession == Profession.Scavenger && ((obj.IsSpawnedObject && !obj.IsForagedMineral()) ||
                                                       obj.IsSpringOnion() || obj.IsArtifactSpot())) ||
               (profession == Profession.Prospector && ((obj.IsStone() && obj.IsResourceNode()) ||
                                                        obj.IsForagedMineral() || obj.IsArtifactSpot()));
    }

    /// <summary>Determines whether the owner of this <paramref name="obj"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="obj"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this SObject obj, IProfession profession, bool prestiged = false)
    {
        return obj.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of the <paramref name="obj"/> has the <see cref="Profession"/> corresponding to <paramref name="index"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="index">A valid profession index.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the owner of <paramref name="obj"/> the <see cref="Profession"/> with the specified <paramref name="index"/>, otherwise <see langword="false"/>.</returns>
    /// <remarks>This overload exists only to be called by emitted ILCode. Expects a vanilla <see cref="Profession"/>.</remarks>
    internal static bool DoesOwnerHaveProfession(this SObject obj, int index, bool prestiged = false)
    {
        return Profession.TryFromValue(index, out var profession) &&
               obj.GetOwner().HasProfession(profession, prestiged);
    }
}
