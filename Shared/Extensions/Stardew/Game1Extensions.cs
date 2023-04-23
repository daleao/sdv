namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

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

    /// <summary>Enumerates all chests in the game instance.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="Chest"/> instances in the <paramref name="game1"/> instance.</returns>
    public static IEnumerable<Chest> IterateAllChests(this Game1 game1)
    {
        for (var i = 0; i < Game1.locations.Count; i++)
        {
            var location1 = Game1.locations[i];
            foreach (var @object in location1.Objects.Values)
            {
                if (@object is Chest chest1)
                {
                    yield return chest1;
                }
                else if (@object.heldObject.Value is Chest chest2)
                {
                    yield return chest2;
                }
            }

            if (location1 is not BuildableGameLocation buildable)
            {
                continue;
            }

            for (var j = 0; j < buildable.buildings.Count; j++)
            {
                var building = buildable.buildings[j];
                if (building.indoors.Value is not { } location2)
                {
                    continue;
                }

                foreach (var @object in location2.Objects.Values)
                {
                    if (@object is Chest chest1)
                    {
                        yield return chest1;
                    }
                    else if (@object.heldObject.Value is Chest chest2)
                    {
                        yield return chest2;
                    }
                }
            }
        }
    }
}
