namespace DaLion.Professions;

#region using directives

using DaLion.Professions.Framework.Events.Limit.Activated;
using DaLion.Professions.Framework.Events.Limit.ChargeIncreased;
using DaLion.Professions.Framework.Events.Limit.ChargeInitiated;
using DaLion.Professions.Framework.Events.Limit.Deactivated;
using DaLion.Professions.Framework.Events.Limit.Emptied;
using DaLion.Professions.Framework.Events.Limit.FullyCharged;
using DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntEnded;
using DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntStarted;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewValley.Tools;

#endregion using directive

/// <summary>The <see cref="ProfessionsMod"/> API implementation.</summary>
public class ProfessionsApi : IProfessionsApi
{
    #region professions

    /// <inheritdoc />
    public int GetEcologistForageQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;
    }

    /// <inheritdoc />
    public int GetGemologistMineralQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <inheritdoc />
    public float GetProducerSaleBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetProducerSaleBonus() + 1f;
    }

    /// <inheritdoc />
    public float GetAnglerSaleBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetAnglerSaleBonus() + 1f;
    }

    /// <inheritdoc />
    public float GetConservationistTaxDeduction(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return Data.ReadAs<float>(farmer, DataKeys.ConservationistActiveTaxDeduction);
    }

    /// <inheritdoc />
    public float GetDesperadoOvercharge(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        if (farmer.CurrentTool is not Slingshot slingshot || !farmer.usingSlingshot)
        {
            return 0f;
        }

        return slingshot.GetOvercharge();
    }

    #endregion professions

    #region tresure hunts

    /// <inheritdoc />
    public bool IsHuntActive(Farmer? farmer = null)
    {
        return farmer?.Get_IsHuntingTreasure().Value ??
               (State.ProspectorHunt?.IsActive == true || State.ScavengerHunt?.IsActive == true);
    }

    /// <inheritdoc />
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback)
    {
        var e = new TreasureHuntStartedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback)
    {
        var e = new TreasureHuntEndedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    #endregion treasure hunts

    #region limit break

    /// <inheritdoc />
    public string GetLimitBreakId(Farmer? farmer = null)
    {
        return farmer?.Get_LimitBreakId().Value ?? State.LimitBreak?.Name ?? string.Empty;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitActivatedEvent(Action<object?, ILimitActivatedEventArgs> callback)
    {
        var e = new LimitActivatedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitDeactivatedEvent(Action<object?, ILimitDeactivatedEventArgs> callback)
    {
        var e = new LimitDeactivatedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitChargeInitiatedEvent(Action<object?, ILimitChargeInitiatedEventArgs> callback)
    {
        var e = new LimitChargeInitiatedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitChargeIncreasedEvent(Action<object?, ILimitChargeIncreasedEventArgs> callback)
    {
        var e = new LimitChargeIncreasedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitFullyChargedEvent(Action<object?, ILimitFullyChargedEventArgs> callback)
    {
        var e = new LimitFullyChargedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    /// <inheritdoc />
    public IManagedEvent RegisterLimitEmptiedEvent(
        Action<object?, ILimitEmptiedEventArgs> callback)
    {
        var e = new LimitEmptiedEvent(callback);
        ProfessionsMod.EventManager.Manage(e);
        return e;
    }

    #endregion limit break
}
