namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RevertArsenalCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevertArsenalCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevertArsenalCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clean_arsenal", "clean", "revert_arsenal", "revert", "undo_arsenal", "undo", "ars" };

    /// <inheritdoc />
    public override string Documentation => "Undoes changes made by the Arsenal module to avoid issues before disabling.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        ModHelper.GameContent.InvalidateCache("Data/weapons");
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon || weapon.isScythe())
                {
                    return;
                }

                weapon.RemoveIntrinsicEnchantments();

                if (weapon.type.Value == MeleeWeapon.stabbingSword)
                {
                    weapon.type.Value = MeleeWeapon.defenseSword;
                }

                if (weapon.InitialParentTileIndex == Constants.GalaxySwordIndex && !Game1.player.hasOrWillReceiveMail("galaxySword"))
                {
                    Game1.player.mailReceived.Add("galaxySword");
                }

                weapon.RecalculateAppliedForges(true);
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                if (weapon.isScythe())
                {
                    continue;
                }

                weapon.RemoveIntrinsicEnchantments();

                if (weapon.type.Value == MeleeWeapon.stabbingSword)
                {
                    weapon.type.Value = MeleeWeapon.defenseSword;
                }

                if (weapon.InitialParentTileIndex == Constants.GalaxySwordIndex && !Game1.player.hasOrWillReceiveMail("galaxySword"))
                {
                    Game1.player.mailReceived.Add("galaxySword");
                }

                weapon.RecalculateAppliedForges(true);
            }
        }

        Log.I("Arsenal changes have been undone.");
    }
}
