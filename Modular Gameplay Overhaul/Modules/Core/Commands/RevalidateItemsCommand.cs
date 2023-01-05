namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using Arsenal;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
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
    public override string[] Triggers { get; } = { "revalidate_items", "revalidate", "reval", "rev" };

    /// <inheritdoc />
    public override string Documentation => "Applies or removes persistent changes made by modules to existing items.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
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

        if (ArsenalModule.IsEnabled && Game1.player.mailReceived.Contains("galaxySword"))
        {
            Game1.player.WriteIfNotExists(DataFields.GalaxyArsenalObtained, Constants.GalaxySwordIndex.ToString());
        }

        Log.I(
            $"All {(Context.IsMainPlayer ? "global" : "local")} items have been revalidated according to the current configuration settings.");
    }

    private static void RevalidateSingleWeapon(MeleeWeapon weapon)
    {
        weapon.RecalculateAppliedForges();
        if (!ArsenalModule.IsEnabled)
        {
            weapon.RemoveIntrinsicEnchantments();
        }
        else if (ArsenalModule.Config.InfinityPlusOne || ArsenalModule.Config.Weapons.EnableRebalance)
        {
            weapon.AddIntrinsicEnchantments();
        }

        if (ArsenalModule.IsEnabled && ArsenalModule.Config.Weapons.EnableStabbySwords &&
            Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex))
        {
            weapon.type.Value = MeleeWeapon.stabbingSword;
        }
        else if ((!ArsenalModule.IsEnabled || !ArsenalModule.Config.Weapons.EnableStabbySwords) &&
                 weapon.type.Value == MeleeWeapon.stabbingSword)
        {
            weapon.type.Value = MeleeWeapon.defenseSword;
        }

        if (ArsenalModule.IsEnabled && ArsenalModule.Config.InfinityPlusOne && (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon()
            || weapon.InitialParentTileIndex is Constants.DarkSwordIndex or Constants.HolyBladeIndex))
        {
            weapon.specialItem = true;
        }
    }
}
