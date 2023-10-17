namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Buff = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class BurntOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BurntOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BurntOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Game1.game1.ShouldTimePass() || !e.IsMultipleOf(300))
        {
            return;
        }

        var player = Game1.player;
        if (!player.hasBuff((int)Buff.Burnt))
        {
            this.Disable();
            return;
        }

        player.health -= (int)(player.maxHealth / 16f);
        player.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
            30,
            player.Position,
            Color.White,
            4,
            Game1.random.NextBool(),
            50f,
            1)
            {
                positionFollowsAttachedCharacter = true,
                attachedCharacter = player,
                layerDepth = 999999f,
            });
    }
}
