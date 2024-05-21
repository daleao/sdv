namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="StatueDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class StatueDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.SkillsToReset.Count > 0;

    /// <summary>Gets the current reset queue.</summary>
    private static Queue<ISkill> ToReset => State.SkillsToReset;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (ToReset.Count > 0)
        {
            var toReset = ToReset.Dequeue();
            toReset.Reset();
            Log.D($"[Masteries]: {Game1.player.Name}'s {toReset.DisplayName} skill has been reset.");
        }
    }
}
