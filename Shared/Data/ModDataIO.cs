namespace DaLion.Shared.ModData;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Multiplayer;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Handles reading from and writing to the <see cref="ModDataDictionary"/> of different objects.</summary>
internal static class ModDataIO
{
    /// <inheritdoc cref="Broadcaster"/>
    private static Broadcaster _broadcaster = null!; // set on init

    /// <summary>Gets the unique of the initialized mod.</summary>
    internal static string? ModId { get; private set; }

    #region farmer rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/> as
    ///     <see cref="string"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(Farmer farmer, string field, string defaultValue = "", string modId = "")
    {
        return Game1.MasterPlayer?.modData.Read(
            $"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue) ?? defaultValue;
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/> as
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(Farmer farmer, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return Game1.MasterPlayer?.modData.Read(
            $"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue) ?? defaultValue;
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/>, or
    ///     removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="field"/>.</param>
    public static void Write(Farmer farmer, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _broadcaster.MessageHost($"Write/{field}/{value ?? string.Empty}", "UpdateData");
            return;
        }

        Game1.player.modData.Write($"{ModId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModDataIO]: Cleared {farmer.Name}'s {field}."
            : $"[ModDataIO]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="field"/> already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Farmer farmer, string field, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.V($"[ModDataIO]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _broadcaster.MessageHost($"Write/{field}/{value ?? string.Empty}", "UpdateData");
        }
        else
        {
            Write(farmer, field, value);
        }

        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/>, or initializes it with that <paramref name="value"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(Farmer farmer, string field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _broadcaster.MessageHost(value, $"UpdateData/Append/{field}");
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

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(Farmer farmer, string field, T amount)
        where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _broadcaster.MessageHost(amount.ToString()!, $"UpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Farmer farmer, string field)
        where T : struct
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            _broadcaster.MessageHost("1", $"UpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModId}/{farmer.UniqueMultiplayerID}/{field}", "1".Parse<T>());
        Log.V($"[ModDataIO]: Incremented {farmer.Name}'s {field} by 1.");
    }

    #endregion farmer rw

    #region building rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>
    ///     as <see cref="string"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(Building building, string field, string defaultValue = "", string modId = "")
    {
        return building.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>
    ///     as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(Building building, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return building.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    public static void Write(Building building, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        building.modData.Write($"{ModId}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {building.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {building.GetType().Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the field already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Building building, string field, string? value)
    {
        if (building.modData.ContainsKey($"{ModId}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(building, field, value);
        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the
    ///     <paramref name="building"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(Building building, string field, string value, string separator = ",")
    {
        var current = Read(building, field);
        if (current.Contains(value))
        {
            Log.V($"[ModData]: {building.GetType().Name}'s {field} already contained {value}.");
        }

        Write(building, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {building.GetType().Name}'s {field} with {value}");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="building"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(Building building, string field, T amount)
        where T : struct
    {
        building.modData.Increment($"{ModId}/{field}", amount);
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the building's <see cref="ModDataDictionary"/>
    ///     by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Building building, string field)
        where T : struct
    {
        building.modData.Increment($"{ModId}/{field}", "1".Parse<T>());
        Log.V($"[ModData]: Incremented {building.GetType().Name}'s {field} by 1.");
    }

    #endregion building rw

    #region character rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> from the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(Character character, string field, string defaultValue = "", string modId = "")
    {
        return character.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> from the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(Character character, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return character.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="character"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="field"/>.</param>
    public static void Write(Character character, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        character.modData.Write($"{ModId}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {character.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {character.Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="character"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="field"/> already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Character character, string field, string? value)
    {
        if (character.modData.ContainsKey($"{ModId}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(character, field, value);
        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the
    ///     <paramref name="character"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(Character character, string field, string value, string separator = ",")
    {
        var current = Read(character, field);
        if (current.Contains(value))
        {
            Log.V($"[ModData]: {character.Name}'s {field} already contained {value}.");
        }

        Write(character, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(Character character, string field, T amount)
        where T : struct
    {
        character.modData.Increment($"{ModId}/{field}", amount);
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Character character, string field)
        where T : struct
    {
        character.modData.Increment($"{ModId}/{field}", "1".Parse<T>());
        Log.V($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }

    #endregion character rw

    #region game location rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> from the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(GameLocation location, string field, string defaultValue = "", string modId = "")
    {
        return location.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> from the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(GameLocation location, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return location.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="location"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    public static void Write(GameLocation location, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        location.modData.Write($"{ModId}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {location.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {location.Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="location"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="field"/> already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(GameLocation location, string field, string? value)
    {
        if (location.modData.ContainsKey($"{ModId}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(location, field, value);
        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the
    ///     <paramref name="location"/>'s <see cref="ModDataDictionary"/>, or initializes it with the that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(GameLocation location, string field, string value, string separator = ",")
    {
        var current = Read(location, field);
        if (current.Contains(value))
        {
            Log.V($"[ModData]: {location.Name}'s {field} already contained {value}.");
        }

        Write(location, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {location.Name}'s {field} with {value}");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(GameLocation location, string field, T amount)
        where T : struct
    {
        location.modData.Increment($"{ModId}/{field}", amount);
        Log.V($"[ModData]: Incremented {location.Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(GameLocation location, string field)
        where T : struct
    {
        location.modData.Increment($"{ModId}/{field}", "1".Parse<T>());
        Log.V($"[ModData]: Incremented {location.Name}'s {field} by 1.");
    }

    #endregion game location rw

    #region item rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/> as
    ///     <see cref="string"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(Item item, string field, string defaultValue = "", string modId = "")
    {
        return item.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/> as
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(Item item, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return item.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/>, or
    ///     removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    public static void Write(Item item, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        item.modData.Write($"{ModId}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {item.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {item.Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/>, only
    ///     if it doesn't yet have a value.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="field"/> already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(Item item, string field, string? value)
    {
        if (item.modData.ContainsKey($"{ModId}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(item, field, value);
        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/>, or initializes it with that <paramref name="value"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(Item item, string field, string value, string separator = ",")
    {
        var current = Read(item, field);
        if (current.Contains(value))
        {
            Log.V($"[ModData]: {item.Name}'s {field} already contained {value}.");
        }

        Write(item, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {item.Name}'s {field} with {value}");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(Item item, string field, T amount)
        where T : struct
    {
        item.modData.Increment($"{ModId}/{field}", amount);
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(Item item, string field)
        where T : struct
    {
        item.modData.Increment($"{ModId}/{field}", "1".Parse<T>());
        Log.V($"[ModData]: Incremented {item.Name}'s {field} by 1.");
    }

    #endregion item rw

    #region terrain feature rw

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static string Read(TerrainFeature terrainFeature, string field, string defaultValue = "", string modId = "")
    {
        return terrainFeature.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Reads from a <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="field"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name = "terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="field"/> does not exist.</param>
    /// <param name="modId">The unique ID of the mod to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="field"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public static T Read<T>(TerrainFeature terrainFeature, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return terrainFeature.modData.Read($"{(string.IsNullOrEmpty(modId) ? ModId : modId)}/{field}", defaultValue);
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/>, or removes it if supplied a null or empty <paramref name="value"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    public static void Write(TerrainFeature terrainFeature, string field, string? value)
    {
        if (string.IsNullOrEmpty(field))
        {
            Log.W("[ModData] Received empty field string.");
            return;
        }

        terrainFeature.modData.Write($"{ModId}/{field}", value);
        Log.V(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {terrainFeature.GetType().Name}'s {field}."
            : $"[ModData]: Wrote {value} to {terrainFeature.GetType().Name}'s {field}.");
    }

    /// <summary>
    ///     Writes to a <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/>, only if it doesn't yet have a value.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="field"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="field"/> already existed, otherwise <see langword="false"/>.</returns>
    public static bool WriteIfNotExists(TerrainFeature terrainFeature, string field, string? value)
    {
        if (terrainFeature.modData.ContainsKey($"{ModId}/{field}"))
        {
            Log.V($"[ModData]: The data field {field} already existed.");
            return true;
        }

        Write(terrainFeature, field, value);
        return false;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="field"/> in the
    ///     <paramref name="terrainFeature"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    public static void Append(TerrainFeature terrainFeature, string field, string value, string separator = ",")
    {
        var current = Read(terrainFeature, field);
        if (current.Contains(value))
        {
            Log.V($"[ModData]: {terrainFeature.GetType().Name}'s {field} already contained {value}.");
        }

        Write(terrainFeature, field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.V($"[ModData]: Appended {terrainFeature.GetType().Name}'s {field} with {value}");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    public static void Increment<T>(TerrainFeature terrainFeature, string field, T amount)
        where T : struct
    {
        terrainFeature.modData.Increment($"{ModId}/{field}", amount);
        Log.V($"[ModData]: Incremented {terrainFeature.GetType().Name}'s {field} by {amount}.");
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="field"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="field"/>. This should most likely be an integer type.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="field">The field to update.</param>
    public static void Increment<T>(TerrainFeature terrainFeature, string field)
        where T : struct
    {
        terrainFeature.modData.Increment($"{ModId}/{field}", "1".Parse<T>());
        Log.V($"[ModData]: Incremented {terrainFeature.GetType().Name}'s {field} by 1.");
    }

    #endregion terrain feature rw

    /// <summary>Initializes the <see cref="ModDataIO"/> with the specified <paramref name="modId"/>.</summary>
    /// <param name="helper">The <see cref="IMultiplayerHelper"/> API of the current <see cref="IMod"/>.</param>
    /// <param name="modId">The unique of the active mod.</param>
    internal static void Init(IMultiplayerHelper helper, string modId)
    {
        _broadcaster = new Broadcaster(helper, modId);
        ModId = modId;
    }
}
