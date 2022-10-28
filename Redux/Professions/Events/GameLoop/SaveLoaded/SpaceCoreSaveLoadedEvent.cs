namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
[AlwaysEnabled]
internal sealed class SpaceCoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpaceCoreSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        // get custom luck skill
        if (Redux.Integrations.LuckSkillApi is not null)
        {
            var luckSkill = new LuckSkill(Redux.Integrations.LuckSkillApi);
            CustomSkill.Loaded["spacechase0.LuckSkill"] = luckSkill;
            foreach (var profession in luckSkill.Professions)
            {
                CustomProfession.LoadedProfessions[profession.Id] = (CustomProfession)profession;
            }
        }

        // get remaining SpaceCore skills
        foreach (var skillId in Redux.Integrations.SpaceCoreApi!.GetCustomSkills())
        {
            var customSkill = new CustomSkill(skillId);
            CustomSkill.Loaded[skillId] = customSkill;
            foreach (var profession in customSkill.Professions)
            {
                CustomProfession.LoadedProfessions[profession.Id] = (CustomProfession)profession;
            }
        }

        // revalidate levels
        CustomSkill.Loaded.Values.ForEach(s => s.Revalidate());
    }
}
