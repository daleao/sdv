namespace DaLion.Chargeable.Framework.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether <paramref name="object"/> is a simple Stone.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    public static bool IsStone(this SObject @object)
    {
        return @object.Name == "Stone";
    }

    /// <summary>Determines whether the <paramref name="object"/> is a twig.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is twig, otherwise <see langword="false"/>.</returns>
    public static bool IsTwig(this SObject @object)
    {
        return @object.ParentSheetIndex is 294 or 295;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a weed.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is weed, otherwise <see langword="false"/>.</returns>
    public static bool IsWeed(this SObject @object)
    {
        return @object is not Chest && @object.Name == "Weeds";
    }
}
