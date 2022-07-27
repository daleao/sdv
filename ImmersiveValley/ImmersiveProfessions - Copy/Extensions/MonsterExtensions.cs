namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Stardew;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
public static class MonsterExtensions
{
    /// <summary>Whether the monster is an instance of <see cref="GreenSlime"/> or <see cref="BigSlime"/></summary>
    public static bool IsSlime(this Monster monster) => monster is GreenSlime or BigSlime;

    /// <summary>Whether the monster instance is close enough to see the given player.</summary>
    /// <param name="player">The target player.</param>
    public static bool IsWithinPlayerThreshold(this Monster monster, Farmer? player = null)
    {
        player ??= Game1.player;
        return monster.DistanceTo(player) <= monster.moveTowardPlayerThreshold.Value;
    }

    ///// <summary>Whether the monster can be afflicted with Slow status.</summary>
    //public static bool CanBeFeared(this Monster monster) => !monster.isInvisible.Value && monster is not (BigSlime or Ghost);

    /// <summary>Whether the monster can be afflicted with Slow status.</summary>
    public static bool CanBeSlowed(this Monster monster) => !monster.IsSlime() && monster is not Ghost;
}