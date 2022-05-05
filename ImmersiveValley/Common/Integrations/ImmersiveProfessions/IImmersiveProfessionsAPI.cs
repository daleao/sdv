namespace DaLion.Common.Integrations;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

/// <summary>Interface for the mod API.</summary>
public interface IImmersiveProfessionsAPI
{
    /// <summary>Get the value of a farmer's Ecologist forage quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetForageQuality(Farmer farmer);

    /// <summary>Get the value of a farmer's Gemologist mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetMineralQuality(Farmer farmer);

    /// <summary>Get the value of a farmer's Conservationist taxation price multiplier.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistTaxBonus(Farmer farmer);

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool IsHuntActive(string type);

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool TryStartNewHunt(GameLocation location, string type);

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, string type);

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    /// <returns><c>False</c> if the <see cref="ITreasureHunt"/> instance was not active, otherwise <c>true</c>.</returns>
    public bool InterruptActiveHunt(string type);

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterTreasureHuntStartedEvent(Action<object, IImmersiveProfessions.ITreasureHuntStartedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterTreasureHuntEndedEvent(Action<object, IImmersiveProfessions.ITreasureHuntEndedEventArgs> callback, bool enable = true);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a string representation of the local player's currently registered combat Ultimate.</summary>
    public string GetRegisteredUltimate();

    /// <summary>Check whether the <see cref="UltimateMeter"/> is currently visible.</summary>
    public bool IsShowingUltimateMeter();

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateActivatedEvent(Action<object, IImmersiveProfessions.IUltimateActivatedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateDeactivatedEvent(Action<object, IImmersiveProfessions.IUltimateDeactivatedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateChargeInitiatedEvent(Action<object, IImmersiveProfessions.IUltimateChargeInitiatedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateChargeIncreasedEvent(Action<object, IImmersiveProfessions.IUltimateChargeIncreasedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateFullyChargedEvent(Action<object, IImmersiveProfessions.IUltimateFullyChargedEventArgs> callback, bool enable = true);

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateEmptiedEvent(Action<object, IImmersiveProfessions.IUltimateEmptiedEventArgs> callback,bool enable = true);

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public IImmersiveProfessions.IProfessionsConfig GetConfigs();

    #endregion configs
}