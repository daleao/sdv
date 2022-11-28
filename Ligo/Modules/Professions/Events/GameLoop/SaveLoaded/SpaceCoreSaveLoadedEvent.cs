namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
[AlwaysEnabledEvent]
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
        // revalidate levels
        SCSkill.Loaded.Values.ForEach(s => s.Revalidate());

        // load custom skills
        LuckSkillIntegration.LoadLuckSkill();
        SpaceCoreIntegration.LoadSpaceCoreSkills();
    }
}
