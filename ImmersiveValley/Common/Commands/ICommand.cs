namespace DaLion.Common.Commands;

/// <summary>Interface for a console command.</summary>
internal interface ICommand
{
    /// <summary>The statement that triggers this command.</summary>
    string Trigger { get; }

    /// <summary>The human-readable documentation shown when the player runs the 'help' command.</summary>
    string Documentation { get; }

    /// <summary>The action that will be executed.</summary>
    void Callback(string[] args);
}