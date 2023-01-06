namespace DaLion.Shared.Extensions.Stardew;

using System.Collections.Generic;
using System.Linq;
using StardewValley.Buildings;
using StardewValley.Locations;

using StardewValley.Objects;
using xTile.Dimensions;

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
public static class Game1Extensions
{
    /// <summary>Determines whether the Community Center has been completed in the current save.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <returns><see langword="true"/> if the Community Center is complete, otherwise <see langword="false"/>.</returns>
    public static bool IsCommunityCenterComplete(this Game1 game1)
    {
        return Context.IsWorldReady && (Game1.MasterPlayer.hasCompletedCommunityCenter() ||
               Game1.MasterPlayer.mailReceived.Contains("ccIsComplete"));
    }

    /// <summary>Determines whether the game is active and time should pass.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <returns><see langword="true"/> if the game is active and time should pass, otherwise <see langword="false"/>.</returns>
    public static bool ShouldTimePass(this Game1 game1)
    {
        return (Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass();
    }
}
