namespace DaLion.Common.Data;

#region using directives

using Extensions;
using Extensions.Stardew;
using Multiplayer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

internal static class ModDataIO
{
    private static Broadcaster _Broadcaster = null!;
    private static string _modID = null!;

    public static void Init(IMultiplayerHelper helper, string modID)
    {
        _Broadcaster = new(helper, modID);
        _modID = modID;
    }

    #region farmer io

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(Farmer farmer, string field, string defaultValue = "") =>
        Game1.MasterPlayer.modData.Read($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(Farmer farmer, string field, T defaultValue = default) where T : struct =>
        Game1.MasterPlayer.modData.ReadAs($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadExtFrom(Farmer farmer, string field, string modId, string defaultValue = "") =>
        Game1.MasterPlayer.modData.Read($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadExtFrom<T>(Farmer farmer, string field, string modId, T defaultValue = default) where T : struct =>
         Game1.MasterPlayer.modData.ReadAs($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, or remove the field if supplied an empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(Farmer farmer, string field, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value ?? string.Empty, $"UpdateData/Write/{field}");
            return;
        }

        Game1.player.modData.Write($"{_modID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field, external to this mod, in the farmer's <see cref="ModDataDictionary" />, or remove the field if supplied an empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteExtTo(Farmer farmer, string field, string modId, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value ?? string.Empty, $"UpdateData/Write/{field}", modId);
            return;
        }

        Game1.player.modData.Write($"{modId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(Farmer farmer, string field, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{_modID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            _Broadcaster.MessageHost(value ?? string.Empty, $"UpdateData/Write/{field}");
        else WriteTo(farmer, field, value);

        return false;
    }

    /// <summary>Write to a field, external to this mod, in the farmer's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteExtIfNotExistsTo(Farmer farmer, string field, string modId, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{modId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            _Broadcaster.MessageHost(value ?? string.Empty, $"UpdateData/Write/{field}", modId);
        else WriteTo(farmer, field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the farmer's <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendTo(Farmer farmer, string field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value, $"UpdateData/Append/{field}");
            return;
        }

        var current = ReadFrom(farmer, field);
        if (current.Contains(value))
        {
            Log.V($"[ModDataIO]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            WriteTo(farmer, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
            Log.V($"[ModDataIO]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Append a string to an existing string field, external to this mod, in the farmer's <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void AppendExtTo(Farmer farmer, string field, string value, string modId, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value, $"UpdateData/Append/{field}", modId);
            return;
        }

        var current = ReadExtFrom(farmer, field, modId);
        if (current.Contains(value))
        {
            Log.V($"[ModDataIO]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            WriteExtTo(farmer, field, string.IsNullOrEmpty(current) ? value : current + separator + value, modId);
            Log.V($"[ModDataIO]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Increment the value of a numeric field in the farmer's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Farmer farmer, string field, T amount) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(amount.ToString()!, $"UpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{_modID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the farmer's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Farmer farmer, string field) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost("1", $"UpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{_modID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by 1.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementExtTo<T>(Farmer farmer, string field, T amount, string modId) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(amount.ToString()!, $"UpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the farmer's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementExtTo<T>(Farmer farmer, string field, string modId) where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost("1", $"UpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by 1.");
    }

    #endregion farmer io

    #region building io

    /// <summary>Read a string from the building's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(Building building, string field, string defaultValue = "") =>
        building.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the building's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(Building building, string field, T defaultValue = default) where T : struct =>
        building.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(Building building, string field, string? value)
    {
        building.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {building.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {building.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(Building building, string field, string? value)
    {
        if (building.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteTo(building, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the building's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendTo(Building building, string field, string value, string separator = ",")
    {
        var current = ReadFrom(building, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {building.GetType().Name}'s {field} already contained {value}.");

        WriteTo(building, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {building.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Building building, string field, T amount) where T : struct
    {
        building.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Building building, string field) where T : struct
    {
        building.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by 1.");
    }

    #endregion building io

    #region character io

    /// <summary>Read a string from the character's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(Character character, string field, string defaultValue = "") =>
        character.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the character's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(Character character, string field, T defaultValue = default) where T : struct =>
        character.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(Character character, string field, string? value)
    {
        character.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {character.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {character.Name}'s {field}.");
    }

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(Character character, string field, string? value)
    {
        if (character.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteTo(character, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the character's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendTo(Character character, string field, string value, string separator = ",")
    {
        var current = ReadFrom(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        WriteTo(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Character character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Character character, string field) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion character io

    #region game location io

    /// <summary>Read a string from the game location's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(GameLocation location, string field, string defaultValue = "") =>
        location.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the game location's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(GameLocation location, string field, T defaultValue = default) where T : struct =>
        location.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(GameLocation location, string field, string? value)
    {
        location.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {location.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {location.Name}'s {field}.");
    }

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(GameLocation location, string field, string? value)
    {
        if (location.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteTo(location, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the game location's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendTo(GameLocation character, string field, string value, string separator = ",")
    {
        var current = ReadFrom(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        WriteTo(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(GameLocation character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(GameLocation character, string field) where T : struct
    {
        character.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion game location io

    #region item io

    /// <summary>Read a string from the item's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(Item item, string field, string defaultValue = "") =>
        item.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the item's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(Item item, string field, T defaultValue = default) where T : struct =>
        item.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(Item item, string field, string? value)
    {
        item.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {item.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {item.Name}'s {field}.");
    }

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(Item item, string field, string? value)
    {
        if (item.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteTo(item, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the item's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendTo(Item item, string field, string value, string separator = ",")
    {
        var current = ReadFrom(item, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {item.Name}'s {field} already contained {value}.");

        WriteTo(item, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {item.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Item item, string field, T amount) where T : struct
    {
        item.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Item item, string field) where T : struct
    {
        item.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by 1.");
    }

    #endregion item io

    #region tree io

    /// <summary>Read a string from the terrain feature's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadFrom(TerrainFeature feature, string field, string defaultValue = "") =>
        feature.modData.Read($"{_modID}/{field}", defaultValue);

    /// <summary>Read a field from the terrain feature's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadFrom<T>(TerrainFeature feature, string field, T defaultValue = default) where T : struct =>
        feature.modData.ReadAs($"{_modID}/{field}", defaultValue);

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteTo(TerrainFeature feature, string field, string? value)
    {
        feature.modData.Write($"{_modID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {feature.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {feature.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteIfNotExistsTo(TerrainFeature feature, string field, string? value)
    {
        if (feature.modData.ContainsKey($"{_modID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        WriteTo(feature, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the terrain feature's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendTo(TerrainFeature feature, string field, string value, string separator = ",")
    {
        var current = ReadFrom(feature, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {feature.GetType().Name}'s {field} already contained {value}.");

        WriteTo(feature, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {feature.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(TerrainFeature feature, string field, T amount) where T : struct
    {
        feature.modData.Increment($"{_modID}/{field}", amount);
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(TerrainFeature feature, string field) where T : struct
    {
        feature.modData.Increment($"{_modID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by 1.");
    }

    #endregion tree io
}