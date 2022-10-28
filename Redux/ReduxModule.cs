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
    public static readonly ReduxModule Core = new("DaLion.Redux.Core", 0);

    /// <summary>Gets the namespace ID of the Debug module.</summary>
    public static readonly ReduxModule Debug = new("DaLion.Redux.Debug", 1);

    /// <summary>Gets the namespace ID of the Professions module.</summary>
    public static readonly ReduxModule Professions = new("DaLion.Redux.Professions", 2);

    /// <summary>Gets the namespace ID of the Arsenal module.</summary>
    public static readonly ReduxModule Arsenal = new("DaLion.Redux.Arsenal", 3);

    /// <summary>Gets the namespace ID of the Rings module.</summary>
    public static readonly ReduxModule Rings = new("DaLion.Redux.Rings", 4);

    /// <summary>Gets the namespace ID of the Ponds module.</summary>
    public static readonly ReduxModule Ponds = new("DaLion.Redux.Ponds", 5);

    /// <summary> Gets the namespace ID of the Taxes module.</summary>
    public static readonly ReduxModule Taxes = new("DaLion.Redux.Taxes", 6);

    /// <summary>Gets the namespace ID of the Tools module.</summary>
    public static readonly ReduxModule Tools = new("DaLion.Redux.Tools", 7);

    /// <summary>Gets the namespace ID of the Tweex module.</summary>
    public static readonly ReduxModule Tweex = new("DaLion.Redux.Tweex", 8);

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
                this.EntryCommand = "rxc";
                break;
            case 1:
                this.DisplayName = "Redux Debug";
                this.EntryCommand = "rxd";
                break;
            case 2:
                this.DisplayName = "Redux Professions";
                this.EntryCommand = "wol";
                break;
            case 3:
                this.DisplayName = "Redux Arsenal";
                this.EntryCommand = "ars";
                break;
            case 4:
                this.DisplayName = "Redux Rings";
                this.EntryCommand = "lotr";
                break;
            case 5:
                this.DisplayName = "Redux Ponds";
                this.EntryCommand = "aq";
                break;
            case 6:
                this.DisplayName = "Redux Taxes";
                this.EntryCommand = "serf";
                break;
            case 7:
                this.DisplayName = "Redux Tools";
                this.EntryCommand = "tan";
                break;
            case 8:
                this.DisplayName = "Redux Tweaks";
                this.EntryCommand = "qol";
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
    }
}
