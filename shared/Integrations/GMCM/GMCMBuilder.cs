namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Functional;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Handles the logic for integrating with the Generic Mod Configuration Menu mod.</summary>
/// <typeparam name="TGenericModConfigMenu">The type that is inheriting from this class.</typeparam>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
[ModRequirement("spacechase0.GenericModConfigMenu", "GenericModConfigMenu", "1.6.0")]
internal abstract class GMCMBuilder<TGenericModConfigMenu> :
    ModIntegration<TGenericModConfigMenu, IGenericModConfigMenuApi>
    where TGenericModConfigMenu : GMCMBuilder<TGenericModConfigMenu>
{
    private readonly Queue<(string Meta, Action Add)> _formFields = new();

    /// <summary>Initializes a new instance of the <see cref="GMCMBuilder{TGenericModConfigMenu}"/> class.</summary>
    /// <param name="translation">The <see cref="ITranslationHelper"/> instance.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The manifest of the mod consuming the API.</param>
    protected GMCMBuilder(
        ITranslationHelper translation,
        IModRegistry modRegistry,
        IManifest manifest)
        : base(modRegistry)
    {
        Guard.IsNotNull(manifest);
        this.Manifest = manifest;
        this._I18n = translation;
    }

    /// <summary>Gets the manifest for the mod consuming the API.</summary>
    internal IManifest Manifest { get; }

    /// <summary>Gets the manifest for the mod consuming the API.</summary>
    // ReSharper disable once InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Distinguish from static Pathoschild.TranslationBuilder")]
    internal ITranslationHelper _I18n { get; }

    /// <summary>Gets the API for registering complex options, if available.</summary>
    private static IGenericModConfigMenuOptionsApi? ComplexOptionsApi =>
        GMCMOptionsIntegration.Instance?.ModApi;

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

    /// <summary>Constructs the config menu manually.</summary>
    protected abstract void BuildMenu();

    /// <summary>Constructs the config menu automatically via reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void BuildImplicitly<TConfig>(Func<TConfig> getConfig)
        where TConfig : class
    {
        if (this._formFields.Count > 0)
        {
            // build from queued
            foreach (var field in this._formFields)
            {
                field.Add();
            }

            return;
        }

        var configQueue = new Queue<Func<object>>();
        configQueue.Enqueue(getConfig);
        var pages = new Dictionary<Type, GMCMPage?> { { typeof(TConfig), null } };
        while (configQueue.TryDequeue(out var nextConfig))
        {
            var config = nextConfig();
            var configType = config.GetType();
            Log.D($"Building menu for {configType.Name}.");

            GMCMPage? currentPage = null;
            if (pages.TryGetValue(configType, out var page) && page is not null)
            {
                this._formFields.Enqueue(($"NewPage:{page.PageId}", () => this.AddPage(page.PageId, () => this._I18n.Get($"gmcm.pages.{page.PageTitleKey}"))));
                if (page.LinkToParentPage && pages.TryGetValue(page.ParentConfigType, out var parentPage) &&
                    parentPage is not null)
                {
                    this._formFields.Enqueue(($"PageLink:{parentPage.PageId}", () => this.AddPageLink(
                            parentPage.PageId,
                            () => this._I18n.Get($"gmcm.back_to.{parentPage.PageTitleKey}"))));
                    this._formFields.Enqueue(("VSpace", () => this.AddVerticalSpace()));
                }

                currentPage = page;
            }

            // crawl config properties and sort by priority
            var properties = configType.GetProperties();
            var prioritized = new SortedDictionary<uint, PropertyInfo>();
            var unprioritized = new List<PropertyInfo>();
            var links = new List<GMCMPage>();
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.GetCustomAttribute<GMCMIgnoreAttribute>() is not null)
                {
                    continue;
                }

                if (property.GetCustomAttribute<GMCMInnerConfigAttribute>() is { } innerConfigAttribute &&
                    property.GetGetMethod() is { } getterInfo)
                {
                    configQueue.Enqueue(() => getterInfo.Invoke(config, null)!);
                    if (!string.IsNullOrEmpty(innerConfigAttribute.PageId))
                    {
                        var nextPage = new GMCMPage(
                            innerConfigAttribute.PageId,
                            innerConfigAttribute.PageTitleKey,
                            configType,
                            innerConfigAttribute.LinkToParentPage);
                        pages[property.PropertyType] = nextPage;
                        if (innerConfigAttribute.LinkToParentPage)
                        {
                            links.Add(nextPage);
                        }
                    }

                    continue;
                }

                if (property.GetCustomAttribute<GMCMPriorityAttribute>() is not { } priorityAttribute)
                {
                    unprioritized.Add(property);
                    continue;
                }

                if (prioritized.TryAdd(priorityAttribute.Priority, property))
                {
                    continue;
                }

                var sb = new StringBuilder($"Duplicate priority value {priorityAttribute.Priority}");
                if (currentPage is not null)
                {
                    sb.Append($" in page \"{currentPage.PageId}\"");
                }

                sb.Append(". Subsequent properties beyond the first will be unprioritized.");
                Log.W(sb.ToString());
                unprioritized.Add(property);
            }

            // add page links
            if (links.Count > 0)
            {
                this._formFields.Enqueue(("MultiPageLink", () => this.AddMultiPageLinkOption(
                    () => string.Empty,
                    links.ToArray(),
                    link => link.PageId,
                    link => this._I18n.Get($"gmcm.pages.{link.PageTitleKey}"))));
                this._formFields.Enqueue(("VSpace", () => this.AddVerticalSpace()));
            }

            if (prioritized.Count == 0)
            {
                continue;
            }

            // add sectioned form fields
            var currentSection = string.Empty;
            var others = new List<PropertyInfo>();
            foreach (var property in prioritized.Values)
            {
                if (property.GetCustomAttribute<GMCMSectionAttribute>() is { } sectionAttribute &&
                    (string.IsNullOrEmpty(currentSection) || sectionAttribute.SectionTitleKey != currentSection))
                {
                    for (var i = 0; i < unprioritized.Count; i++)
                    {
                        var otherProperty = unprioritized[i];
                        if (otherProperty.GetCustomAttribute<GMCMSectionAttribute>() is not
                                { } otherSectionAttribute ||
                            otherSectionAttribute.SectionTitleKey != currentSection)
                        {
                            continue;
                        }

                        this._formFields.Enqueue(($"FormField:{otherProperty.Name}", () => this.AddFromPropertyInfo(otherProperty, nextConfig)));
                        unprioritized.Remove(otherProperty);
                    }

                    if (sectionAttribute.SectionTitleKey == "other")
                    {
                        others.Add(property);
                        break;
                    }

                    if (!string.IsNullOrEmpty(currentSection))
                    {
                        this._formFields.Enqueue(("HRule", () => this.AddHorizontalRule()));
                    }

                    this._formFields.Enqueue(($"Section:{sectionAttribute.SectionTitleKey}", () => this.AddSectionTitle(() => this._I18n.Get($"gmcm.sections.{sectionAttribute.SectionTitleKey}"))));
                    currentSection = sectionAttribute.SectionTitleKey;
                }

                this._formFields.Enqueue(($"FormField:{property.Name}", () => this.AddFromPropertyInfo(property, nextConfig)));
            }

            if (unprioritized.Count == 0)
            {
                continue;
            }

            // add unprioritized and other form fields
            var grouped = unprioritized.GroupBy(pi =>
                pi.GetCustomAttribute<GMCMSectionAttribute>() is { } sectionAttribute
                    ? sectionAttribute.SectionTitleKey
                    : "other");
            foreach (var group in grouped)
            {
                if (group.Key == "other")
                {
                    others.AddRange(group);
                    continue;
                }

                this._formFields.Enqueue(($"Section:{group.Key}", () => this.AddSectionTitle(() => this._I18n.Get($"gmcm.sections.{group.Key}"))));
                currentSection = group.Key;
                foreach (var property in group)
                {
                    this._formFields.Enqueue(($"FormField:{property.Name}", () => this.AddFromPropertyInfo(property, nextConfig)));
                    unprioritized.Remove(property);
                }
            }

            if (others.Count == 0)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(currentSection))
            {
                this._formFields.Enqueue(("HRule", () => this.AddHorizontalRule()));
                this._formFields.Enqueue(("Section:other", () => this.AddSectionTitle(() => this._I18n.Get("gmcm.sections.other"))));
            }

            for (var i = 0; i < others.Count; i++)
            {
                var otherProperty = others[i];
                this._formFields.Enqueue(($"FormField:{otherProperty.Name}", () => this.AddFromPropertyInfo(otherProperty, nextConfig)));
            }
        }

        // invoke queued actions
        foreach (var field in this._formFields)
        {
            field.Add();
        }
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();
        this.ModApi.Register(this.Manifest, this.ResetConfig, this.SaveAndApply);
        return true;
    }

    #region organization

    /// <summary>
    ///     Starts a new page in the mod's config UI, or switch to that page if it already exists. All options registered
    ///     after this will be part of that page.
    /// </summary>
    /// <param name="pageId">The unique page ID.</param>
    /// <param name="pageTitle">The page title shown in its UI, or <see langword="null"/> to show the <paramref name="pageId"/> value.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    /// <remarks>
    ///     You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a
    ///     multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.
    /// </remarks>
    protected TGenericModConfigMenu AddPage(string pageId, Func<string>? pageTitle = null)
    {
        this.AssertRegistered();
        this.ModApi.AddPage(this.Manifest, pageId, pageTitle);
        Log.D($"Added GMCM page: {pageId}");
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
    /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
    /// <param name="getText">Gets the link text shown in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the link, or <see langword="null"/> to disable the tooltip.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddPageLink(
        string pageId, Func<string> getText, Func<string>? getTooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddPageLink(this.Manifest, pageId, getText, getTooltip);
        Log.D($"Added GMCM page link to {pageId}");
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a section title at the current position in the form.</summary>
    /// <param name="getText">Gets the title text shown in the form.</param>
    /// <param name="getTooltip">
    ///     Gets the tooltip text shown when the cursor hovers on the title, or <see langword="null"/> to disable the
    ///     tooltip.
    /// </param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddSectionTitle(Func<string> getText, Func<string>? getTooltip = null)
    {
        this.AssertRegistered();
        this.ModApi.AddSectionTitle(this.Manifest, getText, getTooltip);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a paragraph of text at the current position in the form.</summary>
    /// <param name="getText">Gts the paragraph text to display.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddParagraph(Func<string> getText)
    {
        this.AssertRegistered();
        this.ModApi.AddParagraph(this.Manifest, getText);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds some empty vertical space to the form.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddHorizontalRule()
    {
        this.AssertRegistered();
        if (ComplexOptionsApi is not null)
        {
            ComplexOptionsApi.AddSimpleHorizontalSeparator(this.Manifest);
        }
        else
        {
            this.ModApi.AddParagraph(this.Manifest, () => "\n");
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds some empty vertical space to the form.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddVerticalSpace()
    {
        this.AssertRegistered();
        this.ModApi.AddParagraph(this.Manifest, () => "\n");
        return (TGenericModConfigMenu)this;
    }

    #endregion organization

    #region bool fields

    /// <summary>Adds a checkbox to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddCheckbox<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, bool> getValue,
        Action<TConfig, bool> setValue,
        Func<TConfig> getConfig,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddBoolOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a checkbox to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddCheckbox<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(bool))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, bool>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, bool>>();
        this.AddCheckbox(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            property.Name);
    }

    #endregion bool fields

    #region text fields

    /// <summary>Adds a free-form text input box to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddTextBox<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, string> getValue,
        Action<TConfig, string> setValue,
        Func<TConfig> getConfig,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a free-form text input box to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddTextBox<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(string))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, string>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, string>>();
        this.AddTextBox(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            property.Name);
    }

    /// <summary>Adds a coordinate input box to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="default">The default value.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddCoordinateBox<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, Vector2> getValue,
        Action<TConfig, Vector2> setValue,
        Func<TConfig> getConfig,
        Vector2? @default = null,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () =>
            {
                var v = getValue(getConfig());
                return $"{v.X}, {v.Y}";
            },
            setValue: value => setValue(
                getConfig(),
                value.TryParseVector2(out var parsed) ? parsed.Value : @default ?? default),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a coordinate input box to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddCoordinateBox<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(Vector2))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Vector2>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, Vector2>>();

        Vector2 @default = default;
        if (property.GetCustomAttribute<GMCMDefaultVector2Attribute>() is { } defaultAttribute)
        {
            @default = new Vector2(defaultAttribute.X, defaultAttribute.Y);
        }

        this.AddCoordinateBox(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            @default,
            property.Name);
    }

    /// <summary>Adds a color input box to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="default">The default value.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddColorBox<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, Color> getValue,
        Action<TConfig, Color> setValue,
        Func<TConfig> getConfig,
        Color? @default = null,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () =>
            {
                var c = getValue(getConfig());
                return $"{c.R}, {c.G}, {c.B}, {c.A}";
            },
            setValue: value => setValue(
                getConfig(),
                value.TryParseColor(out var parsed) ? parsed : @default.GetValueOrDefault()),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a color input box to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddColorBox<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(Color))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Color>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, Color>>();

        Color @default = default;
        if (property.GetCustomAttribute<GMCMDefaultColorAttribute>() is { } defaultAttribute)
        {
            @default = new Color(defaultAttribute.R, defaultAttribute.G, defaultAttribute.B, defaultAttribute.A);
        }

        this.AddColorBox(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            @default,
            property.Name);
    }

    /// <summary>Adds a dropdown to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="allowedValues">The values that can be selected.</param>
    /// <param name="formatAllowedValue">
    ///     Get the display text to show for a value from <paramref name="allowedValues"/>, or
    ///     <see langword="null"/> to show the values as-is.
    /// </param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDropdown<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, string> getValue,
        Action<TConfig, string> setValue,
        Func<TConfig> getConfig,
        string[] allowedValues,
        Func<string, string>? formatAllowedValue,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            allowedValues: allowedValues,
            formatAllowedValue: formatAllowedValue,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a dropdown to the form based on <typeparamref name="TEnum"/>.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <typeparam name="TEnum">A <see cref="Enum"/> type.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDropdown<TConfig, TEnum>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, TEnum> getValue,
        Action<TConfig, TEnum> setValue,
        Func<TConfig> getConfig,
        string? id = null)
        where TEnum : struct, Enum
    {
        this.AssertRegistered();
        this.ModApi.AddTextOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()).ToString(),
            setValue: value => setValue(getConfig(), Enum.Parse<TEnum>(value)),
            allowedValues: Enum.GetNames<TEnum>(),
            formatAllowedValue: value => this._I18n.Get($"gmcm.{typeof(TEnum).Name.CamelToSnakeCase()}.{value.CamelToSnakeCase()}"),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a dropdown to the form based on <typeparamref name="TEnum"/> using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <typeparam name="TEnum">A <see cref="Enum"/> type.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddDropdown<TConfig, TEnum>(PropertyInfo property, Func<TConfig> getConfig)
        where TEnum : struct, Enum
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(TEnum))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, TEnum>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, TEnum>>();
        this.AddDropdown(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            property.Name);
    }

    /// <summary>Adds a dropdown to the form based on an enum property using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddDropdown<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        var enumType = property.PropertyType;
        var addDropdownMethod = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(mi => mi.Name == nameof(this.AddDropdown) && mi.GetGenericArguments().Length == 2 && mi.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TConfig), enumType);

        addDropdownMethod.Invoke(this, new object?[] { property, getConfig });
    }

    #endregion text fields

    #region numeric fields

    /// <summary>Adds a numeric integer field to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddIntSlider<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, int> getValue,
        Action<TConfig, int> setValue,
        Func<TConfig> getConfig,
        int? min = null,
        int? max = null,
        int? interval = null,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            min: min,
            max: max,
            interval: interval,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric integer field to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddIntSlider<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            (property.PropertyType != typeof(int) && property.PropertyType != typeof(uint)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, int>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, int>>();
        int? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = (int)rangeAttribute.Min;
            max = (int)rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        int? interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? (int)intervalAttribute.Interval
            : null;

        this.AddIntSlider(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    /// <summary>Adds a group of integer fields based on an array to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="length">The length of the array.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddIntSliderGroupFromArray<TConfig>(
        int length,
        Func<int, string> getName,
        Func<int, string>? getTooltip,
        Func<TConfig, int, int> getValue,
        Action<TConfig, int, int> setValue,
        Func<TConfig> getConfig,
        int? min = null,
        int? max = null,
        int? interval = null,
        string? id = null)
    {
        this.AssertRegistered();
        for (var i = 0; i < length; i++)
        {
            this.AddIntSlider(
                getName.Partial(arg1: i),
                getTooltip?.Partial(arg1: i),
                getValue.Partial(arg2: i),
                setValue.Partial(arg2: i),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of integer fields based on an array to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddIntSliderGroupFromArray<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(int[]) && property.PropertyType != typeof(uint[])))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var arrayGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, int[]>>();
        var length = arrayGetterDelegate(getConfig()).Length;

        int? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = (int)rangeAttribute.Min;
            max = (int)rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        int? interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? (int)intervalAttribute.Interval
            : null;

        this.AddIntSliderGroupFromArray(
            length,
            i => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{i}.title"),
            i => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{i}.desc"),
            (config, i) => arrayGetterDelegate(config)[i],
            (config, i, value) => arrayGetterDelegate(config)[i] = value,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    /// <summary>Adds a group of integer fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="int"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="keys">A <see cref="IEnumerable{T}"/> of <see cref="int"/> keys with which to retrieve values from the dictionary.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddIntSliderGroupFromIntDict<TConfig>(
        IEnumerable<int> keys,
        Func<int, string> getName,
        Func<int, string>? getTooltip,
        Func<TConfig, int, int> getValue,
        Action<TConfig, int, int> setValue,
        Func<TConfig> getConfig,
        int? min = null,
        int? max = null,
        int? interval = null,
        string? id = null)
    {
        this.AssertRegistered();
        foreach (var key in keys)
        {
            this.AddIntSlider(
                getName.Partial(arg1: key),
                getTooltip?.Partial(arg1: key),
                getValue.Partial(arg2: key),
                setValue.Partial(arg2: key),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of integer fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="int"/> keys to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddIntSliderGroupFromIntDict<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(Dictionary<int, int>) &&
             property.PropertyType != typeof(Dictionary<int, uint>)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var dictGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Dictionary<int, int>>>();
        var keys = dictGetterDelegate(getConfig()).Keys;

        int? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = (int)rangeAttribute.Min;
            max = (int)rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        int? interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? (int)intervalAttribute.Interval
            : null;

        this.AddIntSliderGroupFromIntDict(
            keys,
            key => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{key}.title"),
            key => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{key}.desc"),
            (config, key) => dictGetterDelegate(config)[key],
            (config, key, value) => dictGetterDelegate(config)[key] = value,
            getConfig,
            min: min,
            max: max,
            interval: interval,
            property.Name);
    }

    /// <summary>Adds a group of integer fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="string"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="keys">A <see cref="IEnumerable{T}"/> of <see cref="string"/> keys with which to retrieve values from the dictionary.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddIntSliderGroupFromStringDict<TConfig>(
        IEnumerable<string> keys,
        Func<string, string> getName,
        Func<string, string>? getTooltip,
        Func<TConfig, string, int> getValue,
        Action<TConfig, string, int> setValue,
        Func<TConfig> getConfig,
        int? min = null,
        int? max = null,
        int? interval = null,
        string? id = null)
    {
        this.AssertRegistered();
        foreach (var key in keys)
        {
            this.AddIntSlider(
                getName.Partial(arg1: key),
                getTooltip?.Partial(arg1: key),
                getValue.Partial(arg2: key),
                setValue.Partial(arg2: key),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of integer fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="string"/> keys to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddIntSliderGroupFromStringDict<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(Dictionary<string, int>) &&
             property.PropertyType != typeof(Dictionary<string, uint>)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var dictGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Dictionary<string, int>>>();
        var keys = dictGetterDelegate(getConfig()).Keys;

        int? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = (int)rangeAttribute.Min;
            max = (int)rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        int? interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? (int)intervalAttribute.Interval
            : null;

        this.AddIntSliderGroupFromStringDict(
            keys,
            key => this._I18n.TryGet(
                $"gmcm.{property.Name.CamelToSnakeCase()}.{key.CamelToSnakeCase()}.title",
                out var translation)
                ? translation
                : key,
            key => this._I18n.TryGet(
                $"gmcm.{property.Name.CamelToSnakeCase()}.{key.CamelToSnakeCase()}.desc",
                out var translation)
                ? translation
                : string.Empty,
            (config, key) => dictGetterDelegate(config)[key],
            (config, key, value) => dictGetterDelegate(config)[key] = value,
            getConfig,
            min: min,
            max: max,
            interval: interval,
            property.Name);
    }

    /// <summary>Adds a numeric field to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddFloatSlider<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, float> getValue,
        Action<TConfig, float> setValue,
        Func<TConfig> getConfig,
        float? min = null,
        float? max = null,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddNumberOption(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            min: min,
            max: max,
            interval: interval,
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a numeric float field to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddFloatSlider<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            (property.PropertyType != typeof(float) && property.PropertyType != typeof(double)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, float>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, float>>();
        float? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = rangeAttribute.Min;
            max = rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        var interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? intervalAttribute.Interval
            : 0.1f;

        this.AddFloatSlider(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    /// <summary>Adds a group of float fields based on an array to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="length">The length of the array.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddFloatSliderGroupFromArray<TConfig>(
        int length,
        Func<int, string> getName,
        Func<int, string>? getTooltip,
        Func<TConfig, int, float> getValue,
        Action<TConfig, int, float> setValue,
        Func<TConfig> getConfig,
        float? min = null,
        float? max = null,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        for (var i = 0; i < length; i++)
        {
            this.AddFloatSlider(
                getName.Partial(arg1: i),
                getTooltip?.Partial(arg1: i),
                getValue.Partial(arg2: i),
                setValue.Partial(arg2: i),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of float fields based on an array to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddFloatSliderGroupFromArray<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(float[]) && property.PropertyType != typeof(double[])))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var arrayGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, float[]>>();
        var length = arrayGetterDelegate(getConfig()).Length;

        float? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = rangeAttribute.Min;
            max = rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        var interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? intervalAttribute.Interval
            : 0.1f;

        this.AddFloatSliderGroupFromArray(
            length,
            i => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{i}.title"),
            i => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{i}.desc"),
            (config, i) => arrayGetterDelegate(config)[i],
            (config, i, value) => arrayGetterDelegate(config)[i] = value,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    /// <summary>Adds a group of float fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="int"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="keys">A <see cref="IEnumerable{T}"/> of <see cref="string"/> keys with which to retrieve values from the dictionary.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddFloatSliderGroupFromIntDict<TConfig>(
        IEnumerable<int> keys,
        Func<int, string> getName,
        Func<int, string>? getTooltip,
        Func<TConfig, int, float> getValue,
        Action<TConfig, int, float> setValue,
        Func<TConfig> getConfig,
        float? min = null,
        float? max = null,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        foreach (var key in keys)
        {
            this.AddFloatSlider(
                getName.Partial(arg1: key),
                getTooltip?.Partial(arg1: key),
                getValue.Partial(arg2: key),
                setValue.Partial(arg2: key),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of float fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="int"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddFloatSliderGroupFromIntDict<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(Dictionary<int, float>) &&
             property.PropertyType != typeof(Dictionary<int, double>)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var arrayGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Dictionary<int, float>>>();
        var keys = arrayGetterDelegate(getConfig()).Keys;

        float? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = rangeAttribute.Min;
            max = rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        var interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? intervalAttribute.Interval
            : 0.1f;

        this.AddFloatSliderGroupFromIntDict(
            keys,
            key => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{key}.title"),
            key => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.{key}.desc"),
            (config, key) => arrayGetterDelegate(config)[key],
            (config, key, value) => arrayGetterDelegate(config)[key] = value,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    /// <summary>Adds a group of float fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="string"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="keys">A <see cref="IEnumerable{T}"/> of <see cref="string"/> keys with which to retrieve values from the dictionary.</param>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddFloatSliderGroupFromStringDict<TConfig>(
        IEnumerable<string> keys,
        Func<string, string> getName,
        Func<string, string>? getTooltip,
        Func<TConfig, string, float> getValue,
        Action<TConfig, string, float> setValue,
        Func<TConfig> getConfig,
        float? min = null,
        float? max = null,
        float interval = 0.1f,
        string? id = null)
    {
        this.AssertRegistered();
        foreach (var key in keys)
        {
            this.AddFloatSlider(
                getName.Partial(arg1: key),
                getTooltip?.Partial(arg1: key),
                getValue.Partial(arg2: key),
                setValue.Partial(arg2: key),
                getConfig,
                min: min,
                max: max,
                interval: interval,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a group of float fields based on a <see cref="Dictionary{TKey,TValue}"/> of <see cref="string"/> keys to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddFloatSliderGroupFromStringDict<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo ||
            (property.PropertyType != typeof(Dictionary<string, float>) &&
             property.PropertyType != typeof(Dictionary<string, double>)))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var arrayGetterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Dictionary<string, float>>>();
        var keys = arrayGetterDelegate(getConfig()).Keys;

        float? min, max;
        if (property.GetCustomAttribute<GMCMRangeAttribute>() is { } rangeAttribute)
        {
            min = rangeAttribute.Min;
            max = rangeAttribute.Max;
        }
        else
        {
            min = null;
            max = null;
        }

        var interval = property.GetCustomAttribute<GMCMIntervalAttribute>() is { } intervalAttribute
            ? intervalAttribute.Interval
            : 0.1f;

        this.AddFloatSliderGroupFromStringDict(
            keys,
            key => this._I18n.TryGet(
                $"gmcm.{property.Name.CamelToSnakeCase()}.{key.CamelToSnakeCase()}.title",
                out var translation)
                ? translation
                : key,
            key => this._I18n.TryGet(
                $"gmcm.{property.Name.CamelToSnakeCase()}.{key.CamelToSnakeCase()}.desc",
                out var translation)
                ? translation
                : string.Empty,
            (config, key) => arrayGetterDelegate(config)[key],
            (config, key, value) => arrayGetterDelegate(config)[key] = value,
            getConfig,
            min,
            max,
            interval,
            property.Name);
    }

    #endregion numeric fields

    #region keybinds

    /// <summary>Adds a key binding field to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddKeyBinding<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, KeybindList> getValue,
        Action<TConfig, KeybindList> setValue,
        Func<TConfig> getConfig,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddKeybindList(
            this.Manifest,
            name: getName,
            tooltip: getTooltip,
            getValue: () => getValue(getConfig()),
            setValue: value => setValue(getConfig(), value),
            fieldId: id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a key binding field to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddKeyBinding<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(KeybindList))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, KeybindList>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, KeybindList>>();
        this.AddKeyBinding(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            property.Name);
    }

    #endregion keybinds

    #region complex options

    /// <summary>Adds a color picking option to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="getName">Gets the label text to show in the form.</param>
    /// <param name="getTooltip">Gets the tooltip text shown when the cursor hovers on the field.</param>
    /// <param name="getValue">Gets the current value from the mod config.</param>
    /// <param name="setValue">Sets a new value in the mod config.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    /// <param name="default">A fallback value in case the user's input is invalid.</param>
    /// <param name="showAlpha">If GMCM Options is installed, show the alpha picker or not.</param>
    /// <param name="colorPickerStyle">GMCM Option's picker style.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddColorPicker<TConfig>(
        Func<string> getName,
        Func<string>? getTooltip,
        Func<TConfig, Color> getValue,
        Action<TConfig, Color> setValue,
        Func<TConfig> getConfig,
        Color? @default = null,
        bool showAlpha = true,
        uint colorPickerStyle = 0,
        string? id = null)
    {
        this.AssertRegistered();
        if (ComplexOptionsApi is not null)
        {
            ComplexOptionsApi.AddColorOption(
                this.Manifest,
                getValue: () => getValue(getConfig()),
                setValue: value => setValue(getConfig(), value),
                name: getName,
                tooltip: getTooltip,
                showAlpha: showAlpha,
                colorPickerStyle: colorPickerStyle,
                fieldId: id);
        }
        else
        {
            this.AddColorBox(
                getName,
                getTooltip,
                getValue,
                setValue,
                getConfig,
                @default,
                id);
        }

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a color picking option to the form using reflection.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> associated with a config property.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    protected void AddColorPicker<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        this.AssertRegistered();

        if (property.GetGetMethod() is not { } getterInfo || property.GetSetMethod(true) is not { } setterInfo ||
            property.PropertyType != typeof(Color))
        {
            Log.E($"{property.Name} is misconfigured and could not be added to the Generic Mod Config Menu.");
            return;
        }

        var getterDelegate = getterInfo.CompileUnboundDelegate<Func<TConfig, Color>>();
        var setterDelegate = setterInfo.CompileUnboundDelegate<Action<TConfig, Color>>();
        Color @default = default;
        if (property.GetCustomAttribute<GMCMDefaultColorAttribute>() is { } defaultAttribute)
        {
            @default = new Color(defaultAttribute.R, defaultAttribute.G, defaultAttribute.B, defaultAttribute.A);
        }

        var showAlpha = true;
        uint colorPickerStyle = 0;
        if (property.GetCustomAttribute<GMCMColorPickerAttribute>() is { } colorPickerAttribute)
        {
            showAlpha = colorPickerAttribute.ShowAlpha;
            colorPickerStyle = colorPickerAttribute.ColorPickerStyle;
        }

        this.AddColorPicker(
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.title"),
            () => this._I18n.Get($"gmcm.{property.Name.CamelToSnakeCase()}.desc"),
            getterDelegate,
            setterDelegate,
            getConfig,
            @default,
            showAlpha,
            colorPickerStyle,
            property.Name);
    }

    /// <summary>Adds a multi-column list of checkbox options to the form.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getValues">A delegate for getting the target list as a list of string items.</param>
    /// <param name="setValues">A delegate for setting the target list given a list of string items.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDynamicListOption(
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<string>> getValues,
        Action<IList<string>> setValues,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddDynamicListOption(
            mod: this.Manifest,
            getOptionName,
            getOptionTooltip,
            getValues,
            setValues,
            getColumnsFromWidth,
            id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a multi-column list of checkbox options to the form.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getPairs">A delegate for getting the target list of pairs as a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="setPairs">A delegate for setting the target list of pairs given a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="getTextBoxLabel">Gets the text box label.</param>
    /// <param name="getTextBoxTooltip">Gets the text box tooltip.</param>
    /// <param name="enumerateLabels">Whether to enumerate the labels.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddDynamicKeyValuePairListOption(
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<KeyValuePair<string, string>>> getPairs,
        Action<IList<KeyValuePair<string, string>>> setPairs,
        Func<int, string>? getTextBoxLabel = null,
        Func<int, string>? getTextBoxTooltip = null,
        bool enumerateLabels = false,
        string? id = null)
    {
        this.AssertRegistered();
        this.ModApi.AddDynamicKeyValuePairListOption(
            mod: this.Manifest,
            getOptionName,
            getOptionTooltip,
            getPairs,
            setPairs,
            getTextBoxLabel,
            getTextBoxTooltip,
            enumerateLabels,
            id);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Adds a multi-column list of checkbox options to the form.</summary>
    /// <typeparam name="TCheckbox">The type of the object which represents the page.</typeparam>
    /// <param name="getOptionName">Gets the label text to show in the form.</param>
    /// <param name="checkboxes">The checkbox values.</param>
    /// <param name="getCheckboxValue">Gets the checkbox value.</param>
    /// <param name="setCheckboxValue">Sets the checkbox value.</param>
    /// <param name="getCheckboxLabel">Gets the display text to show for the checkbox.</param>
    /// <param name="getCheckboxTooltip">Gets the tooltip text to show for the checkbox.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu AddMultiCheckboxOption<TCheckbox>(
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Func<TCheckbox, string>? getCheckboxTooltip = null,
        Action<TCheckbox, bool>? onValueUpdated = null,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
        where TCheckbox : notnull
    {
        this.AssertRegistered();
        this.ModApi.AddMultiCheckboxOption(
            mod: this.Manifest,
            getOptionName,
            checkboxes,
            getCheckboxValue,
            setCheckboxValue,
            getCheckboxLabel,
            getCheckboxTooltip,
            onValueUpdated,
            getColumnsFromWidth,
            id);

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
        Func<float, int>? getColumnsFromWidth = null)
    {
        this.AssertRegistered();
        this.ModApi.AddMultiPageLinkOption(
            mod: this.Manifest,
            getOptionName,
            pages,
            getPageId,
            getPageName,
            getColumnsFromWidth);

        return (TGenericModConfigMenu)this;
    }

    #endregion complex options

    /// <summary>Sets whether the options registered after this point can only be edited from the title screen.</summary>
    /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
    protected TGenericModConfigMenu SetTitleScreenOnlyForNextOptions(bool titleScreenOnly)
    {
        this.AssertRegistered();
        this.ModApi.SetTitleScreenOnlyForNextOptions(this.Manifest, titleScreenOnly);
        return (TGenericModConfigMenu)this;
    }

    /// <summary>Registers an action to invoke when a field's value is changed.</summary>
    /// <param name="action">Whether the field is enabled.</param>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    protected TGenericModConfigMenu OnFieldChanged(Action<string, object> action)
    {
        this.AssertRegistered();
        this.ModApi.OnFieldChanged(
            this.Manifest,
            onChange: action);

        return (TGenericModConfigMenu)this;
    }

    /// <summary>Resets the config model to the default values.</summary>
    protected abstract void ResetConfig();

    /// <summary>Saves and applies the current config model.</summary>
    protected abstract void SaveAndApply();

    /// <summary>Adds a single reflected <paramref name="property"/> to the form.</summary>
    /// <typeparam name="TConfig">The type of the config instance.</typeparam>
    /// <param name="property">The reflected <see cref="PropertyInfo"/>.</param>
    /// <param name="getConfig">Gets the config instance.</param>
    private void AddFromPropertyInfo<TConfig>(PropertyInfo property, Func<TConfig> getConfig)
    {
        var titleScreenOnly = property.GetCustomAttribute<GMCMTitleScreenOnlyAttribute>() is not null;
        if (titleScreenOnly)
        {
            this.SetTitleScreenOnlyForNextOptions(true);
        }

        if (property.GetCustomAttribute<GMCMOverrideAttribute>() is { } overrideAttribute)
        {
            overrideAttribute.Override();
            if (titleScreenOnly)
            {
                this.SetTitleScreenOnlyForNextOptions(false);
            }

            return;
        }

        Action<PropertyInfo, Func<TConfig>>? action = null;
        var propertyType = property.PropertyType;
        if (propertyType == typeof(bool))
        {
            action = this.AddCheckbox;
        }
        else if (propertyType == typeof(string))
        {
            action = this.AddTextBox;
        }
        else if (propertyType == typeof(Vector2))
        {
            action = this.AddCoordinateBox;
        }
        else if (propertyType == typeof(Color))
        {
            action = this.AddColorPicker;
        }
        else if (propertyType.IsAssignableTo(typeof(Enum)))
        {
            action = this.AddDropdown;
        }
        else if (propertyType == typeof(int) || propertyType == typeof(uint))
        {
            action = this.AddIntSlider;
        }
        else if (propertyType == typeof(int[]) || propertyType == typeof(uint[]))
        {
            action = this.AddIntSliderGroupFromArray;
        }
        else if (propertyType == typeof(Dictionary<int, int>) || propertyType == typeof(Dictionary<int, uint>))
        {
            action = this.AddIntSliderGroupFromIntDict;
        }
        else if (propertyType == typeof(Dictionary<string, int>) ||
                 propertyType == typeof(Dictionary<string, uint>))
        {
            action = this.AddIntSliderGroupFromStringDict;
        }
        else if (propertyType == typeof(float) || propertyType == typeof(double))
        {
            action = this.AddFloatSlider;
        }
        else if (propertyType == typeof(float[]) || propertyType == typeof(double[]))
        {
            action = this.AddFloatSliderGroupFromArray;
        }
        else if (propertyType == typeof(Dictionary<int, float>) || propertyType == typeof(Dictionary<int, double>))
        {
            action = this.AddFloatSliderGroupFromIntDict;
        }
        else if (propertyType == typeof(Dictionary<string, float>) ||
                 propertyType == typeof(Dictionary<string, double>))
        {
            action = this.AddFloatSliderGroupFromStringDict;
        }
        else if (propertyType == typeof(KeybindList))
        {
            action = this.AddKeyBinding;
        }

        if (action is null)
        {
            Log.E($"No action is defined for handling property of type {propertyType}");
            return;
        }

        action(property, getConfig);
        if (titleScreenOnly)
        {
            this.SetTitleScreenOnlyForNextOptions(false);
        }
    }

    /// <summary>Unregisters the mod config.</summary>
    /// <returns>The <typeparamref name="TGenericModConfigMenu"/> instance.</returns>
    private TGenericModConfigMenu Unregister()
    {
        if (!this.IsRegistered)
        {
            return (TGenericModConfigMenu)this;
        }

        this.ModApi.Unregister(this.Manifest);
        this.IsRegistered = false;
        Log.T("[GMCM]: The config menu has been unregistered.");
        return (TGenericModConfigMenu)this;
    }
}
