namespace DaLion.Overhaul.Modules.Core.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DebugCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DebugCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "debug" };

    /// <inheritdoc />
    public override string Documentation => "Wildcard command for on-demand debugging.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
    }
}
