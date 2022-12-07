namespace DaLion.Stardew.Alchemy.Extensions;

#region using directives

using Framework;
using Framework.Enums;
using SpaceCore;
using StardewValley;
using System.Linq;

#endregion using directives

public static class FarmerExtensions
{
    public static void AddAlchemyExperience(this Farmer farmer, int howMuch)
    {
        Skills.AddExperience(farmer, AlchemySkill.InternalName, howMuch);
    }

    public static int GetAlchemyLevel(this Farmer farmer) =>
        Skills.GetSkillLevel(farmer, AlchemySkill.InternalName);

    public static int GetTotalCurrentAlchemyExperience(this Farmer farmer) =>
        Skills.GetExperienceFor(farmer, AlchemySkill.InternalName);

    public static bool HasEnoughSubstanceInInventory(this Farmer farmer, PrimarySubstance substance, int amount) =>
        farmer.Items.Any(item => item.ContainsPrimarySubstance(substance, out var density) && item.Stack * density >= amount);

    public static bool HasEnoughBaseInInventory(this Farmer farmer, BaseType type) =>
        farmer.Items.Any(item => item.IsAlchemicalBase(type, out var purity) && item.Stack * purity >= 4);
}