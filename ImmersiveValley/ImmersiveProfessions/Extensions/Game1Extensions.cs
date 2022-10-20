namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Linq;
using DaLion.Common.Extensions.Collections;
using DaLion.Stardew.Professions.Framework;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
public static class Game1Extensions
{
    /// <summary>Determines whether any farmer in the current game session has the specified <paramref name="profession"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="count">How many players have this profession.</param>
    /// <returns><see langword="true"/> is at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool DoesAnyPlayerHaveProfession(
        this Game1 game1, IProfession profession, out int count)
    {
        if (!Context.IsMultiplayer)
        {
            if (Game1.player.HasProfession(profession))
            {
                count = 1;
                return true;
            }
        }

        count = Game1.getOnlineFarmers()
            .Count(f => f.HasProfession(profession));
        return count > 0;
    }

    /// <summary>Checks for and corrects invalid <see cref="FishPond"/> populations in the game session.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    public static void RevalidateFishPondPopulations(this Game1 game1)
    {
        Game1.getFarm().buildings.OfType<FishPond>()
            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());
    }
}
