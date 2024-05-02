namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using System.Linq;
using DaLion.Core;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PiperWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PiperWarpedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var isDungeon = e.NewLocation.IsDungeon();
        if (!isDungeon && !CoreMod.State.AreEnemiesNearby)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        var maxWidth = e.NewLocation.Map.Layers[0].LayerWidth;
        var maxHeight = e.NewLocation.Map.Layers[0].LayerHeight;
        var spawnTiles = new HashSet<Vector2>();
        var (playerX, playerY) = e.Player.Tile;
        for (var x = playerX - 2; x <= playerX + 2; x++)
        {
            if (x < 0 || x > maxWidth || x == playerX)
            {
                continue;
            }

            for (var y = playerY - 2; y <= playerY + 2; y++)
            {
                if (y < 0 || y > maxHeight || y == playerY)
                {
                    continue;
                }

                var tile = new Vector2(x, y);
                if (e.NewLocation.CanSpawnCharacterHere(tile))
                {
                    spawnTiles.Add(tile);
                }
            }
        }

        if (!spawnTiles.Any())
        {
            return;
        }

        var numberToSpawn = e.Player.HasProfession(Profession.Piper, true) ? 2 : 1;
        var spawned = 0;
        while (spawnTiles.Any() && spawned < numberToSpawn)
        {
            var tile = spawnTiles.Choose(r);
            spawnTiles.Remove(tile);
            GreenSlime piped;
            switch (e.NewLocation)
            {
                case MineShaft shaft:
                {
                    // from MineShaft.getMonsterForThisLevel
                    piped = new GreenSlime(tile, shaft.mineLevel);
                    shaft.BuffMonsterIfNecessary(piped);
                    break;
                }

                case Woods:
                {
                    // from Woods.resetSharedState
                    piped = e.NewLocation.GetSeason() switch
                    {
                        Season.Winter => new GreenSlime(tile, 40),
                        Season.Fall => new GreenSlime(tile, r.NextBool() ? 40 : 0),
                        _ => new GreenSlime(tile, 0),
                    };
                    break;
                }

                case VolcanoDungeon:
                {
                    // from VolcanoDungeon.GenerateEntities
                    piped = new GreenSlime(tile, 0);
                    piped.makeTigerSlime();
                    break;
                }

                default:
                {
                    piped = new GreenSlime(tile, 0);
                    break;
                }
            }

            var numberRaised = e.Player.CountRaisedSlimes();
            var powerup = Math.Min(numberRaised / 10f, 3f);
            piped.moveTowardPlayerThreshold.Value = 0;
            piped.MaxHealth = (int)(piped.MaxHealth * powerup);
            piped.Health = piped.MaxHealth;
            piped.DamageToFarmer = (int)(piped.DamageToFarmer * powerup);
            piped.resilience.Value = (int)(piped.resilience.Value * powerup);
            e.NewLocation.characters.Add(piped);
            State.AllySlimes.Add(piped);
            spawned++;
        }
    }
}
