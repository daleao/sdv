namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using static System.FormattableString;

using System;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using Common.Extensions;
using Common.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Calculate due income tax for the player.</summary>
    public static int DoTaxes(this Farmer farmer)
    {
        var income = farmer.ReadDataAs<int>(ModData.SeasonIncome);
        var deductible = Game1.player.ReadDataAs<float>(ModData.DeductionPct);
        var taxable = (int) (income * (1f - deductible));
        var bracket = Framework.Utility.GetTaxBracket(taxable);
        var due = (int) Math.Round(income * bracket);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {Game1.game1.GetPrecedingSeason()} season:" +
            $"\n\t- Total income: {income}g" +
            CurrentCulture($"\n\t- Tax deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Total due income tax: {due}g."
        );
        return due;
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Farmer farmer, ModData field, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this Farmer farmer, ModData field, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Farmer farmer, ModData field, [CanBeNull] string value)
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

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Farmer farmer, ModData field, [CanBeNull] string value)
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

    /// <summary>Append a string to an existing string field in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Farmer farmer, ModData field, string value, string separator = ",")
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

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Farmer farmer, ModData field, T amount)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(amount.ToString(), $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Farmer farmer, ModData field)
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
}