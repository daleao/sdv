namespace DaLion.Shared.Data;

#region using directives

using DaLion.Shared.Extensions;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Mods;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Handles reading from and writing to the <see cref="ModDataDictionary"/> of different objects.</summary>
// ReSharper disable once InconsistentNaming
public class ModDataManager
{
    private readonly Logger _log;
    private readonly string _id;

    private readonly Dictionary<string, Action<string>> _readCallbacks = [];
    private readonly Dictionary<string, Action<string, string>> _writeCallbacks = [];

    /// <summary>Initializes a new instance of the <see cref="ModDataManager"/> class.</summary>
    /// <param name="id">The ID of the parent <see cref="Mod"/>.</param>
    /// <param name="logger">Reference to a <see cref="Logger"/> instance.</param>
    public ModDataManager(string id, Logger logger)
    {
        this._id = id;
        this._log = logger;
    }

    /// <summary>Adds the specified <paramref name="callback"/> to be triggered when reading from the specified <paramref name="key"/>.</summary>
    /// <param name="key">The data key.</param>
    /// <param name="callback">A <see cref="Action"/> to invoke.</param>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="key"/> already has an associated callback.</exception>
    /// <remarks>Does not distinguish between object types; i.e., data keys must be universally unique.</remarks>
    public void RegisterReadCallback(string key, Action<string> callback)
    {
        if (!this._readCallbacks.TryAdd(key, callback))
        {
            ThrowHelper.ThrowInvalidOperationException($"The key {key} already has an associated Read callback.");
        }
    }

    /// <summary>Adds the specified <paramref name="callback"/> to be triggered when writing to the specified <paramref name="key"/>.</summary>
    /// <param name="key">The data key.</param>
    /// <param name="callback">A <see cref="Action"/> to invoke.</param>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="key"/> already has an associated callback.</exception>
    /// <remarks>Does not distinguish between object types; i.e., data keys must be universally unique.</remarks>
    public void RegisterWriteCallback(string key, Action<string, string> callback)
    {
        if (!this._writeCallbacks.TryAdd(key, callback))
        {
            ThrowHelper.ThrowInvalidOperationException($"The key {key} already has an associated Write callback.");
        }
    }

    #region farmer rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/> as
    ///     <see cref="string"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(Farmer farmer, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = farmer.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/> as
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(Farmer farmer, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = farmer.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/>, or
    ///     removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> or empty to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(Farmer farmer, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = farmer.modData.Read($"{modId}/{key}");
        farmer.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {farmer.Name}'s {key}."
            : $"[ModDataManager]: {farmer.Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="farmer"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> already existed, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(Farmer farmer, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (farmer.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(farmer, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/>, or initializes it with that <paramref name="value"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(Farmer farmer, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(farmer, key, value))
        {
            return;
        }

        var oldValue = farmer.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        farmer.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {farmer.Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(Farmer farmer, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = farmer.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        farmer.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {farmer.Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="farmer"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(Farmer farmer, string key, string? modId = null)
    {
        this.Increment(farmer, key, 1, modId);
    }

    #endregion farmer rw

    #region building rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>
    ///     as <see cref="string"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(Building building, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = building.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>
    ///     as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(Building building, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = building.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(Building building, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = building.modData.Read($"{modId}/{key}");
        building.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {building.GetType().Name}'s {key}."
            : $"[ModDataManager]: {building.GetType().Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="building"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if data was written, or <see langword="false"/> if the key already existed.</returns>
    public bool WriteIfNotExists(Building building, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (building.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(building, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the
    ///     <paramref name="building"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(Building building, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(building, key, value))
        {
            return;
        }

        var oldValue = building.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        building.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {building.GetType().Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="building"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(Building building, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = building.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        building.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {building.GetType().Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the building's <see cref="ModDataDictionary"/>
    ///     by 1.
    /// </summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(Building building, string key, string? modId = null)
    {
        modId ??= this._id;
        this.Increment(building, key, 1, modId);
    }

    #endregion building rw

    #region character rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> from the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(Character character, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = character.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> from the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(Character character, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = character.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="character"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> or empty to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(Character character, string key, string? newValue, string? modId = null)
    {
        this.AssertKeyNotEmpty(key);
        var oldValue = character.modData.Read($"{modId}/{key}");
        character.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {character.Name}'s {key}."
            : $"[ModDataManager]: {character.Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="character"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> or empty to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> already existed, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(Character character, string key, string? value, string? modId = null)
    {
        if (character.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(character, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the
    ///     <paramref name="character"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(Character character, string key, string value, char separator = ',', string? modId = null)
    {
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(character, key, value))
        {
            return;
        }

        var oldValue = character.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        character.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {character.Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(Character character, string key, T amount, string? modId = null)
        where T : struct
    {
        this.AssertKeyNotEmpty(key);
        var oldValue = character.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        character.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {character.Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="character"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(Character character, string key, string? modId = null)
    {
        this.Increment(character, key, 1, modId);
    }

    #endregion character rw

    #region gamelocation rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> from the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(GameLocation location, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = location.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> from the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(GameLocation location, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = location.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="location"/>'s <see cref="ModDataDictionary"/>,
    ///     or removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(GameLocation location, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = location.modData.Read($"{modId}/{key}");
        location.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {location.Name}'s {key}."
            : $"[ModDataManager]: {location.Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="location"/>'s <see cref="ModDataDictionary"/>,
    ///     only if it doesn't yet have a value.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> already existed, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(GameLocation location, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (location.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(location, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the
    ///     <paramref name="location"/>'s <see cref="ModDataDictionary"/>, or initializes it with the that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(GameLocation location, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(location, key, value))
        {
            return;
        }

        var oldValue = location.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        location.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {location.Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(GameLocation location, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = location.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        location.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {location.Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="location"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(GameLocation location, string key, string? modId = null)
    {
        modId ??= this._id;
        this.Increment(location, key, 1, modId);
    }

    #endregion gamelocation rw

    #region item rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/> as
    ///     <see cref="string"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(Item item, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = item.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/> as
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(Item item, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = item.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/>, or
    ///     removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(Item item, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = item.modData.Read($"{modId}/{key}");
        item.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {item.Name}'s {key}."
            : $"[ModDataManager]: {item.Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="item"/>'s <see cref="ModDataDictionary"/>, only
    ///     if it doesn't yet have a value.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> already existed, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(Item item, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (item.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(item, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/>, or initializes it with that <paramref name="value"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(Item item, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(item, key, value))
        {
            return;
        }

        var oldValue = item.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        item.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {item.Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(Item item, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = item.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        item.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {item.Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="item"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(Item item, string key, string? modId = null)
    {
        this.Increment(item, key, 1, modId);
    }

    #endregion item rw

    #region terrainfeature rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(TerrainFeature terrainFeature, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = terrainFeature.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name = "terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(TerrainFeature terrainFeature, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = terrainFeature.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/>, or removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(TerrainFeature terrainFeature, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = terrainFeature.modData.Read($"{modId}/{key}");
        terrainFeature.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared {terrainFeature.GetType().Name}'s {key}."
            : $"[ModDataManager]: {terrainFeature.GetType().Name}'s {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/>, only if it doesn't yet have a value.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> was written to, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(TerrainFeature terrainFeature, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (terrainFeature.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(terrainFeature, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the
    ///     <paramref name="terrainFeature"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(TerrainFeature terrainFeature, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(terrainFeature, key, value))
        {
            return;
        }

        var oldValue = terrainFeature.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        terrainFeature.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended {terrainFeature.GetType().Name}'s {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(TerrainFeature terrainFeature, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = terrainFeature.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        terrainFeature.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented {terrainFeature.GetType().Name}'s {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="terrainFeature"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(TerrainFeature terrainFeature, string key, string? modId = null)
    {
        this.Increment(terrainFeature, key, 1, modId);
    }

    #endregion terrainfeature rw

    #region crop rw

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/> as <see cref="string"/>.
    /// </summary>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue">The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as a <see cref="string"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public string Read(Crop crop, string key, string defaultValue = "", string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = crop.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value);
        }

        return value;
    }

    /// <summary>
    ///     Reads from a <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/> as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type for the <paramref name="key"/>'s value. This should most likely be a primitive.</typeparam>
    /// <param name = "crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="defaultValue"> The value to return if the <paramref name="key"/> does not exist.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns>The value of the <paramref name="key"/> as <typeparamref name="T"/>, if it exists, or <paramref name="defaultValue"/> if not.</returns>
    public T ReadAs<T>(Crop crop, string key, T defaultValue = default, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var value = crop.modData.Read($"{modId}/{key}", defaultValue);
        if (this._readCallbacks.TryGetValue(key, out var callback))
        {
            callback(value.ToString() ?? string.Empty);
        }

        return value;
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/>, or removes it if supplied a null or empty <paramref name="newValue"/>.
    /// </summary>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="newValue">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Write(Crop crop, string key, string? newValue, string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = crop.modData.Read($"{modId}/{key}");
        crop.modData.Write($"{modId}/{key}", newValue);
        this._log.V(string.IsNullOrEmpty(newValue)
            ? $"[ModDataManager]: Cleared crop's {key}."
            : $"[ModDataManager]: crop's {key} changed from {oldValue} to {newValue}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue ?? string.Empty);
        }
    }

    /// <summary>
    ///     Writes to a <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/>, only if it doesn't yet have a value.
    /// </summary>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to write to.</param>
    /// <param name="value">The value to write, or <see langword="null"/> to remove the <paramref name="key"/>.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    /// <returns><see langword="true"/> if the <paramref name="key"/> was written to, otherwise <see langword="false"/>.</returns>
    public bool WriteIfNotExists(Crop crop, string key, string? value, string? modId = null)
    {
        modId ??= this._id;
        if (crop.modData.ContainsKey($"{modId}/{key}"))
        {
            this._log.V($"[ModDataManager]: The data key {key} already existed.");
            return false;
        }

        this.Write(crop, key, value, modId);
        return true;
    }

    /// <summary>
    ///     Appends a <paramref name="value"/> to an existing <paramref name="key"/> in the
    ///     <paramref name="crop"/>'s <see cref="ModDataDictionary"/>, or initializes it with that
    ///     <paramref name="value"/>.
    /// </summary>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">A <see cref="string"/> with which to separate appended values.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Append(Crop crop, string key, string value, char separator = ',', string? modId = null)
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        if (this.WriteIfNotExists(crop, key, value))
        {
            return;
        }

        var oldValue = crop.modData.Read($"{modId}/{key}");
        var newValue = oldValue + separator + value;
        crop.modData.Write($"{modId}/{key}", newValue);
        this._log.V($"[ModDataManager]: Appended crop's {key} with {value}");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue, newValue);
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/> by an arbitrary <paramref name="amount"/>.
    /// </summary>
    /// <typeparam name="T">A numeric type with which to increment the <paramref name="key"/>. This should most likely be an integer type.</typeparam>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment<T>(Crop crop, string key, T amount, string? modId = null)
        where T : struct
    {
        modId ??= this._id;
        this.AssertKeyNotEmpty(key);
        var oldValue = crop.modData.Read<T>($"{modId}/{key}");
        var newValue = oldValue.GenericAdd(amount);
        crop.modData.Write($"{modId}/{key}", newValue.ToString());
        this._log.V($"[ModDataManager]: Incremented crop's {key} by {amount}.");
        if (this._writeCallbacks.TryGetValue(key, out var callback))
        {
            callback(oldValue.ToString() ?? "0", newValue.ToString() ?? amount.ToString() ?? "0");
        }
    }

    /// <summary>
    ///     Increments the value of a numeric <paramref name="key"/> in the <paramref name="crop"/>'s
    ///     <see cref="ModDataDictionary"/> by 1.
    /// </summary>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <param name="key">The key to update.</param>
    /// <param name="modId">The unique ID of the owner mod, to be used as an identifier.</param>
    public void Increment(Crop crop, string key, string? modId = null)
    {
        this.Increment(crop, key, 1, modId);
    }

    #endregion crop rw

    private void AssertKeyNotEmpty(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            ThrowHelper.ThrowInvalidOperationException("ModDataManager received empty key string.");
        }
    }
}
