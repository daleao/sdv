using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace TheLion.Stardew.Common.Integrations
{
	/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
	/// <typeparam name="TConfig">The mod configuration type.</typeparam>
	/// <remarks>Credit to Pathoschild.</remarks>
	internal class GenericModConfigMenuIntegration<TConfig> : BaseIntegration
		where TConfig : new()
	{
		/// <summary>The mod's public API.</summary>
		private readonly IGenericModConfigMenuAPI _modAPI;

		/// <summary>The manifest for the mod consuming the API.</summary>
		private readonly IManifest _consumerManifest;

		/// <summary>Get the current config model.</summary>
		private readonly Func<TConfig> _getConfig;

		/// <summary>_reset the config model to the default values.</summary>
		private readonly Action _reset;

		/// <summary>Save and apply the current config model.</summary>
		private readonly Action _saveAndApply;

		/// <summary>Construct an instance.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		/// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
		/// <param name="getConfig">Get the current config model.</param>
		/// <param name="reset">_reset the config model to the default values.</param>
		/// <param name="saveAndApply">Save and apply the current config model.</param>
		/// <param name="log">Encapsulates monitoring and logging.</param>
		public GenericModConfigMenuIntegration(IModRegistry modRegistry, IManifest consumerManifest,
			Func<TConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
			: base("Generic Mod Config Menu", "spacechase0.GenericModConfigMenu", "1.1.0", modRegistry, log)
		{
			// init
			_consumerManifest = consumerManifest;
			_getConfig = getConfig;
			_reset = reset;
			_saveAndApply = saveAndApply;

			if (!IsLoaded) return;

			// get mod API
			_modAPI = GetValidatedApi<IGenericModConfigMenuAPI>();
			IsLoaded = _modAPI != null;
		}

		/// <summary>Register the mod config.</summary>
		public GenericModConfigMenuIntegration<TConfig> RegisterConfig()
		{
			AssertLoaded();
			_modAPI.RegisterModConfig(_consumerManifest, _reset, _saveAndApply);
			return this;
		}

		/// <summary>Add a label to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		public GenericModConfigMenuIntegration<TConfig> AddLabel(string label, string description = null)
		{
			AssertLoaded();
			_modAPI.RegisterLabel(_consumerManifest, label, description);
			return this;
		}

		/// <summary>Start a new form page.</summary>
		/// <param name="pageName">The name of the page.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNewPage(string pageName)
		{
			AssertLoaded();
			_modAPI.StartNewPage(_consumerManifest, pageName);
			return this;
		}

		/// <summary>Add a label to a different form page.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="page">The target page name.</param>
		public GenericModConfigMenuIntegration<TConfig> AddPageLabel(string label, string description = null,
			string page = "")
		{
			AssertLoaded();
			_modAPI.RegisterPageLabel(_consumerManifest, label, description, page);
			return this;
		}

		/// <summary>Add a checkbox to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddCheckbox(string label, string description,
			Func<TConfig, bool> get, Action<TConfig, bool> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterSimpleOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val)
				);

			return this;
		}

		/// <summary>Add a dropdown to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="choices">The choices to choose from.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddDropdown(string label, string description,
			Func<TConfig, string> get, Action<TConfig, string> set, string[] choices, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterChoiceOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val),
					choices
				);

			return this;
		}

		/// <summary>Add a checkbox to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddTextbox(string label, string description,
			Func<TConfig, string> get, Action<TConfig, string> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterSimpleOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val)
				);

			return this;
		}

		/// <summary>Add a numeric field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNumberField(string label, string description,
			Func<TConfig, int> get, Action<TConfig, int> set, int min, int max, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterClampedOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val),
					min,
					max
				);

			return this;
		}

		/// <summary>Add a numeric field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddNumberField(string label, string description,
			Func<TConfig, float> get, Action<TConfig, float> set, float min, float max, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterClampedOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val),
					min,
					max
				);

			return this;
		}

		/// <summary>Add a key binding field to the form.</summary>
		/// <param name="label">The label text.</param>
		/// <param name="description">A description shown on hover, if any.</param>
		/// <param name="get">Get the current value.</param>
		/// <param name="set">Set a new value.</param>
		/// <param name="enable">Whether the field is enabled.</param>
		public GenericModConfigMenuIntegration<TConfig> AddKeyBinding(string label, string description,
			Func<TConfig, KeybindList> get, Action<TConfig, KeybindList> set, bool enable = true)
		{
			AssertLoaded();

			if (enable)
				_modAPI.RegisterSimpleOption(
					_consumerManifest,
					label,
					description,
					() => get(_getConfig()),
					val => set(_getConfig(), val)
				);

			return this;
		}
	}
}