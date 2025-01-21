namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HealingSlimeOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class HealingSlimeOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => FarmersInRange.Any();

    internal static Dictionary<Farmer, int> FarmersInRange { get; } = [];

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        foreach (var (farmer, counter) in FarmersInRange)
        {
            if (counter % 5 == 0)
            {
                farmer.health = Math.Min(farmer.health + 5, farmer.maxHealth);
                farmer.Stamina = Math.Min(farmer.Stamina + 5, farmer.MaxStamina);
                farmer.currentLocation.debris.Add(new Debris(
                    5,
                    new Vector2(farmer.StandingPixel.X, farmer.StandingPixel.Y),
                    Color.Lime,
                    1f,
                    farmer));
            }

            FarmersInRange[farmer] += 1;
        }

        this.Disable();
    }
}
