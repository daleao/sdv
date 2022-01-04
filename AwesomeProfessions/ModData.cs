using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions;

public class ModData
{
    /// <summary>Easy look-up table for data fields required by each profesion.</summary>
    private static readonly Dictionary<string, List<KeyValuePair<string, string>>> DataFieldsByProfession = new()
    {
        { "Conservationist", new() { new("WaterTrashCollectedThisSeason", "0"), new("ActiveTaxBonusPercent", "0") } },
        { "Ecologist", new() { new("ItemsForaged", "0") } },
        { "Gemologist", new() { new("MineralsCollected", "0") } },
        { "Prospector", new() { new("ProspectorHuntStreak", "0") } },
        { "Scavenger", new() { new("ScavengerHuntStreak", "0") } }
    };

    private readonly string _id;

    /// <summary>Construct an instance.</summary>
    internal ModData(string uniqueID)
    {
        _id = uniqueID;
    }

    private ModDataDictionary Data { get; set; }

    /// <summary>Load reference to local player's persisted mod data.</summary>
    public void Load()
    {
        if (!Context.IsWorldReady) throw new InvalidOperationException("Tried to load mod data before save file.");

        ModEntry.Log("[ModData]: Loading persisted mod data.", LogLevel.Trace);
        Data = Game1.player.modData;
        InitializeDataIfNecessary();
        ModEntry.Log("[ModData]: Done loading data.", LogLevel.Trace);
    }

    /// <summary>Unload local player's persisted mod data.</summary>
    public void Unload()
    {
        ModEntry.Log("[ModData]: Unloading mod data.", LogLevel.Trace);
        Data = null;
    }

    /// <summary>Initialize all data fields for the local player.</summary>
    public void InitializeDataIfNecessary()
    {
        Data.WriteIfNotExists($"{_id}/Initialized", true.ToString(), out var exists);
        if (exists)
        {
            ModEntry.Log($"[ModData]: Data already initialized for farmer {Game1.player.Name}.", LogLevel.Trace);
            return;
        }

        ModEntry.Log($"[ModData]: Initializing data fields for farmer {Game1.player.Name}...", LogLevel.Trace);
        foreach (var professionIndex in Game1.player.professions)
            try
            {
                InitializeDataForProfession(Framework.Utility.Professions.NameOf(professionIndex));
            }
            catch (IndexOutOfRangeException)
            {
                ModEntry.Log($"[ModData]: Unexpected profession index {professionIndex} will be ignored.",
                    LogLevel.Trace);
            }

        Data.WriteIfNotExists($"{_id}/SuperModeIndex", "-1");
        ModEntry.Log("[ModData]: Done initializing data fields for local player.", LogLevel.Trace);
    }

    /// <summary>Initialize data fields for a profession.</summary>
    /// <param name="whichProfession">The profession index.</param>
    public void InitializeDataForProfession(string whichProfession)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        if (!DataFieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

        fields.ForEach(field => Data.WriteIfNotExists($"{_id}/{field.Key}", $"{field.Value}"));
        ModEntry.Log($"[ModData]: Initialized data fields for {whichProfession}.", LogLevel.Trace);
    }

    /// <summary>Clear data entries for a removed profession.</summary>
    /// <param name="whichProfession">The profession index.</param>
    public void RemoveProfessionData(string whichProfession)
    {
        if (Data is null)
        {
            ModEntry.Log("[ModData]: Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        if (!DataFieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

        ModEntry.Log($"[ModData]: Removing data fields for {whichProfession}.", LogLevel.Trace);
        fields.ForEach(field => Data.Write($"{_id}/{field.Key}", null));
    }

    /// <summary>Check if there are rogue data feids and remove them.</summary>
    public void CleanUpRogueData()
    {
        if (Data is null)
        {
            ModEntry.Log("[ModData]: Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        ModEntry.Log("[ModData]: Checking for rogue data fields...", LogLevel.Trace);
        var professionsToRemove =
            from fieldsByProfession in DataFieldsByProfession
            where !fieldsByProfession.Key.IsAnyOf("Scavenger", "Prospector")
            from field in fieldsByProfession.Value
            where Data.ContainsKey(field.Key) && !Game1.player.HasProfession(fieldsByProfession.Key)
            select fieldsByProfession.Key;
        foreach (var profession in professionsToRemove) RemoveProfessionData(profession);
        ModEntry.Log("[ModData]: Done removing rogue data fields.", LogLevel.Trace);
    }

    /// <summary>Read a field from the <see cref="ModData" /> as string.</summary>
    /// <param name="field">The field to read from.</param>
    public string Read(string field)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        return Data.Read($"{_id}/{field}", string.Empty);
    }

    /// <summary>Read a field from the <see cref="ModData" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    public T Read<T>(string field) where T : IComparable
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        return Data.Read<T>($"{_id}/{field}");
    }

    /// <summary>Write to a field in the <see cref="ModData" />, or remove the field if supplied with null.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public void Write(string field, string value)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        Data.Write($"{_id}/{field}", value);
        ModEntry.Log($"[ModData]: Wrote {value} to {field}.", LogLevel.Trace);
    }

    /// <summary>Write to a field in the <see cref="ModData" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public void WriteIfNotExists(string field, string value)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        Data.WriteIfNotExists($"{_id}/{field}", value);
        ModEntry.Log($"[ModData]: Tried initializing {field} with {value}.", LogLevel.Trace);
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModData" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public void Increment<T>(string field, T amount)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        Data.Increment($"{_id}/{field}", amount);
        ModEntry.Log($"[ModData]: Incremented {field} by {amount}.", LogLevel.Trace);
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModData" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public void Increment<T>(string field)
    {
        if (Data is null)
        {
            ModEntry.Log("Mod data was not loaded correctly.", LogLevel.Warn);
            Load();
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32:
                Data.Increment($"{_id}/{field}", 1);
                break;

            case TypeCode.UInt32:
                Data.Increment<uint>($"{_id}/{field}", 1);
                break;
        }

        ModEntry.Log($"[ModData]: Incremented {field} by 1.", LogLevel.Trace);
    }
}