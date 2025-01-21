namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using SpaceShared.APIs;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.27.0")]
[UsedImplicitly]
internal sealed class SpaceCoreIntegration()
    : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>(ModHelper.ModRegistry)
{
    /// <summary>Gets the SpaceCore API.</summary>>
    internal static ISpaceCoreApi Api => Instance!.ModApi!; // guaranteed not null by dependency

    /// <summary>Gets SpaceCore's internal list of unrealized new levels.</summary>
    /// <returns>A <see cref="List{T}"/> of <see cref="KeyValuePair"/>s with <see cref="SCSkill"/> ID <see cref="string"/> keys and <see cref="int"/> level values.</returns>
    internal List<KeyValuePair<string, int>> GetNewLevels()
    {
        return Reflector
            .GetStaticPropertyGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
            .Invoke();
    }

    /// <summary>Sets SpaceCore's internal list of unrealized new levels.</summary>
    /// <param name="newLevels">A list of skill ID and integer level pairs.</param>
    internal void SetNewLevels(List<KeyValuePair<string, int>> newLevels)
    {
        var perScreen = Reflector.GetStaticFieldGetter<object>(typeof(SCSkills), "_State").Invoke();
        var value = Reflector.GetUnboundPropertyGetter<object, object>(perScreen, "Value").Invoke(perScreen);
        Reflector
            .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(value.GetType(), "NewLevels")
            .Invoke(newLevels);
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.TryLoadSpaceCoreSkills();
        Log.D("Registered the SpaceCore integration.");
        return base.RegisterImpl();
    }

    /// <summary>Attempts to instantiate and cache one instance of every <see cref="CustomSkill"/>.</summary>
    /// <returns><see langword="true"/> if a new instance of <see cref="CustomSkill"/> was loaded, otherwise <see langword="false"/>.</returns>
    private bool TryLoadSpaceCoreSkills()
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

            CustomSkill.Initialize(skillId);
            anyLoaded = true;
        }

        return anyLoaded;
    }
}
