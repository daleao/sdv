namespace DaLion.Common.Integrations;

#region using directives

using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewValley;

#endregion using directives

public interface IImmersiveProfessionsAPI
{
    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetEcologistForageQuality(Farmer farmer);

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetGemologistMineralQuality(Farmer farmer);

    /// <summary>Get the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistProjectedTaxBonus(Farmer farmer);

    /// <summary>Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistEffectiveTaxBonus(Farmer farmer);

    #region tresure hunts

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.IsActive"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool IsHuntActive(string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.TryStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool TryStartNewHunt(GameLocation location, string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.ForceStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, string type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.Fail"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    /// <returns><see langword="false"> if the <see cref="IImmersiveProfessions.ITreasureHunt"/> instance was not active, otherwise <see langword="true">.</returns>
    public bool InterruptActiveHunt(string type);

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterTreasureHuntStartedEvent(Action<object, IImmersiveProfessions.ITreasureHuntStartedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterTreasureHuntEndedEvent(Action<object, IImmersiveProfessions.ITreasureHuntEndedEventArgs> callback, bool hook = true);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a string representation of the local player's currently registered combat Ultimate.</summary>
    public string GetRegisteredUltimate();

    /// <summary>Check whether the <see cref="IImmersiveProfessions.UltimateMeter"/> is currently visible.</summary>
    public bool IsShowingUltimateMeter();

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateActivatedEvent(Action<object, IImmersiveProfessions.IUltimateActivatedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateDeactivatedEvent(Action<object, IImmersiveProfessions.IUltimateDeactivatedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateChargeInitiatedEvent(Action<object, IImmersiveProfessions.IUltimateChargeInitiatedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateChargeIncreasedEvent(Action<object, IImmersiveProfessions.IUltimateChargeIncreasedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateFullyChargedEvent(Action<object, IImmersiveProfessions.IUltimateFullyChargedEventArgs> callback, bool hook = true);

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IImmersiveProfessions.IEvent RegisterUltimateEmptiedEvent(Action<object, IImmersiveProfessions.IUltimateEmptiedEventArgs> callback,bool hook = true);

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public JObject GetConfigs();

    #endregion configs
}