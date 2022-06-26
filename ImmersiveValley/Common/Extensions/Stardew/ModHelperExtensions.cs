namespace DaLion.Common.Extensions.Stardew;

#region using directives

using System.IO;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    /// <summary>Read an external mod's configuration file.</summary>
    /// <param name="uniqueID">The external mod's unique id.</param>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static JObject? ReadConfigExt(this IModHelper helper, string uniqueID)
    {
        var modInfo = helper.ModRegistry.Get(uniqueID);
        if (modInfo is null)
        {
            Log.V($"{uniqueID} mod not found. Integrations disabled.");
            return null;
        }

        Log.V($"{uniqueID} mod found. Integrations will be enabled.");
        var modEntry = (IMod) modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo)!;
        return modEntry.Helper.ReadConfig<JObject>();
    }

    /// <summary>Read an external content pack's configuration file.</summary>
    /// <param name="uniqueID">The external mod's unique id.</param>
    /// <remarks>Will work for any mod, but is reserved for content packs.</remarks>
    public static JObject? ReadContentPackConfig(this IModHelper helper, string uniqueID)
    {
        var modInfo = helper.ModRegistry.Get(uniqueID);
        if (modInfo is null)
        {
            Log.V($"{uniqueID} mod not found. Integrations disabled.");
            return null;
        }

        Log.V($"{uniqueID} mod found. Integrations will be enabled.");
        var modPath = (string) modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
        try
        {
            return JObject.Parse(File.ReadAllText(modPath + "\\config.json"));
        }
        catch (FileNotFoundException)
        {
            Log.W(
                $"Did not find a config file for {uniqueID}. Please restart the game once a config file has been generated.");
            return null;
        }
    }
}