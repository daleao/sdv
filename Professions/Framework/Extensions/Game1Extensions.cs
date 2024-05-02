namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
internal static class Game1Extensions
{
    /// <summary>Determines whether any <see cref="Farmer"/> in the current game session has the specified <paramref name="profession"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="includeOffline">Whether to include offline farmers.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> is at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHaveProfession(this Game1 game1, IProfession profession, bool includeOffline = false, bool prestiged = false)
    {
        return !Context.IsMultiplayer
            ? Game1.player.HasProfession(profession, prestiged)
            : (includeOffline ? Game1.getAllFarmers() : Game1.getOnlineFarmers()).Any(f =>
                f.HasProfession(profession, prestiged));
    }

    /// <summary>Determines whether any <see cref="Farmer"/> in the current game session has the specified <paramref name="profession"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="players">Which <see cref="Farmer"/>s have this profession.</param>
    /// <param name="includeOffline">Whether to include offline farmers.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> is at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHaveProfession(
        this Game1 game1, IProfession profession, out IEnumerable<Farmer> players, bool includeOffline = false, bool prestiged = false)
    {
        if (!Context.IsMultiplayer)
        {
            if (Game1.player.HasProfession(profession, prestiged))
            {
                players = Game1.player.Collect();
                return true;
            }

            players = [];
            return false;
        }

        players = (includeOffline ? Game1.getAllFarmers() : Game1.getOnlineFarmers())
            .Where(f => f.HasProfession(profession, prestiged));
        return players.Any();
    }

    /// <summary>Enumerates all <see cref="CrabPot"/> instances currently placed in a location.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all placed <see cref="CrabPot"/> instances in the game world.</returns>
    internal static IEnumerable<CrabPot> EnumerateAllCrabPots(this Game1 game1)
    {
        foreach (var location in Game1.locations)
        {
            foreach (var @object in location.Objects.Values)
            {
                if (@object is CrabPot crabPot)
                {
                    yield return crabPot;
                }
            }
        }
    }
}
