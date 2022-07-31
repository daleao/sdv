namespace DaLion.Stardew.Professions;

#region using directives

using Common.Events;
using Common.Extensions.Stardew;
using Extensions;
using Framework;
using Framework.Events.TreasureHunt;
using Framework.Events.Ultimate;
using Framework.TreasureHunts;
using Framework.Ultimates;
using Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using System;

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
        return farmer.Read<int>("ConservationistTrashCollectedThisSeason") /
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
            TreasureHuntType.Prospector => ModEntry.State.ProspectorHunt.Value.IsActive,
            TreasureHuntType.Scavenger => ModEntry.State.ScavengerHunt.Value.IsActive,
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
                                            ModEntry.State.ProspectorHunt.Value.TryStart(location),
            TreasureHuntType.Scavenger => Game1.player.HasProfession(Profession.Scavenger) &&
                                            ModEntry.State.ScavengerHunt.Value.TryStart(location),
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
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Prospector profession.");
                ModEntry.State.ProspectorHunt.Value.ForceStart(location, target);
                break;
            case TreasureHuntType.Scavenger:
                if (!Game1.player.HasProfession(Profession.Scavenger))
                    ThrowHelper.ThrowInvalidOperationException("Player does not have the Scavenger profession.");
                ModEntry.State.ScavengerHunt.Value.ForceStart(location, target);
                break;
        }
    }

    /// <inheritdoc cref="ITreasureHunt.Fail"/>
    /// <param name="type">The type of treasure hunt.</param>
    /// <returns><see langword="false"/> if the <see cref="ITreasureHunt"/> instance was not active, otherwise <see langword="true"/>.</returns>
    public bool InterruptActiveHunt(TreasureHuntType type)
    {
#pragma warning disable CS8524
        var hunt = type switch
#pragma warning restore CS8524
        {
            TreasureHuntType.Prospector => ModEntry.State.ProspectorHunt,
            TreasureHuntType.Scavenger => ModEntry.State.ScavengerHunt,
        };
        if (!hunt.IsValueCreated || !hunt.Value.IsActive) return false;

        hunt.Value.Fail();
        return true;
    }

    /// <summary>Register a new <see cref="TreasureHuntStartedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterTreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new TreasureHuntStartedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="TreasureHuntEndedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterTreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new TreasureHuntEndedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion treasure hunts

    #region ultimate

    /// <summary>Get a player's currently registered combat Ultimate, if any.</summary>
    /// <param name="farmer">The player.</param>
    public IUltimate? GetRegisteredUltimate(Farmer? farmer = null) =>
        farmer is null ? Game1.player.get_Ultimate() : farmer.get_Ultimate();

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateActivatedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateDeactivatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateDeactivatedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeInitiatedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateChargeInitiatedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateChargeIncreasedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateChargeIncreasedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateFullyChargedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateFullyChargedEvent(Action<object?, IUltimateFullyChargedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateFullyChargedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    /// <summary>Register a new <see cref="UltimateEmptiedEvent"/> instance.</summary>
    /// <param name="callback">The delegate that will be called when the event is triggered.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    public IManagedEvent RegisterUltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback, bool alwaysEnabled = false)
    {
        var e = new UltimateEmptiedEvent(callback, alwaysEnabled);
        ModEntry.EventManager.Manage(e);
        return e;
    }

    #endregion ultimate

    #region configs

    /// <summary>Get an interface for this mod's config settings.</summary>
    public ModConfig GetConfigs() => ModEntry.Config;

    #endregion configs
}