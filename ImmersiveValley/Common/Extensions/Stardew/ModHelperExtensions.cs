namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using System.IO;

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
        var modEntry = (IMod)modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo)!;
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

        var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
        try
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
            Log.V("Success. Integrations will be enabled.");
            return config;
        }
        catch (FileNotFoundException)
        {
            Log.W(
                $"Detected {uniqueID}, but a corresponding config file was not found in the expected location '{modPath}'.\nIntegrations will not be enabled until the next restart.");
            return null;
        }
    }
}