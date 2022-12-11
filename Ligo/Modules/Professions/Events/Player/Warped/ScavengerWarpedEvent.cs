namespace DaLion.Ligo.Modules.Professions.Events.Player;

#region using directives

using System.Collections.Generic;
using System.Globalization;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using static DaLion.Ligo.Modules.LigoModule;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ScavengerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ScavengerWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Scavenger);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        var scavengerHunt = e.Player.Get_ScavengerHunt();
        if (scavengerHunt.IsActive)
        {
            scavengerHunt.Fail();
        }

        if (!e.NewLocation.IsOutdoors ||
            (e.NewLocation.IsFarm && !ProfessionsModule.Config.AllowScavengerHuntsOnFarm) ||
            scavengerHunt.TryStart(e.NewLocation))
        {
            return;
        }

        var streak = e.Player.Read<int>(DataFields.ScavengerHuntStreak);
        if (streak > 1)
        {
            TrySpawnForageables(streak / 2, e.NewLocation);
        }
    }

    private static void TrySpawnForageables(int amount, GameLocation location)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());
        var locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
        if (!locationData.ContainsKey(location.Name))
        {
            return;
        }

        var rawData = locationData[location.Name].Split('/')[Utility.getSeasonNumber(location.GetSeasonForLocation())];
        if (rawData.Equals("-1") || location.numberOfSpawnedObjectsOnMap >= 6)
        {
            return;
        }

        var split = rawData.Split(' ');
        var numberToSpawn = r.Next(1, Math.Min(5, 7 - amount));
        for (var i = 0; i < numberToSpawn; i++)
        {
            for (var j = 0; j < 11; j++)
            {
                var x = r.Next(location.Map.DisplayWidth / 64);
                var y = r.Next(location.Map.DisplayHeight / 64);
                var position = new Vector2(x, y);
                location.Objects.TryGetValue(position, out var @object);
                var whichObject = r.Next(split.Length / 2) * 2;
                if (@object != null || location.doesTileHaveProperty(x, y, "Spawnable", "Back") == null ||
                    location.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable", "Back", "F") ||
                    r.NextDouble() > Convert.ToDouble(split[whichObject + 1], CultureInfo.InvariantCulture) ||
                    !location.isTileLocationTotallyClearAndPlaceable(x, y) || location.getTileIndexAt(x, y, "AlwaysFront") != -1 ||
                    location.getTileIndexAt(x, y, "Front") != -1 || location.isBehindBush(position) ||
                    (Game1.random.NextDouble() > 0.1 && location.isBehindTree(position)) || !location.dropObject(
                        new SObject(
                            position,
                            Convert.ToInt32(split[whichObject]),
                            null,
                            canBeSetDown: false,
                            canBeGrabbed: true,
                            isHoedirt: false,
                            isSpawnedObject: true),
                        new Vector2(x * 64, y * 64),
                        Game1.viewport,
                        initialPlacement: true))
                {
                    continue;
                }

                location.numberOfSpawnedObjectsOnMap++;
                break;
            }
        }
    }
}
