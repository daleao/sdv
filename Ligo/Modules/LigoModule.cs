namespace DaLion.Ligo.Modules;

#region using directives

using Ardalis.SmartEnum;
using DaLion.Shared.Commands;
using DaLion.Shared.Harmony;

#endregion using directives

/// <summary>The individual modules within the Ligo mod.</summary>
internal sealed class LigoModule : SmartEnum<LigoModule>
{
    #region enum entries

    /// <summary>Gets the namespace ID of the Core module.</summary>
    public static readonly LigoModule Core = new("Core", 0);

    /// <summary>Gets the namespace ID of the Professions module.</summary>
    public static readonly LigoModule Professions = new("Professions", 1);

    /// <summary>Gets the namespace ID of the Arsenal module.</summary>
    public static readonly LigoModule Arsenal = new("Arsenal", 2);

    /// <summary>Gets the namespace ID of the Rings module.</summary>
    public static readonly LigoModule Rings = new("Rings", 3);

    /// <summary>Gets the namespace ID of the Ponds module.</summary>
    public static readonly LigoModule Ponds = new("Ponds", 4);

    /// <summary> Gets the namespace ID of the Taxes module.</summary>
    public static readonly LigoModule Taxes = new("Taxes", 5);

    /// <summary>Gets the namespace ID of the Tools module.</summary>
    public static readonly LigoModule Tools = new("Tools", 6);

    /// <summary>Gets the namespace ID of the Tweex module.</summary>
    public static readonly LigoModule Tweex = new("Tweex", 7);

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="LigoModule"/> class.</summary>
    /// <param name="name">The module name.</param>
    /// <param name="value">The module index.</param>
    private LigoModule(string name, int value)
        : base(name, value)
    {
        this.DisplayName = "Ligo " + name;
        this.Namespace = "DaLion.Ligo.Modules." + name;
        this.EntryCommand = name switch
        {
            "Core" => "ligo",
            "Professions" => "profs",
            "Arsenal" => "ars",
            "Rings" => "rings",
            "Ponds" => "ponds",
            "Taxes" => "tax",
            "Tools" => "tools",
            "Tweex" => "tweex",
            _ => ThrowHelper.ThrowArgumentException<string>($"Invalid module {name}."),
        };
    }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the entry command of the module.</summary>
    internal string EntryCommand { get; }

    /// <summary>Initializes the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal void Initialize(IModHelper helper)
    {
        ModEntry.Events.ManageNamespace(this.Namespace);
        Harmonizer.ApplyFromNamespace(helper.ModRegistry, this.Namespace);
        CommandHandler.FromNamespace(helper.ConsoleCommands, this.Namespace, this.EntryCommand, this.DisplayName);

#if DEBUG
        if (this != Core)
        {
            return;
        }

        // start FPS counter
        Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
        helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
    }
}
