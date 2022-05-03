namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class PrestigeDayEndingEvent : DayEndingEvent
{
    public PerScreen<Queue<SkillType>> SkillsToReset { get; } = new(() => new());

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        while (SkillsToReset.Value.Any()) Game1.player.ResetSkill(SkillsToReset.Value.Dequeue());
        this.Disable();
    }
}