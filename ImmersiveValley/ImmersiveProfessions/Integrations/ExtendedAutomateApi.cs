namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.SMAPI;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Integrations.Automate;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Provides needed functionality missing from <see cref="IAutomateApi"/>.</summary>
internal static class ExtendedAutomateApi
{
    private static readonly Lazy<Func<IMod, object>> GetMachineManager = new(() =>
        "Pathoschild.Stardew.Automate.ModEntry"
            .ToType()
            .RequireField("MachineManager")
            .CompileUnboundFieldGetterDelegate<IMod, object>());

    private static readonly Lazy<Func<object, object>> GetMachineData = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineManager"
            .ToType()
            .RequireField("MachineData")
            .CompileUnboundFieldGetterDelegate<object, object>());

    private static readonly Lazy<Func<object, object>> GetActiveMachineGroups = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineManager"
            .ToType()
            .RequireMethod("GetActiveMachineGroups")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetJunimoGroup = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineManager"
            .ToType()
            .RequirePropertyGetter("JunimoMachineGroup")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetLocationActiveTiles = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineDataForLocation"
            .ToType()
            .RequirePropertyGetter("ActiveTiles")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetGroupContainers = new(() =>
        "Pathoschild.Stardew.Automate.Framework.IMachineGroup"
            .ToType()
            .RequirePropertyGetter("Containers")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetGroupMachines = new(() =>
        "Pathoschild.Stardew.Automate.Framework.IMachineGroup"
            .ToType()
            .RequirePropertyGetter("Machines")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, object>> GetGroupTiles = new(() =>
        "Pathoschild.Stardew.Automate.Framework.IMachineGroup"
            .ToType()
            .RequireMethod("GetGroupTiles")
            .CompileUnboundDelegate<Func<object, object>>());

    private static readonly Lazy<Func<object, string?>> GetGroupLocationKey = new(() =>
        "Pathoschild.Stardew.Automate.Framework.IMachineGroup"
            .ToType()
            .RequirePropertyGetter("LocationKey")
            .CompileUnboundDelegate<Func<object, string?>>());

    private static readonly Lazy<Func<object, object>> GetJunimoSubGroups = new(() =>
        "Pathoschild.Stardew.Automate.Framework.JunimoMachineGroup"
            .ToType()
            .RequireField("MachineGroups")
            .CompileUnboundFieldGetterDelegate<object, object>());

    private static readonly Lazy<Func<object, GameLocation>> GetWrapperLocation = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineWrapper"
            .ToType()
            .RequirePropertyGetter("Location")
            .CompileUnboundDelegate<Func<object, GameLocation>>());

    private static readonly Lazy<Func<object, Rectangle>> GetWrapperTileArea = new(() =>
        "Pathoschild.Stardew.Automate.Framework.MachineWrapper"
            .ToType()
            .RequirePropertyGetter("TileArea")
            .CompileUnboundDelegate<Func<object, Rectangle>>());

    private static readonly Lazy<Func<object, Chest>> GetContainerChest = new(() =>
        "Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer"
            .ToType()
            .RequireField("Chest")
            .CompileUnboundFieldGetterDelegate<object, Chest>());

    private static IDictionary? _machineData;
    private static object? _machineManager;

    /// <summary>Initialize reflected fields and compile delegates.</summary>
    public static void Init()
    {
        var mod = ModEntry.ModHelper.GetModEntryFor("Pathoschild.Automate") ??
                  ThrowHelper.ThrowMissingMemberException<IMod>("Pathoschild.Automate", "ModEntry");
        _machineManager = GetMachineManager.Value(mod);
        _machineData = (IDictionary)GetMachineData.Value(_machineManager);
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

        var locationKey = GetLocationKey(Game1.getFarm());
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)GetLocationActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0)
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
            var junimoMachineGroup = GetJunimoGroup.Value(_machineManager!);
            var machineGroups = (IList)GetJunimoSubGroups.Value(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                if (GetGroupLocationKey.Value(group) != locationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)GetGroupMachines.Value(group);
                foreach (var wrapper in wrappers)
                {
                    if (GetWrapperLocation.Value(wrapper) is not Farm)
                    {
                        continue;
                    }

                    var tileArea = GetWrapperTileArea.Value(wrapper);
                    if (tileArea.X != machine.tileX.Value || tileArea.Y != machine.tileY.Value ||
                        tileArea.Height != machine.tilesHigh.Value || tileArea.Width != machine.tilesWide.Value)
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

        var containers = (Array)GetGroupContainers.Value(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => GetContainerChest.Value(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length <= 0
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

        var locationKey = GetLocationKey(location);
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)GetLocationActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0)
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
            var junimoMachineGroup = GetJunimoGroup.Value(_machineManager!);
            var machineGroups = (IList)GetJunimoSubGroups.Value(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                if (GetGroupLocationKey.Value(group) != locationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)GetGroupMachines.Value(group);
                foreach (var wrapper in wrappers)
                {
                    // ReSharper disable once PossibleUnintendedReferenceComparison
                    if (GetWrapperLocation.Value(wrapper) != location)
                    {
                        continue;
                    }

                    var tileArea = GetWrapperTileArea.Value(wrapper);
                    if (tileArea.X != machine.TileLocation.X || tileArea.Y != machine.TileLocation.Y)
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

        var containers = (Array)GetGroupContainers.Value(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => GetContainerChest.Value(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length <= 0
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

        var locationKey = GetLocationKey(machine.currentLocation);
        var mdIndex = _machineData?.Keys.Cast<string>().ToList().FindIndex(s => s == locationKey);
        if (mdIndex is null or < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex.Value);
        var activeTiles = (IDictionary)GetLocationActiveTiles.Value(machineDataForLocation);
        if (activeTiles.Count <= 0)
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
            var junimoMachineGroup = GetJunimoGroup.Value(_machineManager!);
            var machineGroups = (IList)GetJunimoSubGroups.Value(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                if (GetGroupLocationKey.Value(group) != locationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)GetGroupMachines.Value(group);
                foreach (var wrapper in wrappers)
                {
                    // ReSharper disable once PossibleUnintendedReferenceComparison
                    if (GetWrapperLocation.Value(wrapper) != machine.currentLocation)
                    {
                        continue;
                    }

                    var tileArea = GetWrapperTileArea.Value(wrapper);
                    if (tileArea.X != machine.currentTileLocation.X || tileArea.Y != machine.currentTileLocation.Y)
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

        var containers = (Array)GetGroupContainers.Value(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => GetContainerChest.Value(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length <= 0
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
