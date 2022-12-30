namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class LateLoadOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LateLoadOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LateLoadOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // hard dependency
        // we load all custom skills on the 2nd second update tick because Love of Cooking registers on the 1st
        SpaceCoreIntegration.Instance!.LoadSpaceCoreSkills();

        // soft dependency
        LuckSkillIntegration.Instance?.LoadLuckSkill();

        // revalidate levels
        SCSkill.Loaded.Values.ForEach(s => s.Revalidate());

        this.Dispose();
    }
}
