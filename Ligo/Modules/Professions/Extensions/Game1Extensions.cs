namespace DaLion.Ligo.Modules.Professions.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Collections;
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
        Game1.getFarm().buildings.OfType<FishPond>()
            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer ||
                         ProfessionsModule.Config.LaxOwnershipRequirements) && !p.isUnderConstruction())
            .ForEach(p => p.UpdateMaximumOccupancy());
    }

    /// <summary>Upgrades the quality of gems or minerals held by all existing Crystalariums owned by <paramref name="who"/>.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <param name="newQuality">The new quality.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal static void GlobalUpgradeCrystalariums(this Game1 game1, int newQuality, Farmer who)
    {
        Utility.ForAllLocations(location =>
        {
            location.Objects.Values
                .Where(o =>
                    o.bigCraftable.Value && o.ParentSheetIndex == (int)Machine.Crystalarium &&
                    (o.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer ||
                     ProfessionsModule.Config.LaxOwnershipRequirements) &&
                    o.heldObject.Value?.Quality < newQuality)
                .ForEach(crystalarium => crystalarium.heldObject.Value.Quality = newQuality);
        });
    }
}
