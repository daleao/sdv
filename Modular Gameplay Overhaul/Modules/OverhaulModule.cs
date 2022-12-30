namespace DaLion.Overhaul.Modules;

#region using directives

using System.Diagnostics.CodeAnalysis;
using Ardalis.SmartEnum;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;

#endregion using directives

/// <summary>The individual modules within the Overhaul mod.</summary>
internal abstract class OverhaulModule : SmartEnum<OverhaulModule>
{
    #region enum entries

    /// <summary>The Core module.</summary>
    public static readonly OverhaulModule Core = new CoreModule();

    /// <summary>The Professions module.</summary>
    public static readonly OverhaulModule Professions = new ProfessionsModule();

    /// <summary>The Arsenal module.</summary>
    public static readonly OverhaulModule Arsenal = new ArsenalModule();

    /// <summary>The Rings module.</summary>
    public static readonly OverhaulModule Rings = new RingsModule();

    /// <summary>The Ponds module.</summary>
    public static readonly OverhaulModule Ponds = new PondsModule();

    /// <summary>The Taxes module.</summary>
    public static readonly OverhaulModule Taxes = new TaxesModule();

    /// <summary>The Tools module.</summary>
    public static readonly OverhaulModule Tools = new ToolsModule();

    /// <summary>The Tweex module.</summary>
    public static readonly OverhaulModule Tweex = new TweexModule();

    #endregion enum entries

    private Harmonizer? _harmonizer;
    private CommandHandler? _commandHandler;

    /// <summary>Initializes a new instance of the <see cref="OverhaulModule"/> class.</summary>
    /// <param name="name">The module name.</param>
    /// <param name="value">The module index.</param>
    /// <param name="entry">The entry keyword for the module's <see cref="IConsoleCommand"/>s.</param>
    protected OverhaulModule(string name, int value, string entry)
        : base(name, value)
    {
        this.DisplayName = "Modular Overhaul :: " + name;
        this.Namespace = "DaLion.Overhaul.Modules." + name;
        this.EntryCommand = entry;
    }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the entry command of the module.</summary>
    internal string EntryCommand { get; }

    /// <summary>Gets a value indicating whether the module is currently active.</summary>
    [MemberNotNullWhen(true, nameof(_harmonizer), nameof(_commandHandler))]
    internal bool IsActive { get; private set; }

    /// <summary>Activates the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    [MemberNotNull(nameof(_harmonizer), nameof(_commandHandler))]
    internal virtual void Activate(IModHelper helper)
    {
        if (this.IsActive)
        {
            Log.D($"{this.Name} module is already active.");
            return;
        }

        EventManager.ManageNamespace(this.Namespace);
        this._harmonizer = Harmonizer.FromNamespace(helper.ModRegistry, this.Namespace);
        this._commandHandler ??= CommandHandler.FromNamespace(
            helper.ConsoleCommands,
            this.Namespace,
            this.DisplayName,
            this.EntryCommand,
            () => this.IsActive);
        this.IsActive = true;
        this.InvalidateAssets();
    }

    /// <summary>Deactivates the module.</summary>
    internal virtual void Deactivate()
    {
        if (!this.IsActive)
        {
            Log.D($"{this.Name} module is not active.");
            return;
        }

        EventManager.UnmanageNamespace(this.Namespace);
        this._harmonizer = this._harmonizer.Unapply();
        this.IsActive = false;
        this.InvalidateAssets();
    }

    /// <summary>Causes SMAPI to reload all assets edited by this module.</summary>
    protected virtual void InvalidateAssets()
    {
    }

    #region implementations

    internal sealed class ProfessionsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ProfessionsModule"/> class.</summary>
        internal ProfessionsModule()
            : base("Professions", 1, "profs")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ProfessionsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableProfessions;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.Config Config => ModEntry.Config.Professions;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.State State => ModEntry.State.Professions;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/achievements");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/FishPondData");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprite/Cursors");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class ArsenalModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ArsenalModule"/> class.</summary>
        internal ArsenalModule()
            : base("Arsenal", 2, "ars")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ArsenalModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableArsenal;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ArsenalModule"/>.</summary>
        internal static Arsenal.Config Config => ModEntry.Config.Arsenal;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ArsenalModule"/>.</summary>
        internal static Arsenal.State State => ModEntry.State.Arsenal;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/AdventureGuild");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class RingsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.RingsModule"/> class.</summary>
        internal RingsModule()
            : base("Rings", 3, "rings")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.RingsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableRings;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.Config Config => ModEntry.Config.Rings;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.State State => ModEntry.State.Rings;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    internal sealed class PondsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.PondsModule"/> class.</summary>
        internal PondsModule()
            : base("Ponds", 4, "ponds")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.PondsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnablePonds;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.PondsModule"/>.</summary>
        internal static Ponds.Config Config => ModEntry.Config.Ponds;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCache("Data/FishPondData");
        }
    }

    internal sealed class TaxesModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TaxesModule"/> class.</summary>
        internal TaxesModule()
            : base("Taxes", 5, "tax")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TaxesModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTaxes;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.Config Config => ModEntry.Config.Taxes;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.State State => ModEntry.State.Taxes;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        }
    }

    internal sealed class ToolsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", 6, "tools")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ToolsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTools;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.Config Config => ModEntry.Config.Tools;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.State State => ModEntry.State.Tools;

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
        }
    }

    internal sealed class TweexModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TweexModule"/> class.</summary>
        internal TweexModule()
            : base("Tweex", 7, "tweex")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TweexModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTweex;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TweexModule"/>.</summary>
        internal static Tweex.Config Config => ModEntry.Config.Tweex;
    }

    private sealed class CoreModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.CoreModule"/> class.</summary>
        internal CoreModule()
            : base("Core", 0, "margo")
        {
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            base.Activate(helper);
#if DEBUG
            // start FPS counter
            Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
            helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
        }
    }

    #endregion implementations
}
