namespace DaLion.Taxes.Framework.Extensions;

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
internal static class Game1Extensions
{
    /// <summary>Counts the total number of placed Artisan Machines in the game world.</summary>
    /// <param name="game1">The <see cref="Game1"/> instance.</param>
    /// <returns>The total number of placed Artisan Machines in the game world.</returns>
    internal static int CountPlayerArtisanMachines(this Game1 game1)
    {
        var playersIds = Game1.getAllFarmers().Select(farmer => farmer.UniqueMultiplayerID).ToHashSet();
        var count = 0;
        Utility.ForEachLocation(location =>
        {
            count += location.Objects.Values.Count(@object => @object.IsArtisanMachine() && playersIds.Contains(@object.owner.Value));
            return true; // continue enumeration
        });

        return count;
    }
}
