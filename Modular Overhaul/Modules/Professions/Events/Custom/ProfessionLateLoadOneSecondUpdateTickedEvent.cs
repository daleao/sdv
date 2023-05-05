namespace DaLion.Overhaul.Modules.Professions.Events.Custom;

#region using directives

using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionLateLoadOneSecondUpdateTickedEvent : SecondSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionLateLoadOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionLateLoadOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSecondSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // we register integration for custom skills on the 2nd second update tick because Love of Cooking registers on the 1st
        IModIntegration? @interface = SpaceCoreIntegration.Instance;
        @interface!.Register();

        @interface = LuckSkillIntegration.Instance;
        @interface?.Register();

        // revalidate levels
        SCSkill.Loaded.Values.ForEach(s => s.Revalidate());

        this.Manager.Unmanage(this);
    }
}
