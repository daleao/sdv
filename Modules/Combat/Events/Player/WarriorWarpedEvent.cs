namespace DaLion.Overhaul.Modules.Combat.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WarriorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WarriorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WarriorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (CombatModule.State.WarriorKillCount <= 0)
        {
            this.Disable();
            return;
        }

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            return;
        }

        CombatModule.State.WarriorKillCount = 0;
        this.Disable();
    }
}
