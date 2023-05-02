namespace DaLion.Alchemy.Framework;

#region using directives

using System.Collections.Generic;
using SpaceCore;

#endregion using directives

/// <summary>The custom Alchemy skill.</summary>
public class AlchemySkill : Skills.Skill
{
    /// <summary>Initializes a new instance of the <see cref="AlchemySkill"/> class.</summary>
    public AlchemySkill()
        : base(InternalName)
    {
        // initialize skill data
        Log.D("Registering Alchemy skill...");

    }

    internal static string InternalName { get; } = ModEntry.Manifest.UniqueID + ".Skill";

    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("skill.name");
    }

    /// <inheritdoc />
    public override List<string> GetExtraLevelUpInfo(int level)
    {
        return new();
    }

    /// <inheritdoc />
    public override string GetSkillPageHoverText(int level)
    {
        return base.GetSkillPageHoverText(level);
    }
}
