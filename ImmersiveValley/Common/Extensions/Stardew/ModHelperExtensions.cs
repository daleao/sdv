namespace DaLion.Common.Extensions.Stardew;

#region using directives

using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    /// <summary>Read an external mod's configuration file.</summary>
    /// <param name="uniqueId">The external mod's unique id.</param>
    /// <param name="log">Interface for logging messages to the console.</param>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    [CanBeNull]
    public static JObject ReadConfigExt(this IModHelper helper, string uniqueId, Action<string, LogLevel> log)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is null)
        {
            log($"{uniqueId} mod not found. Integrations disabled.", LogLevel.Info);
            return null;
        }

        log($"{uniqueId} mod found. Integrations will be enabled.", LogLevel.Info);
        var modEntry = (IMod) modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo);
        return modEntry?.Helper.ReadConfig<JObject>();
    }

    /// <summary>Read an external content pack's configuration file.</summary>
    /// <param name="uniqueId">The external mod's unique id.</param>
    /// <param name="log">Interface for logging messages to the console.</param>
    /// <remarks>Will work for any mod, but is reserved for content packs.</remarks>
    [CanBeNull]
    public static JObject ReadContentPackConfig(this IModHelper helper, string uniqueId, Action<string, LogLevel> log)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is null)
        {
            log($"{uniqueId} mod not found. Integrations disabled.", LogLevel.Info);
            return null;
        }

        log($"{uniqueId} mod found. Integrations will be enabled.", LogLevel.Info);
        var modPath = (string) modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo);
        try
        {
            return JObject.Parse(File.ReadAllText(modPath + "\\config.json"));
        }
        catch (FileNotFoundException)
        {
            log(
                $"Did not find a config file for {uniqueId}. Please restart the game once a config file has been generated.",
                LogLevel.Warn);
            return null;
        }
    }
}