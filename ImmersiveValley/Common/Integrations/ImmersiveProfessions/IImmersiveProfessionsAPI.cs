namespace DaLion.Common.Integrations.ImmersiveProfessions;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The API provided by Immersive Professions.</summary>
/// <remarks>Version 5.2.0.</remarks>
public interface IImmersiveProfessionsApi
{
    /// <summary>Gets the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetEcologistForageQuality(Farmer? farmer = null);

    /// <summary>Gets the value of a Gemologist's mineral quality.</summary>
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

    /// <summary>Gets the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage of tax deductions based current season's collected trash.</returns>
    float GetConservationistProjectedTaxBonus(Farmer? farmer = null);

    /// <summary>
    ///     Gets the value of the a Conservationist's effective tax deduction based on the preceding season's trash
    ///     collection.
    /// </summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage of tax deductions based currently in effect due to the preceding season's collected trash.</returns>
    float GetConservationistEffectiveTaxBonus(Farmer? farmer = null);

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    float GetDesperadoOvercharge(Farmer? farmer = null);

    #region configs

    /// <summary>Gets an interface for this mod's config settings.</summary>
    /// <returns>The <see cref="IImmersiveProfessions.IModConfig"/>.</returns>
    IImmersiveProfessions.IModConfig GetConfigs();

    #endregion configs

    #region tresure hunts

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IImmersiveProfessions.ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    bool IsHuntActive(IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    bool TryStartNewHunt(GameLocation location, IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The tarGets tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    void ForceStartNewHunt(GameLocation location, Vector2 target, IImmersiveProfessions.TreasureHuntType type);

    /// <inheritdoc cref="IImmersiveProfessions.ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="IImmersiveProfessions.ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    bool InterruptActiveHunt(IImmersiveProfessions.TreasureHuntType type);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.ITreasureHunt"/> begins.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntStartedEvent(
        Action<object?, IImmersiveProfessions.ITreasureHuntStartedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterTreasureHuntEndedEvent(
        Action<object?, IImmersiveProfessions.ITreasureHuntEndedEventArgs> callback, bool alwaysEnabled = false);

    #endregion treasure hunts

    #region ultimate

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="IImmersiveProfessions.IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="IImmersiveProfessions.IUltimate"/>, or the local player's if supplied null.</returns>
    IImmersiveProfessions.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> activates.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateActivatedEvent(
        Action<object?, IImmersiveProfessions.IUltimateActivatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> deactivates.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateDeactivatedEvent(
        Action<object?, IImmersiveProfessions.IUltimateDeactivatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> gains any charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeInitiatedEvent(
        Action<object?, IImmersiveProfessions.IUltimateChargeInitiatedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> gains any charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateChargeIncreasedEvent(
        Action<object?, IImmersiveProfessions.IUltimateChargeIncreasedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateFullyChargedEvent(
        Action<object?, IImmersiveProfessions.IUltimateFullyChargedEventArgs> callback, bool alwaysEnabled = false);

    /// <summary>Registers a new instance of an event that is raised when a <see cref="IImmersiveProfessions.IUltimate"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    /// <returns>A new <see cref="IImmersiveProfessions.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IImmersiveProfessions.IManagedEvent RegisterUltimateEmptiedEvent(
        Action<object?, IImmersiveProfessions.IUltimateEmptiedEventArgs> callback, bool alwaysEnabled = false);

    #endregion ultimate
}
