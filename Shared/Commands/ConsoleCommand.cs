namespace DaLion.Shared.Commands;

/// <summary>Base implementation of a console command for a mod.</summary>
public abstract class ConsoleCommand : IConsoleCommand
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
    public virtual void Callback(string trigger, string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var usage = this.GetUsage();
            if (!string.IsNullOrEmpty(usage))
            {
                this.Handler.Log.I(usage);
            }

            return;
        }

        if (!this.CallbackImpl(trigger, args))
        {
            var usage = this.GetUsage();
            if (!string.IsNullOrEmpty(usage))
            {
                this.Handler.Log.W(usage);
            }
        }
    }

    /// <inheritdoc cref="Callback"/>
    /// <returns><see lamngword="true"/> if the command ran successfully, or <see langword="false"/> if the user should be shown usage instructions.</returns>
    public abstract bool CallbackImpl(string trigger, string[] args);

    /// <summary>Gets the usage documentation for the command.</summary>
    /// <returns>A formatted <see cref="string"/> with the usage documentation of the command.</returns>
    protected virtual string? GetUsage()
    {
        return null;
    }
}
