namespace DaLion.Professions;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.Limits.Events;
using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Professions.Framework.TreasureHunts.Events;
using DaLion.Shared.Events;

#endregion using directive

/// <summary>The public interface for the Professions mod API.</summary>
public interface IProfessionsApi
{
    #region professions

    /// <summary>Gets the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetEcologistForageQuality(Farmer? farmer = null);

    /// <summary>Gets the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    int GetGemologistMineralQuality(Farmer? farmer = null);

    /// <summary>Gets the price bonus applied to animal produce sold by Producer.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A bonus applied to Producer animal product prices.</returns>
    float GetProducerSaleBonus(Farmer? farmer = null);

    /// <summary>Gets the price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A bonus applied to Angler fish prices.</returns>
    float GetAnglerSaleBonus(Farmer? farmer = null);

    /// <summary>
    ///     Gets the value of the Conservationist's effective tax deduction based on the preceding season's trash
    ///     collection.
    /// </summary>
    /// <param name="farmer">The player.</param>
    /// <returns>The percentage of tax deductions currently in effect due to the preceding season's collected trash.</returns>
    float GetConservationistTaxDeduction(Farmer? farmer = null);

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    float GetDesperadoOvercharge(Farmer? farmer = null);

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    bool IsHuntActive(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when a <see cref="ITreasureHunt"/> begins.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when a <see cref="ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback);

    #endregion treasure hunts

    #region limit break

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="ILimitBreak"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="ILimitBreak"/>'s technical name, or the local player's if supplied null.</returns>
    int GetLimitBreakId(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> is activated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitActivatedEvent(Action<object?, ILimitActivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> is deactivated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitDeactivatedEvent(Action<object?, ILimitDeactivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> gains charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitChargeInitiatedEvent(Action<object?, ILimitChargeInitiatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> charge increases.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitChargeIncreasedEvent(Action<object?, ILimitChargeChangedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitFullyChargedEvent(Action<object?, ILimitFullyChargedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="ILimitBreak"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IManagedEvent RegisterLimitEmptiedEvent(Action<object?, ILimitEmptiedEventArgs> callback);

    #endregion limit break

    /// <summary>Gets the mod's current config schema.</summary>
    /// <returns>The current <see cref="ProfessionsConfig"/> instance.</returns>
    ProfessionsConfig GetConfig();
}
