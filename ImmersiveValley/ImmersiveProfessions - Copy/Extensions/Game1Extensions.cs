namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Framework;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
public static class Game1Extensions
{
    /// <summary>Whether any farmer in the current game session has a specific profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="count">How many players have this profession.</param>
    public static bool DoesAnyPlayerHaveProfession(this Game1 game1, IProfession profession,
        out int count)
    {
        if (!Context.IsMultiplayer)
            if (Game1.player.HasProfession(profession))
            {
                count = 1;
                return true;
            }

        count = Game1.getOnlineFarmers()
            .Count(f => f.HasProfession(profession));
        return count > 0;
    }
}