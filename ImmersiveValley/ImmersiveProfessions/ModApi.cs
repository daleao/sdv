using System;

namespace DaLion.Stardew.Professions;

#region using directives

using StardewValley;

using Extensions;
using Framework;

#endregion using directives

/// <summary>Provides an API for reading this mod's internal saved data.</summary>
public class ModApi
{
    #region mod data

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

    public float GetConservationistTaxBonus(Farmer farmer)
    {
        return farmer.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
    }

    public int GetConservationistTrashCollected(Farmer farmer)
    {
        return farmer.ReadDataAs<int>(DataField.ConservationistTrashCollectedThisSeason);
    }

    #endregion mod data

    #region mod configs

    public uint GetForagesNeededForBestQuality()
    {
        return ModEntry.Config.ForagesNeededForBestQuality;
    }

    public uint GetMineralsNeededForBestQuality()
    {
        return ModEntry.Config.MineralsNeededForBestQuality;
    }

    public uint GetTrashNeededPerTaxLevel()
    {
        return ModEntry.Config.TrashNeededPerTaxLevel;
    }

    public uint GetTrashNeededPerFriendshipPoint()
    {
        return ModEntry.Config.TrashNeededPerFriendshipPoint;
    }

    public float[] GetBaseExperienceMultipliers()
    {
        return ModEntry.Config.BaseSkillExpMultiplierPerSkill;
    }

    public uint GetRequiredExperiencePerExtendedLevel()
    {
        return ModEntry.Config.RequiredExpPerExtendedLevel;
    }

    #endregion mod configs

    #region ultimate

    public int GetRegisteredUltimate()
    {
        return (int) ModEntry.PlayerState.RegisteredUltimate.Index;
    }

    public int GetRegisteredUltimate(Farmer farmer)
    {
        return farmer.ReadDataAs<int>(DataField.UltimateIndex);
    }
    public bool IsUltimateActive()
    {
        return ModEntry.PlayerState.RegisteredUltimate.IsActive;
    }

    public bool IsUltimateCharged()
    {
        return ModEntry.PlayerState.RegisteredUltimate.IsFullyCharged;
    }

    public bool IsUltimateActive(Farmer farmer)
    {
        throw new NotImplementedException();
    }

    public bool IsUltimateCharged(Farmer farmer)
    {
        throw new NotImplementedException();
    }

    public bool IsShowingUltimateMeter()
    {
        return ModEntry.PlayerState.RegisteredUltimate.Meter.IsVisible;
    }

    #endregion
}