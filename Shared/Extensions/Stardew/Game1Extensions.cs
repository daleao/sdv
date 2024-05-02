namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
public static class Game1Extensions
{
    /// <summary>Gets a representation of the current date as a single number.</summary>
    /// <param name="game1">The current <see cref="Game1"/> session.</param>
    /// <returns>A number representation of the current in-game day.</returns>
    public static int GetCurrentDateNumber(this Game1 game1)
    {
        return ((Game1.year - 1) * 112) + (Game1.seasonIndex * 28) + Game1.dayOfMonth;
    }

    /// <summary>Determines whether the Community Center has been completed in the current save.</summary>
    /// <param name="game1">The current <see cref="Game1"/> session.</param>
    /// <returns><see langword="true"/> if the Community Center is complete, otherwise <see langword="false"/>.</returns>
    public static bool IsCommunityCenterComplete(this Game1 game1)
    {
        return Context.IsWorldReady && (Game1.MasterPlayer.hasCompletedCommunityCenter() ||
               Game1.MasterPlayer.mailReceived.Contains("ccIsComplete"));
    }

    /// <summary>Determines whether the game is active and time should pass.</summary>
    /// <param name="game1">The current <see cref="Game1"/> session.</param>
    /// <returns><see langword="true"/> if the game is active and time should pass, otherwise <see langword="false"/>.</returns>
    public static bool ShouldTimePass(this Game1 game1)
    {
        return (Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass();
    }

    /// <summary>Gets the total value of shipped items by the specified <paramref name="farmer"/> during the current game day.</summary>
    /// <param name="game1">The current <see cref="Game1"/> session.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The total value of shipped items by the <paramref name="farmer"/>.</returns>
    public static int GetTotalSoldByPlayer(this Game1 game1, Farmer farmer)
    {
        var total = Game1.getFarm().getShippingBin(farmer)
            .WhereNotNull()
            .Sum(item => Utility.getSellToStorePriceOfItem(item));
        Utility.ForEachLocation(location =>
        {
            total += location.Objects.Values
                .OfType<Chest>()
                .Where(c => c.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
                .Sum(miniBin => miniBin
                    .GetItemsForPlayer(farmer.UniqueMultiplayerID)
                    .Sum(item => Utility.getSellToStorePriceOfItem(item)));
            return true;
        });

        return total;
    }

    /// <summary>Gets the localized display name of the <see cref="NPC"/> with the specified <paramref name="name"/>.</summary>
    /// <param name="game1">The current <see cref="Game1"/> session.</param>
    /// <param name="name">The public name of an <see cref="NPC"/>.</param>
    /// <returns>The localized display name of the <see cref="NPC"/> with the specified <paramref name="name"/>.</returns>
    public static string GetLocalizedCharacterName(this Game1 game1, string name)
    {
        if (Game1.characterData.TryGetValue(name, out var data))
        {
            return data.DisplayName;
        }

        return name;
    }
}
