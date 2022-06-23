namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley;

using Enums;
using Extensions;

#endregion using directives

public class Formula : CraftingRecipe
{
    /// <summary>The integer coefficients that represent the amount of each primary substance required to mix this formula.</summary>
    public int[] Coefficients { get; }

    /// <summary>The <see cref="BaseType"/> required to mix this formula.</summary>
    public BaseType RequiredBase { get;}

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The name of the generated item.</param>
    /// <param name="coefficients">The integer coefficients that represent the amount of each primary substance required to mix this formula.</param>
    /// <param name="requiredBase">The <see cref="BaseType"/> required to mix this formula.</param>
    public Formula(string name, int[] coefficients, BaseType requiredBase)
    : base(name)
    {
        Coefficients = coefficients;
        RequiredBase = requiredBase;
    }

    /// <summary>Whether the local player has all necessary ingredients in their personal inventory to mix this formula.</summary>
    public bool CanMix()
    {
        if (!Game1.player.HasEnoughBaseInInventory(RequiredBase)) return false;

        for (var i = 0; i < 6; ++i)
            if (!Game1.player.HasEnoughSubstanceInInventory((PrimarySubstance) i, Coefficients[i]))
                return false;

        return true;
    }

    /// <summary>Whether the local player has access to all necessary ingredients to mix this formula.</summary>
    /// <param name="availableIngredients">A list of alchemical ingredients available to the local player.</param>
    public bool CanMix(List<Item> availableIngredients)
    {
        if (!Game1.player.HasEnoughBaseInInventory(RequiredBase)) return false;

        for (var i = 0; i < 6; ++i)
            if (!availableIngredients.Any(
                    item => item.ContainsPrimarySubstance((PrimarySubstance) i, out var density) &&
                            item.Stack * density >= Coefficients[i]))
                return false;

        return true;
    }
}