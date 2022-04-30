namespace DaLion.Stardew.Tools.Extensions;

#region using directives

using StardewValley.Objects;
using SObject = StardewValley.Object;

#endregion using directives

public static class SObjectExtensions
{
    /// <summary>Get whether a given object is a twig.</summary>
    public static bool IsTwig(this SObject obj)
    {
        return obj?.ParentSheetIndex is 294 or 295;
    }

    /// <summary>Get whether a given object is a stone.</summary>
    public static bool IsStone(this SObject obj)
    {
        return obj?.Name == "Stone";
    }

    /// <summary>Get whether a given object is a weed.</summary>
    public static bool IsWeed(this SObject obj)
    {
        return obj is not Chest && obj?.Name == "Weeds";
    }
}