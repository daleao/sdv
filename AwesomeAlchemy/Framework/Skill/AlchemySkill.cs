using System.Collections.Generic;
using DaLion.Stardew.Alchemy.Framework.Extensions;
using SpaceCore;
using StardewValley;

namespace DaLion.Stardew.Alchemy.Framework.Skill;

public class AlchemySkill : SpaceCore.Skills.Skill
{
    internal static readonly string InternalName { get; } = ModEntry.Manifest.UniqueID + ".AlchemySkill";

    public AlchemySkill()
        : base(_internalName)
    {
        // initialize skill data
        Log.D("Registering Alchemy skill...");

    }

    /// <inheritdoc />
    public override string GetName()
    {
        return string.Empty;
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

    public int GetLevel()
    {
        return Skills.GetSkillLevel();
    }
}