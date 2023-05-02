namespace DaLion.Alchemy.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Stardew.Alchemy.Framework.UI;

#endregion using directives

[UsedImplicitly]
internal sealed class OpenMenuCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="OpenMenuCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal OpenMenuCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "menu" };

    /// <inheritdoc />
    public override string Documentation => "Opens the alchemy menu.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        Game1.activeClickableMenu = new AlchemyMenu();
    }
}
