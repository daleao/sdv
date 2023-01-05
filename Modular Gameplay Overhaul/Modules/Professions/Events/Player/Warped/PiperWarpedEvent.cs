namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperWarpedEvent : WarpedEvent
{
    private readonly Func<int, double> _pipeChance = x => 19f / (x + 18f);

    /// <summary>Initializes a new instance of the <see cref="PiperWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PiperWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        var isDungeon = e.NewLocation.IsDungeon();
        var hasMonsters = e.NewLocation.HasMonsters();
        if (!isDungeon && !hasMonsters)
        {
            return;
        }

        if (!isDungeon || (e.NewLocation is MineShaft shaft1 && shaft1.isLevelSlimeArea()))
        {
            return;
        }

        var enemyCount = Game1.player.currentLocation.characters.OfType<Monster>().Count(m => !m.IsSlime());
        if (enemyCount == 0)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        var raisedSlimes = e.Player.GetRaisedSlimes().ToArray();
        var chance = this._pipeChance(raisedSlimes.Length);
        var pipedCount = 0;
        foreach (var raised in raisedSlimes)
        {
            if (r.NextDouble() > chance)
            {
                continue;
            }

            // choose spawn tile
            var x = r.Next(e.NewLocation.Map.Layers[0].LayerWidth);
            var y = r.Next(e.NewLocation.Map.Layers[0].LayerHeight);
            var spawnTile = new Vector2(x, y);

            if (!e.NewLocation.isTileOnMap(spawnTile) ||
                !e.NewLocation.isTileLocationTotallyClearAndPlaceable(spawnTile))
            {
                continue;
            }

            if (e.NewLocation is MineShaft shaft2)
            {
                shaft2.checkForMapAlterations(x, y);
                if (!shaft2.isTileClearForMineObjects(spawnTile))
                {
                    continue;
                }
            }

            // choose slime variation
            GreenSlime piped;
            switch (e.NewLocation)
            {
                case MineShaft shaft:
                {
                    piped = new GreenSlime(Vector2.Zero, shaft.mineLevel);
                    if (shaft.GetAdditionalDifficulty() > 0 &&
                        r.NextDouble() < Math.Min(shaft.GetAdditionalDifficulty() * 0.1f, 0.5f))
                    {
                        piped.stackedSlimes.Value = r.NextDouble() < 0.0099999997764825821 ? 4 : 2;
                    }

                    shaft.BuffMonsterIfNecessary(piped);
                    break;
                }

                case Woods:
                {
                    piped = Game1.currentSeason switch
                    {
                        "fall" => new GreenSlime(Vector2.Zero, r.NextDouble() < 0.5 ? 40 : 0),
                        "winter" => new GreenSlime(Vector2.Zero, 40),
                        _ => new GreenSlime(Vector2.Zero, 0),
                    };
                    break;
                }

                case VolcanoDungeon:
                {
                    piped = new GreenSlime(Vector2.Zero, 0);
                    piped.makeTigerSlime();
                    break;
                }

                default:
                {
                    piped = new GreenSlime(Vector2.Zero, 121);
                    break;
                }
            }

            // adjust color
            if (raised.Name == "Tiger Slime" && piped.Name != raised.Name)
            {
                piped.makeTigerSlime();
            }
            else
            {
                piped.color.R = (byte)(raised.color.R + r.Next(-20, 21));
                piped.color.G = (byte)(raised.color.G + r.Next(-20, 21));
                piped.color.B = (byte)(raised.color.B + r.Next(-20, 21));
            }

            // make friendly
            piped.moveTowardPlayerThreshold.Value = 0;

            // spawn
            piped.setTileLocation(spawnTile);
            e.NewLocation.characters.Add(piped);
            if (++pipedCount >= enemyCount)
            {
                break;
            }
        }

        Log.D($"[Professions]: Spawned {pipedCount} Slimes after {raisedSlimes.Length} attempts.");
    }
}
