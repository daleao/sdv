namespace DaLion.Common.Extensions.Stardew;

#region using directives

using ModData;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class GameLocationExtensions
{
    /// <inheritdoc cref="ModDataIO.Read"/>
    public static string Read(this GameLocation location, string field, string defaultValue = "", string modId = "") =>
        ModDataIO.Read(location, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Read{T}"/>
    public static T Read<T>(this GameLocation location, string field, T defaultValue = default, string modId = "") where T : struct =>
        ModDataIO.Read(location, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Write"/>
    public static void Write(this GameLocation building, string field, string? value) =>
        ModDataIO.Write(building, field, value);

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists"/>
    public static void WriteIfNotExists(this GameLocation location, string field, string? value) =>
        ModDataIO.WriteIfNotExists(location, field, value);

    /// <inheritdoc cref="ModDataIO.Append"/>
    public static void Append(this GameLocation location, string field, string value, string separator = ",") =>
        ModDataIO.Append(location, field, value, separator);

    /// <inheritdoc cref="ModDataIO.Increment{T}"/>
    public static void Increment<T>(this GameLocation location, string field, T amount) where T : struct =>
        ModDataIO.Increment(location, field, amount);

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment(this GameLocation location, string field) =>
        ModDataIO.Increment(location, field, 1);
}