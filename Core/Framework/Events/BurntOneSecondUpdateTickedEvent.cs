namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

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
    public override bool IsEnabled => Game1.player.hasBuff(BuffIDs.Burnt);

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        var player = Game1.player;
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
