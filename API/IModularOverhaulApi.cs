namespace DaLion.Overhaul.API;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Objects;

#endregion using directives

/// <summary>Interface for the mod API.</summary>
public interface IModularOverhaulApi
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
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    float GetProducerProducePriceBonus(Farmer? farmer = null);

    /// <summary>Gets the price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    float GetAnglerFishPriceBonus(Farmer? farmer = null);

    /// <summary>
    ///     Gets the value of the a Conservationist's effective tax deduction based on the preceding season's trash
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
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, IModularOverhaul.ITreasureHuntStartedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when a <see cref="IModularOverhaul.ITreasureHunt"/> ends.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, IModularOverhaul.ITreasureHuntEndedEventArgs> callback);

    #endregion treasure hunts

    #region limit break

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="IModularOverhaul.IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="IModularOverhaul.IUltimate"/>, or the local player's if supplied null.</returns>
    IModularOverhaul.IUltimate? GetRegisteredUltimate(Farmer? farmer = null);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> is activated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IModularOverhaul.IUltimateActivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> is deactivated.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IModularOverhaul.IUltimateDeactivatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> gains charge from zero.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IModularOverhaul.IUltimateChargeInitiatedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> charge increases.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IModularOverhaul.IUltimateChargeIncreasedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> reaches full charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IModularOverhaul.IUltimateFullyChargedEventArgs> callback);

    /// <summary>Registers a new instance of an event raised when the player's <see cref="IModularOverhaul.IUltimate"/> returns to zero charge.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IModularOverhaul.IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    IModularOverhaul.IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IModularOverhaul.IUltimateEmptiedEventArgs> callback);

    #endregion limit break

    #region resonances

    /// <summary>Gets the <see cref="IModularOverhaul.IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IModularOverhaul.IChord"/>.</param>
    /// <returns>The <see cref="IModularOverhaul.IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two resonating gemstones, otherwise <see langword="null"/>.</returns>
    IModularOverhaul.IChord? GetChord(CombinedRing ring);

    #endregion resonances

    #region status effects

    /// <summary>Causes bleeding on the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="bleeder">The <see cref="Farmer"/> who caused the bleeding.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the bleeding effect (how many stacks).</param>
    public void Bleed(Monster monster, Farmer bleeder, int duration = 30000, int intensity = 1);

    /// <summary>Removes bleeding from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unbleed(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is bleeding.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero bleeding stacks, otherwise <see langword="false"/>.</returns>
    public bool IsBleeding(Monster monster);

    /// <summary>Burns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="burner">The <see cref="Farmer"/> who inflicted the burn.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Burn(Monster monster, Farmer burner, int duration = 15000);

    /// <summary>Removes burn from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unburn(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is burning.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero burn timer, otherwise <see langword="false"/>.</returns>
    public bool IsBurning(Monster monster);

    /// <summary>Chills the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Chill(Monster monster, int duration = 5000);

    /// <summary>Removes chilled status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unchill(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is chilled.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns>The <paramref name="monster"/>'s chilled flag.</returns>
    public bool IsChilled(Monster monster);

    /// <summary>Fears the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Fear(Monster monster, int duration);

    /// <summary>Removes fear from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unfear(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is feared.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero fear timer, otherwise <see langword="false"/>.</returns>
    public bool IsFeared(Monster monster);

    /// <summary>Freezes the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Freeze(Monster monster, int duration = 30000);

    /// <summary>Removes frozen status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Defrost(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is frozen.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero freeze stacks, otherwise <see langword="false"/>.</returns>
    public bool IsFrozen(Monster monster);

    /// <summary>Poisons the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="poisoner">The <see cref="Farmer"/> who inflicted the poison.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the poison effect (how many stacks).</param>
    public void Poison(Monster monster, Farmer poisoner, int duration = 15000, int intensity = 1);

    /// <summary>Removes poison from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Detox(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is poisoned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero poison stacks, otherwise <see langword="false"/>.</returns>
    public bool IsPoisoned(Monster monster);

    /// <summary>Slows the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect.</param>
    public void Slow(Monster monster, int duration, double intensity = 0.5);

    /// <summary>Removes slow from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unslow(Monster monster);

    /// <summary>Checks whether the <paramref name="monster"/> is slowed.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero slow timer, otherwise <see langword="false"/>.</returns>
    public bool IsSlowed(Monster monster);

    /// <summary>Stuns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Stun(Monster monster, int duration);

    /// <summary>Checks whether the <paramref name="monster"/> is stunned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero stun timer, otherwise <see langword="false"/>.</returns>
    public bool IsStunned(Monster monster);

    #endregion status effects

    #region taxes

    /// <summary>Evaluates the due income tax and other relevant stats for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>. Defaults to <see cref="Game1.player"/>.</param>
    /// <returns>The amount of income tax due in gold, along with total income, business expenses, eligible deductions and total taxable amount (in that order).</returns>
    (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateIncomeTax(Farmer? farmer = null);

    /// <summary>Determines the total property value of the farm.</summary>
    /// <returns>The total values of agriculture activities, livestock and buildings on the farm, as well as the total number of tiles used by all of those activities.</returns>
    (int AgricultureValue, int LivestockValue, int BuildingValue, int UsedTiles) CalculatePropertyTax();

    #endregion taxes
}
