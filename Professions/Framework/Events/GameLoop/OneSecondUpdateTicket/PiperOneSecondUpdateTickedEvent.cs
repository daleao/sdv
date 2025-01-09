namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper) && Game1.player.hasBuff("13");

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        var player = Game1.player;
        player.health = Math.Min(player.health + (int)(player.maxHealth * 0.01f), player.maxHealth);
    }
}
