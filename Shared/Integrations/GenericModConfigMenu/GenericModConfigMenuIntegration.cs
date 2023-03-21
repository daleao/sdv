namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using static System.Net.Mime.MediaTypeNames;

#endregion using directives

/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
/// <typeparam name="TGenericModConfigMenu">The type that is inheriting from this class.</typeparam>
/// <typeparam name="TConfig">The mod configuration type.</typeparam>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
[RequiresMod("spacechase0.GenericModConfigMenu", "GenericModConfigMenu", "1.6.0")]
internal abstract class GenericModConfigMenuIntegration<TGenericModConfigMenu, TConfig> :
    ModIntegration<TGenericModConfigMenu, IGenericModConfigMenuApi>
    where TGenericModConfigMenu : GenericModConfigMenuIntegration<TGenericModConfigMenu, TConfig>
    where TConfig : new()
{
    /// <summary>The manifest for the mod consuming the API.</summary>
    private readonly IManifest _consumerManifest;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegration{TGenericModConfigMenu, TConfig}"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="consumerManifest">The manifest for the mod consuming the API.</param>
    protected GenericModConfigMenuIntegration(
        IModRegistry modRegistry,
        IManifest consumerManifest)
        : base("spacechase0.GenericModConfigMenu", "GenericModConfigMenu", "1.6.0", modRegistry)
    {
        this._consumerManifest = consumerManifest;
        GenericModConfigMenuOptions.Instance?.Register();
    }

    /// <summary>Gets the API for registering complex options.</summary>
    private static IGenericModConfigMenuOptionsApi? ComplexOptions => GenericModConfigMenuOptions.Instance?.ModApi;

    /// <summary>Registers the mod config.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu Register(bool titleScreenOnly = false)
    {
        if (this.IsRegistered)
        {
            return (TGenericModConfigMenu)this;
        }

        this.AssertLoaded();
        this.ModApi.Register(this._consumerManifest, this.ResetConfig, this.SaveAndApply, titleScreenOnly);
        this.IsRegistered = true;
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Unregisters the mod config.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu Unregister()
    {
        if (!this.IsRegistered)
        {
            return (TGenericModConfigMenu)this;
        }

        this.ModApi.Unregister(this._consumerManifest);
        this.IsRegistered = false;
        return (TGenericModConfigMenu)this;
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
        this.ModApi.AddPage(this._consumerManifest, pageId, pageTitle);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="text">The link text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddPageLink(
        string pageId, Func<string> text, Func<string>? tooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddPageLink(this._consumerManifest, pageId, text, tooltip);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a section title at the current position in the form.</summary>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">
    ///     The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the
    ///     tooltip.
    /// </param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddSectionTitle(Func<string> text, Func<string>? tooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddSectionTitle(this._consumerManifest, text, tooltip);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a paragraph of text at the current position in the form.</summary>
    /// <param name="text">The paragraph text to display.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddParagraph(Func<string> text)
    {
        this.AssertRegistered();
        this.ModApi.AddParagraph(this._consumerManifest, text);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a checkbox to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">Get the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddCheckbox(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, bool> get,
        Action<TConfig, bool> set,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddBoolOption(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a dropdown to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">Get the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="allowedValues">The values that can be selected.</param>
    /// <param name="formatAllowedValue">
    ///     Get the display text to show for a value from <paramref name="allowedValues"/>, or
    ///     <c>null</c> to show the values as-is.
    /// </param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDropdown(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, string> get,
        Action<TConfig, string> set,
        string[] allowedValues,
        Func<string, string>? formatAllowedValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            allowedValues: allowedValues,
            formatAllowedValue: formatAllowedValue,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a checkbox to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddTextbox(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, string> get,
        Action<TConfig, string> set,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddNumberField(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, int> get,
        Action<TConfig, int> set,
        int min,
        int max,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            min: min,
            max: max,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddNumberField(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, float> get,
        Action<TConfig, float> set,
        float min,
        float max,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            min: min,
            max: max,
            interval: interval,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a key binding field to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddKeyBinding(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, KeybindList> get,
        Action<TConfig, KeybindList> set,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddKeybindList(
            this._consumerManifest,
            name: name,
            tooltip: tooltip,
            getValue: () => get(this.GetConfig()),
            setValue: value => set(this.GetConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a color picking option to the form.</summary>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="get">GetInstructions the current value from the mod config.</param>
    /// <param name="set">Set a new value in the mod config.</param>
    /// <param name="fallback">A fallback value in case the user's input is invalid.</param>
    /// <param name="showAlpha">If GMCM Options is installed, show the alpha picker or not.</param>
    /// <param name="colorPickerStyle">GMCM Option's picker style.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddColorPicker(
        Func<string> name,
        Func<string> tooltip,
        Func<TConfig, Color> get,
        Action<TConfig, Color> set,
        Color fallback,
        bool showAlpha = true,
        uint colorPickerStyle = 0,
        string? id = null)
    {
        this.AssertRegistered();
        if (ComplexOptions is not null)
        {
            ComplexOptions.AddColorOption(
                this._consumerManifest,
                getValue: () => get(this.GetConfig()),
                setValue: value => set(this.GetConfig(), value),
                name: name,
                tooltip: tooltip,
                showAlpha: showAlpha,
                colorPickerStyle: colorPickerStyle,
                fieldId: id);
        }
        else
        {
            this.ModApi.AddTextOption(
                this._consumerManifest,
                name: name,
                tooltip: tooltip,
                getValue: () => get(this.GetConfig()).ToHtml(),
                setValue: value => set(this.GetConfig(), value.TryGetColorFromHtml(out var color) ? color : fallback),
                fieldId: id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds some empty vertical space to the form.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddSeparator()
    {
        this.AssertRegistered();
        if (ComplexOptions is not null)
        {
            ComplexOptions.AddSimpleHorizontalSeparator(this._consumerManifest);
        }
        else
        {
            this.ModApi.AddParagraph(this._consumerManifest, () => "\n");
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Sets whether the options registered after this point can only be edited from the title screen.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
    protected void SetTitleScreenOnlyForNextOptions(bool titleScreenOnly)
    {
        this.AssertRegistered();
        this.ModApi.SetTitleScreenOnlyForNextOptions(this._consumerManifest, titleScreenOnly);
    }

    /// <summary>Registers an action to invoke when a field's value is changed.</summary>
    /// <param name="action">Whether the field is enabled.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu OnFieldChanged(Action<string, object> action)
    {
        this.AssertRegistered();
        this.ModApi.OnFieldChanged(
            this._consumerManifest,
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

    [RequiresMod("jltaylor-us.GMCMOptions", "GMCM Options")]
    internal sealed class GenericModConfigMenuOptions : ModIntegration<GenericModConfigMenuOptions, IGenericModConfigMenuOptionsApi>
    {
        private GenericModConfigMenuOptions()
            : base("jltaylor-us.GMCMOptions", "GMCM Options", "1.2.0", ModHelper.ModRegistry)
        {
        }
    }
}
