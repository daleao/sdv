namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using System.Threading.Tasks;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LuremasterDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.EnumerateAllCrabPots(), crabPot =>
        {
            crabPot.ResetCatchAttempts();
            crabPot.UnblockAdditionalCatches();
        });
    }
}
