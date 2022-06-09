namespace DaLion.Stardew.Alchemy.Extensions;

#region using directives

using StardewValley;

using Framework;
using Framework.Enums;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Check whether this item is an alchemy ingredient and contains the specified <see cref="PrimarySubstance"/>.</summary>
    /// <param name="which">The <see cref="PrimarySubstance"/>.</param>
    /// <param name="density">Density of the substance in the ingredient.</param>
    public static bool ContainsPrimarySubstance(this Item item, PrimarySubstance which, out int density)
    {
        density = 0;
        if (!SubstanceManager.Ingredients.TryGetValue(item.Name, out var composition) || composition.Primary != which) return false;

        density = (int) composition.Density;
        return true;
    }

    /// <summary>Whether the item can be used as an alchemy ingredient.</summary>
    internal static bool IsValidIngredient(this Item item)
    {
        return SubstanceManager.Ingredients.ContainsKey(item.Name);
    }

    /// <summary>Whether the item can be used as an alchemical base.</summary>
    internal static bool IsAlchemicalBase(this Item item)
    {
        return SubstanceManager.Bases.ContainsKey(item.Name);
    }

    /// <summary>Whether the item can be used as an alchemical base of the specified type.</summary>
    /// <param name="type">The desired <see cref="BaseType"/>.</param>
    /// <param name="density">Density of the substance in the ingredient.</param>
    internal static bool IsAlchemicalBase(this Item item, BaseType type, out int purity)
    {
        purity = 0;
        if (!SubstanceManager.Bases.TryGetValue(item.Name, out var @base) || @base.Type != type) return false;

        purity = (int) @base.Purity;
        return true;
    }
}