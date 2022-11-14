namespace DaLion.Shared.Ligo;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Integrations.Ligo;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public interface ILigoApi
{
    #region professions

    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetEcologistForageQuality(Farmer? farmer = null);

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetGemologistMineralQuality(Farmer? farmer = null);

    /// <summary>The price bonus applied to animal produce sold by Producer.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    float GetProducerProducePriceBonus(Farmer? farmer = null);

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    float GetAnglerFishPriceBonus(Farmer? farmer = null);

    /// <summary>
    ///     Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash
    ///     collection.
    /// </summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage of tax deductions based currently in effect due to the preceding season's collected trash.</returns>
    float GetConservationistTaxDeduction(Farmer? farmer = null);

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    float GetDesperadoOvercharge(Farmer? farmer = null);

    /// <summary>Sets a flag to allow the specified SpaceCore skill to level past 10 and offer prestige professions.</summary>
    /// <param name="id">The SpaceCore skill id.</param>
    void RegisterCustomSkillForPrestige(string id);

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="ILigo.ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if the specified <see cref="ILigo.ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    bool IsHuntActive(ILigo.TreasureHuntType type);

    /// <inheritdoc cref="ILigo.ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    bool TryStartNewHunt(GameLocation location, ILigo.TreasureHuntType type);

    /// <inheritdoc cref="ILigo.ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, ILigo.TreasureHuntType type);

    /// <inheritdoc cref="ILigo.ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="ILigo.ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    bool InterruptActiveHunt(ILigo.TreasureHuntType type);

    /// <summary>Registers a new instance of an event raised when a <see cref="ILigo.ITreasureHunt"/> begins.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ILigo.ITreasureHuntStartedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when a <see cref="ILigo.ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ILigo.ITreasureHuntEndedEventArgs> callback);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="ILigo.IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="ILigo.IUltimate"/>, or the local player's if supplied null.</returns>
    ILigo.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> is activated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, ILigo.IUltimateActivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> is deactivated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, ILigo.IUltimateDeactivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> gains charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, ILigo.IUltimateChargeInitiatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> charge increases.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, ILigo.IUltimateChargeIncreasedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, ILigo.IUltimateFullyChargedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILigo.IUltimate"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    ILigo.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, ILigo.IUltimateEmptiedEventArgs> callback);

    #endregion ultimate

    #region resonance

    /// <summary>Gets the <see cref="ILigo.IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="ILigo.IChord"/>.</param>
    /// <returns>The <see cref="ILigo.IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    public ILigo.IChord? GetChord(CombinedRing ring);

    #endregion resonance
}
