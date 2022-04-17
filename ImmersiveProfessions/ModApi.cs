namespace DaLion.Stardew.Professions;

#region using directives

using StardewValley;

using Framework;
using Framework.Extensions;

#endregion using directives

/// <summary>Provides an API for reading this mod's internal saved data.</summary>
public class ModApi
{
    public int GetForageQuality(Farmer farmer)
    {
        return farmer.GetEcologistForageQuality();
    }

    public int GetEcologistItemsForaged(Farmer farmer)
    {
        return farmer.ReadDataAs<int>(DataField.EcologistItemsForaged);
    }

    public int GetMineralQuality(Farmer farmer)
    {
        return farmer.GetGemologistMineralQuality();
    }

    public int GetGemologistMineralsCollected(Farmer farmer)
    {
        return farmer.ReadDataAs<int>(DataField.GemologistMineralsCollected);
    }

    public int GetConservationistTrashCollected(Farmer farmer)
    {
        return farmer.ReadDataAs<int>(DataField.ConservationistTrashCollectedThisSeason);
    }

    public float GetConservationistTaxBonus(Farmer farmer)
    {
        return farmer.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
    }

    public int GetRegisteredUltimate(Farmer farmer)
    {
        return (int) ModEntry.PlayerState.RegisteredUltimate.Index;
    }
}