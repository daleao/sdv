namespace DaLion.Ligo.Modules;

#region using directives

using Ardalis.SmartEnum;
using DaLion.Ligo.Modules.Arsenal;
using DaLion.Ligo.Modules.Ponds;
using DaLion.Ligo.Modules.Professions;
using DaLion.Ligo.Modules.Rings;
using DaLion.Ligo.Modules.Taxes;
using DaLion.Ligo.Modules.Tools;
using DaLion.Ligo.Modules.Tweex;
using DaLion.Shared.Commands;
using DaLion.Shared.Harmony;

#endregion using directives

/// <summary>The individual modules within the Ligo mod.</summary>
internal abstract class LigoModule : SmartEnum<LigoModule>
{
    #region enum entries

    /// <summary>The Core module.</summary>
    public static readonly LigoModule Core = new CoreModule();

    /// <summary>The Professions module.</summary>
    public static readonly LigoModule Professions = new ProfessionsModule();

    /// <summary>The Arsenal module.</summary>
    public static readonly LigoModule Arsenal = new ArsenalModule();

    /// <summary>The Rings module.</summary>
    public static readonly LigoModule Rings = new RingsModule();

    /// <summary>The Ponds module.</summary>
    public static readonly LigoModule Ponds = new PondsModule();

    /// <summary>The Taxes module.</summary>
    public static readonly LigoModule Taxes = new TaxesModule();

    /// <summary>The Tools module.</summary>
    public static readonly LigoModule Tools = new ToolsModule();

    /// <summary>The Tweex module.</summary>
    public static readonly LigoModule Tweex = new TweexModule();

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="LigoModule"/> class.</summary>
    /// <param name="name">The module name.</param>
    /// <param name="value">The module index.</param>
    /// <param name="entry">The entry keyword for the module's <see cref="IConsoleCommand"/>s.</param>
    protected LigoModule(string name, int value, string entry)
        : base(name, value)
    {
        this.DisplayName = "Ligo" + name;
        this.Namespace = "DaLion.Ligo.Modules." + name;
        this.EntryCommand = entry;
    }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the entry command of the module.</summary>
    internal string EntryCommand { get; }

    /// <summary>Initializes the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal virtual void Initialize(IModHelper helper)
    {
        EventManager.ManageNamespace(this.Namespace);
        Harmonizer.ApplyFromNamespace(helper.ModRegistry, this.Namespace);
        CommandHandler.FromNamespace(helper.ConsoleCommands, this.Namespace, this.EntryCommand, this.DisplayName);
    }

    #region implementations

    private sealed class CoreModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.CoreModule"/> class.</summary>
        internal CoreModule()
            : base("Core", 0, "ligo")
        {
        }

        internal override void Initialize(IModHelper helper)
        {
            base.Initialize(helper);
#if DEBUG
            // start FPS counter
            Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
            helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
        }
    }

    internal sealed class ProfessionsModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.ProfessionsModule"/> class.</summary>
        internal ProfessionsModule()
            : base("Professions", 1, "profs")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.ProfessionsModule"/>.</summary>
        internal static ProfessionsConfig Config => ModEntry.Config.Professions;
    }

    internal sealed class ArsenalModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.ArsenalModule"/> class.</summary>
        internal ArsenalModule()
            : base("Arsenal", 2, "ars")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.ArsenalModule"/>.</summary>
        internal static ArsenalConfig Config => ModEntry.Config.Arsenal;
    }

    internal sealed class RingsModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.RingsModule"/> class.</summary>
        internal RingsModule()
            : base("Rings", 3, "rings")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.RingsModule"/>.</summary>
        internal static RingsConfig Config => ModEntry.Config.Rings;
    }

    internal sealed class PondsModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.PondsModule"/> class.</summary>
        internal PondsModule()
            : base("Ponds", 4, "ponds")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.PondsModule"/>.</summary>
        internal static PondsConfig Config => ModEntry.Config.Ponds;
    }

    internal sealed class TaxesModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.TaxesModule"/> class.</summary>
        internal TaxesModule()
            : base("Taxes", 5, "tax")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.TaxesModule"/>.</summary>
        internal static TaxesConfig Config => ModEntry.Config.Taxes;
    }

    internal sealed class ToolsModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", 6, "tools")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.ToolsModule"/>.</summary>
        internal static ToolsConfig Config => ModEntry.Config.Tools;
    }

    internal sealed class TweexModule : LigoModule
    {
        /// <summary>Initializes a new instance of the <see cref="LigoModule.TweexModule"/> class.</summary>
        internal TweexModule()
            : base("Tweex", 7, "tweex")
        {
        }

        /// <summary>Gets the config instance for the <see cref="LigoModule.TweexModule"/>.</summary>
        internal static TweexConfig Config => ModEntry.Config.Tweex;
    }

    #endregion implementations
}
