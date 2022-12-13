namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Spelunker);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            Game1.player.Increment_SpelunkerLadderStreak();
            if (e.Player.HasProfession(Profession.Spelunker, true))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int)(player.maxHealth * 0.025f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + (player.MaxStamina * 0.01f), player.MaxStamina);
            }

            this.Manager.Enable<SpelunkerUpdateTickedEvent>();
        }
        else if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            Game1.player.Set_SpelunkerLadderStreak(0);
            this.Manager.Disable<SpelunkerUpdateTickedEvent>();
        }
    }
}
