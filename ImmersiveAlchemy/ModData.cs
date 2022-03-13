namespace DaLion.Stardew.Alchemy;

#region using directives

using StardewModdingAPI;
using StardewValley;

using Common.Extensions;

#endregion using directives

/// <summary>Wrapper to facilitate reading from and writing to the main player's <see cref="ModDataDictionary" />.</summary>
internal static class ModData
{
    /// <summary>Check if there are rogue data fields and remove them.</summary>
    internal static void CleanUpRogueData()
    {
        Log.D("[ModData]: Checking for rogue data fields...");
        var data = Game1.player.modData;
        var count = 0;
        
        // ...

        Log.D($"[ModData]: Found {count} rogue data fields.");
    }

    /// <summary>Read a string from the <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="who">The farmer whose data should be read.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    internal static string Read(DataField field, Farmer who = null, string defaultValue = "")
    {
        who ??= Game1.player;
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read a field from the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="who">The farmer whose data should read.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    internal static T ReadAs<T>(DataField field, Farmer who = null, T defaultValue = default)
    {
        who ??= Game1.player;
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with null.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <param name="who">The farmer whose data should be written.</param>
    internal static void Write(DataField field, string value, Farmer who = null)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
            return;
        }

        who ??= Game1.player;
        Game1.MasterPlayer.modData.Write($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {who.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    /// <param name="who">The farmer whose data should be written.</param>
    internal static bool WriteIfNotExists(DataField field, string value, Farmer who = null)
    {
        who ??= Game1.player;
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] {ModEntry.Manifest.UniqueID},
                new[] {Game1.MasterPlayer.UniqueMultiplayerID}); // request the main player
        else Write(field, value);

        return false;
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <param name="who">The farmer whose data should be incremented.</param>
    internal static void Increment<T>(DataField field, T amount, Farmer who = null)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(amount, $"RequestUpdateData/Increment/{field}",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
            return;
        }

        who ??= Game1.player;
        Game1.MasterPlayer.modData.Increment($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {who.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="who">The farmer whose data should be incremented.</param>
    internal static void Increment<T>(DataField field, Farmer who = null)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(1, $"RequestUpdateData/Increment/{field}",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
            return;
        }

        who ??= Game1.player;
        Game1.MasterPlayer.modData.Increment($"{ModEntry.Manifest.UniqueID}/{who.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {who.Name}'s {field} by 1.");
    }
}