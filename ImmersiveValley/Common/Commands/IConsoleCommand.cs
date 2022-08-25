namespace DaLion.Common.Commands;

/// <summary>Interface for a console command for a mod.</summary>
internal interface IConsoleCommand
{
    /// <summary>The statement that triggers this command.</summary>
    string[] Triggers { get; }

    /// <summary>The human-readable documentation shown when the player runs the 'help' command.</summary>
    string Documentation { get; }

    /// <summary>The action that will be executed.</summary>
    /// <param name="args">The command arguments.</param>
    void Callback(string[] args);
}