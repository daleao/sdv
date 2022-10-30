﻿namespace DaLion.Redux;

#region using directives

using DaLion.Redux.Professions;
using DaLion.Redux.Professions.Events.TreasureHunt;
using DaLion.Redux.Professions.Events.Ultimate;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.TreasureHunts;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Redux.Rings.Resonance;
using DaLion.Redux.Rings.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
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

        return slingshot.GetOvercharge(farmer);
    }

    #endregion professions

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if the specified <see cref="ITreasureHunt"/> <paramref name="type"/> is currently active, otherwise <see langword="false"/>.</returns>
    public bool IsHuntActive(TreasureHuntType type) =>
        type switch
        {
            TreasureHuntType.Prospector => ModEntry.State.Professions.ProspectorHunt.Value.IsActive,
            TreasureHuntType.Scavenger => ModEntry.State.Professions.ScavengerHunt.Value.IsActive,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, bool>(type),
        };

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
    public bool TryStartNewHunt(GameLocation location, TreasureHuntType type) =>
        type switch
        {
            TreasureHuntType.Prospector => Game1.player.HasProfession(Profession.Prospector) &&
                                           ModEntry.State.Professions.ProspectorHunt.Value.TryStart(location),
            TreasureHuntType.Scavenger => Game1.player.HasProfession(Profession.Scavenger) &&
                                          ModEntry.State.Professions.ScavengerHunt.Value.TryStart(location),
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, bool>(type),
        };

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, TreasureHuntType type)
    {
        switch (type)
        {
            case TreasureHuntType.Prospector:
                if (!Game1.player.HasProfession(Profession.Prospector))
                {
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Prospector profession.");
                }

                ModEntry.State.Professions.ProspectorHunt.Value.ForceStart(location, target);
                break;
            case TreasureHuntType.Scavenger:
                if (!Game1.player.HasProfession(Profession.Scavenger))
                {
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Scavenger profession.");
                }

                ModEntry.State.Professions.ScavengerHunt.Value.ForceStart(location, target);
                break;
        }
    }

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns>
    ///     <see langword="false"/> if the <see cref="ITreasureHunt"/> instance was not active, otherwise
    ///     <see langword="true"/>.
    /// </returns>
    public bool InterruptActiveHunt(TreasureHuntType type)
    {
        var hunt = type switch
        {
            TreasureHuntType.Prospector => ModEntry.State.Professions.ProspectorHunt,
            TreasureHuntType.Scavenger => ModEntry.State.Professions.ScavengerHunt,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<TreasureHuntType, Lazy<TreasureHunt>>(type),
        };

        if (!hunt.IsValueCreated || !hunt.Value.IsActive)
        {
            return false;
        }

        hunt.Value.Fail();
        return true;
    }

    /// <summary>Registers a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback)
    {
        var e = new TreasureHuntStartedEvent(callback);
        ModEntry.Events.Manage(e);
        TreasureHunt.Started += e.OnStarted;
        return e;
    }

    /// <summary>Registers a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback)
    {
        var e = new TreasureHuntEndedEvent(callback);
        ModEntry.Events.Manage(e);
        TreasureHunt.Ended += e.OnEnded;
        return e;
    }

    #endregion treasure hunts

    #region ultimate

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
        ModEntry.Events.Manage(e);
        Ultimate.Activated += e.OnActivated;
        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback)
    {
        var e = new UltimateDeactivatedEvent(callback);
        ModEntry.Events.Manage(e);
        Ultimate.Deactivated += e.OnDeactivated;
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback)
    {
        var e = new UltimateChargeInitiatedEvent(callback);
        ModEntry.Events.Manage(e);
        Ultimate.ChargeInitiated += e.OnChargeInitiated;
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback)
    {
        var e = new UltimateChargeIncreasedEvent(callback);
        ModEntry.Events.Manage(e);
        Ultimate.ChargeIncreased += e.OnChargeIncreased;
        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback)
    {
        var e = new UltimateFullyChargedEvent(callback);
        ModEntry.Events.Manage(e);
        Ultimate.FullyCharged += e.OnFullyCharged;
        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <returns>A new <see cref="IManagedEvent"/> instance which encapsulates the specified <paramref name="callback"/>.</returns>
    public IManagedEvent RegisterUltimateEmptiedEvent(
        Action<object?, IUltimateEmptiedEventArgs> callback)
    {
        var e = new UltimateEmptiedEvent(callback);
        ModEntry.Events.Manage(e);
        Ultimate.Emptied += e.OnEmptied;
        return e;
    }

    #endregion ultimate

    #region resonance

    /// <summary>Gets the <see cref="IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IChord"/>.</param>
    /// <returns>The <see cref="IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    public IChord? GetChord(CombinedRing ring)
    {
        return ring.Get_Chord();
    }

    #endregion resonance

    #region configs

    /// <summary>Gets the mod's config instance, which can be used in a read-only way.</summary>
    /// <returns>The <see cref="ModConfig"/> instance.</returns>
    public ModConfig GetConfig()
    {
        return ModEntry.Config;
    }

    #endregion configs
}