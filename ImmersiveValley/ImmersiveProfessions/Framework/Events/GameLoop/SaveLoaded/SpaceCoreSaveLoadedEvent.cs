namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SpaceCoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SpaceCoreSaveLoadedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore")) AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        // get custom luck skill
        if (ModEntry.LuckSkillApi is not null)
        {
            var luckSkill = new LuckSkill(ModEntry.LuckSkillApi);
            CustomSkill.LoadedSkills["spacechase0.LuckSkill"] = luckSkill;
            foreach (var profession in luckSkill.Professions)
                CustomProfession.LoadedProfessions[profession.Id] = (CustomProfession)profession;
        }

        // get remaining SpaceCore skills
        foreach (var skillId in ModEntry.SpaceCoreApi!.GetCustomSkills())
        {
            var customSkill = new CustomSkill(skillId, ModEntry.SpaceCoreApi);
            CustomSkill.LoadedSkills[skillId] = customSkill;
            foreach (var profession in customSkill.Professions)
                CustomProfession.LoadedProfessions[profession.Id] = (CustomProfession)profession;
        }

        // revalidate custom skill levels
        foreach (var skill in CustomSkill.LoadedSkills.Values)
        {
            var currentExp = skill.CurrentExp;
            if (currentExp > ISkill.VANILLA_EXP_CAP_I)
                ModEntry.SpaceCoreApi.AddExperienceForCustomSkill(Game1.player, skill.StringId,
                    ISkill.VANILLA_EXP_CAP_I - currentExp);
        }
    }
}