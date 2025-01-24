namespace DaLion.Taxes.Framework;

#region using directives

using static System.FormattableString;

#endregion using directives

/// <summary>Collects federal taxes.</summary>
internal static class CountyService
{
    private static readonly Func<double, double> Sigmoid = x => 1d / (1d + Math.Exp(-(x > 200 ? 0.005 : 0.01) * (x - 200)));

    /// <summary>Calculates due property tax for farm.</summary>
    /// <returns>The amount of income tax due in gold, along with other relevant stats.</returns>
    internal static (int Valuation, int Due) CalculateTaxes()
    {
        var farm = Game1.getFarm();
        var agricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var livestockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var artisanValue = Data.ReadAs<int>(farm, DataKeys.ArtisanValue);
        var buildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        var treeCount = Data.ReadAs<int>(farm, DataKeys.TreeCount);
        var usableTiles = Data.ReadAs(farm, DataKeys.UsableTiles, -1) - treeCount;
        if (usableTiles < 0)
        {
            return (0, 0);
        }

        var usedTiles = Data.ReadAs<int>(farm, DataKeys.UsedTiles);
        var unusedTiles = usableTiles - usedTiles;
        var currentUsePct = (float)usedTiles / usableTiles;
        var rawGoodsValue = agricultureValue + livestockValue;
        var artisanValueWeight = Sigmoid(Game1.game1.CountPlayerArtisanMachines());
        var totalValuation =
            (int)((rawGoodsValue * (1f - artisanValueWeight)) + (artisanValue * artisanValueWeight));
        var owedOverUsedLand = (int)(totalValuation * currentUsePct * Config.UsedTileTaxRate);
        var owedOverUnusedLand = (int)(totalValuation * (1f - currentUsePct) * Config.UnusedTileTaxRate) +
                                 (Config.BaselineUnusedTileCost * unusedTiles);
        var owedOverBuildings = (int)(buildingValue * Config.BuildingTaxRate);
        var amountDue = owedOverUsedLand + owedOverUnusedLand + owedOverBuildings;
        Log.I($@"
            Use-Value Assessment Report for {farm.Name}
            =======================================================================
            Farming & Land Usage:
                - Agricultural Value:            {agricultureValue}g
                - Livestock Value:               {livestockValue}g
                - Estimated Artisan Value:       {artisanValue}g
                - Weight of Artisan Value:       {artisanValueWeight:0.0%}
                - Total Valuation:               {totalValuation}g

                - Ecological Exemptions (trees): {treeCount} tiles
                - Used Tiles:                    {usedTiles} ({currentUsePct:0.0%})

                - Tax Owed on Used Land:         {owedOverUsedLand}g
                - Tax Owed on Unused Land:       {owedOverUnusedLand}g

            Real-Estate:
                - Real-Estate Value:             {buildingValue}g
                - Tax Owed on Real Estate:       {owedOverBuildings}g

            Total Tax Owed:                      {amountDue}g

            Generated on {Game1.currentSeason} {Game1.dayOfMonth}, Year {Game1.year}
            =======================================================================
        ");

        return (totalValuation, amountDue);
    }
}
