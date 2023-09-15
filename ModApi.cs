namespace DaLion.Overhaul;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntEnded;
using DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntStarted;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Activated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeIncreased;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeInitiated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Deactivated;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.Emptied;
using DaLion.Overhaul.Modules.Professions.Events.Ultimate.FullyCharged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Overhaul.Modules.Taxes;
using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public sealed class ModApi
{
    #region professions

    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    public int GetEcologistForageQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    public int GetGemologistMineralQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <summary>The price bonus applied to animal produce sold by Producer.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    public float GetProducerProducePriceBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetProducerPriceBonus();
    }

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    public float GetAnglerFishPriceBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetAnglerPriceBonus();
    }

    /// <summary>
    ///     Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash
    ///     collection.
    /// </summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage of tax deductions based currently in effect due to the preceding season's collected trash.</returns>
    public float GetConservationistTaxDeduction(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetConservationistPriceMultiplier() - 1f;
    }

    /// <summary>Determines the extra power of Desperado shots.</summary>
    /// <param name="farmer">The player.</param>
    /// <returns>A percentage between 0 and 1.</returns>
    public float GetDesperadoOvercharge(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        if (farmer.CurrentTool is not Slingshot slingshot || !farmer.usingSlingshot)
        {
            return 0f;
        }

        return slingshot.GetOvercharge();
    }

    /// <summary>Sets a flag to allow the specified SpaceCore skill to level past 10 and offer prestige professions.</summary>
    /// <param name="id">The SpaceCore skill id.</param>
    public void RegisterCustomSkillForPrestige(string id)
    {
        if (!SCSkill.Loaded.TryGetValue(id, out var skill))
        {
            ThrowHelper.ThrowInvalidOperationException($"The custom skill {id} is not loaded.");
        }

        ((SCSkill)skill).CanPrestige = true;
    }

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    public bool IsHuntActive(TreasureHuntType type, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return type switch
        {
            TreasureHuntType.Prospector => farmer.Get_ProspectorHunt().IsActive,
            TreasureHuntType.Scavenger => farmer.Get_ScavengerHunt().IsActive,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, bool>(type),
        };
    }

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    public bool TryStartNewHunt(GameLocation location, TreasureHuntType type, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return type switch
        {
            TreasureHuntType.Prospector => Game1.player.HasProfession(Profession.Prospector) &&
                                           farmer.Get_ProspectorHunt().TryStart(location),
            TreasureHuntType.Scavenger => Game1.player.HasProfession(Profession.Scavenger) &&
                                          farmer.Get_ScavengerHunt().TryStart(location),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, bool>(type),
        };
    }

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, TreasureHuntType type, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        switch (type)
        {
            case TreasureHuntType.Prospector:
                if (!Game1.player.HasProfession(Profession.Prospector))
                {
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Prospector profession.");
                }

                farmer.Get_ProspectorHunt().ForceStart(location, target);
                break;
            case TreasureHuntType.Scavenger:
                if (!Game1.player.HasProfession(Profession.Scavenger))
                {
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Scavenger profession.");
                }

                farmer.Get_ScavengerHunt().ForceStart(location, target);
                break;
            default:
                ThrowHelperExtensions.ThrowUnexpectedEnumValueException(type);
                return;
        }
    }

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    public bool InterruptActiveHunt(TreasureHuntType type, Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        var hunt = type switch
        {
            TreasureHuntType.Prospector => farmer.Get_ProspectorHunt(),
            TreasureHuntType.Scavenger => farmer.Get_ScavengerHunt(),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, TreasureHunt>(type),
        };

        if (!hunt.IsActive)
        {
            return false;
        }

        hunt.Fail();
        return true;
    }

    /// <summary>Registers a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback)
    {
        var e = new TreasureHuntStartedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Registers a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback)
    {
        var e = new TreasureHuntEndedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion treasure hunts

    #region limit break

    /// <summary>Gets the <paramref name="farmer"/>'s currently registered <see cref="IUltimate"/>, if any.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <paramref name="farmer"/>'s <see cref="IUltimate"/>, or the local player's if supplied null.</returns>
    public IUltimate? GetRegisteredUltimate(Farmer? farmer = null)
    {
        return farmer is null ? Game1.player.Get_Ultimate() : farmer.Get_Ultimate();
    }

    /// <summary>Registers a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback)
    {
        var e = new UltimateActivatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback)
    {
        var e = new UltimateDeactivatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback)
    {
        var e = new UltimateChargeInitiatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback)
    {
        var e = new UltimateChargeIncreasedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback)
    {
        var e = new UltimateFullyChargedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateEmptiedEvent(
        Action<object?, IUltimateEmptiedEventArgs> callback)
    {
        var e = new UltimateEmptiedEvent(callback);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion limit break

    #region resonance

    /// <summary>Gets the <see cref="IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IChord"/>.</param>
    /// <returns>The <see cref="IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    public IChord? GetChord(CombinedRing ring)
    {
        return ring.Get_Chord();
    }

    #endregion resonance

    #region status effects

    /// <summary>Causes bleeding on the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="bleeder">The <see cref="Farmer"/> who caused the bleeding.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the bleeding effect (how many stacks).</param>
    public void Bleed(Monster monster, Farmer bleeder, int duration = 30000, int intensity = 1)
    {
        monster.Bleed(bleeder, duration, intensity);
    }

    /// <summary>Removes bleeding from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unbleed(Monster monster)
    {
        monster.Unbleed();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is bleeding.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero bleeding stacks, otherwise <see langword="false"/>.</returns>
    public bool IsBleeding(Monster monster)
    {
        return monster.IsBleeding();
    }

    /// <summary>Burns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="burner">The <see cref="Farmer"/> who inflicted the burn.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Burn(Monster monster, Farmer burner, int duration = 15000)
    {
        monster.Burn(burner, duration);
    }

    /// <summary>Removes burn from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unburn(Monster monster)
    {
        monster.Unburn();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is burning.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero burn timer, otherwise <see langword="false"/>.</returns>
    public bool IsBurning(Monster monster)
    {
        return monster.IsBurning();
    }

    /// <summary>Chills the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Chill(Monster monster, int duration = 5000)
    {
        monster.Chill(duration);
    }

    /// <summary>Removes chilled status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unchill(Monster monster)
    {
        monster.Unchill();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is chilled.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns>The <paramref name="monster"/>'s chilled flag.</returns>
    public bool IsChilled(Monster monster)
    {
        return monster.IsChilled();
    }

    /// <summary>Fears the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Fear(Monster monster, int duration)
    {
        monster.Fear(duration);
    }

    /// <summary>Removes fear from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unfear(Monster monster)
    {
        monster.Unfear();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is feared.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero fear timer, otherwise <see langword="false"/>.</returns>
    public bool IsFeared(Monster monster)
    {
        return monster.IsFeared();
    }

    /// <summary>Freezes the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Freeze(Monster monster, int duration = 30000)
    {
        monster.Freeze(duration);
    }

    /// <summary>Removes frozen status from the <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Defrost(Monster monster)
    {
        monster.Defrost();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is frozen.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero freeze stacks, otherwise <see langword="false"/>.</returns>
    public bool IsFrozen(Monster monster)
    {
        return monster.IsFrozen();
    }

    /// <summary>Poisons the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="poisoner">The <see cref="Farmer"/> who inflicted the poison.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the poison effect (how many stacks).</param>
    public void Poison(Monster monster, Farmer poisoner, int duration = 15000, int intensity = 1)
    {
        monster.Poison(poisoner, duration, intensity);
    }

    /// <summary>Removes poison from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Detox(Monster monster)
    {
        monster.Detox();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is poisoned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero poison stacks, otherwise <see langword="false"/>.</returns>
    public bool IsPoisoned(Monster monster)
    {
        return monster.IsPoisoned();
    }

    /// <summary>Slows the <paramref name="monster"/> for the specified <paramref name="duration"/> and with the specified <paramref name="intensity"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="intensity">The intensity of the slow effect.</param>
    public void Slow(Monster monster, int duration, double intensity = 0.5)
    {
        monster.Slow(duration, intensity);
    }

    /// <summary>Removes slow from <paramref name="monster"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    public void Unslow(Monster monster)
    {
        monster.Unslow();
    }

    /// <summary>Checks whether the <paramref name="monster"/> is slowed.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero slow timer, otherwise <see langword="false"/>.</returns>
    public bool IsSlowed(Monster monster)
    {
        return monster.IsSlowed();
    }

    /// <summary>Stuns the <paramref name="monster"/> for the specified <paramref name="duration"/>.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public void Stun(Monster monster, int duration)
    {
        monster.Stun(duration);
    }

    /// <summary>Checks whether the <paramref name="monster"/> is stunned.</summary>
    /// <param name="monster">The <see cref="Monster"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="monster"/> has non-zero stun timer, otherwise <see langword="false"/>.</returns>
    public bool IsStunned(Monster monster)
    {
        return monster.IsStunned();
    }

    #endregion status effects

    #region taxes

    /// <summary>Evaluates the due income tax and other relevant stats for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>. Defaults to <see cref="Game1.player"/>.</param>
    /// <returns>The amount of income tax due in gold, along with total income, business expenses, eligible deductions and total taxable amount (in that order).</returns>
    public (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateIncomeTax(Farmer? farmer = null)
    {
        return RevenueService.CalculateTaxes(farmer ?? Game1.player);
    }

    /// <summary>Determines the total property value of the farm.</summary>
    /// <returns>The total values of agriculture activities, livestock and buildings on the farm, as well as the total number of tiles used by all of those activities.</returns>
    public (int AgricultureValue, int LivestockValue, int BuildingValue, int UsedTiles) CalculatePropertyTax()
    {
        return Game1.getFarm().Appraise(false);
    }

    #endregion taxes

    #region configs

    /// <summary>Gets the mod's config instance, which can be used in a read-only way.</summary>
    /// <returns>The <see cref="ModConfig"/> instance.</returns>
    public ModConfig GetConfig()
    {
        return ModEntry.Config;
    }

    #endregion configs
}
