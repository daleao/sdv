namespace DaLion.Redux.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
    /// <summary>
    ///     Determines whether the <paramref name="monster"/> is an instance of <see cref="GreenSlime"/> or
    ///     <see cref="BigSlime"/>.
    /// </summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is a <see cref="GreenSlime"/> or <see cref="BigSlime"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsSlime(this Monster monster)
    {
        return monster is GreenSlime or BigSlime;
    }

    /// <summary>Determines whether the <paramref name="monster"/> is close enough to see the given player.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="player">The target player.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/>'s distance to the <paramref name="player"/> is less than it's aggro threshold, otherwise <see langword="false"/>.</returns>
    internal static bool IsWithinPlayerThreshold(this Monster monster, Farmer? player = null)
    {
        player ??= Game1.player;
        return monster.DistanceTo(player) <= monster.moveTowardPlayerThreshold.Value;
    }

    ///// <summary>Determines whether the monster can be afflicted with Fear status.</summary>
    //internal static bool CanBeFeared(this Monster monster) => !monster.isInvisible.Value && monster is not (BigSlime or Ghost);

    /// <summary>Determines whether the <paramref name="monster"/> can be afflicted with Slow status.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> is not a <see cref="GreenSlime"/>, <see cref="BigSlime"/> or <see cref="Ghost"/>, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeSlowed(this Monster monster)
    {
        return !monster.IsSlime() && monster is not Ghost;
    }
}
