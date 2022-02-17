namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

internal class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        // countdown contact timer
        if (ModEntry.State.Value.SlimeContactTimer > 0 && Game1.game1.IsActive && Game1.shouldTimePass())
            --ModEntry.State.Value.SlimeContactTimer;

        // countdown key press accumulator
        if (ModEntry.State.Value.KeyPressAccumulator == 1 && e.IsMultipleOf(40)) --ModEntry.State.Value.KeyPressAccumulator;
    }
}