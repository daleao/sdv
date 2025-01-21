namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static readonly Func<Vector2, GameLocation, bool> IsMineShaftTileSpawnable = (tile, location) =>
        location.CanSpawnCharacterHere(tile) && location is MineShaft shaft &&
        shaft.isTileClearForMineObjects(tile) && !shaft.IsTileOccupiedBy(tile);

    private static readonly Func<Vector2, GameLocation, bool> IsVolcanoDungeonTileSpawnable = (tile, location) =>
        location.CanSpawnCharacterHere(tile) && location is VolcanoDungeon volcano &&
        volcano.isTileClearForMineObjects(tile) && !volcano.IsTileOccupiedBy(tile);

    private static readonly Func<Vector2, GameLocation, bool> IsLocationTileSpawnable = (tile, location) =>
        location.CanSpawnCharacterHere(tile) && !location.IsTileOccupiedBy(tile);

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var piper = e.Player;
        var newLocation = e.NewLocation;
        var toDangerZone = newLocation.IsEnemyArea() || newLocation.Name.ContainsAnyOf("Mine", "SkullCave");
        if (!toDangerZone)
        {
            this.Manager.Enable<PipedSelfDestructOneSecondUpdateTickedEvent>();
            if (!newLocation.IsOutdoors)
            {
                return;
            }

            foreach (var (_, piped) in GreenSlime_Piped.Values)
            {
                piped.WarpToPiper();
            }

            return;
        }

        var oldLocation = e.OldLocation;
        var fromDangerZone = oldLocation.IsEnemyArea() || oldLocation.Name.ContainsAnyOf("Mine", "SkullCave");
        if (!fromDangerZone)
        {
            SpawnMinions(piper, newLocation);
        }

        foreach (var (_, piped) in GreenSlime_Piped.Values)
        {
            if (!ReferenceEquals(piped.Slime.currentLocation, newLocation))
            {
                piped.WarpToPiper();
            }
        }
    }

    private static void SpawnMinions(Farmer piper, GameLocation location)
    {
        var numberRaised = piper.CountRaisedSlimes();
        var toSpawn = numberRaised / 10;
        var r = new Random(Guid.NewGuid().GetHashCode());
        for (var i = 0; i < toSpawn; i++)
        {
            var condition = location switch
            {
                MineShaft => IsMineShaftTileSpawnable,
                VolcanoDungeon => IsVolcanoDungeonTileSpawnable,
                _ => IsLocationTileSpawnable,
            };

            var spawnTile = piper.CountPipedSlimes() < 8
                ? piper.ChooseFromEightNeighboringTiles(condition, location)
                : piper.ChooseFromTwentyFourNeighboringTiles(condition, location);
            var toBeCloned = piper.GetRaisedSlimes().Choose(r);
            var spawn = new GreenSlime(spawnTile * Game1.tileSize, toBeCloned.color.Value)
            {
                currentLocation = location,
                Health = toBeCloned.Health,
                DamageToFarmer = toBeCloned.DamageToFarmer,
                resilience = { Value = toBeCloned.resilience.Value },
            };

            spawn.MaxHealth = spawn.Health;
            switch (toBeCloned.Name)
            {
                case "Tiger Slime":
                    spawn.makeTigerSlime(onlyAppearance: true);
                    break;
                case "Gold Slime":
                    spawn.MakeGoldSlime();
                    break;
                default:
                {
                    if (toBeCloned.prismatic.Value)
                    {
                        spawn.prismatic.Value = true;
                        spawn.Name = "Prismatic Slime";
                    }

                    break;
                }
            }

            location.characters.Add(spawn);
            spawn.Set_Piped(piper);
        }
    }
}
