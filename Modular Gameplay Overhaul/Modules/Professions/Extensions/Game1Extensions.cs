namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
internal static class Game1Extensions
{
    /// <summary>Determines whether any farmer in the current game session has the specified <paramref name="profession"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="count">How many players have this profession.</param>
    /// <returns><see langword="true"/> is at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHaveProfession(
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
    internal static void RevalidateFishPondPopulations(this Game1 game1)
    {
        foreach (var building in Game1.getFarm().buildings)
        {
            if (building is FishPond pond &&
                (pond.IsOwnedBy(Game1.player) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                pond.isUnderConstruction())
            {
                pond.UpdateMaximumOccupancy();
            }
        }
    }

    /// <summary>Upgrades the quality of gems or minerals held by all existing Crystalariums owned by <paramref name="who"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="newQuality">The new quality.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal static void GlobalUpgradeCrystalariums(this Game1 game1, int newQuality, Farmer who)
    {
        Utility.ForAllLocations(location =>
        {
            foreach (var @object in location.Objects.Values)
            {
                if (@object.bigCraftable.Value && @object.ParentSheetIndex == (int)Machine.Crystalarium &&
                    (@object.IsOwnedBy(who) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                    @object.heldObject?.Value.Quality < newQuality)
                {
                    @object.heldObject.Value.Quality = newQuality;
                }
            }
        });
    }
}
