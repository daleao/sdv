namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Stardew;
using Framework;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
public static class BuildingExtensions
{
    /// <summary>Whether the owner of this instance has the specified profession.</summary>
    /// <param name="index">A valid profession index.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <remarks>This extension is only called by emitted ILCode, so we use a simpler <see cref="int"/> interface instead of the standard <see cref="Profession"/>.</remarks>>
    public static bool DoesOwnerHaveProfession(this Building building, int index, bool prestiged = false) =>
        Profession.TryFromValue(index, out var profession) && building.GetOwner().HasProfession(profession, prestiged);
}