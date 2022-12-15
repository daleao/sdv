namespace DaLion.Overhaul.Modules.Arsenal.Extensions;

#region using directives

using DaLion.Shared.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>Randomizes the stats of the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    internal static void RandomizeStats(this Monster monster)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());
        var mean = 1d - (Game1.player.DailyLuck * 3d);

        var g = Math.Max(r.NextGaussian(mean, 0.5), 0.25);
        monster.MaxHealth = (int)Math.Round(monster.MaxHealth * g);

        g = Math.Max(r.NextGaussian(mean, 0.5), 0.5);
        monster.DamageToFarmer = (int)Math.Round(monster.DamageToFarmer * g);

        g = Math.Max(r.NextGaussian(mean, 0.5), 0.5);
        monster.resilience.Value = (int)Math.Round(monster.resilience.Value * g);

        var addedSpeed = r.NextDouble() > 0.5 + (Game1.player.DailyLuck * 2d) ? 1 :
            r.NextDouble() < 0.5 - (Game1.player.DailyLuck * 2d) ? -1 : 0;
        monster.speed = Math.Max(monster.speed + addedSpeed, 1);

        monster.durationOfRandomMovements.Value =
            (int)(monster.durationOfRandomMovements.Value * (0.5d + r.NextDouble()));
        monster.moveTowardPlayerThreshold.Value =
            Math.Max(monster.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
    }
}
