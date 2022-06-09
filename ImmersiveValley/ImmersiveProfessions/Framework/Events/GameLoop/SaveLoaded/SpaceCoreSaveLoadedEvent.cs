#nullable enable
namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Integrations;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class SpaceCoreSaveLoadedEvent : SaveLoadedEvent
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
        SpaceCoreIntegration.InitializeReflectedFields();

        // get custom skills
        foreach (var skillId in ModEntry.SpaceCoreApi!.GetCustomSkills())
            ModEntry.CustomSkills.Add(new CustomSkill(skillId));

        // revalidate custom skill levels
        foreach (var skill in ModEntry.CustomSkills)
        {
            var currentExp = skill.CurrentExp;
            if (currentExp > Experience.VANILLA_CAP_I)
                ModEntry.SpaceCoreApi.AddExperienceForCustomSkill(Game1.player, skill.StringId,
                    Experience.VANILLA_CAP_I - currentExp);
        }
    }
}