namespace DaLion.Common.Integrations.Automate;

#region using directives

using Extensions.Reflection;
using Extensions.SMAPI;
using Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Linq;

#endregion using directives

/// <summary>Provides functionality missing from <see cref="IAutomateAPI"/>.</summary>
internal static class ExtendedAutomateAPI
{
    private static IMod? _Mod;

    private static readonly Lazy<Func<IMod, object>> GetMachineManager = new(() =>
        "Pathoschild.Stardew.Automate.ModEntry".ToType().RequireField("MachineManager")
            .CompileUnboundFieldGetterDelegate<IMod, object>());

    private static readonly Lazy<Func<object, object>> GetMachineData = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineManager".ToType().RequireField("MachineData")
            .CompileUnboundFieldGetterDelegate<object, object>());

    private static readonly Lazy<Func<object, object>> GetActiveTiles = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineDataForLocation".ToType().RequirePropertyGetter("ActiveTiles")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetContainers = new(() =>
        "Pathoschild.Stardew.Automate.Framework.IMachineGroup".ToType().RequirePropertyGetter("Containers")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, Chest>> GetChest = new(() =>
        "Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer".ToType().RequireField("Chest")
            .CompileUnboundFieldGetterDelegate<object, Chest>());

    #region public methods

    /// <summary>Initialize reflected fields and compile delegates.</summary>
    public static void Init(IModHelper helper)
    {
        _Mod = helper.GetModEntryFor("Pathoschild.Automate")!;
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="Building"/> machine.</param>
    public static Chest? GetClosestContainerTo(Building machine)
    {
        if (_Mod is null) ThrowHelper.ThrowInvalidOperationException("The extended API was not initialized.");

        var machineManager = GetMachineManager.Value(_Mod);
        var machineData = (IDictionary)GetMachineData.Value(machineManager);
        var locationKey = GetLocationKey(Game1.getFarm());
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0) return null;

        var index2 = activeTiles.Keys.Cast<Vector2>().ToList()
            .FindIndex(v => (int)v.X == machine.tileX.Value && (int)v.Y == machine.tileY.Value);
        var machineGroup = activeTiles.Keys.Cast<object>().ElementAt(index2);
        var containers = (Array)GetContainers.Value(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest.Value(c));

        return machine.GetClosestObject(chests);
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="SObject"/> machine.</summary>
    /// <param name="machine">An automated <see cref="SObject"/> machine.</param>
    /// <param name="location">The machine's location.</param>
    public static Chest? GetClosestContainerTo(SObject machine, GameLocation location)
    {
        if (_Mod is null) ThrowHelper.ThrowInvalidOperationException("The extended API was not initialized.");

        var machineManager = GetMachineManager.Value(_Mod);
        var machineData = (IDictionary)GetMachineData.Value(machineManager);
        var locationKey = GetLocationKey(location);
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0) return null;

        var index2 = activeTiles.Keys.Cast<Vector2>().ToList().FindIndex(v =>
            (int)v.X == (int)machine.TileLocation.X && (int)v.Y == (int)machine.TileLocation.Y);
        var machineGroup = activeTiles.Values.Cast<object>().ElementAt(index2);
        var containers = (Array)GetContainers.Value(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest.Value(c));

        return machine.GetClosestObject(location, chests);
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="TerrainFeature"/> machine.</summary>
    /// <param name="machine">An automated <see cref="TerrainFeature"/> machine.</param>
    public static Chest? GetClosestContainerTo(TerrainFeature machine)
    {
        if (_Mod is null) ThrowHelper.ThrowInvalidOperationException("The extended API was not initialized.");

        var machineManager = GetMachineManager.Value(_Mod);
        var machineData = (IDictionary)GetMachineData.Value(machineManager);
        var locationKey = GetLocationKey(machine.currentLocation);
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0) return null;

        var tileLocation = machine is LargeTerrainFeature large
            ? large.tilePosition.Value
            : machine.currentTileLocation;
        var index2 = activeTiles.Keys.Cast<Vector2>().ToList()
            .FindIndex(v => (int)v.X == (int)tileLocation.X && (int)v.Y == (int)tileLocation.Y);
        var machineGroup = activeTiles.Values.Cast<object>().ElementAt(index2);
        var containers = (Array)GetContainers.Value(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest.Value(c));

        return machine.GetClosestObject(chests);
    }

    #endregion public methods

    #region private methods

    /// <summary>Get a location key for looking up location-specific machine data.</summary>
    /// <param name="location">A machine group's location.</param>
    private static string GetLocationKey(GameLocation location)
    {
        if (location.uniqueName.Value == null || location.uniqueName.Value == location.Name)
            return location.Name;

        return location.Name + " (" + location.uniqueName.Value + ")";
    }

    #endregion private methods
}