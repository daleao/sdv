namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class ReadyForgeCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ReadyForgeCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ReadyForgeCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "ready_forge", "rdy_forge" };

    /// <inheritdoc />
    public override string Documentation => "Forcefully sets the flag which allows the player to use the Forge option at Clint's.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        Game1.player.mailReceived.Add("clintForge");
    }
}
