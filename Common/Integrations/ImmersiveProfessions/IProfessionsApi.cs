namespace DaLion.Stardew.Common.Integrations;

#region using directives

using StardewValley;

#endregion using directives

public interface IProfessionsApi
{
    public int GetForageQuality(Farmer farmer);
    public int GetEcologistItemsForaged(Farmer farmer);
    public int GetMineralQuality(Farmer farmer);
    public int GetGemologistMineralsCollected(Farmer farmer);
    public int GetConservationistTrashCollected(Farmer farmer);
    public float GetConservationistTaxBonus(Farmer farmer);
    public int GetRegisteredUltimate(Farmer farmer);
}