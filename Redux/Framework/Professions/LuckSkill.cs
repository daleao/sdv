namespace DaLion.Redux.Framework.Professions;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

/// <summary>Represents spacechase0's implementation of the Luck skill.</summary>
/// <remarks>
///     This is technically a vanilla skill and therefore does not use SpaceCore in its implementation despite being a
///     mod-provided skill. As such, it stands in a murky place, as it is treated like a <see cref="SCSkill"/> despite
///     not being implemented as one.
/// </remarks>
public sealed class LuckSkill : Skill
{
    /// <summary>Initializes a new instance of the <see cref="LuckSkill"/> class.</summary>
    /// <param name="api">The <see cref="ILuckSkillApi"/>.</param>
    internal LuckSkill(ILuckSkillApi? api)
        : base("Luck", Farmer.luckSkill)
    {
        if (api is null)
        {
            return;
        }

        this.StringId = "spacechase0.LuckSkill";

        var i18n = "LuckSkill.I18n".ToType();
        this.DisplayName = ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Skill_Name").Invoke();
        this.Professions.Add(new SCProfession(
            30,
            "LuckSkill.Fortunate",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Fortunate_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Fortunate_Desc"),
            null,
            5,
            this));
        this.Professions.Add(new SCProfession(
            31,
            "LuckSkill.PopularHelper",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "PopularHelper_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "PopularHelper_Desc"),
            null,
            5,
            this));
        this.Professions.Add(new SCProfession(
            32,
            "LuckSkill.Lucky",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Lucky_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Lucky_Desc"),
            null,
            10,
            this));
        this.Professions.Add(new SCProfession(
            33,
            "LuckSkill.UnUnlucky",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "UnUnlucky_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "UnUnlucky_Desc"),
            null,
            10,
            this));
        this.Professions.Add(new SCProfession(
            34,
            "LuckSkill.ShootingStar",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "ShootingStar_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "ShootingStar_Desc"),
            null,
            10,
            this));
        this.Professions.Add(new SCProfession(
            35,
            "LuckSkill.SpiritChild",
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "SpiritChild_Name"),
            ModEntry.Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "SpiritChild_Desc"),
            null,
            10,
            this));

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);
    }

    /// <inheritdoc />
    public override int MaxLevel => 10;

    /// <inheritdoc />
    public override void Revalidate()
    {
        if (Framework.Integrations.LuckSkillApi is null)
        {
            this.Reset();
            return;
        }

        base.Revalidate();
    }
}
