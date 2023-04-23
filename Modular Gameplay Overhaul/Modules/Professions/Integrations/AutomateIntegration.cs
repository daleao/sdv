namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Integrations;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

#endregion using directives

[RequiresMod("Pathoschild.Automate", "Automate", "1.27.3")]
internal sealed class AutomateIntegration : ModIntegration<AutomateIntegration>
{
    private IDictionary? _machineData;
    private object? _machineManager;

    private AutomateIntegration()
        : base("Pathoschild.Automate", "Automate", "1.27.3", ModHelper.ModRegistry)
    {
    }

    /// <summary>Get the closest <see cref="Chest"/> to the given automated <see cref="Building"/> machine.</summary>
    /// <param name="machine">An automated <see cref="Building"/> machine.</param>
    /// <returns>The <see cref="Chest"/> instance closest to the <paramref name="machine"/>, or <see langword="null"/> is none are found.</returns>
    internal Chest? GetClosestContainerTo(Building machine)
    {
        if (this._machineData is null)
        {
            return null;
        }

        var machineLocationKey = this.GetLocationKey(Game1.getFarm());
        var mdIndex = this._machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = this._machineData!.Values.Cast<object>().ElementAt(mdIndex);
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
                .GetUnboundPropertyGetter<object, object>(this._machineManager!, "JunimoMachineGroup")
                .Invoke(this._machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            for (var i = 0; i < machineGroups.Count; i++)
            {
                var group = machineGroups[i];
                if (group is null)
                {
                    continue;
                }

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
        var chests = containers
            .Cast<object>()
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
    internal Chest? GetClosestContainerTo(SObject machine, GameLocation location)
    {
        if (this._machineData is null)
        {
            return null;
        }

        var machineLocationKey = this.GetLocationKey(location);
        var mdIndex = this._machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = this._machineData!.Values.Cast<object>().ElementAt(mdIndex);
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
                .GetUnboundPropertyGetter<object, object>(this._machineManager!, "JunimoMachineGroup")
                .Invoke(this._machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            for (var i = 0; i < machineGroups.Count; i++)
            {
                var group = machineGroups[i];
                if (group is null)
                {
                    continue;
                }

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
        var chests = containers
            .Cast<object>()
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
    internal Chest? GetClosestContainerTo(TerrainFeature machine)
    {
        if (this._machineData is null)
        {
            return null;
        }

        var machineLocationKey = this.GetLocationKey(machine.currentLocation);
        var mdIndex = this._machineData.Keys.Cast<string>().ToList().FindIndex(s => s == machineLocationKey);
        if (mdIndex < 0)
        {
            return null;
        }

        var machineDataForLocation = this._machineData!.Values.Cast<object>().ElementAt(mdIndex);
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
                .GetUnboundPropertyGetter<object, object>(this._machineManager!, "JunimoMachineGroup")
                .Invoke(this._machineManager!);
            var machineGroups = (IList)Reflector
                .GetUnboundFieldGetter<object, object>(junimoMachineGroup, "MachineGroups")
                .Invoke(junimoMachineGroup);
            for (var i = 0; i < machineGroups.Count; i++)
            {
                var group = machineGroups[i];
                if (group is null)
                {
                    continue;
                }

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
        var chests = containers
            .Cast<object>()
            .Select(c => Reflector.GetUnboundFieldGetter<object, Chest>(c, "Chest").Invoke(c))
            .Where(c => c.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
            .ToArray();
        return chests.Length == 0
            ? null
            : chests.Length == 1
                ? chests[0] :
                machine.GetClosestObject(chests);
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_machineData))]
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        try
        {
            var mod = ModHelper.GetModEntryFor("Pathoschild.Automate") ??
                      ThrowHelper.ThrowMissingMemberException<IMod>("Pathoschild.Automate", "ModEntry");
            this._machineManager = Reflector.GetUnboundFieldGetter<IMod, object>(mod, "MachineManager").Invoke(mod);
            this._machineData = (IDictionary)Reflector
                .GetUnboundFieldGetter<object, object>(this._machineManager, "MachineData")
                .Invoke(this._machineManager);
            if (this._machineData is not null)
            {
                return true;
            }

            Log.W("[PROFS]: Failed to grab Automate's machine data.");
            return false;
        }
        catch (Exception ex)
        {
            Log.W($"[PROFS]: Failed to grab Automate's machine data.\n{ex}");
            return false;
        }
    }

    /// <summary>Get a location key for looking up location-specific machine data.</summary>
    /// <param name="location">A machine group's location.</param>
    /// <returns>The <see cref="string"/> key for the given <paramref name="location"/>.</returns>
    private string GetLocationKey(GameLocation location)
    {
        if (location.uniqueName.Value == null || location.uniqueName.Value == location.Name)
        {
            return location.Name;
        }

        return location.Name + " (" + location.uniqueName.Value + ")";
    }
}
