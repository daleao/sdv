namespace DaLion.Ligo.Modules.Core.Events.Player;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class OutOfCombatWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OutOfCombatWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal OutOfCombatWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
        else
        {
            this.Manager.Disable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
    }
}
