namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Objects;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HopperDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class HopperDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Utility.ForEachLocation(location =>
        {
            foreach (var chest in location.Objects.Values.OfType<Chest>())
            {
                if (chest.SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
                {
                    chest.CheckAutoLoad(chest.GetOwner());
                }
            }

            return true; // continue enumeration
        });
    }
}
