namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using SpaceShared.APIs;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <summary>Gets the SpaceCore API.</summary>>
    internal static ISpaceCoreApi Api => Instance!.ModApi!; // guaranteed not null by dependency

    /// <summary>Attempts to instantiate and cache one instance of every <see cref="CustomSkill"/>.</summary>
    /// <returns><see langword="true"/> if a new instance of <see cref="CustomSkill"/> was loaded, otherwise <see langword="false"/>.</returns>
    internal bool TryLoadSpaceCoreSkills()
    {
        this.AssertLoaded();

        var anyLoaded = false;
        foreach (var skillId in this.ModApi.GetCustomSkills())
        {
            // checking if the skill is loaded first avoids re-instantiating the skill
            if (CustomSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            CustomSkill.Loaded[skillId] = new CustomSkill(skillId);
            Log.D($"Successfully loaded the custom skill {skillId}.");
            anyLoaded = true;
        }

        return anyLoaded;
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.TryLoadSpaceCoreSkills();
        Log.D("Registered the SpaceCore integration.");
        return base.RegisterImpl();
    }
}
