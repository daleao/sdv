namespace DaLion.Shared.Commands;

/// <summary>Base implementation of a console command for a mod.</summary>
internal abstract class ConsoleCommand : IConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ConsoleCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    protected ConsoleCommand(CommandHandler handler)
    {
        this.Handler = handler;
    }

    /// <inheritdoc />
    public abstract string[] Triggers { get; }

    /// <inheritdoc />
    public abstract string Documentation { get; }

    /// <summary>Gets the <see cref="CommandHandler"/> instance that handles this command.</summary>
    protected CommandHandler Handler { get; }

    /// <inheritdoc />
    public abstract void Callback(string[] args);
}
