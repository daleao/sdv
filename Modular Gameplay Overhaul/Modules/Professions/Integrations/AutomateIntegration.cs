namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Integrations;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

#endregion using directives

internal sealed class AutomateIntegration : BaseIntegration
{
    private static IDictionary? _machineData;
    private static object? _machineManager;

    /// <summary>Initializes a new instance of the <see cref="AutomateIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal AutomateIntegration(IModRegistry modRegistry)
        : base("Automate", "Pathoschild.Automate", "1.27.3", modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="Building"/> machine.</param>
    /// <returns>The <see cref="Chest"/> instance closest to the <paramref name="machine"/>, or <see langword="null"/> is none are found.</returns>
    internal static Chest? GetClosestContainerTo(Building machine)
    {
        if (_machineData is null)
        {
            return null;
        }

        var machineLocationKey = GetLocationKey(Game1.getFarm());
        var mdIndex = _machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex);
        var activeTiles = (IDictionary)Reflector
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
            var junimoMachineGroup = Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = Reflector
                    .GetUnboundPropertyGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    if (wrapperLocation is not Farm)
                    {
                        continue;
                    }

                    var wrapperTileArea = Reflector
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

        var containers = (Array)Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
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
    internal static Chest? GetClosestContainerTo(SObject machine, GameLocation location)
    {
        if (_machineData is null)
        {
            return null;
        }

        var machineLocationKey = GetLocationKey(location);
        var mdIndex = _machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex);
        var activeTiles = (IDictionary)Reflector
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
            var junimoMachineGroup = Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = Reflector
                    .GetUnboundPropertyGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    // ReSharper disable once PossibleUnintendedReferenceComparison
                    if (wrapperLocation != location)
                    {
                        continue;
                    }

                    var wrapperTileArea = Reflector
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

        var containers = (Array)Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
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
    internal static Chest? GetClosestContainerTo(TerrainFeature machine)
    {
        if (_machineData is null)
        {
            return null;
        }

        var machineLocationKey = GetLocationKey(machine.currentLocation);
        var mdIndex = _machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = _machineData!.Values.Cast<object>().ElementAt(mdIndex);
        var activeTiles = (IDictionary)Reflector
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
            var junimoMachineGroup = Reflector
                .GetUnboundPropertyGetter<object, object>(_machineManager!, "JunimoMachineGroup")
                .Invoke(_machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            foreach (var group in machineGroups)
            {
                var groupLocationKey = Reflector
                    .GetUnboundPropertyGetter<object, string?>(group, "LocationKey")
                    .Invoke(group);
                if (groupLocationKey != machineLocationKey)
                {
                    continue;
                }

                var wrappers = (IEnumerable)Reflector
                    .GetUnboundPropertyGetter<object, object>(group, "Machines")
                    .Invoke(group);
                foreach (var wrapper in wrappers)
                {
                    var wrapperLocation = Reflector
                        .GetUnboundPropertyGetter<object, GameLocation>(wrapper, "Location")
                        .Invoke(wrapper);
                    if (!ReferenceEquals(wrapperLocation, machine.currentLocation))
                    {
                        continue;
                    }

                    var wrapperTileArea = Reflector
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

        var containers = (Array)Reflector
            .GetUnboundPropertyGetter<object, object>(machineGroup, "Containers")
            .Invoke(machineGroup);
        var chests = containers.Cast<object>()
            .Select(c => Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length == 0
            ? null
            : chests.Length == 1
                ? chests[0] :
                machine.GetClosestObject(chests);
    }

    /// <summary>Initialize reflected Automate fields.</summary>
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        var mod = ModHelper.GetModEntryFor("Pathoschild.Automate") ??
                  ThrowHelper.ThrowMissingMemberException<IMod>("Pathoschild.Automate", "ModEntry");
        _machineManager = Reflector.GetUnboundFieldGetter<IMod, object>(mod, "MachineManager").Invoke(mod);
        _machineData = (IDictionary)Reflector
            .GetUnboundFieldGetter<object, object>(_machineManager, "MachineData")
            .Invoke(_machineManager);
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
