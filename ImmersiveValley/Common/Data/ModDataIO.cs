namespace DaLion.Common.ModData;

#region using directives

using Extensions;
using Multiplayer;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

internal static class ModDataIO
{
    private static Broadcaster _Broadcaster = null!;
    public static string? ModID { get; private set; }

    internal static void Init(IMultiplayerHelper helper, string modID)
    {
        _Broadcaster = new(helper, modID);
        ModID = modID;
    }

    #region farmer rw

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(Farmer farmer, string field, string defaultValue = "", string modId = "") =>
        Game1.MasterPlayer.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Read from a field in the farmer's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(Farmer farmer, string field, T defaultValue = default, string modId = "") where T : struct =>
        Game1.MasterPlayer.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, or remove the field if supplied an empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(Farmer farmer, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost($"Write/{field}/{value ?? string.Empty}", "UpdateData");
            return;
        }

        Game1.player.modData.Write($"{ModID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the farmer's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Farmer farmer, string field, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            _Broadcaster.MessageHost($"Write/{field}/{value ?? string.Empty}", "UpdateData");
        else Write(farmer, field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the farmer's <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void Append(Farmer farmer, string field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _Broadcaster.MessageHost(value, $"UpdateData/Append/{field}");
            return;
        }

        var current = Read(farmer, field);
        if (current.Contains(value))
        {
            Log.V($"[ModDataIO]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            Write(farmer, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
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

        Game1.player.modData.Increment($"{ModID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    #endregion farmer rw

    #region building rw

    /// <summary>Read a string from the building's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(Building building, string field, string defaultValue = "", string modId = "") =>
        building.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Read a field from the building's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(Building building, string field, T defaultValue = default, string modId = "") where T : struct =>
        building.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(Building building, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        building.modData.Write($"{ModID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {building.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {building.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the building's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Building building, string field, string? value)
    {
        if (building.modData.ContainsKey($"{ModID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(building, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the building's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void Append(Building building, string field, string value, string separator = ",")
    {
        var current = Read(building, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {building.GetType().Name}'s {field} already contained {value}.");

        Write(building, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {building.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Building building, string field, T amount) where T : struct
    {
        building.modData.Increment($"{ModID}/{field}", amount);
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Building building, string field) where T : struct
    {
        building.modData.Increment($"{ModID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by 1.");
    }

    #endregion building rw

    #region character rw

    /// <summary>Read a string from the character's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(Character character, string field, string defaultValue = "", string modId = "") =>
        character.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Read a field from the character's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(Character character, string field, T defaultValue = default, string modId = "") where T : struct =>
        character.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(Character character, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        character.modData.Write($"{ModID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {character.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {character.Name}'s {field}.");
    }

    /// <summary>Write to a field in the character's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Character character, string field, string? value)
    {
        if (character.modData.ContainsKey($"{ModID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(character, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the character's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void Append(Character character, string field, string value, string separator = ",")
    {
        var current = Read(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        Write(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Character character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{ModID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Character character, string field) where T : struct
    {
        character.modData.Increment($"{ModID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion character rw

    #region game location rw

    /// <summary>Read a string from the game location's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(GameLocation location, string field, string defaultValue = "", string modId = "") =>
        location.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Read a field from the game location's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(GameLocation location, string field, T defaultValue = default, string modId = "") where T : struct =>
        location.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(GameLocation location, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        location.modData.Write($"{ModID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {location.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {location.Name}'s {field}.");
    }

    /// <summary>Write to a field in the game location's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(GameLocation location, string field, string? value)
    {
        if (location.modData.ContainsKey($"{ModID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(location, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the game location's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void Append(GameLocation character, string field, string value, string separator = ",")
    {
        var current = Read(character, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");

        Write(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(GameLocation character, string field, T amount) where T : struct
    {
        character.modData.Increment($"{ModID}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the game location's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(GameLocation character, string field) where T : struct
    {
        character.modData.Increment($"{ModID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion game location rw

    #region item rw

    /// <summary>Read a string from the item's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(Item item, string field, string defaultValue = "", string modId = "") =>
        item.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Read a field from the item's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(Item item, string field, T defaultValue = default, string modId = "") where T : struct =>
        item.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(Item item, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        item.modData.Write($"{ModID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {item.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {item.Name}'s {field}.");
    }

    /// <summary>Write to a field in the item's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Item item, string field, string? value)
    {
        if (item.modData.ContainsKey($"{ModID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(item, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the item's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void Append(Item item, string field, string value, string separator = ",")
    {
        var current = Read(item, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {item.Name}'s {field} already contained {value}.");

        Write(item, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {item.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(Item item, string field, T amount) where T : struct
    {
        item.modData.Increment($"{ModID}/{field}", amount);
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the item's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Item item, string field) where T : struct
    {
        item.modData.Increment($"{ModID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by 1.");
    }

    #endregion item rw

    #region terrain feature rw

    /// <summary>Read a string from the terrain feature's <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static string Read(TerrainFeature feature, string field, string defaultValue = "", string modId = "") =>
        feature.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Read a field from the terrain feature's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    public static T Read<T>(TerrainFeature feature, string field, T defaultValue = default, string modId = "") where T : struct =>
        feature.modData.ReadAs($"{(string.IsNullOrEmpty(modId) ? ModID : modId)}/{field}", defaultValue);

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void Write(TerrainFeature feature, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        feature.modData.Write($"{ModID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {feature.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {feature.GetType().Name}'s {field}.");
    }

    /// <summary>Write to a field in the terrain feature's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(TerrainFeature feature, string field, string? value)
    {
        if (feature.modData.ContainsKey($"{ModID}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(feature, field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in the terrain feature's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void Append(TerrainFeature feature, string field, string value, string separator = ",")
    {
        var current = Read(feature, field);
        if (current.Contains(value))
            Log.V($"[ModData]: {feature.GetType().Name}'s {field} already contained {value}.");

        Write(feature, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {feature.GetType().Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void Increment<T>(TerrainFeature feature, string field, T amount) where T : struct
    {
        feature.modData.Increment($"{ModID}/{field}", amount);
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the terrain feature's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(TerrainFeature feature, string field) where T : struct
    {
        feature.modData.Increment($"{ModID}/{field}",
            "1".Parse<T>());
        Log.V($"[ModData]: Incremented {feature.GetType().Name}'s {field} by 1.");
    }

    #endregion terrain feature rw
}