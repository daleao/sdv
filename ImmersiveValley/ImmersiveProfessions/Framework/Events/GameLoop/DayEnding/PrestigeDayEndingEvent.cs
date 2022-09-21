namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeDayEndingEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <summary>Gets the current reset queue.</summary>
    private static Queue<ISkill> ToReset => ModEntry.State.SkillsToReset;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (ToReset.Count > 0)
        {
            var toReset = ToReset.Dequeue();
            switch (toReset)
            {
                case Skill skill:
                    Game1.player.ResetSkill(skill);
                    break;
                case CustomSkill customSkill:
                    Game1.player.ResetCustomSkill(customSkill);
                    break;
            }
        }

        this.Disable();
    }
}
