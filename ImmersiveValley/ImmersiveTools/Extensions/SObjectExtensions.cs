namespace DaLion.Stardew.Tools.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="obj"/> is a twig.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is twig, otherwise <see langword="false"/>.</returns>
    public static bool IsTwig(this SObject obj)
    {
        return obj.ParentSheetIndex is 294 or 295;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a stone.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    public static bool IsStone(this SObject obj)
    {
        return obj?.Name == "Stone";
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a weed.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is weed, otherwise <see langword="false"/>.</returns>
    public static bool IsWeed(this SObject obj)
    {
        return obj is not Chest && obj?.Name == "Weeds";
    }
}
