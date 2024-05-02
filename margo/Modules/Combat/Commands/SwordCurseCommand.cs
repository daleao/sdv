namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
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
    public override string Documentation => "Set the curse strength of a currently held Dark Sword to the specified number. 50 is the base curse level at which the sword's quest begins.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword })
        {
            Log.W("You must be holding the Dark Sword to use this command.");
            return;
        }

        if (args.Length == 0 || !int.TryParse(args[0], out var points))
        {
            points = 500;
        }

        player.CurrentTool.Write(DataKeys.CursePoints, points.ToString());
        if (points >= 50 && !player.hasOrWillReceiveMail("viegoCurse"))
        {
            player.mailForTomorrow.Add("viegoCurse");
        }
    }
}
