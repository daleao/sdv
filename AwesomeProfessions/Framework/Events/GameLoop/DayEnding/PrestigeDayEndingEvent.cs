using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

internal class PrestigeDayEndingEvent : DayEndingEvent
{
    public Queue<SkillType> SkillsToReset { get; } = new();

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        while (SkillsToReset.Any()) Game1.player.ResetSkill(SkillsToReset.Dequeue());
        ModEntry.State.Value.UsedDogStatueToday = false;
        Disable();
    }
}