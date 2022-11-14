namespace DaLion.Ligo.Modules.Arsenal.Weapons.Commands;

#region using directives

using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class RevertStabbySwordsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevertStabbySwordsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevertStabbySwordsCommand(CommandHandler handler)
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
        Utils.RevertStabbySwords();
    }
}
