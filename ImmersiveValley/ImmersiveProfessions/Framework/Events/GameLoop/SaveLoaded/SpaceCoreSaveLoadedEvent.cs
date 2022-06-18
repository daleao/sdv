#nullable enable
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Integrations;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class SpaceCoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    internal SpaceCoreSaveLoadedEvent()
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore")) Enable();
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e)
    {
        // initialize reflected SpaceCore fields
        if (!ExtendedSpaceCoreAPI.Initialized) ExtendedSpaceCoreAPI.Init();

        // get custom luck skill
        if (ModEntry.LuckSkillApi is not null)
        {
            // initialize reflected SpaceCore fields
            if (!ExtendedSpaceCoreAPI.Initialized) ExtendedSpaceCoreAPI.Init();

            var luckSkill = new LuckSkill(ModEntry.LuckSkillApi);
            ModEntry.CustomSkills["spacechase0.LuckSkill"] = luckSkill;
            foreach (var profession in luckSkill.Professions)
                ModEntry.CustomProfessions[profession.Id] = (CustomProfession) profession;
        }

        // get remaining SpaceCore skills
        foreach (var skillId in ModEntry.SpaceCoreApi!.GetCustomSkills())
        {
            var customSkill = new CustomSkill(skillId, ModEntry.SpaceCoreApi);
            ModEntry.CustomSkills[skillId] = customSkill;
            foreach (var profession in customSkill.Professions)
                ModEntry.CustomProfessions[profession.Id] = (CustomProfession) profession;
        }

        // revalidate custom skill levels
        foreach (var skill in ModEntry.CustomSkills.Values)
        {
            var currentExp = skill.CurrentExp;
            if (currentExp > Experience.VANILLA_CAP_I)
                ModEntry.SpaceCoreApi.AddExperienceForCustomSkill(Game1.player, skill.StringId,
                    Experience.VANILLA_CAP_I - currentExp);
        }
    }
}