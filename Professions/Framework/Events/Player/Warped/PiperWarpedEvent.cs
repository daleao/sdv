namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using System.Linq;
using DaLion.Core;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
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
        var areEnemiesAround = CoreMod.State.AreEnemiesNearby && e.NewLocation is not SlimeHutch;
        if (!isDungeon && !areEnemiesAround)
        {
            State.SummonedSlimes[0] = null;
            State.SummonedSlimes[1] = null;
            return;
        }

        var numberToSpawn = 0;
        for (var i = 0; i < 2; i++)
        {
            if (State.SummonedSlimes[i] is { } piped)
            {
                e.OldLocation.characters.Remove(piped.Instance);
                e.NewLocation.characters.Add(piped.Instance);
            }
            else if (State.PiperCooldown[i] <= 0 && (i == 0 || e.Player.HasProfession(Profession.Piper, true)))
            {
                numberToSpawn++;
            }
        }

        if (numberToSpawn == 0)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        var mapWidth = e.NewLocation.Map.Layers[0].LayerWidth;
        var mapHeight = e.NewLocation.Map.Layers[0].LayerHeight;
        var spawnTiles = e.Player.Tile
            .GetTwentyFourNeighbors(mapWidth, mapHeight)
            .Where(e.NewLocation.CanSpawnCharacterHere)
            .ToHashSet();
        if (!spawnTiles.Any())
        {
            return;
        }

        var numberRaised = e.Player.CountRaisedSlimes();
        var powerUp = e.Player.HasProfession(Profession.Piper, true)
            ? MathHelper.Lerp(1f, 3f, numberRaised / 60f)
            : MathHelper.Lerp(1f, 2f, numberRaised / 40f);

        for (var i = 0; i < numberToSpawn; i++)
        {
            var tile = spawnTiles.Choose(r);
            spawnTiles.Remove(tile);
            var position = tile * Game1.tileSize;
            GreenSlime piped;
            switch (e.NewLocation)
            {
                case MineShaft shaft:
                {
                    // from MineShaft.getMonsterForThisLevel
                    var level = powerUp switch
                    {
                        >= 2.5f => 9999899,
                        >= 2f / 1f => 200,
                        >= 3f / 2f => 80,
                        >= 4f / 3f => 40,
                        _ => 0,
                    };

                    piped = new GreenSlime(position, level);
                    shaft.BuffMonsterIfNecessary(piped);
                    break;
                }

                case Woods:
                {
                    // from Woods.resetSharedState
                    piped = e.NewLocation.GetSeason() switch
                    {
                        Season.Winter => new GreenSlime(position, 40),
                        Season.Fall => new GreenSlime(position, r.NextBool() ? 40 : 0),
                        _ => new GreenSlime(position, 0),
                    };
                    break;
                }

                case VolcanoDungeon:
                {
                    // from VolcanoDungeon.GenerateEntities
                    piped = new GreenSlime(position, 0);
                    piped.makeTigerSlime();
                    break;
                }

                default:
                {
                    piped = new GreenSlime(position, 0);
                    break;
                }
            }

            piped.MaxHealth = (int)(piped.MaxHealth * powerUp);
            piped.Health = piped.MaxHealth;
            piped.DamageToFarmer = (int)(piped.DamageToFarmer * powerUp);
            piped.resilience.Value = (int)(piped.resilience.Value * powerUp);
            e.NewLocation.characters.Add(piped);
            piped.Set_Piped(e.Player);
            if (State.SummonedSlimes[0] is null)
            {
                State.SummonedSlimes[0] = piped.Get_Piped();
            }
            else if (State.SummonedSlimes[1] is null)
            {
                State.SummonedSlimes[1] = piped.Get_Piped();
            }
        }
    }
}
