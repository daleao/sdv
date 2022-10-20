namespace DaLion.Common.Integrations.GenericModConfigMenu;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
/// <typeparam name="TConfig">The mod configuration type.</typeparam>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
internal sealed class GenericModConfigMenuIntegration<TConfig> : BaseIntegration<IGenericModConfigMenuApi>
    where TConfig : new()
{
    /// <summary>The manifest for the mod consuming the API.</summary>
    private readonly IManifest _consumerManifest;

    /// <summary>Get the current config model.</summary>
    private readonly Func<TConfig> _getConfig;

    /// <summary>Reset the config model to the default values.</summary>
    private readonly Action _reset;

    /// <summary>Save and apply the current config model.</summary>
    private readonly Action _saveAndApply;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegration{TConfig}"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the mod's config to its default values.</param>
    /// <param name="saveAndApply">Save the mod's current config to the <c>config.json</c> file.</param>
    public GenericModConfigMenuIntegration(
        IModRegistry modRegistry, IManifest consumerManifest, Func<TConfig> getConfig, Action reset, Action saveAndApply)
        : base("Generic Mod Config Menu", "spacechase0.GenericModConfigMenu", "1.6.0", modRegistry)
    {
        this._consumerManifest = consumerManifest;
        this._getConfig = getConfig;
        this._reset = reset;
        this._saveAndApply = saveAndApply;
    }

    /// <summary>Register the mod config.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> Register(bool titleScreenOnly = false)
    {
        this.AssertLoaded();
        this.ModApi!.Register(this._consumerManifest, this._reset, this._saveAndApply, titleScreenOnly);
        return this;
    }

    /// <summary>
    ///     Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered
    ///     after this will be part of that page.
    /// </summary>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    /// <remarks>
    ///     You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a
    ///     multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.
    /// </remarks>
    public GenericModConfigMenuIntegration<TConfig> AddPage(string pageId, Func<string>? pageTitle = null)
    {
        this.AssertLoaded();
        this.ModApi!.AddPage(this._consumerManifest, pageId, pageTitle);
        return this;
    }

    /// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="text">The link text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddPageLink(
        string pageId, Func<string> text, Func<string>? tooltip = null)
    {
        this.AssertLoaded();
        this.ModApi!.AddPageLink(this._consumerManifest, pageId, text, tooltip);
        return this;
    }

    /// <summary>Add a section title at the current position in the form.</summary>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">
    ///     The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the
    ///     tooltip.
    /// </param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddSectionTitle(Func<string> text, Func<string>? tooltip = null)
    {
        this.AssertLoaded();
        this.ModApi!.AddSectionTitle(this._consumerManifest, text, tooltip);
        return this;
    }

    /// <summary>Add a paragraph of text at the current position in the form.</summary>
    /// <param name="text">The paragraph text to display.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddParagraph(Func<string> text)
    {
        this.AssertLoaded();
        this.ModApi!.AddParagraph(this._consumerManifest, text);
        return this;
    }

    /// <summary>Add a checkbox to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">Get the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddCheckbox(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, bool> get,
        Action<TConfig, bool> set,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddBoolOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value));
        }

        return this;
    }

    /// <summary>Add a dropdown to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">Get the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="allowedValues">The values that can be selected.</param>
    /// <param name="formatAllowedValue">
    ///     Get the display text to show for a value from <paramref name="allowedValues"/>, or
    ///     <c>null</c> to show the values as-is.
    /// </param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddDropdown(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, string> get,
        Action<TConfig, string> set,
        string[] allowedValues,
        Func<string, string>? formatAllowedValue,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddTextOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value),
                allowedValues: allowedValues,
                formatAllowedValue: formatAllowedValue);
        }

        return this;
    }

    /// <summary>Add a checkbox to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddTextbox(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, string> get,
        Action<TConfig, string> set,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddTextOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value));
        }

        return this;
    }

    /// <summary>Add a numeric field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddNumberField(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, int> get,
        Action<TConfig, int> set,
        int min,
        int max,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddNumberOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value),
                min: min,
                max: max);
        }

        return this;
    }

    /// <summary>Add a numeric field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddNumberField(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, float> get,
        Action<TConfig, float> set,
        float min,
        float max,
        float interval = 0.1f,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddNumberOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value),
                min: min,
                max: max,
                interval: interval);
        }

        return this;
    }

    /// <summary>Add a key binding field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="enable">Whether the field is enabled.</param>
    /// <returns>The <see cref="GenericModConfigMenuIntegration{TConfig}"/> instance.</returns>
    public GenericModConfigMenuIntegration<TConfig> AddKeyBinding(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, KeybindList> get,
        Action<TConfig, KeybindList> set,
        bool enable = true)
    {
        this.AssertLoaded();
        if (enable)
        {
            this.ModApi!.AddKeybindList(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this._getConfig()),
                setValue: value => set(this._getConfig(), value));
        }

        return this;
    }
}
