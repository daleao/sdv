namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

using GameLoop;
using Extensions;

#endregion using directives

internal class PiperWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation is not (Woods or VolcanoDungeon or MineShaft) ||
            e.NewLocation is MineShaft shaft1 && shaft1.IsTreasureOrSafeRoom())
        {
            EventManager.Disable(typeof(PiperUpdateTickedEvent));
            return;
        }

        var attempts = e.Player.GetPiperSlimeSpawnAttempts();
        var spawned = 0;
        var r = new Random(Guid.NewGuid().GetHashCode());
        while (attempts-- > 0 || spawned < 1)
        {
            var x = r.Next(e.NewLocation.Map.Layers[0].LayerWidth);
            var y = r.Next(e.NewLocation.Map.Layers[0].LayerHeight);
            var spawnPosition = new Vector2(x, y);

            GreenSlime slime;
            switch (e.NewLocation)
            {
                case MineShaft shaft2:
                {
                    shaft2.checkForMapAlterations(x, y);
                    if (!shaft2.isTileClearForMineObjects(spawnPosition) || shaft2.isTileOccupied(spawnPosition))
                        continue;

                    slime = new(Vector2.Zero, shaft2.mineLevel);
                    if (shaft2.GetAdditionalDifficulty() > 0 &&
                        r.NextDouble() < Math.Min(shaft2.GetAdditionalDifficulty() * 0.1f, 0.5f))
                        slime.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;

                    slime.setTilePosition(x, y);
                    shaft2.characters.Add(shaft2.BuffMonsterIfNecessary(slime));
                    ++spawned;
                    break;
                }
                case Woods woods:
                {
                    if (!woods.isTileLocationTotallyClearAndPlaceable(spawnPosition)) continue;

                    slime = Game1.currentSeason switch
                    {
                        "fall" => new(spawnPosition * 64f, r.NextDouble() < 0.5 ? 40 : 0),
                        "winter" => new(spawnPosition * 64f, 40),
                        _ => new(spawnPosition * 64f, 0)
                    };
                    woods.characters.Add(slime);
                    ++spawned;
                    break;
                }
                case VolcanoDungeon dungeon:
                {
                    if (!dungeon.isTileLocationTotallyClearAndPlaceable(spawnPosition)) continue;

                    slime = new(spawnPosition * 64f, 1);
                    slime.makeTigerSlime();
                    dungeon.characters.Add(slime);
                    ++spawned;
                    break;
                }
            }

            --attempts;
        }

        Log.D($"Spawned {spawned} Slimes after {attempts} attempts.");

        EventManager.Enable(typeof(PiperUpdateTickedEvent));
    }
}