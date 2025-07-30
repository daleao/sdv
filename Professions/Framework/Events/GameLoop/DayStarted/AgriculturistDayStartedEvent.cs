namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="AgriculturistDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class AgriculturistDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.game1.DoesAnyPlayerHaveProfession(Profession.Agriculturist);

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (Game1.dayOfMonth == 1)
        {
            Game1.game1.EnumerateAllHoeDirt().ForEach(dirt => Data.Write(dirt, DataKeys.SoilMemory, null));
        }
    }
}
