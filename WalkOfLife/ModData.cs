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
		private ModDataDictionary _data;
		private readonly string _id;

		/// <summary>Easy look-up table for data fields required by each profesion.</summary>
		private static readonly Dictionary<int, List<KeyValuePair<string, string>>> FieldsByProfession = new()
		{
			{
				Framework.Util.Professions.IndexOf("Conservationist"),
				new()
				{ new("WaterTrashCollectedThisSeason", "0"), new("ActiveTaxBonusPercent", "0") }
			},
			{
				Framework.Util.Professions.IndexOf("Ecologist"),
				new()
				{ new("ItemsForaged", "0") }
			},
			{
				Framework.Util.Professions.IndexOf("Gemologist"),
				new()
				{ new("MineralsCollected", "0") }
			},
			{
				Framework.Util.Professions.IndexOf("Prospector"),
				new()
				{ new("ProspectorHuntStreak", "0") }
			},
			{
				Framework.Util.Professions.IndexOf("Scavenger"),
				new()
				{ new("ScavengerHuntStreak", "0") }
			}
		};

		/// <summary>Construct an instance.</summary>
		internal ModData()
		{
			_id = ModEntry.UniqueID;
		}

		public void Load()
		{
			if (!Context.IsWorldReady) throw new InvalidOperationException("Tried to load mod data before save file.");

			ModEntry.Log("Loading persisted mod data.", LogLevel.Trace);
			_data = Game1.player.modData;
			InitializeDataFieldsForLocalPlayer();
			ModEntry.Log("Done loading data.", LogLevel.Trace);
		}

		public void Unload()
		{
			ModEntry.Log("Unloading mod data.", LogLevel.Info);
			_data = null;
		}

		/// <summary>Initialize all data fields for the local player.</summary>
		public void InitializeDataFieldsForLocalPlayer()
		{
			ModEntry.Log("Initializing data fields for local player...", LogLevel.Trace);
			foreach (var professionIndex in Game1.player.professions) InitializeDataFieldsForProfession(professionIndex);
			_data.WriteIfNotExists($"{_id}/SuperModeIndex", "-1");
			ModEntry.Log("Done initializing data fields for local player.", LogLevel.Trace);
		}

		/// <summary>Initialize data fields for a profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void InitializeDataFieldsForProfession(int whichProfession)
		{
			if (!FieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

			ModEntry.Log($"Initializing data fields for {Framework.Util.Professions.NameOf(whichProfession)}.", LogLevel.Trace);
			fields.ForEach(field => _data.WriteIfNotExists($"{_id}/{field.Key}", $"{field.Value}"));
		}

		/// <summary>Clear data entries for a removed profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void RemoveProfessionDataFields(int whichProfession)
		{
			if (!FieldsByProfession.TryGetValue(whichProfession, out var fields)) return;

			ModEntry.Log($"Removing data fields for {Framework.Util.Professions.NameOf(whichProfession)}.", LogLevel.Trace);
			fields.ForEach(field => _data.Write($"{_id}/{field.Key}", null));
		}

		/// <summary>Check if there are rogue data feids and remove them.</summary>
		public void CleanUpRogueDataFields()
		{
			ModEntry.Log("Checking for rogue data fields...", LogLevel.Trace);
			foreach (var kvp in from kvp in FieldsByProfession
								from field in kvp.Value
								where _data.ContainsKey(field.Key) && !Game1.player.HasProfession(kvp.Key)
								select kvp)
				RemoveProfessionDataFields(kvp.Key);

			ModEntry.Log("Done cleaning up rogue data fields.", LogLevel.Trace);
		}

		/// <summary>Read a field from the <see cref="ModData"/> as string.</summary>
		/// <param name="field">The field to read from.</param>
		public string ReadField(string field)
		{
			return _data.Read($"{_id}/{field}");
		}

		/// <summary>Read a field from the <see cref="ModData"/> as <typeparamref name="T"/>.</summary>
		/// <param name="field">The field to read from.</param>
		public T ReadField<T>(string field)
		{
			return _data.Read<T>($"{_id}/{field}");
		}

		/// <summary>Write to a field in the <see cref="ModData"/>, or remove the field if supplied with null.</summary>
		/// <param name="field">The field to write to.</param>
		/// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
		public void WriteField(string field, string value)
		{
			_data.Write($"{_id}/{field}", value);
			ModEntry.Log($"Wrote {value} to {field}.", LogLevel.Trace);
		}

		/// <summary>Increment the value of a numeric field in the <see cref="ModData"/> by an arbitrary amount.</summary>
		/// <param name="field">The field to update.</param>
		/// <param name="amount">Amount to increment by.</param>
		public void IncrementField<T>(string field, T amount)
		{
			_data.Increment($"{_id}/{field}", amount);
			ModEntry.Log($"Incremented {field} by {amount}.", LogLevel.Trace);
		}

		/// <summary>Increment the value of a numeric field in the <see cref="ModData"/> by 1.</summary>
		/// <param name="field">The field to update.</param>
		public void IncrementField<T>(string field)
		{
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Int16:
					_data.Increment<short>($"{_id}/{field}", 1);
					break;
				case TypeCode.UInt16:
					_data.Increment<ushort>($"{_id}/{field}", 1);
					break;
				case TypeCode.Int32:
					_data.Increment<int>($"{_id}/{field}", 1);
					break;
				case TypeCode.UInt32:
					_data.Increment<uint>($"{_id}/{field}", 1);
					break;
				case TypeCode.Int64:
					_data.Increment<long>($"{_id}/{field}", 1);
					break;
				case TypeCode.UInt64:
					_data.Increment<ulong>($"{_id}/{field}", 1);
					break;
			}

			ModEntry.Log($"Incremented {field} by 1.", LogLevel.Trace);
		}
	}
}
