namespace DaLion.Overhaul.Modules.Professions.Events.Custom;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionLateLoadSecondUpdateTickedEvent : SecondSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionLateLoadSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionLateLoadSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSecondSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!SpaceCoreIntegration.Instance!.IsRegistered)
        {
            Log.E("[PRFS]: The SpaceCore integration was not registered.");
            return;
        }

        Log.D("[PRFS]: Doing second pass of custom skills...");
        // this is required because because Love of Cooking only registers to SpaceCore on the FirstSecondUpdateTicked
        SpaceCoreIntegration.Instance.LoadSpaceCoreSkills();
        GenericModConfigMenu.Instance?.Reload();
        this.Manager.Unmanage(this);
    }
}
