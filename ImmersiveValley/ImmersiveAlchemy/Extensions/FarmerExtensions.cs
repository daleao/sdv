using System.Linq;
using DaLion.Stardew.Alchemy.Framework.Enums;
using JetBrains.Annotations;

namespace DaLion.Stardew.Alchemy.Extensions;

#region using directives

using SpaceCore;
using StardewValley;
using StardewModdingAPI;

using Common.Extensions;
using Common.Extensions.Stardew;
using Framework;

using SObject = StardewValley.Object;

#endregion using directives

public static class FarmerExtensions
{
    public static void AddAlchemyExperience(this Farmer farmer, int howMuch)
    {
        Skills.AddExperience(farmer, AlchemySkill.InternalName, howMuch);
    }

    public static int GetAlchemyLevel(this Farmer farmer)
    {
        return Skills.GetSkillLevel(farmer, AlchemySkill.InternalName);
    }

    public static int GetTotalCurrentAlchemyExperience(this Farmer farmer)
    {
        return Skills.GetExperienceFor(farmer, AlchemySkill.InternalName);
    }

    public static bool HasEnoughSubstanceInInventory(this Farmer farmer, PrimarySubstance substance, int amount)
    {
        return farmer.Items.Any(item => item.ContainsPrimarySubstance(substance, out var density) && item.Stack * density >= amount);
    }

    public static bool HasEnoughBaseInInventory(this Farmer farmer, BaseType type)
    {
        return farmer.Items.Any(item => item.IsAlchemicalBase(type, out var purity) && item.Stack * purity >= 4);
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Farmer farmer, DataField field, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field, external to this mod, in the <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadDataExt(this Farmer farmer, string field, string modId, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this Farmer farmer, DataField field, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field, external to this mod, in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataExtAs<T>(this Farmer farmer, string field, string modId, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Farmer farmer, DataField field, [CanBeNull] string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}");
            return;
        }

        Game1.player.modData.Write($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {farmer.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteDataExt(this Farmer farmer, string field, string modId, [CanBeNull] string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}", modId);
            return;
        }

        Game1.player.modData.Write($"{modId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {farmer.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Farmer farmer, DataField field, [CanBeNull] string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}");
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataExtIfNotExists(this Farmer farmer, DataField field, string modId, [CanBeNull] string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{modId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}", modId);
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Farmer farmer, DataField field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}");
            return;
        }

        var current = Game1.player.ReadData(field);
        if (current.Contains(value))
        {
            Log.D($"[ModData]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            Game1.player.WriteData(field, string.IsNullOrEmpty(current) ? value : current + separator + value);
            Log.D($"[ModData]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Append a string to an existing string field, external to this mod, in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void AppendDataExt(this Farmer farmer, string field, string value, string modId, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}", modId);
            return;
        }

        var current = Game1.player.ReadDataExt(field, modId);
        if (current.Contains(value))
        {
            Log.D($"[ModData]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            Game1.player.WriteDataExt(field, string.IsNullOrEmpty(current) ? value : current + separator + value, modId);
            Log.D($"[ModData]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Farmer farmer, DataField field, T amount)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(amount.ToString(), $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(this Farmer farmer, string field, T amount, string modId)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(amount.ToString(), $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Farmer farmer, DataField field)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(this Farmer farmer, string field, string modId)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }
}