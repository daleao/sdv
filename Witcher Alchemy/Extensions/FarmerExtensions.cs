namespace DaLion.Alchemy.Extensions;

#region using directives

using System.Linq;
using Framework;
using Enums;
using SpaceCore;
using StardewValley;
using DaLion.Stardew.Alchemy.Extensions;

#endregion using directives

public static class FarmerExtensions
{
    /// <summary>Adds experience to the <paramref name="farmer"/>'s alchemy skill.</summary>
    /// <param name="farmer">Th <see cref="Farmer"/>.</param>
    /// <param name="amount">The amount of experience points to add.</param>
    public static void AddAlchemyExperience(this Farmer farmer, int amount)
    {
        Skills.AddExperience(farmer, AlchemySkill.InternalName, amount);
    }

    /// <summary>Gets the the <paramref name="farmer"/>'s alchemy skill level.</summary>
    /// <param name="farmer">Th <see cref="Farmer"/>.</param>
    /// <returns>The alchemy skill level.</returns>
    public static int GetAlchemyLevel(this Farmer farmer)
    {
        return Skills.GetSkillLevel(farmer, AlchemySkill.InternalName);
    }

    /// <summary>Gets the total alchemy experience earned by the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">Th <see cref="Farmer"/>.</param>
    /// <returns>The total alchemy experience.</returns>
    public static int GetTotalAlchemyExperience(this Farmer farmer)
    {
        return Skills.GetExperienceFor(farmer, AlchemySkill.InternalName);
    }

    /// <summary>Checks whether the <paramref name="farmer"/> has enough of the specified <see cref="BaseType"/> in their inventory.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="base">The <see cref="BaseType"/>.</param>
    /// 
    /// <returns></returns>
    public static bool HasEnoughBaseInInventory(this Farmer farmer, BaseType @base)
    {
        return farmer.Items.Any(item => item.IsAlchemicalBase(@base, out var purity) && item.Stack* purity >= 4);
    }

    public static bool HasEnoughSubstanceInInventory(this Farmer farmer, PrimarySubstance substance, int amount)
    {
        return farmer.Items.Any(item => item.ContainsPrimarySubstance(substance, out var density) && item.Stack * density >= amount);
    }
}
