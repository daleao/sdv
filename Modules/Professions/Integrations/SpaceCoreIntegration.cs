namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <summary>Instantiates and caches one instance of every <see cref="CustomSkill"/>.</summary>
    internal void LoadSpaceCoreSkills()
    {
        this.AssertLoaded();
        foreach (var skillId in this.ModApi.GetCustomSkills())
        {
            // checking if the skill is loaded first avoids re-instantiating the skill
            if (CustomSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            CustomSkill.Loaded[skillId] = new CustomSkill(skillId);
            Log.D($"[PRFS]: Successfully loaded the custom skill {skillId}.");
        }
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.LoadSpaceCoreSkills();
        Log.D("[PRFS]: Registered the SpaceCore integration.");
        return base.RegisterImpl();
    }
}
