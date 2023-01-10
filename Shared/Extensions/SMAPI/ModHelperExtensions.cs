namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    private static JsonSerializerSettings? JsonSerializerSettings { get; set; }

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
        var modEntry = helper.GetModEntryFor(uniqueId);
        return modEntry?.Helper.ReadConfig<JObject>();
    }

    /// <summary>Reads the configuration file of the content pack with the specified <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the content pack.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will work for any mod, but is reserved for content packs.</remarks>
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
            var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
            return config;
        }
        catch (FileNotFoundException)
        {
            Log.W(
                $"Detected {uniqueId}, but a corresponding config file was not found in the expected location '{modPath}'.");
            return null;
        }
    }

    /// <summary>Serializes and logs a mod's config settings to the console.</summary>
    /// <typeparam name="TConfig">The type of the current <see cref="IMod"/>'s config object.</typeparam>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="config">The current <see cref="IMod"/>'s config settings.</param>
    public static void LogConfig<TConfig>(this IModHelper helper, TConfig config)
    {
        JsonSerializerSettings ??= getSmapiSerializerSettings(helper.Data);
        var json = JsonConvert.SerializeObject(config, JsonSerializerSettings);
        Log.T($"[Config]:\n{json}");

        JsonSerializerSettings getSmapiSerializerSettings(IDataHelper dataHelper)
        {
            var dataHelperType = Type.GetType("StardewModdingAPI.Framework.ModHelpers.DataHelper, StardewModdingAPI")!;
            var jsonHelperType = Type.GetType("StardewModdingAPI.Toolkit.Serialization.JsonHelper, SMAPI.Toolkit")!;

            var jsonHelperField = dataHelperType.GetField(
                "JsonHelper",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            var jsonSettingsGetter =
                jsonHelperType.GetProperty(
                    "JsonSettings",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod()!;

            var jsonHelper = jsonHelperField.GetValue(dataHelper)!;
            return (JsonSerializerSettings)jsonSettingsGetter.Invoke(jsonHelper, null)!;
        }
    }
}
