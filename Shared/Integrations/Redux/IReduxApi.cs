namespace DaLion.Shared.Redux;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Integrations.Redux;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public interface IReduxApi
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
    /// <param name="getPrestigeDescriptionTier1Path1">A delegate which returns the prestige description for the level-5 profession first path.</param>
    /// <param name="getPrestigeDescriptionTier1Path2">A delegate which returns the prestige description for the level-5 profession second path.</param>
    /// <param name="getPrestigeDescriptionTier2Path1A">A delegate which returns the prestige description for the level-10 profession first path, option A.</param>
    /// <param name="getPrestigeDescriptionTier2Path1B">A delegate which returns the prestige description for the level-10 profession first path, option B.</param>
    /// <param name="getPrestigeDescriptionTier2Path2A">A delegate which returns the prestige description for the level-10 profession second path, option A.</param>
    /// <param name="getPrestigeDescriptionTier2Path2B">A delegate which returns the prestige description for the level-10 profession second path, option B.</param>
    void RegisterCustomSkillPrestige(
        string id,
        Func<string> getPrestigeDescriptionTier1Path1,
        Func<string> getPrestigeDescriptionTier1Path2,
        Func<string> getPrestigeDescriptionTier2Path1A,
        Func<string> getPrestigeDescriptionTier2Path1B,
        Func<string> getPrestigeDescriptionTier2Path2A,
        Func<string> getPrestigeDescriptionTier2Path2B);

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="IRedux.ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IRedux.ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    bool IsHuntActive(IRedux.TreasureHuntType type);

    /// <inheritdoc cref="IRedux.ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    bool TryStartNewHunt(GameLocation location, IRedux.TreasureHuntType type);

    /// <inheritdoc cref="IRedux.ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, IRedux.TreasureHuntType type);

    /// <inheritdoc cref="IRedux.ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="IRedux.ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    bool InterruptActiveHunt(IRedux.TreasureHuntType type);

    /// <summary>Registers a new instance of an event raised when a <see cref="IRedux.ITreasureHunt"/> begins.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, IRedux.ITreasureHuntStartedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when a <see cref="IRedux.ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, IRedux.ITreasureHuntEndedEventArgs> callback);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="IRedux.IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="IRedux.IUltimate"/>, or the local player's if supplied null.</returns>
    IRedux.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> is activated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IRedux.IUltimateActivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> is deactivated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IRedux.IUltimateDeactivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> gains charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IRedux.IUltimateChargeInitiatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> charge increases.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IRedux.IUltimateChargeIncreasedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IRedux.IUltimateFullyChargedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IRedux.IUltimate"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IRedux.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IRedux.IUltimateEmptiedEventArgs> callback);

    #endregion ultimate

    #region resonance

    /// <summary>Gets the <see cref="IRedux.IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IRedux.IChord"/>.</param>
    /// <returns>The <see cref="IRedux.IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    public IRedux.IChord? GetChord(CombinedRing ring);

    #endregion resonance
}
