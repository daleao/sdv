namespace DaLion.Common.Commands;

/// <summary>Base implementation of a console command for a mod.</summary>
internal abstract class ConsoleCommand : IConsoleCommand
{
    protected readonly CommandHandler Handler;

    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    protected ConsoleCommand(CommandHandler handler)
    {
        Handler = handler;
    }

    /// <inheritdoc />
    public abstract string Trigger { get; }

    /// <inheritdoc />
    public abstract string Documentation { get; }

    /// <inheritdoc />
    public abstract void Callback(string[] args);
}