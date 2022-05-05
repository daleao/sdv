namespace DaLion.Stardew.Professions;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

using Extensions;
using Framework;
using Framework.Events;
using Framework.Events.TreasureHunt;
using Framework.Events.Ultimate;
using Framework.TreasureHunt;
using Framework.Ultimate;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public class ModAPI
{
    /// <summary>Get the value of a farmer's Ecologist forage quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetForageQuality(Farmer farmer)
    {
        return farmer.HasProfession(Profession.Ecologist) ? farmer.GetEcologistForageQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of a farmer's Gemologist mineral quality.</summary>
    /// <param name="farmer">The player.</param>
    public int GetMineralQuality(Farmer farmer)
    {
        return farmer.HasProfession(Profession.Gemologist) ? farmer.GetGemologistMineralQuality() : SObject.lowQuality;
    }

    /// <summary>Get the value of the a farmer's Conservationist taxation price multiplier.</summary>
    /// <param name="farmer">The player.</param>
    public float GetConservationistTaxBonus(Farmer farmer)
    {
        return farmer.GetConservationistPriceMultiplier() - 1f;
    }

    #region tresure hunts

    /// <inheritdoc cref="ITreasureHunt.IsActive"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool IsHuntActive(string type)
    {
        return type.ToLower() switch
        {
            "prospector" => ModEntry.PlayerState.ProspectorHunt.IsActive,
            "scavenger" => ModEntry.PlayerState.ScavengerHunt.IsActive,
            _ => throw new ArgumentException(
                $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.")
        };
    }

    /// <inheritdoc cref="ITreasureHunt.TryStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public bool TryStartNewHunt(GameLocation location, string type)
    {
        return type.ToLower() switch
        {
            "prospector" => Game1.player.HasProfession(Profession.Prospector) &&
                            ModEntry.PlayerState.ProspectorHunt.TryStart(location),
            "scavenger" => Game1.player.HasProfession(Profession.Scavenger) &&
                           ModEntry.PlayerState.ScavengerHunt.TryStart(location),
            _ => throw new ArgumentException(
                $"{type} is not a valid Treasure Hunt type. Should be either Prospector or Scavenger.")
        };
    }

    /// <inheritdoc cref="ITreasureHunt.ForceStart"/>
    /// <param name="type">Either "Prospector" or "Scavenger" (case insensitive).</param>
    public void ForceStartNewHunt(GameLocation location, Vector2 target, string type)
    {
        switch (type.ToLower())
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
    /// <returns><c>False</c> if the <see cref="ITreasureHunt"/> instance was not active, otherwise <c>true</c>.</returns>
    public bool InterruptActiveHunt(string type)
    {
        var hunt = type.ToLower() switch
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
    public IEvent RegisterTreasureHuntStartedEvent(Action<object, ITreasureHuntStartedEventArgs> callback, bool enable)
    {
        var e = new TreasureHuntStartedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterTreasureHuntEndedEvent(Action<object, ITreasureHuntEndedEventArgs> callback, bool enable)
    {
        var e = new TreasureHuntEndedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a string representation of the local player's currently registered combat Ultimate.</summary>
    public string GetRegisteredUltimate()
    {
        return ModEntry.PlayerState.RegisteredUltimate.ToString();
    }

    /// <summary>Check whether the <see cref="UltimateMeter"/> is currently visible.</summary>
    public bool IsShowingUltimateMeter()
    {
        return ModEntry.PlayerState.RegisteredUltimate.Meter.IsVisible;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateActivatedEvent(Action<object, IUltimateActivatedEventArgs> callback, bool enable)
    {
        var e = new UltimateActivatedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateDeactivatedEvent(Action<object, IUltimateDeactivatedEventArgs> callback, bool enable)
    {
        var e = new UltimateDeactivatedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateChargeInitiatedEvent(Action<object, IUltimateChargeInitiatedEventArgs> callback, bool enable)
    {
        var e = new UltimateChargeInitiatedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateChargeIncreasedEvent(Action<object, IUltimateChargeIncreasedEventArgs> callback, bool enable)
    {
        var e = new UltimateChargeIncreasedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateFullyChargedEvent(Action<object, IUltimateFullyChargedEventArgs> callback, bool enable)
    {
        var e = new UltimateFullyChargedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    public IEvent RegisterUltimateEmptiedEvent(Action<object, IUltimateEmptiedEventArgs> callback, bool enable)
    {
        var e = new UltimateEmptiedEvent(callback);
        EventManager.Manage(e);
        if (enable) e.Enable();

        return e;
    }

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public ModConfig GetConfigs()
    {
        return ModEntry.Config;
    }

    #endregion configs
}