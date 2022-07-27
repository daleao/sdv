using DaLion.Stardew.Professions.Framework.VirtualProperties;

namespace DaLion.Stardew.Professions;

#region using directives

using Common.Events;
using Common.ModData;
using Extensions;
using Framework;
using Framework.Events.TreasureHunt;
using Framework.Events.Ultimate;
using Framework.TreasureHunts;
using Framework.Ultimates;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public class ModAPI
{
    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetEcologistForageQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetGemologistMineralQuality(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistProjectedTaxBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        // ReSharper disable once PossibleLossOfFraction
        return ModDataIO.Read<int>(farmer, "ConservationistTrashCollectedThisSeason") /
               ModEntry.Config.TrashNeededPerTaxBonusPct / 100f;
    }

    /// <summary>Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistEffectiveTaxBonus(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return farmer.GetConservationistPriceMultiplier() - 1f;
    }

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">The type of treasure hunt.</param>
    public bool IsHuntActive(TreasureHuntType type) =>
#pragma warning disable CS8524
        type switch
#pragma warning restore CS8524
        {
            TreasureHuntType.Prospector => ModEntry.Player.ProspectorHunt.IsActive,
            TreasureHuntType.Scavenger => ModEntry.Player.ScavengerHunt.IsActive,
        };

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// /// <param name="location">The hunt location.</param>
    /// <param name="type">The type of treasure hunt.</param>
    public bool TryStartNewHunt(GameLocation location, TreasureHuntType type) =>
#pragma warning disable CS8524
        type switch
#pragma warning restore CS8524
        {
            TreasureHuntType.Prospector => Game1.player.HasProfession(Profession.Prospector) &&
                                            ModEntry.Player.ProspectorHunt.TryStart(location),
            TreasureHuntType.Scavenger => Game1.player.HasProfession(Profession.Scavenger) &&
                                            ModEntry.Player.ScavengerHunt.TryStart(location),
        };

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="location">The hunt location.</param>
    /// <param name="target">The target tile.</param>
    /// <param name="type">The type of treasure hunt.</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, TreasureHuntType type)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (type)
        {
            case TreasureHuntType.Prospector:
                if (!Game1.player.HasProfession(Profession.Prospector))
                    throw new InvalidOperationException("Player does not have the Prospector profession.");
                ModEntry.Player.ProspectorHunt.ForceStart(location, target);
                break;
            case TreasureHuntType.Scavenger:
                if (!Game1.player.HasProfession(Profession.Scavenger))
                    throw new InvalidOperationException("Player does not have the Scavenger profession.");
                ModEntry.Player.ScavengerHunt.ForceStart(location, target);
                break;
        }
    }

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="false"> if the <see cref="ITreasureHunt"/> instance was not active, otherwise <see langword="true">.</returns>
    public bool InterruptActiveHunt(TreasureHuntType type)
    {
#pragma warning disable CS8524
        var hunt = type switch
#pragma warning restore CS8524
        {
            TreasureHuntType.Prospector => ModEntry.Player.ProspectorHunt,
            TreasureHuntType.Scavenger => ModEntry.Player.ScavengerHunt,
        };
        if (!hunt.IsActive) return false;

        hunt.Fail();
        return true;
    }

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new TreasureHuntStartedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new TreasureHuntEndedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a player's currently registered combat Ultimate, if any.</summary>
    /// <param name="farmer">The player.</param>
    public IUltimate? GetRegisteredUltimate(Farmer? farmer = null) =>
        farmer is null ? ModEntry.Player.RegisteredUltimate : farmer.get_Ultimate();

    /// <summary>Check whether the <see cref="UltimateHUD"/> is currently visible for the local player.</summary>
    public bool IsShowingUltimateMeter() =>
        ModEntry.Player.RegisteredUltimate?.Hud.IsVisible ?? false;

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateActivatedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateDeactivatedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateChargeInitiatedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateChargeIncreasedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateFullyChargedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    public IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback, bool alwaysHooked = false)
    {
        var e = new UltimateEmptiedEvent(callback, alwaysHooked);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public ModConfig GetConfigs() => ModEntry.Config;

    #endregion configs
}