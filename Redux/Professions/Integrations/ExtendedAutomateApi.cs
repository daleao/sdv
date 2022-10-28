namespace DaLion.Redux.Professions.Integrations;

#region using directives

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Integrations.Automate;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Provides needed functionality missing from <see cref="IAutomateApi"/>.</summary>
internal static class ExtendedAutomateApi
{
    private static IDictionary? _machineData;
    private static object? _machineManager;

    /// <summary>Initialize reflected fields and compile delegates.</summary>
    public static void Init()
    {
        var mod = ModEntry.ModHelper.GetModEntryFor("Pathoschild.Automate") ??
                  ThrowHelper.ThrowMissingMemberException<IMod>("Pathoschild.Automate", "ModEntry");
        _machineManager = ModEntry.Reflector.GetUnboundFieldGetter<IMod, object>(mod, "MachineManager").Invoke(mod);
        _machineData = (IDictionary)ModEntry.Reflector
            .GetUnboundFieldGetter<object, object>(_machineManager, "MachineData")
            .Invoke(_machineManager);
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="Building"/> machine.</param>
    /// <returns>The <see cref="Chest"/> instance closest to the <paramref name="machine"/>, or <see langword="null"/> is none are found.</returns>
    public static Chest? GetClosestContainerTo(Building machine)
    {
        if (_machineData is null)
        {
            Init();
        }

        var machineLocationKey = GetLocationKey(Game1.getFarm());
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineDataForLocation, "ActiveTiles")
            .Invoke(machineDataForLocation);
        if (activeTiles.Count == 0)
        {
            return null;
        }

        var atIndex = activeTiles.Keys.Cast<Vector2>().ToList()
            .FindIndex(v => (int)v.X == machine.tileX.Value && (int)v.Y == machine.tileY.Value);
        object? machineGroup = null;
        if (atIndex >= 0)
        {
            machineGroup = activeTiles.Keys.Cast<object>().ElementAt(atIndex);
        }
        else
        {
            var junimoMachineGroup = ModEntry.Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)ModEntry.Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = ModEntry.Reflector
                    .GetUnboundFieldGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)ModEntry.Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    if (wrapperLocation is not Farm)
                    {
                        continue;
                    }

                    var wrapperTileArea = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, Rectangle>(wrapper, "TileArea")
                        .Invoke(wrapper);
                    if (wrapperTileArea.X != machine.tileX.Value || wrapperTileArea.Y != machine.tileY.Value ||
                        wrapperTileArea.Height != machine.tilesHigh.Value || wrapperTileArea.Width != machine.tilesWide.Value)
                    {
                        continue;
                    }

                    machineGroup = group;
                    break;
                }

                if (machineGroup is not null)
                {
                    break;
                }
            }
        }

        if (machineGroup is null)
        {
            return null;
        }

        var containers = (Array)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => ModEntry.Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length == 0
            ? null
            : chests.Length == 1
                ? chests[0] :
                machine.GetClosestObject(chests);
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="SObject"/> machine.</summary>
    /// <param name="machine">An automated <see cref="SObject"/> machine.</param>
    /// <param name="location">The machine's location.</param>
    /// <returns>The <see cref="Chest"/> instance closest to the <paramref name="machine"/>, or <see langword="null"/> is none are found.</returns>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Should be reference equality.")]
    public static Chest? GetClosestContainerTo(SObject machine, GameLocation location)
    {
        if (_machineData is null)
        {
            Init();
        }

        var machineLocationKey = GetLocationKey(location);
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineDataForLocation, "ActiveTiles")
            .Invoke(machineDataForLocation);
        if (activeTiles.Count == 0)
        {
            return null;
        }

        var atIndex = activeTiles.Keys.Cast<Vector2>().ToList().FindIndex(v =>
            (int)v.X == (int)machine.TileLocation.X && (int)v.Y == (int)machine.TileLocation.Y);
        object? machineGroup = null;
        if (atIndex >= 0)
        {
            machineGroup = activeTiles.Values.Cast<object>().ElementAt(atIndex);
        }
        else
        {
            var junimoMachineGroup = ModEntry.Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)ModEntry.Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = ModEntry.Reflector
                    .GetUnboundFieldGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)ModEntry.Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    if (wrapperLocation != location)
                    {
                        continue;
                    }

                    var wrapperTileArea = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, Rectangle>(wrapper, "TileArea")
                        .Invoke(wrapper);
                    if (wrapperTileArea.X != machine.TileLocation.X || wrapperTileArea.Y != machine.TileLocation.Y)
                    {
                        continue;
                    }

                    machineGroup = group;
                    break;
                }

                if (machineGroup is not null)
                {
                    break;
                }
            }
        }

        if (machineGroup is null)
        {
            return null;
        }

        var containers = (Array)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => ModEntry.Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length == 0
            ? null
            : chests.Length == 1
                ? chests[0]
                : machine.GetClosestObject(location, chests);
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="TerrainFeature"/> machine.</summary>
    /// <param name="machine">An automated <see cref="TerrainFeature"/> machine.</param>
    /// <returns>The <see cref="Chest"/> instance closest to the <paramref name="machine"/>, or <see langword="null"/> is none are found.</returns>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Should be reference equality.")]
    public static Chest? GetClosestContainerTo(TerrainFeature machine)
    {
        if (_machineData is null)
        {
            Init();
        }

        var machineLocationKey = GetLocationKey(machine.currentLocation);
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineDataForLocation, "ActiveTiles")
            .Invoke(machineDataForLocation);
        if (activeTiles.Count == 0)
        {
            return null;
        }

        var tileLocation = machine is LargeTerrainFeature large
            ? large.tilePosition.Value
            : machine.currentTileLocation;
        var atIndex = activeTiles.Keys.Cast<Vector2>().ToList()
            .FindIndex(v => (int)v.X == (int)tileLocation.X && (int)v.Y == (int)tileLocation.Y);
        object? machineGroup = null;
        if (atIndex >= 0)
        {
            machineGroup = activeTiles.Values.Cast<object>().ElementAt(atIndex);
        }
        else
        {
            var junimoMachineGroup = ModEntry.Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)ModEntry.Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = ModEntry.Reflector
                    .GetUnboundFieldGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)ModEntry.Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    if (wrapperLocation != machine.currentLocation)
                    {
                        continue;
                    }

                    var wrapperTileArea = ModEntry.Reflector
                        .GetUnboundPropertyGetter<object, Rectangle>(wrapper, "TileArea")
                        .Invoke(wrapper);
                    if (wrapperTileArea.X != machine.currentTileLocation.X || wrapperTileArea.Y != machine.currentTileLocation.Y)
                    {
                        continue;
                    }

                    machineGroup = group;
                    break;
                }
            }
        }

        if (machineGroup is null)
        {
            return null;
        }

        var containers = (Array)ModEntry.Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => ModEntry.Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length == 0
            ? null
            : chests.Length == 1
                ? chests[0] :
                machine.GetClosestObject(chests);
    }

    /// <summary>Get a location key for looking up location-specific machine data.</summary>
    /// <param name="location">A machine group's location.</param>
    /// <returns>The <see cref="string"/> key for the given <paramref name="location"/>.</returns>
    private static string GetLocationKey(GameLocation location)
    {
        if (location.uniqueName.Value == null || location.uniqueName.Value == location.Name)
        {
            return location.Name;
        }

        return location.Name + " (" + location.uniqueName.Value + ")";
    }
}
