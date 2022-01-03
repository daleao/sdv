using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Enums;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds common methods and properties related to prestige mechanics.</summary>
public static class Prestige
{
    /// <summary>Get the cost of resetting the specified skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static int GetResetCost(SkillType skillType)
    {
        var multiplier = ModEntry.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f) return 0;

        var count = Game1.player.NumberOfProfessionsInSkill((int) skillType, true);
#pragma warning disable 8509
        var baseCost = count switch
#pragma warning restore 8509
        {
            1 => 10000,
            2 => 50000,
            3 => 100000
        };

        return (int) (baseCost * multiplier);
    }
}