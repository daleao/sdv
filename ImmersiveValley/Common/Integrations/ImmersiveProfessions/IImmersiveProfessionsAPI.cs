namespace DaLion.Common.Integrations.WalkOfLife;

#region using directives

using Microsoft.Xna.Framework;
using System;

#endregion using directives

/// <summary>Interface for the Immersive Professions' API.</summary>
/// <remarks>Version 5.0.2</remarks>
public interface IImmersiveProfessionsAPI
{
    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    int GetEcologistForageQuality(Farmer? farmer = null);

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    int GetGemologistMineralQuality(Farmer? farmer = null);

    /// <summary>The price bonus applied to animal produce sold by Producer.</summary>
    /// <param name="farmer">The player.</param>
    float GetProducerProducePriceBonus(Farmer? farmer = null);

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    float GetAnglerFishPriceBonus(Farmer? farmer = null);

    /// <summary>Get the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    float GetConservationistProjectedTaxBonus(Farmer? farmer = null);

    /// <summary>Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    float GetConservationistEffectiveTaxBonus(Farmer? farmer = null);

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    float GetDesperadoOvercharge(Farmer? farmer = null);

    #region tresure hunts

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    bool IsHuntActive(IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    bool TryStartNewHunt(GameLocation location, IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="false"/> if the <see cref="IImmersiveProfessions.ITreasureHunt"/> instance was not active, otherwise <see langword="true"/>.</returns>
    bool InterruptActiveHunt(IImmersiveProfessions.TreasureHuntType type);

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, IImmersiveProfessions.ITreasureHuntStartedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, IImmersiveProfessions.ITreasureHuntEndedEventArgs> callback, bool alwaysEnabled = false);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a player's currently registered combat Ultimate, if any.</summary>
    IImmersiveProfessions.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether this event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IImmersiveProfessions.IUltimateActivatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IImmersiveProfessions.IUltimateDeactivatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IImmersiveProfessions.IUltimateChargeInitiatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IImmersiveProfessions.IUltimateChargeIncreasedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IImmersiveProfessions.IUltimateFullyChargedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IImmersiveProfessions.IUltimateEmptiedEventArgs> callback, bool alwaysEnabled = false);

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    IImmersiveProfessions.IModConfig GetConfigs();

    #endregion configs
}