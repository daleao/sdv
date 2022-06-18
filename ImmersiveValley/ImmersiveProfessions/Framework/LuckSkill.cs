#nullable enable
namespace DaLion.Stardew.Professions.Framework;

#region using directives

using StardewValley;

using Common.Extensions.Reflection;
using Common.Integrations;

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
        DisplayName = (string) "LuckSkill.I18n".ToType().RequireMethod("Skill_Name").Invoke(null, null)!;

        //foreach (var (key, value) in api.GetProfessions())
        //{
        //    var vanillaId = key;
        //    var stringId = value.DefaultName;
        //    var displayName = value.Name;
        //    var description = value.Description;
        //    var level = vanillaId % 6 < 2 ? 5 : 10;
        //    Professions.Add(new CustomProfession(stringId, displayName, description, vanillaId, level, this));
        //}

        Professions.Add(new CustomProfession("LuckSkill.Fortunate", "Fortunate", "Better daily luck.", 30, 5, this));
        Professions.Add(new CustomProfession("LuckSkill.PopularHelper", "Popular Helper", "Daily quests occur three times as often.", 31, 5, this));
        Professions.Add(new CustomProfession("LuckSkill.Lucky", "Lucky", "20% chance for max daily luck.", 32, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.UnUnlucky", "Un-unlucky", "Never have bad luck.", 33, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.ShootingStar", "Shooting Star", "Nightly events occur twice as often.", 34, 10, this));
        Professions.Add(new CustomProfession("LuckSkill.SpiritChild", "Spirit Child", "Giving gifts makes junimos happy. They might help your farm.\n(15% chance for some form of farm advancement.)", 35, 10, this));

        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);
    }
}