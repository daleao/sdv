namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using Common;
using Extensions;
using SpaceCore;
using StardewValley;
using System.Collections.Generic;

#endregion using directives

public class AlchemySkill : Skills.Skill
{
    internal static string InternalName { get; } = ModEntry.Manifest.UniqueID + ".Skill";

    public AlchemySkill()
        : base(InternalName)
    {
        // initialize skill data
        Log.D("Registering Alchemy skill...");

    }

    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("skill.name");
    }

    public override List<string> GetExtraLevelUpInfo(int level)
    {
        return new();
    }

    public override string GetSkillPageHoverText(int level)
    {
        return base.GetSkillPageHoverText(level);
    }

    public void AddExperienceDirectly(int howMuch)
    {
        Game1.player.AddAlchemyExperience(howMuch);
    }

    public int GetLevel(Farmer farmer)
    {
        return Skills.GetSkillLevel(farmer, GetName());
    }
}