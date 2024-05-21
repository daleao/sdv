namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LateLoadSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LateLoadSecondUpdateTickedEvent(EventManager? manager = null)
    : SecondSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSecondSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // we register integrations late because Love of Cooking (and therefore possibly others)
        // may themselves register to SpaceCore on FirstSecondUpdateTicked
        Log.D("Doing first pass load of custom skills...");
        SpaceCoreIntegration.Instance!.Register();
        if (ProfessionsConfigMenu.Instance?.IsLoaded == true)
        {
            ProfessionsConfigMenu.Instance.Register();
        }
    }
}
