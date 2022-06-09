namespace DaLion.Stardew.Tweaks;

#region using directives

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using Extensions;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(this ICommandHelper helper)
    {
        helper.Add("qol_clearmushroomboxdata",
            "Clear the age data of every mushroom box in the farm cave.", ClearMushroomBoxData);
        helper.Add("qol_cleartreedata",
            "Clear the age data of every tree in the world.", ClearTreeData);
    }

    #region command handlers

    /// <summary>Clear the age data of every mushroom box in the farm cave.</summary>
    /// <param name="command">The console command.</param>
    /// <param name="args">The supplied arguments.</param>
    private static void ClearMushroomBoxData(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        foreach (var @object in Game1.locations.OfType<FarmCave>().SelectMany(fc => fc.Objects.Values).Where(o => o.IsMushroomBox()))
            @object.WriteData("Age", null);
    }

    /// <summary>Clear the age data of every tree in the world.</summary>
    /// <param name="command">The console command.</param>
    /// <param name="args">The supplied arguments.</param>
    private static void ClearTreeData(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        foreach (var tree in Game1.locations.SelectMany(l => l.terrainFeatures.Values).OfType<Tree>())
            tree.WriteData("Age", null);
    }

    #endregion command handlers
}