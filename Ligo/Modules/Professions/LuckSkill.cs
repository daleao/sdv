namespace DaLion.Ligo.Modules.Professions;

#region using directives

using DaLion.Shared.Extensions.Reflection;

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
    internal LuckSkill()
        : base("Luck", Farmer.luckSkill)
    {
        this.StringId = "spacechase0.LuckSkill";

        var i18n = "LuckSkill.I18n".ToType();
        this.DisplayName = Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Skill_Name").Invoke();
        this.Professions.Add(new SCProfession(
            30,
            "LuckSkill.Fortunate",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Fortunate_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Fortunate_Desc"),
            5,
            this));
        this.Professions.Add(new SCProfession(
            31,
            "LuckSkill.PopularHelper",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "PopularHelper_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "PopularHelper_Desc"),
            5,
            this));
        this.Professions.Add(new SCProfession(
            32,
            "LuckSkill.Lucky",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Lucky_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "Lucky_Desc"),
            10,
            this));
        this.Professions.Add(new SCProfession(
            33,
            "LuckSkill.UnUnlucky",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "UnUnlucky_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "UnUnlucky_Desc"),
            10,
            this));
        this.Professions.Add(new SCProfession(
            34,
            "LuckSkill.ShootingStar",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "ShootingStar_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "ShootingStar_Desc"),
            10,
            this));
        this.Professions.Add(new SCProfession(
            35,
            "LuckSkill.SpiritChild",
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "SpiritChild_Name"),
            Reflector.GetStaticMethodDelegate<Func<string>>(i18n, "SpiritChild_Desc"),
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
        if (Ligo.Integrations.LuckSkillApi is null)
        {
            this.Reset();
            return;
        }

        base.Revalidate();
    }
}
