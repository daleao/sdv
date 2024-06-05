namespace DaLion.Core.Framework.Extensions;

#region using directives

using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Triggers a barrage of lightning strikes at the specified <paramref name="tileLocation"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tileLocation">The <see cref="Vector2"/> tile location.</param>
    /// <param name="radius">The area-of-effect radius.</param>
    /// <param name="who">The <see cref="Farmer"/> who is attributed the damage.</param>
    public static void DoLightningBarrage(this GameLocation location, Vector2 tileLocation, int radius, Farmer? who = null)
    {
        who ??= Game1.player;
        if (!who.IsLocalPlayer)
        {
            return;
        }

        var aoe = new Rectangle(
            (int)tileLocation.X * Game1.tileSize,
            (int)tileLocation.Y * Game1.tileSize,
            Game1.tileSize,
            Game1.tileSize);
        aoe.Inflate(radius * Game1.tileSize, radius * Game1.tileSize);
        var boltStrikeOrigin = (tileLocation * Game1.tileSize) + new Vector2(32f, 32f);
        var boltStrikeOffset = new Vector2(256f, 0f);
        Game1.flashAlpha = (float)(2f + Game1.random.NextDouble());

        boltStrikeOffset = boltStrikeOffset.Rotate(Game1.random.Next(10, 80));
        Utility.drawLightningBolt(boltStrikeOrigin + boltStrikeOffset, location);
        location.playSound("thunder");

        boltStrikeOffset = boltStrikeOffset.Rotate(120d);
        Utility.drawLightningBolt(boltStrikeOrigin + boltStrikeOffset, location);
        location.playSound("thunder");

        boltStrikeOffset = boltStrikeOffset.Rotate(120d);
        Utility.drawLightningBolt(boltStrikeOrigin + boltStrikeOffset, location);
        location.playSound("thunder");
        location.damageMonster(
            aoe,
            150,
            250,
            false,
            1f,
            0,
            0f,
            1f,
            false,
            who);
    }
}
