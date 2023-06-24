namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
/// <typeparam name="TGenericModConfigMenu">The type that is inheriting from this class.</typeparam>
/// <typeparam name="TConfig">The mod configuration type.</typeparam>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
[ModRequirement("spacechase0.GenericModConfigMenu", "GenericModConfigMenu", "1.6.0")]
internal abstract class GenericModConfigMenuIntegration<TGenericModConfigMenu, TConfig> :
    ModIntegration<TGenericModConfigMenu, IGenericModConfigMenuApi>
    where TGenericModConfigMenu : GenericModConfigMenuIntegration<TGenericModConfigMenu, TConfig>
    where TConfig : new()
{
    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegration{TGenericModConfigMenu, TConfig}"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
    protected GenericModConfigMenuIntegration(
        IModRegistry modRegistry,
        IManifest consumerManifest)
        : base("spacechase0.GenericModConfigMenu", "GenericModConfigMenu", "1.6.0", modRegistry)
    {
        this.ConsumerManifest = consumerManifest;
    }

    /// <summary>Gets the manifest for the mod consuming the API.</summary>
    internal IManifest ConsumerManifest { get; }

    /// <summary>Gets the API for registering complex options, if available.</summary>
    private static IGenericModConfigMenuOptionsApi? ComplexOptions =>
        GenericModConfigMenuOptionsIntegration.Instance?.ModApi;

    /// <summary>Registers the mod config.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    internal TGenericModConfigMenu Register()
    {
        if ((this as IModIntegration).Register())
        {
            this.BuildMenu();
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Resets the mod config menu.</summary>
    internal void Reload()
    {
        this.Unregister().Register();
    }

    /// <summary>Constructs the config menu.</summary>
    protected abstract void BuildMenu();

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();
        this.ModApi.Register(this.ConsumerManifest, this.ResetConfig, this.SaveAndApply);
        return true;
    }

    /// <summary>
    ///     Starts a new page in the mod's config UI, or switch to that page if it already exists. All options registered
    ///     after this will be part of that page.
    /// </summary>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    /// <remarks>
    ///     You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a
    ///     multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.
    /// </remarks>
    protected TGenericModConfigMenu AddPage(string pageId, Func<string>? pageTitle = null)
    {
        this.AssertRegistered();
        this.ModApi.AddPage(this.ConsumerManifest, pageId, pageTitle);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="getText">Gets the link text shown in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddPageLink(
        string pageId, Func<string> getText, Func<string>? getTooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddPageLink(this.ConsumerManifest, pageId, getText, getTooltip);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a section title at the current position in the form.</summary>
    /// <param name="getText">Gets the title text shown in the form.</param>
    /// <param name="getTooltip">
    ///     Gets the tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the
    ///     tooltip.
    /// </param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddSectionTitle(Func<string> getText, Func<string>? getTooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddSectionTitle(this.ConsumerManifest, getText, getTooltip);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a paragraph of text at the current position in the form.</summary>
    /// <param name="getText">Gts the paragraph text to display.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddParagraph(Func<string> getText)
    {
        this.AssertRegistered();
        this.ModApi.AddParagraph(this.ConsumerManifest, getText);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a checkbox to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddCheckbox(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, bool> getValue,
        Action<TConfig, bool> setValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddBoolOption(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a dropdown to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="allowedValues">The values that can be selected.</param>
    /// <param name="formatAllowedValue">
    ///     Get the display text to show for a value from <paramref name="allowedValues"/>, or
    ///     <c>null</c> to show the values as-is.
    /// </param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDropdown(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, string> getValue,
        Action<TConfig, string> setValue,
        string[] allowedValues,
        Func<string, string>? formatAllowedValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            allowedValues: allowedValues,
            formatAllowedValue: formatAllowedValue,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a checkbox to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddTextbox(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, string> getValue,
        Action<TConfig, string> setValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric field to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddNumberField(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, int> getValue,
        Action<TConfig, int> setValue,
        int min,
        int max,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            min: min,
            max: max,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric field to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddNumberField(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, float> getValue,
        Action<TConfig, float> setValue,
        float min,
        float max,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            min: min,
            max: max,
            interval: interval,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a key binding field to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddKeyBinding(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, KeybindList> getValue,
        Action<TConfig, KeybindList> setValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddKeybindList(
            this.ConsumerManifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(this.GetConfig()),
            setValue: value => setValue(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds some empty vertical space to the form.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddHorizontalRule()
    {
        this.AssertRegistered();
        if (ComplexOptions is not null)
        {
            ComplexOptions.AddSimpleHorizontalSeparator(this.ConsumerManifest);
        }
        else
        {
            this.ModApi.AddParagraph(this.ConsumerManifest, () => "\n");
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds some empty vertical space to the form.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddVerticalSpace()
    {
        this.AssertRegistered();
        this.ModApi.AddParagraph(this.ConsumerManifest, () => "\n");
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a multi-column list of checkbox options to the form.</summary>
    /// <typeparam name="TPage">The type of the object which represents the page.</typeparam>
    /// <param name="getOptionName">Gets the label text to show in the form.</param>
    /// <param name="pages">The page values.</param>
    /// <param name="getPageId">Gets the destination page ID.</param>
    /// <param name="getPageName">Gets the destination page name.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddMultiPageLinkOption<TPage>(
        Func<string> getOptionName,
        TPage[] pages,
        Func<TPage, string> getPageId,
        Func<TPage, string> getPageName,
        Func<float, int> getColumnsFromWidth)
    {
        this.AssertRegistered();
        this.ModApi.AddMultiPageLinkOption(
            mod: this.ConsumerManifest,
            getOptionName,
            pages,
            getPageId,
            getPageName,
            getColumnsFromWidth);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a multi-column list of checkbox options to the form.</summary>
    /// <typeparam name="TCheckbox">The type of the object which represents the page.</typeparam>
    /// <param name="getOptionName">Gets the label text to show in the form.</param>
    /// <param name="checkboxes">The checkbox values.</param>
    /// <param name="getCheckboxValue">Gets the checkbox value.</param>
    /// <param name="setCheckboxValue">Sets the checkbox value.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="getCheckboxLabel">Gets the display text to show for the checkbox.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddMultiCheckboxOption<TCheckbox>(
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<float, int> getColumnsFromWidth,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Action<TCheckbox, bool>? onValueUpdated = null)
        where TCheckbox : notnull
    {
        this.AssertRegistered();
        this.ModApi.AddMultiCheckboxOption(
            mod: this.ConsumerManifest,
            getOptionName,
            checkboxes,
            getCheckboxValue,
            setCheckboxValue,
            getColumnsFromWidth,
            getCheckboxLabel,
            onValueUpdated);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a color picking option to the form.</summary>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="fallback">A fallback value in case the user's input is invalid.</param>
    /// <param name="showAlpha">If GMCM Options is installed, show the alpha picker or not.</param>
    /// <param name="colorPickerStyle">GMCM Option's picker style.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddColorPicker(
        Func<string> getName,
        Func<string> getTooltip,
        Func<TConfig, Color> getValue,
        Action<TConfig, Color> setValue,
        Color fallback,
        bool showAlpha = true,
        uint colorPickerStyle = 0,
        string? id = null)
    {
        this.AssertRegistered();
        if (ComplexOptions is not null)
        {
            ComplexOptions.AddColorOption(
                this.ConsumerManifest,
                getValue: () => getValue(this.GetConfig()),
                setValue: value => setValue(this.GetConfig(), value),
                name: getName,
                tooltip: getTooltip,
                showAlpha: showAlpha,
                colorPickerStyle: colorPickerStyle,
                fieldId: id);
        }
        else
        {
            this.ModApi.AddTextOption(
                this.ConsumerManifest,
                name: getName,
                tooltip: getTooltip,
                getValue: () => getValue(this.GetConfig()).ToHtml(),
                setValue: value => setValue(this.GetConfig(), value.TryGetColorFromHtml(out var color) ? color : fallback),
                fieldId: id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Sets whether the options registered after this point can only be edited from the title screen.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
    protected TGenericModConfigMenu SetTitleScreenOnlyForNextOptions(bool titleScreenOnly)
    {
        this.AssertRegistered();
        this.ModApi.SetTitleScreenOnlyForNextOptions(this.ConsumerManifest, titleScreenOnly);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Registers an action to invoke when a field's value is changed.</summary>
    /// <param name="action">Whether the field is enabled.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu OnFieldChanged(Action<string, object> action)
    {
        this.AssertRegistered();
        this.ModApi.OnFieldChanged(
            this.ConsumerManifest,
            onChange: action);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Get the current config model.</summary>
    /// <returns>The <typeparamref name="TConfig"/> model.</returns>
    protected abstract TConfig GetConfig();

    /// <summary>Reset the config model to the default values.</summary>
    protected abstract void ResetConfig();

    /// <summary>Save and apply the current config model.</summary>
    protected abstract void SaveAndApply();

    /// <summary>Unregisters the mod config.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    private TGenericModConfigMenu Unregister()
    {
        if (!this.IsRegistered)
        {
            return (TGenericModConfigMenu)this;
        }

        this.ModApi.Unregister(this.ConsumerManifest);
        this.IsRegistered = false;
        Log.T("[GMCM]: The config menu has been unregistered.");
        return (TGenericModConfigMenu)this;
    }
}
