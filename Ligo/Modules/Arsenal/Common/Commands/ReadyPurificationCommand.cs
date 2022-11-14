namespace DaLion.Ligo.Modules.Arsenal.Common.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class ReadyPurificationCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ReadyPurificationCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ReadyPurificationCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "ready_purification" };

    /// <inheritdoc />
    public override string Documentation => "Ready a currently held Dark Sword for purification.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var darkSword = Game1.player.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: Constants.DarkSwordIndex
        });

        if (darkSword is null)
        {
            Log.W("You are not carrying the Dark Sword.");
            return;
        }

        // implement...
    }
}
