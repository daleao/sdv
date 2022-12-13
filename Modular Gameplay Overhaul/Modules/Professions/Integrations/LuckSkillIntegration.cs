namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

internal sealed class LuckSkillIntegration : BaseIntegration<ILuckSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LuckSkillIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal LuckSkillIntegration(IModRegistry modRegistry)
        : base("LuckSkill", "spacechase0.LuckSkill", "1.2.3", modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="ILuckSkillApi"/>.</summary>
    internal static ILuckSkillApi? Api { get; private set; }

    /// <summary>Instantiates and caches the <see cref="LuckSkill"/> instance.</summary>
    internal static void LoadLuckSkill()
    {
        if (SCSkill.Loaded.ContainsKey("spacechase0.LuckSkill"))
        {
            return;
        }

        Skill.Luck = new LuckSkill();
        SCSkill.Loaded["spacechase0.LuckSkill"] = Skill.Luck;
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;
    }
}
