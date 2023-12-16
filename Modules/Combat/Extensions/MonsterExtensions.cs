namespace DaLion.Overhaul.Modules.Combat.Extensions;

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
        var g = r.NextGaussian(1d - (Game1.player.DailyLuck * 2d), 0.1);
        monster.MaxHealth = Math.Max((int)Math.Round(monster.MaxHealth * g), 1);
        monster.DamageToFarmer = Math.Max((int)Math.Round(monster.DamageToFarmer * g), 1);
        monster.resilience.Value = Math.Max((int)Math.Round(monster.resilience.Value * g), 1);

        var addedSpeed = r.NextDouble() > (2d / 3d) + (Game1.player.DailyLuck * 2d)
            ? 1
            : r.NextDouble() < (1d / 3d) + (Game1.player.DailyLuck * 2d) ? -1 : 0;
        monster.speed = Math.Max(monster.speed + addedSpeed, 1);

        monster.durationOfRandomMovements.Value =
            (int)(monster.durationOfRandomMovements.Value * (0.5d + r.NextDouble()));
        monster.moveTowardPlayerThreshold.Value =
            Math.Max(monster.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
    }
}
