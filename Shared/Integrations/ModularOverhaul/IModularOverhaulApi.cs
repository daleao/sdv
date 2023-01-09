namespace DaLion.Shared.Integrations.ModularOverhaul;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public interface IModularOverhaulApi
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
    /// <remarks>
    ///     All this does is increase the level cap for the skill with the specified <paramref name="id"/>.
    ///     The custom Skill mod author is responsible for making sure their professions return the correct
    ///     description and icon when prestiged. To check if a <see cref="Farmer"/> instance has a given prestiged
    ///     profession, simply add 100 to the profession's base ID.
    /// </remarks>
    void RegisterCustomSkillForPrestige(string id);

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="IModularOverhaul.ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IModularOverhaul.ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    bool IsHuntActive(IModularOverhaul.TreasureHuntType type, Farmer? farmer = null);

    /// <inheritdoc cref="IModularOverhaul.ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    bool TryStartNewHunt(GameLocation location, IModularOverhaul.TreasureHuntType type, Farmer? farmer = null);

    /// <inheritdoc cref="IModularOverhaul.ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, IModularOverhaul.TreasureHuntType type, Farmer? farmer = null);

    /// <inheritdoc cref="IModularOverhaul.ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="IModularOverhaul.ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    bool InterruptActiveHunt(IModularOverhaul.TreasureHuntType type, Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when a <see cref="IModularOverhaul.ITreasureHunt"/> begins.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, IModularOverhaul.ITreasureHuntStartedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when a <see cref="IModularOverhaul.ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, IModularOverhaul.ITreasureHuntEndedEventArgs> callback);

    #endregion treasure hunts

    #region ultimates

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="IModularOverhaul.IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="IModularOverhaul.IUltimate"/>, or the local player's if supplied null.</returns>
    IModularOverhaul.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> is activated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IModularOverhaul.IUltimateActivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> is deactivated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IModularOverhaul.IUltimateDeactivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> gains charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IModularOverhaul.IUltimateChargeInitiatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> charge increases.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IModularOverhaul.IUltimateChargeIncreasedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IModularOverhaul.IUltimateFullyChargedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IModularOverhaul.IUltimateEmptiedEventArgs> callback);

    #endregion ultimates

    #region resonances

    /// <summary>Gets the <see cref="IModularOverhaul.IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IModularOverhaul.IChord"/>.</param>
    /// <returns>The <see cref="IModularOverhaul.IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two resonating gemstones, otherwise <see langword="null"/>.</returns>
    public IModularOverhaul.IChord? GetChord(CombinedRing ring);

    #endregion resonances
}
