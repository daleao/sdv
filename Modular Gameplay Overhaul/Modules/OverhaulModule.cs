namespace DaLion.Overhaul.Modules;

#region using directives

using System.Diagnostics.CodeAnalysis;
using Ardalis.SmartEnum;
using DaLion.Overhaul.Modules.Arsenal;
using DaLion.Overhaul.Modules.Ponds;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Rings;
using DaLion.Overhaul.Modules.Taxes;
using DaLion.Overhaul.Modules.Tools;
using DaLion.Overhaul.Modules.Tweex;
using DaLion.Shared.Commands;
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
    }

    #region implementations

    internal sealed class ProfessionsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ProfessionsModule"/> class.</summary>
        internal ProfessionsModule()
            : base("Professions", 1, "profs")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static ProfessionsConfig Config => ModEntry.Config.Professions;
    }

    internal sealed class ArsenalModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ArsenalModule"/> class.</summary>
        internal ArsenalModule()
            : base("Arsenal", 2, "ars")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ArsenalModule"/>.</summary>
        internal static ArsenalConfig Config => ModEntry.Config.Arsenal;
    }

    internal sealed class RingsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.RingsModule"/> class.</summary>
        internal RingsModule()
            : base("Rings", 3, "rings")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static RingsConfig Config => ModEntry.Config.Rings;
    }

    internal sealed class PondsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.PondsModule"/> class.</summary>
        internal PondsModule()
            : base("Ponds", 4, "ponds")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.PondsModule"/>.</summary>
        internal static PondsConfig Config => ModEntry.Config.Ponds;
    }

    internal sealed class TaxesModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TaxesModule"/> class.</summary>
        internal TaxesModule()
            : base("Taxes", 5, "tax")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static TaxesConfig Config => ModEntry.Config.Taxes;
    }

    internal sealed class ToolsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", 6, "tools")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static ToolsConfig Config => ModEntry.Config.Tools;
    }

    internal sealed class TweexModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TweexModule"/> class.</summary>
        internal TweexModule()
            : base("Tweex", 7, "tweex")
        {
        }

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TweexModule"/>.</summary>
        internal static TweexConfig Config => ModEntry.Config.Tweex;
    }

    private sealed class CoreModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.CoreModule"/> class.</summary>
        internal CoreModule()
            : base("Core", 0, "mgo")
        {
        }

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
