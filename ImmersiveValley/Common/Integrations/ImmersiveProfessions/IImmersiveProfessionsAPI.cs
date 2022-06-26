namespace DaLion.Common.Integrations;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using System;

#endregion using directives

/// <summary>Interface for the Immersive Professions' API.</summary>
public interface IImmersiveProfessionsAPI
{
    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    int GetEcologistForageQuality(Farmer farmer);

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    int GetGemologistMineralQuality(Farmer farmer);

    /// <summary>Get the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    float GetConservationistProjectedTaxBonus(Farmer farmer);

    /// <summary>Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    float GetConservationistEffectiveTaxBonus(Farmer farmer);

    #region tresure hunts

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.IsActive"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    bool IsHuntActive(string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.TryStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    bool TryStartNewHunt(GameLocation location, string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.ForceStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.Fail"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    /// <returns><see langword="false"> if the <see cref="IImmersiveProfessions.ITreasureHunt"/> instance was not active, otherwise <see langword="true">.</returns>
    bool InterruptActiveHunt(string type);

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, IImmersiveProfessions.ITreasureHuntStartedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, IImmersiveProfessions.ITreasureHuntEndedEventArgs> callback, bool alwaysHooked = false);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a the local player's currently registered combat Ultimate.</summary>
    IImmersiveProfessions.IUltimate? GetRegisteredUltimate();

    /// <summary>Check whether the <see cref="IImmersiveProfessions.UltimateMeter"/> is currently visible.</summary>
    bool IsShowingUltimateMeter();

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IImmersiveProfessions.IUltimateActivatedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IImmersiveProfessions.IUltimateDeactivatedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IImmersiveProfessions.IUltimateChargeInitiatedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IImmersiveProfessions.IUltimateChargeIncreasedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IImmersiveProfessions.IUltimateFullyChargedEventArgs> callback, bool alwaysHooked = false);

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether to immediately hook the event.</param>
    IImmersiveProfessions.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IImmersiveProfessions.IUltimateEmptiedEventArgs> callback, bool alwaysHooked = false);

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    IImmersiveProfessions.IModConfig GetConfigs();

    #endregion configs
}