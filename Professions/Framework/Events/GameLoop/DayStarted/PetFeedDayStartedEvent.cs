namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Characters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PetFeedDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class PetFeedDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Utility.ForEachLocation(location =>
        {
            foreach (var character in location.characters)
            {
                if (character is Pet pet)
                {
                    Data.Write(pet, DataKeys.WasSupplementedToday, "false".ToString());
                }
            }

            return true;
        });
    }
}
