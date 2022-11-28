namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.Get_HasSkillsToReset();

    /// <summary>Gets the current reset queue.</summary>
    private static Queue<ISkill> ToReset => Game1.player.Get_SkillsToReset();

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (ToReset.Count > 0)
        {
            var toReset = ToReset.Dequeue();
            toReset.Reset();
            Log.D($"{Game1.player.Name}'s {toReset.DisplayName} skill has been reset.");
        }
    }
}
