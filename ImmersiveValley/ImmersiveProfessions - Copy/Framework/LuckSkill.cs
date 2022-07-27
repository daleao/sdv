namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Extensions.Reflection;
using Common.Integrations.LuckSkill;
using StardewValley;

#endregion using directives

/// <summary>Represents spacechase0's implementation of the Luck skill.</summary>
/// <remarks>This is technically a vanilla skill and therefore does not use SpaceCore in its implementation despite being a mod-provided skill. As such, it stands in a murky place, as it is treated like a <see cref="CustomSkill"/> despite not being implemented as one.</remarks>
public sealed class LuckSkill : Skill
{
    private readonly ILuckSkillAPI? _api;

    /// <summary>Construct an instance.</summary>
    internal LuckSkill(ILuckSkillAPI? api) : base("Luck", Farmer.luckSkill)
    {
        if (api is null) return;

        _api = api;
        StringId = "spacechase0.LuckSkill";
        DisplayName = (string)"LuckSkill.I18n".ToType().RequireMethod("Skill_Name").Invoke(null, null)!;

        var fortunateName = (string)"LuckSkill.I18n".ToType().RequireMethod("Fortunate_Name").Invoke(null, null)!;
        var fortunateDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("Fortunate_Desc").Invoke(null, null)!;
        var popularHelpeName = (string)"LuckSkill.I18n".ToType().RequireMethod("PopularHelper_Name").Invoke(null, null)!;
        var popularHelpeDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("PopularHelper_Desc").Invoke(null, null)!;
        var luckyName = (string)"LuckSkill.I18n".ToType().RequireMethod("Lucky_Name").Invoke(null, null)!;
        var luckyDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("Lucky_Desc").Invoke(null, null)!;
        var unUnluckyName = (string)"LuckSkill.I18n".ToType().RequireMethod("UnUnlucky_Name").Invoke(null, null)!;
        var unUnluckyDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("UnUnlucky_Desc").Invoke(null, null)!;
        var shootingStarName = (string)"LuckSkill.I18n".ToType().RequireMethod("ShootingStar_Name").Invoke(null, null)!;
        var shootingStarDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("ShootingStar_Desc").Invoke(null, null)!;
        var spiritChildName = (string)"LuckSkill.I18n".ToType().RequireMethod("SpiritChild_Name").Invoke(null, null)!;
        var spiritChildDesc = (string)"LuckSkill.I18n".ToType().RequireMethod("SpiritChild_Desc").Invoke(null, null)!;

        Professions.Add(new CustomProfession("LuckSkill.Fortunate", fortunateName, fortunateDesc, 30, 5, this));
        Professions.Add(new CustomProfession("LuckSkill.PopularHelper", popularHelpeName, popularHelpeDesc, 31, 5, this));
        Professions.Add(new CustomProfession("LuckSkill.Lucky", luckyName, luckyDesc, 32, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.UnUnlucky", unUnluckyName, unUnluckyDesc, 33, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.ShootingStar", shootingStarName, shootingStarDesc, 34, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.SpiritChild", spiritChildName, spiritChildDesc, 35, 10, this));

        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);
    }
}