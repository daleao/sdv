namespace DaLion.Ligo.Modules.Arsenal.Extensions;

#region using directives

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

        var luckModifier = (Game1.player.DailyLuck * 3d) + 1d;
        monster.Health = (int)(monster.Health * r.Next(80, 121) / 1000d * luckModifier);
        monster.DamageToFarmer = (int)(monster.DamageToFarmer * r.Next(10, 41) / 10d * luckModifier);
        monster.resilience.Value = (int)(monster.resilience.Value * r.Next(10, 21) / 10d * luckModifier);

        var addedSpeed = r.NextDouble() > 0.5 + (Game1.player.DailyLuck * 2d) ? 1 :
            r.NextDouble() < 0.5 - (Game1.player.DailyLuck * 2d) ? -1 : 0;
        monster.speed = Math.Max(monster.speed + addedSpeed, 1);

        monster.durationOfRandomMovements.Value =
            (int)(monster.durationOfRandomMovements.Value * (r.NextDouble() - 0.5));
        monster.moveTowardPlayerThreshold.Value =
            Math.Max(monster.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
    }
}
