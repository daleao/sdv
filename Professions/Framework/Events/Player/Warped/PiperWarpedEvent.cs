namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Core;
using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;
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

/// <summary>Initializes a new instance of the <see cref="PiperWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    private readonly Func<int, double> _pipeChance = x => 29f / (x + 28f);

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var newLocation = e.NewLocation;
        var isEnemyArea = newLocation.IsEnemyArea();
        var areEnemiesAround = CoreMod.State.AreEnemiesNearby && newLocation is not SlimeHutch;
        if (!e.NewLocation.Name.ContainsAnyOf("Mine", "SkullCave") && !isEnemyArea && !areEnemiesAround)
        {
            State.AlliedSlimes[0] = null;
            State.AlliedSlimes[1] = null;
            this.Manager.Disable<PiperOneSecondUpdateTickedEvent>();
            return;
        }

        var piper = e.Player;
        var mapWidth = newLocation.Map.Layers[0].LayerWidth;
        var mapHeight = newLocation.Map.Layers[0].LayerHeight;
        for (var i = 0; i < 2; i++)
        {
            if (State.AlliedSlimes[i] is not { } piped)
            {
                continue;
            }

            var spawnTile = piper.Tile
                .GetEightNeighbors(mapWidth, mapHeight)
                .Where(newLocation.CanSpawnCharacterHere)
                .Choose();
            Game1.warpCharacter(piped.Instance, e.NewLocation, spawnTile);
            piped.FakeFarmer.currentLocation = e.NewLocation;
            piped.FakeFarmer.Position = spawnTile * Game1.tileSize;
            piped.Instance.ResetIncrementalPathfinder();
            piped.Instance.Set_CurrentStep(null);
        }

        if (!isEnemyArea || !areEnemiesAround || newLocation is MineShaft { isSlimeArea: true })
        {
            this.Manager.Disable<PiperOneSecondUpdateTickedEvent>();
            return;
        }

        var r = new Random();
        var numberRaised = piper.CountRaisedSlimes();
        var spawned = 0;
        while (!r.NextBool(this._pipeChance(numberRaised)))
        {
            var x = r.Next(e.NewLocation.Map.Layers[0].LayerWidth);
            var y = r.Next(e.NewLocation.Map.Layers[0].LayerHeight);
            var spawnTile = new Vector2(x, y);

            if (!e.NewLocation.isTileOnMap(spawnTile) || !e.NewLocation.CanSpawnCharacterHere(spawnTile))
            {
                continue;
            }

            if (e.NewLocation is MineShaft shaft)
            {
                shaft.checkForMapAlterations(x, y);
                if (!shaft.isTileClearForMineObjects(spawnTile))
                {
                    continue;
                }
            }

            GreenSlime slime;
            switch (e.NewLocation)
            {
                case MineShaft shaft2:
                {
                    slime = new GreenSlime(default, shaft2.mineLevel);
                    if (shaft2.GetAdditionalDifficulty() > 0 &&
                        r.NextDouble() < Math.Min(shaft2.GetAdditionalDifficulty() * 0.1f, 0.5f))
                    {
                        slime.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;
                    }

                    shaft2.BuffMonsterIfNecessary(slime);
                    break;
                }

                case Woods:
                {
                    slime = Game1.currentSeason switch
                    {
                        "fall" => new GreenSlime(default, r.NextDouble() < 0.5 ? 40 : 0),
                        "winter" => new GreenSlime(default, 40),
                        _ => new GreenSlime(default, 0),
                    };
                    break;
                }

                case VolcanoDungeon:
                {
                    slime = new GreenSlime(default, 0);
                    slime.makeTigerSlime();
                    break;
                }

                default:
                {
                    slime = new GreenSlime(default, 121);
                    break;
                }
            }

            slime.Position = spawnTile * Game1.tileSize;
            slime.currentLocation = newLocation;
            newLocation.characters.Add(slime);
            spawned++;
        }

        Log.D($"Successfully spawned {spawned} Slimes.");
        this.Manager.Enable<PiperOneSecondUpdateTickedEvent>();
    }
}
