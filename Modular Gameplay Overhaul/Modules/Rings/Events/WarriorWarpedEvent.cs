namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
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
        if (Game1.player.Get_WarriorKillCount() <= 0)
        {
            this.Disable();
            return;
        }

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            return;
        }

        Game1.player.Set_WarriorKillCount(0);
        this.Disable();
    }
}
