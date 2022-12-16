namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RevalidateItemsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevalidateItemsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevalidateItemsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revalidate_items", "revalidate", "reval" };

    /// <inheritdoc />
    public override string Documentation => "Applies or removes persistent changes made by modules to existing items.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        ModHelper.GameContent.InvalidateCache("Data/weapons");
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon)
                {
                    RevalidateSingleWeapon(weapon);
                }
            });
        }
        else
        {
            Game1.player.Items.OfType<MeleeWeapon>().ForEach(RevalidateSingleWeapon);
        }

        if (Config.EnableArsenal && Game1.player.hasOrWillReceiveMail("galaxySword"))
        {
            Game1.player.WriteIfNotExists(DataFields.GalaxyArsenalObtained, Constants.GalaxySwordIndex.ToString());
        }

        Log.I(
            $"All {(Context.IsMainPlayer ? "global" : "local")} items have been revalidated according to the current configuration settings.");
    }

    private static void RevalidateSingleWeapon(MeleeWeapon weapon)
    {
        if (weapon.isScythe())
        {
            Tools.Utils.RevalidateScythes();
            return;
        }

        if (!Config.EnableArsenal)
        {
            weapon.RemoveIntrinsicEnchantments();
        }
        else if (ArsenalModule.Config.InfinityPlusOne || ArsenalModule.Config.Weapons.RebalancedStats)
        {
            weapon.RemoveIntrinsicEnchantments();
            weapon.AddIntrinsicEnchantments();
        }

        if (Config.EnableArsenal && ArsenalModule.Config.Weapons.BringBackStabbySwords &&
            Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex))
        {
            weapon.type.Value = MeleeWeapon.stabbingSword;
        }
        else if ((!Config.EnableArsenal || !ArsenalModule.Config.Weapons.BringBackStabbySwords) &&
                 weapon.type.Value == MeleeWeapon.stabbingSword)
        {
            weapon.type.Value = MeleeWeapon.defenseSword;
        }

        if (Config.EnableArsenal && ArsenalModule.Config.Weapons.RebalancedStats)
        {
            weapon.RefreshStats();
        }
        else if (!Config.EnableArsenal || !ArsenalModule.Config.Weapons.RebalancedStats)
        {
            weapon.RecalculateAppliedForges(true);
        }

        if (!Config.EnableArsenal && weapon.InitialParentTileIndex == Constants.GalaxySwordIndex && !Game1.player.hasOrWillReceiveMail("galaxySword"))
        {
            Game1.player.mailReceived.Add("galaxySword");
        }
    }
}
