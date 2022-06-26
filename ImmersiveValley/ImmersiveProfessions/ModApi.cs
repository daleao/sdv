namespace DaLion.Stardew.Professions;

#region using directives

using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewValley;

using Common.Data;
using Common.Events;
using Extensions;
using Framework;
using Framework.Events.TreasureHunt;
using Framework.Events.Ultimate;
using Framework.TreasureHunts;
using Framework.Ultimates;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public class ModAPI
{
    /// <summary>Get the value of an Ecologist's forage quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetEcologistForageQuality(Farmer farmer) =>
        farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;

    /// <summary>Get the value of a Gemologist's mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetGemologistMineralQuality(Farmer farmer)
    {
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of the a Conservationist's projected tax deduction based on current season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistProjectedTaxBonus(Farmer farmer) =>
        // ReSharper disable once PossibleLossOfFraction
        ModDataIO.ReadDataAs<int>(farmer, ModData.ConservationistTrashCollectedThisSeason.ToString()) /
               ModEntry.Config.TrashNeededPerTaxBonusPct / 100f;

    /// <summary>Get the value of the a Conservationist's effective tax deduction based on the preceding season's trash collection.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistEffectiveTaxBonus(Farmer farmer) =>
        farmer.GetConservationistPriceMultiplier() - 1f;

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool IsHuntActive(string type) =>
        type.ToLowerInvariant() switch
        {
            "prospector" => ModEntry.PlayerState.ProspectorHunt.IsActive,
            "scavenger" => ModEntry.PlayerState.ScavengerHunt.IsActive,
            _ => throw new ArgumentException(
                $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.")
        };

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool TryStartNewHunt(GameLocation location, string type) =>
        type.ToLowerInvariant() switch
        {
            "prospector" => Game1.player.HasProfession(Profession.Prospector) &&
                            ModEntry.PlayerState.ProspectorHunt.TryStart(location),
            "scavenger" => Game1.player.HasProfession(Profession.Scavenger) &&
                           ModEntry.PlayerState.ScavengerHunt.TryStart(location),
            _ => throw new ArgumentException(
                $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.")
        };

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, string type)
    {
        switch (type.ToLowerInvariant())
        {
            case "prospector":
                if (!Game1.player.HasProfession(Profession.Prospector))
                    throw new InvalidOperationException("Player does not have the Prospector profession.");
                ModEntry.PlayerState.ProspectorHunt.ForceStart(location, target);
                break;
            case "scavenger":
                if (!Game1.player.HasProfession(Profession.Scavenger))
                    throw new InvalidOperationException("Player does not have the Scavenger profession.");
                ModEntry.PlayerState.ScavengerHunt.ForceStart(location, target);
                break;
            default:
                throw new ArgumentException(
                    $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.");
        }
    }

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    /// <returns><see langword="false"> if the <see cref="ITreasureHunt"/> instance was not active, otherwise <see langword="true">.</returns>
    public bool InterruptActiveHunt(string type)
    {
        var hunt = type.ToLowerInvariant() switch
        {
            "prospector" => ModEntry.PlayerState.ProspectorHunt,
            "scavenger" => ModEntry.PlayerState.ScavengerHunt,
            _ => throw new ArgumentException(
                $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.")
        };
        if (!hunt.IsActive) return false;

        hunt.Fail();
        return true;
    }

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback, bool hook)
    {
        var e = new TreasureHuntStartedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback, bool hook)
    {
        var e = new TreasureHuntEndedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get the local player's currently registered combat Ultimate.</summary>
    public IUltimate? GetRegisteredUltimate() =>
        ModEntry.PlayerState.RegisteredUltimate;

    /// <summary>Check whether the <see cref="UltimateHUD"/> is currently visible.</summary>
    public bool IsShowingUltimateMeter() =>
        ModEntry.PlayerState.RegisteredUltimate?.Hud.IsVisible ?? false;

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback, bool hook)
    {
        var e = new UltimateActivatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback, bool hook)
    {
        var e = new UltimateDeactivatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback, bool hook)
    {
        var e = new UltimateChargeInitiatedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback, bool hook)
    {
        var e = new UltimateChargeIncreasedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback, bool hook)
    {
        var e = new UltimateFullyChargedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="hook">Whether to immediately hook the event.</param>
    public IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback, bool hook)
    {
        var e = new UltimateEmptiedEvent(callback);
        ModEntry.EventManager.Manage(e);
        if (hook) e.Hook();

        return e;
    }

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public ModConfig GetConfigs() => ModEntry.Config;

    #endregion configs
}