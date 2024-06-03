namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Core;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="VampiricUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class VampiricUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.health > Game1.player.maxHealth;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.health <= player.maxHealth)
        {
            this.Disable();
            return;
        }

        if (Game1.game1.ShouldTimePass() && CoreMod.State.SecondsOutOfCombat > 15 && e.IsMultipleOf(300))
        {
            // decay counter every 5 seconds after 15 seconds out of combat
            player.health = Math.Max(player.health - Math.Max(player.maxHealth / 100, 1), player.maxHealth);
        }
    }
}
