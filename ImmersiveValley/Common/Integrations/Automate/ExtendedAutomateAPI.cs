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
    private static IMod? _Mod = null!;
    private static Func<IMod, object> GetMachineManager = null!;
    private static Func<object, object> GetMachineData = null!;
    private static Func<object, object> GetActiveTiles = null!;
    private static Func<object, object> GetContainers = null!;
    private static Func<object, Chest> GetChest = null!;

    /// <summary>Whether the reflected fields have been initialized.</summary>
    public static bool Initialized { get; private set; }

    #region public methods

    /// <summary>Initialize reflected fields and compile delegates.</summary>
    public static void Init(IModHelper helper)
    {
        _Mod = helper.GetModEntryFor("Pathoschild.Automate")!;

        GetMachineManager = "Pathoschild.Stardew.Automate.ModEntry".ToType().RequireField("MachineManager")
            .CompileUnboundFieldGetterDelegate<IMod, object>();
        GetMachineData = "Pathoschild.Stardew.Automate.Framework.MachineManager".ToType().RequireField("MachineData")
            .CompileUnboundFieldGetterDelegate<object, object>();
        GetActiveTiles = "Pathoschild.Stardew.Automate.Framework.MachineDataForLocation".ToType().RequirePropertyGetter("ActiveTiles")
            .CompileUnboundDelegate<Func<object, object>>();
        GetContainers = "Pathoschild.Stardew.Automate.Framework.IMachineGroup".ToType().RequirePropertyGetter("Containers")
            .CompileUnboundDelegate<Func<object, object>>();
        GetChest = "Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer".ToType().RequireField("Chest")
            .CompileUnboundFieldGetterDelegate<object, Chest>();

        Initialized = true;
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="Building"/> machine.</param>
    public static Chest GetClosestContainerTo(Building machine)
    {
        var machineManager = GetMachineManager(_Mod!);
        var machineData = (IDictionary)GetMachineData(machineManager);
        var locationKey = GetLocationKey(Game1.getFarm());
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles(machineDataForLocation);
        var index2 = activeTiles.Keys.Cast<Vector2>().ToList()
            .FindIndex(v => v == new Vector2(machine.tileX.Value, machine.tileY.Value));
        var machineGroup = activeTiles.Keys.Cast<object>().ElementAt(index2);

        var containers = (Array)GetContainers(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest(c));

        return machine.GetClosestObject(chests)!;
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="SObject"/> machine.</param>
    /// <param name="location">The machine's location.</param>
    public static Chest GetClosestContainerTo(SObject machine, GameLocation location)
    {
        var machineManager = GetMachineManager(_Mod!);
        var machineData = (IDictionary)GetMachineData(machineManager);
        var locationKey = GetLocationKey(location);
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles(machineDataForLocation);
        var index2 = activeTiles.Keys.Cast<Vector2>().ToList().FindIndex(v => v == machine.TileLocation);
        var machineGroup = activeTiles.Values.Cast<object>().ElementAt(index2);
        var containers = (Array)GetContainers(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest(c));

        return machine.GetClosestObject(location, chests)!;
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="TerrainFeature"/> machine.</param>
    public static Chest GetClosestContainerTo(TerrainFeature machine)
    {
        var machineManager = GetMachineManager(_Mod!);
        var machineData = (IDictionary)GetMachineData(machineManager);
        var locationKey = GetLocationKey(machine.currentLocation);
        var index = machineData.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        var machineDataForLocation = machineData.Values.Cast<object>().ElementAt(index)!;
        var activeTiles = (IDictionary)GetActiveTiles(machineDataForLocation);
        var index2 = activeTiles.Keys.Cast<Vector2>().ToList().FindIndex(v => v == machine.currentTileLocation);
        var machineGroup = activeTiles.Keys.Cast<object>().ElementAt(index2);
        var containers = (Array)GetContainers(machineGroup);
        var chests = containers.Cast<object>().Select(c => GetChest(c));

        return machine.GetClosestObject(chests)!;
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