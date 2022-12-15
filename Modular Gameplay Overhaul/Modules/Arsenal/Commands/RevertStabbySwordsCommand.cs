namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class RevertStabbingSwordsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevertStabbingSwordsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevertStabbingSwordsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revert" };

    /// <inheritdoc />
    public override string Documentation => "Reverts stabbing swords back into vanilla defensive swords.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        Utils.RevertAllStabbingSwords();
    }
}
