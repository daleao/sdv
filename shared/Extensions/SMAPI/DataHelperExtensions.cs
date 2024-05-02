namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.Reflection;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Extensions for the <see cref="IDataHelper"/> interface.</summary>
public static class DataHelperExtensions
{
    /// <summary>Gets the <see cref="JsonSerializerSettings"/> from the data <paramref name="helper"/> instance.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <returns>The <see cref="JsonSerializerSettings"/>.</returns>
    public static JsonSerializerSettings GetJsonSerializerSettings(this IDataHelper helper)
    {
        var dataHelperType = Type.GetType("StardewModdingAPI.Framework.ModHelpers.DataHelper, StardewModdingAPI")!;
        var jsonHelperType = Type.GetType("StardewModdingAPI.Toolkit.Serialization.JsonHelper, SMAPI.Toolkit")!;
        var jsonHelperField = dataHelperType.GetField(
            "JsonHelper",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        var jsonSettingsGetter =
            jsonHelperType.GetProperty(
                "JsonSettings",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetGetMethod()!;
        var jsonHelper = jsonHelperField.GetValue(helper)!;
        return (JsonSerializerSettings)jsonSettingsGetter.Invoke(jsonHelper, null)!;
    }
}
