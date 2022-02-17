namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using StardewValley;
using StardewValley.Monsters;

#endregion using directives

internal static class MonsterExtensions
{
    /// <summary>Whether the monster instance is close enough to see the given player.</summary>
    /// <param name="player">The target player.</param>
    public static bool IsWithinPlayerThreshold(this Monster monster, Farmer player = null)
    {
        player ??= Game1.player;
        return monster.DistanceToCharacter(player) <= monster.moveTowardPlayerThreshold.Value;
    }

    /// <summary>Whether the monster is an instance of <see cref="GreenSlime"/> or <see cref="BigSlime"/></summary>
    public static bool IsSlime(this Monster monster)
    {
        return monster is GreenSlime or BigSlime;
    }
}