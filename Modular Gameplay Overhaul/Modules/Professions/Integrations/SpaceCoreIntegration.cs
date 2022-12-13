namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
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
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="ISpaceCoreApi"/>.</summary>
    internal static ISpaceCoreApi? Api { get; private set; }

    /// <summary>Instantiates and caches one instance of every <see cref="SCSkill"/>.</summary>
    internal static void LoadSpaceCoreSkills()
    {
        if (Api is null)
        {
            Log.W("SpaceCore skills could not be loaded.");
            return;
        }

        foreach (var skillId in Api.GetCustomSkills())
        {
            if (!SCSkill.Loaded.ContainsKey(skillId))
            {
                SCSkill.Loaded[skillId] = new SCSkill(skillId);
            }
        }
    }

    /// <summary>Caches the SpaceCore API.</summary>
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;
    }
}
