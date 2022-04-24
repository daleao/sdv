namespace DaLion.Stardew.Prairie.Training.Framework.Events;

#region using directives

using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using SharpNeat.Core;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;

using Common.Extensions.Reflection;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicking"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class UpdateTickingEvent : IEvent
{
    private static readonly MethodInfo _OverrideButton = Game1.input.GetType().RequireMethod("OverrideButton");

    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
        Log.D("[Prairie] Hooked UpdateTicking event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicking -= OnUpdateTicking;
        Log.D("[Prairie] Unhooked UpdateTicking event.");
    }

    /// <summary>Raised before the game state is updated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
    {
        if (!ModEntry.IsPlayingAbigailGame || ModEntry.Network?.RunState is not RunState.Running) return;

        // reset inputs
        for (var x = 0; x < 16; ++x)
            for (var y = 0; y < 16; ++y)
                ModEntry.Inputs[x, y] = Input.Empty;

        Game1.currentMinigame.tick(Game1.currentGameTime);

        // get player
        ModEntry.Inputs[(int) ModEntry.GameInstance.playerPosition.X / AbigailGame.TileSize,
            (int) ModEntry.GameInstance.playerPosition.Y / AbigailGame.TileSize] = Input.Self;

        // get abby
        if (AbigailGame.playingWithAbigail)
            ModEntry.Inputs[(int) AbigailGame.player2Position.X / AbigailGame.TileSize,
                (int) AbigailGame.player2Position.Y / AbigailGame.TileSize] = Input.Abigail;

        // get gopher
        if (AbigailGame.gopherRunning)
            ModEntry.Inputs[AbigailGame.gopherBox.X / AbigailGame.TileSize,
                AbigailGame.gopherBox.Y / AbigailGame.TileSize] = Input.Gopher;

        // get enemies
        foreach (var monster in AbigailGame.monsters)
            ModEntry.Inputs[monster.position.X / AbigailGame.TileSize, monster.position.Y / AbigailGame.TileSize] =
                Input.Enemy;

        // get friendly bullets
        foreach (var bullet in ModEntry.GameInstance.bullets)
            ModEntry.Inputs[bullet.position.X / AbigailGame.TileSize, bullet.position.Y / AbigailGame.TileSize] =
                Input.FriendlyBullet;

        // get enemy bullets
        foreach (var bullet in AbigailGame.enemyBullets)
            ModEntry.Inputs[bullet.position.X / AbigailGame.TileSize, bullet.position.Y / AbigailGame.TileSize] =
                Input.FriendlyBullet;

        // get power-ups
        foreach (var powerup in AbigailGame.powerups)
            ModEntry.Inputs[powerup.position.X / AbigailGame.TileSize, powerup.position.Y / AbigailGame.TileSize] =
                powerup.which is 0 or 1 ? Input.Coin : Input.Powerup;

        // get obstacles
        for (var x = 0; x < 16; ++x)
            for (var y = 0; y < 16; ++y)
            {
                var r = new Rectangle(x * AbigailGame.TileSize, y * AbigailGame.TileSize, AbigailGame.TileSize,
                    AbigailGame.TileSize);
                if (AbigailGame.isCollidingWithMap(r))
                    ModEntry.Inputs[x, y] = Input.Obstacle;
                else if (AbigailGame.isCollidingWithMapForMonsters(r))
                    ModEntry.Inputs[x, y] = Input.Obstacle;
            }
    }
}