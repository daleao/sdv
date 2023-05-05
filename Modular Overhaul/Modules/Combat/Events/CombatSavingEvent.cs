namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        for (var i = 0; i < Game1.locations.Count; i++)
        {
            var location = Game1.locations[i];
            if (location is null)
            {
                continue;
            }

            location.characters.OnValueRemoved -= NpcExtensions.OnRemoved;
        }
    }
}
