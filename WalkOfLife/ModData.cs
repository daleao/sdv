using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions
{
	public class ModData
	{
		private ModDataDictionary Data { get; set; }
		private Action<string, LogLevel> Log { get; }

		private readonly string _id;

		/// <summary>Easy look-up table for data fields required by each profesion.</summary>
		private static readonly Dictionary<string, List<KeyValuePair<string, string>>> FieldsByProfession = new()
		{
			{ "Conservationist", new() { new("WaterTrashCollectedThisSeason", "0"), new("ActiveTaxBonusPercent", "0") } },
			{ "Ecologist", new() { new("ItemsForaged", "0") } },
			{ "Gemologist", new() { new("MineralsCollected", "0") } },
			{ "Prospector", new() { new("ProspectorHuntStreak", "0") } },
			{ "Scavenger", new() { new("ScavengerHuntStreak", "0") } }
		};

		/// <summary>Construct an instance.</summary>
		internal ModData(Action<string, LogLevel> log, string uniqueID)
		{
			Log = log;
			_id = uniqueID;
		}

		/// <summary>Load reference to local player's persisted mod data.</summary>
		public void Load()
		{
			if (!Context.IsWorldReady) throw new InvalidOperationException("Tried to load mod data before save file.");

			Log("[ModData]: Loading persisted mod data.", LogLevel.Trace);
			Data = Game1.player.modData;
			InitializeDataFieldsForLocalPlayer();
			Log("[ModData]: Done loading data.", LogLevel.Trace);
		}

		/// <summary>Unload local player's persisted mod data.</summary>
		public void Unload()
		{
			Log("[ModData]: Unloading mod data.", LogLevel.Info);
			Data = null;
		}

		/// <summary>Initialize all data fields for the local player.</summary>
		public void InitializeDataFieldsForLocalPlayer()
		{
			Log("[ModData]: Initializing data fields for local player...", LogLevel.Trace);
			foreach (var professionIndex in Game1.player.professions)
				try
				{
					InitializeDataFieldsForProfession(Framework.Util.Professions.NameOf(professionIndex));
				}
				catch (IndexOutOfRangeException)
				{
					Log($"[ModData]: Unexpected profession index {professionIndex} will be ignored.", LogLevel.Trace);
				}

			Data.WriteIfNotExists($"{_id}/SuperModeIndex", "-1");
			Log("[ModData]: Done initializing data fields for local player.", LogLevel.Trace);
		}

		/// <summary>Initialize data fields for a profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void InitializeDataFieldsForProfession(string whichProfession)
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			if (!FieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

			Log($"[ModData]: Initializing data fields for {whichProfession}.", LogLevel.Trace);
			fields.ForEach(field => Data.WriteIfNotExists($"{_id}/{field.Key}", $"{field.Value}"));
		}

		/// <summary>Clear data entries for a removed profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void RemoveProfessionDataFields(string whichProfession)
		{
			if (Data is null)
			{
				Log("[ModData]: Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			if (!FieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

			Log($"[ModData]: Removing data fields for {whichProfession}.", LogLevel.Trace);
			fields.ForEach(field => Data.Write($"{_id}/{field.Key}", null));
		}

		/// <summary>Check if there are rogue data feids and remove them.</summary>
		public void CleanUpRogueDataFields()
		{
			if (Data is null)
			{
				Log("[ModData]: Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			Log("[ModData]: Checking for rogue data fields...", LogLevel.Trace);
			var professionsToRemove =
				from fieldsByProfession in FieldsByProfession
				where !fieldsByProfession.Key.AnyOf("Scavenger", "Prospector")
				from field in fieldsByProfession.Value
				where Data.ContainsKey(field.Key) && !Game1.player.HasProfession(fieldsByProfession.Key)
				select fieldsByProfession.Key;
			foreach (var profession in professionsToRemove) RemoveProfessionDataFields(profession);
			Log("[ModData]: Done removing rogue data fields.", LogLevel.Trace);
		}

		/// <summary>Read a field from the <see cref="ModData"/> as string.</summary>
		/// <param name="field">The field to read from.</param>
		public string ReadField(string field)
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			return Data.Read($"{_id}/{field}", "");
		}

		/// <summary>Read a field from the <see cref="ModData"/> as <typeparamref name="T"/>.</summary>
		/// <param name="field">The field to read from.</param>
		public T ReadField<T>(string field) where T : IComparable
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			return Data.Read<T>($"{_id}/{field}");
		}

		/// <summary>Write to a field in the <see cref="ModData"/>, or remove the field if supplied with null.</summary>
		/// <param name="field">The field to write to.</param>
		/// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
		public void WriteField(string field, string value)
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			Data.Write($"{_id}/{field}", value);
			Log($"[ModData]: Wrote {value} to {field}.", LogLevel.Trace);
		}

		/// <summary>Increment the value of a numeric field in the <see cref="ModData"/> by an arbitrary amount.</summary>
		/// <param name="field">The field to update.</param>
		/// <param name="amount">Amount to increment by.</param>
		public void IncrementField<T>(string field, T amount)
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			Data.Increment($"{_id}/{field}", amount);
			Log($"[ModData]: Incremented {field} by {amount}.", LogLevel.Trace);
		}

		/// <summary>Increment the value of a numeric field in the <see cref="ModData"/> by 1.</summary>
		/// <param name="field">The field to update.</param>
		public void IncrementField<T>(string field)
		{
			if (Data is null)
			{
				Log("Mod data was not loaded correctly.", LogLevel.Warn);
				Load();
			}

			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Int32:
					Data.Increment<int>($"{_id}/{field}", 1);
					break;

				case TypeCode.UInt32:
					Data.Increment<uint>($"{_id}/{field}", 1);
					break;
			}

			Log($"[ModData]: Incremented {field} by 1.", LogLevel.Trace);
		}
	}
}