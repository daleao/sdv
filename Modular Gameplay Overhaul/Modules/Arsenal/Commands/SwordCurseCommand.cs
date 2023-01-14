namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class SwordCurseCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SwordCurseCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SwordCurseCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "curse" };

    /// <inheritdoc />
    public override string Documentation => "Strengthen the curse of a currently held Dark Sword.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex })
        {
            Log.W("You must hold the cursed blade to use this command.");
            return;
        }

        if (args.Length == 0 || !int.TryParse(args[0], out var points))
        {
            points = 500;
        }

        player.CurrentTool.Write(DataFields.CursePoints, points.ToString());
    }
}
