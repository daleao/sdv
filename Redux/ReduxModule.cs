namespace DaLion.Redux;

#region using directives

using Ardalis.SmartEnum;
using DaLion.Shared.Commands;
using DaLion.Shared.Harmony;

#endregion using directives

/// <summary>The individual modules within the Redux mod.</summary>
internal sealed class ReduxModule : SmartEnum<ReduxModule>
{
    #region enum entries

    /// <summary>Gets the namespace ID of the Core module.</summary>
    public static readonly ReduxModule Core = new("DaLion.Redux.Framework.Core", 0);

    /// <summary>Gets the namespace ID of the Professions module.</summary>
    public static readonly ReduxModule Professions = new("DaLion.Redux.Framework.Professions", 1);

    /// <summary>Gets the namespace ID of the Arsenal module.</summary>
    public static readonly ReduxModule Arsenal = new("DaLion.Redux.Framework.Arsenal", 2);

    /// <summary>Gets the namespace ID of the Rings module.</summary>
    public static readonly ReduxModule Rings = new("DaLion.Redux.Framework.Rings", 3);

    /// <summary>Gets the namespace ID of the Ponds module.</summary>
    public static readonly ReduxModule Ponds = new("DaLion.Redux.Framework.Ponds", 4);

    /// <summary> Gets the namespace ID of the Taxes module.</summary>
    public static readonly ReduxModule Taxes = new("DaLion.Redux.Framework.Taxes", 5);

    /// <summary>Gets the namespace ID of the Tools module.</summary>
    public static readonly ReduxModule Tools = new("DaLion.Redux.Framework.Tools", 6);

    /// <summary>Gets the namespace ID of the Tweex module.</summary>
    public static readonly ReduxModule Tweex = new("DaLion.Redux.Framework.Tweex", 7);

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="ReduxModule"/> class.</summary>
    /// <param name="id">The module name.</param>
    /// <param name="value">The module index.</param>
    private ReduxModule(string id, int value)
        : base(id, value)
    {
        switch (value)
        {
            case 0:
                this.DisplayName = "Redux Core";
                this.EntryCommand = "rdx";
                break;
            case 1:
                this.DisplayName = "Redux Professions";
                this.EntryCommand = "profs";
                break;
            case 2:
                this.DisplayName = "Redux Arsenal";
                this.EntryCommand = "ars";
                break;
            case 3:
                this.DisplayName = "Redux Rings";
                this.EntryCommand = "rings";
                break;
            case 4:
                this.DisplayName = "Redux Ponds";
                this.EntryCommand = "ponds";
                break;
            case 5:
                this.DisplayName = "Redux Taxes";
                this.EntryCommand = "tax";
                break;
            case 6:
                this.DisplayName = "Redux Tools";
                this.EntryCommand = "tools";
                break;
            case 7:
                this.DisplayName = "Redux Tweaks";
                this.EntryCommand = "tweex";
                break;
            default:
                ThrowHelper.ThrowArgumentException($"Invalid module {id}.");
                break;
        }
    }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets the entry command of the module.</summary>
    internal string EntryCommand { get; }

    /// <summary>Initializes the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    internal void Initialize(IModHelper helper)
    {
        ModEntry.Events.ManageNamespace(this.Name);
        new Harmonizer(helper.ModRegistry, this.Name).ApplyAll(true);
        new CommandHandler(helper.ConsoleCommands, this.Name).Register(this.EntryCommand, this.DisplayName);

#if DEBUG
        if (this != Core)
        {
            return;
        }

        // start FPS counter
        Framework.Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
        helper.Reflection.GetMethod(Framework.Globals.FpsCounter, "LoadContent").Invoke();
#endif
    }
}
