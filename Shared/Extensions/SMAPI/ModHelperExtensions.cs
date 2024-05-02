namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.IO;
using Newtonsoft.Json.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    /// <summary>Gets the <see cref="IMod"/> interface for the external mod identified by <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the external mod.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static IMod? GetModEntryFor(this IModHelper helper, string uniqueId)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is not null)
        {
            return (IMod)modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo)!;
        }

        Log.V($"{uniqueId} mod not found.");
        return null;
    }

    /// <summary>Reads the configuration file of the mod with the specified <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the external mod.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static JObject? ReadConfigExt(this IModHelper helper, string uniqueId)
    {
        return helper.GetModEntryFor(uniqueId)?.Helper.ReadConfig<JObject>();
    }

    /// <summary>Reads the configuration file of the content pack with the specified <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the content pack.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Should work for any mod, but best reserved for content packs.</remarks>
    public static JObject? ReadContentPackConfig(this IModHelper helper, string uniqueId)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is null)
        {
            Log.V($"{uniqueId} mod not found.");
            return null;
        }

        var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
        try
        {
            return JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
        }
        catch (FileNotFoundException)
        {
            Log.W(
                $"Found {uniqueId}, but did not find a corresponding config file in the expected location '{modPath}'.");
            return null;
        }
    }
}
