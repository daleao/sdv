namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.Saved;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSavedEvent : SavedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSavedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
    {
        CombatModule.ConvertAllStabbingSwords();
    }
}
