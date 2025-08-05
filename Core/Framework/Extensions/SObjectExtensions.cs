namespace DaLion.Core.Framework.Extensions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Checks if the <paramref name="object"/> instance is a hopper chest, and if so returns a <see cref="Chest"/> reference to the instance.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="hopper">A <see cref="Chest"/> reference to the <paramref name="object"/>, if applicable. Otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the instance is a hopper chest, otherwise <see langword="false"/>.</returns>
    internal static bool TryGetHopper(this SObject @object,  [NotNullWhen(true)] out Chest? hopper)
    {
        if (@object is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } chest)
        {
            hopper = chest;
            return true;
        }

        hopper = null;
        return false;
    }
}
