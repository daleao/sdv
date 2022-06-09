namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Integrations;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class PrestigeDayEndingEvent : DayEndingEvent
{
    private static Queue<SkillType> _ToReset => ModEntry.PlayerState.SkillsToReset;
    private static Queue<ICustomSkill> _ToReset_Custom => ModEntry.PlayerState.CustomSkillsToReset;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        while (_ToReset.Any()) Game1.player.ResetSkill(_ToReset.Dequeue());
        while (_ToReset_Custom.Any()) Game1.player.ResetCustomSkill(_ToReset_Custom.Dequeue());
        Disable();
    }
}