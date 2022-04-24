namespace DaLion.Stardew.Prairie.Training;

#region using directives

using StardewValley.Minigames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using Common.Extensions.Xna;

#endregion using directives

internal static class Debug
{
    /// <summary>Pixel texture used to draw borders.</summary>
    private static readonly Texture2D _pixel;

    /// <summary>Initialize static properties.</summary>
    static Debug()
    {
        _pixel = new(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _pixel.SetData(new[] { Color.White });
    }

    /// <summary>Clears enemies and allows advancement to the next game stage.</summary>
    internal static void AdvanceMap()
    {
        AbigailGame.monsters.Clear();
        for (var i = 0; i < 4; ++i)
            ModEntry.GameInstance.spawnQueue[i].Clear();
        
        AbigailGame.waitingForPlayerToMoveDownAMap = true;
        AbigailGame.map[8, 15] = 3;
        AbigailGame.map[7, 15] = 3;
        AbigailGame.map[9, 15] = 3;
    }

    /// <summary>Draws borders around objects to identify them within the current stage.</summary>
    /// <param name="b">The <see cref="SpriteBatch"/> to draw to.</param>
    internal static void DrawBorders(SpriteBatch b)
    {
        // highlight player
        ModEntry.GameInstance.playerPosition.DrawBorder(AbigailGame.TileSize, AbigailGame.TileSize, _pixel, 3, Color.Blue, b,
            new(AbigailGame.topLeftScreenCoordinate.X, AbigailGame.topLeftScreenCoordinate.Y - 16));

        // highlight abby
        if (AbigailGame.playingWithAbigail)
            AbigailGame.player2Position.DrawBorder(AbigailGame.TileSize, AbigailGame.TileSize, _pixel, 3, Color.Blue, b, AbigailGame.topLeftScreenCoordinate);

        // highlight gopher
        if (AbigailGame.gopherRunning)
            AbigailGame.gopherBox.DrawBorder(_pixel, 3, Color.Blue, b, AbigailGame.topLeftScreenCoordinate);

        // highlight enemy tiles
        foreach (var monster in AbigailGame.monsters)
            monster.position.DrawBorder(_pixel, 3, Color.Red, b, AbigailGame.topLeftScreenCoordinate);

        // highlight friendly bullets
        foreach (var bullet in ModEntry.GameInstance.bullets)
            bullet.position.DrawBorder(16, 16, _pixel, 3, Color.Blue, b, AbigailGame.topLeftScreenCoordinate);

        // highlight enemy bullets
        foreach (var bullet in AbigailGame.enemyBullets)
            bullet.position.DrawBorder(16, 16, _pixel, 3, Color.Red, b, AbigailGame.topLeftScreenCoordinate);

        // highlight power-ups
        foreach (var powerup in AbigailGame.powerups)
            powerup.position.DrawBorder(AbigailGame.TileSize, AbigailGame.TileSize, _pixel, 3, Color.Blue, b, AbigailGame.topLeftScreenCoordinate);

        // highlight obstacles
        for (var x = 0; x < 16 * AbigailGame.TileSize; x += AbigailGame.TileSize)
            for (var y = 0; y < 16 * AbigailGame.TileSize; y += AbigailGame.TileSize)
            {
                var r = new Rectangle(x, y, AbigailGame.TileSize, AbigailGame.TileSize);
                if (AbigailGame.isCollidingWithMap(r))
                    r.DrawBorder(_pixel, 3, Color.White, b, AbigailGame.topLeftScreenCoordinate);
                else if (AbigailGame.isCollidingWithMapForMonsters(r))
                    r.DrawBorder(_pixel, 3, Color.White, b, AbigailGame.topLeftScreenCoordinate);
            }
    }
}