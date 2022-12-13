namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Monster"/> class.</summary>
internal static class MonsterExtensions
{
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
