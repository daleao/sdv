namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>Determines whether the <paramref name="monster"/> is close enough to see the given player.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="player">The target player.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/>'s distance to the <paramref name="player"/> is less than it's aggro threshold, otherwise <see langword="false"/>.</returns>
    internal static bool IsWithinPlayerThreshold(this Monster monster, Farmer? player = null)
    {
        player ??= Game1.player;
        return monster.DistanceTo(player) <= monster.moveTowardPlayerThreshold.Value;
    }
}
