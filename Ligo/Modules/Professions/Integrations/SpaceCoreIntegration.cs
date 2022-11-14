namespace DaLion.Ligo.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry)
    {
    }

    /// <summary>Instantiates and caches one instance of every <see cref="SCSkill"/>.</summary>
    internal static void LoadSpaceCoreSkills()
    {
        foreach (var skillId in Ligo.Integrations.SpaceCoreApi!.GetCustomSkills())
        {
            if (SCSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            var customSkill = new SCSkill(skillId);
            SCSkill.Loaded[skillId] = customSkill;
            foreach (var profession in customSkill.Professions)
            {
                SCProfession.Loaded[profession.Id] = (SCProfession)profession;
            }
        }
    }

    /// <summary>Caches the SpaceCore API.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.SpaceCoreApi = this.ModApi;
    }
}
