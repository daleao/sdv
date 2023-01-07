namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RevalidateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevalidateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevalidateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revalidate", "reval", "rev" };

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

        var removed = 0;
        if (ArsenalModule.IsEnabled)
        {
            foreach (var chest in IterateAllChests())
            {
                while (chest.items.FirstOrDefault(
                           i => i is MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex }) is { } darkSword)
                {
                    chest.items.Remove(darkSword);
                    removed++;
                }
            }
        }

        Log.I(
            $"All {(Context.IsMainPlayer ? "global" : "local")} items have been revalidated according to the current configuration settings.");
        if (removed <= 0)
        {
            return;
        }

        Log.W($"{removed} Dark Swords were removed from Chests.");
        if (Game1.player.hasOrWillReceiveMail("viegoCurse") &&
            Game1.player.Items.FirstOrDefault(
                i => i is MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex }) is null &&
            !Game1.player.addItemToInventoryBool(new MeleeWeapon(Constants.DarkSwordIndex)))
        {
            Log.E($"Failed adding Dark Sword to {Game1.player.Name}. Use CJB Item Spawner to obtain a new copy.");
        }
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
            (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex) ||
             ArsenalModule.Config.Weapons.CustomStabbingSwords.Contains(weapon.Name)))
        {
            weapon.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"The type of {weapon.Name} was converted to Stabbing sword.");
        }
        else if ((!ArsenalModule.IsEnabled || !ArsenalModule.Config.Weapons.EnableStabbySwords) &&
                 weapon.type.Value == MeleeWeapon.stabbingSword)
        {
            weapon.type.Value = MeleeWeapon.defenseSword;
            Log.D($"The type of {weapon.Name} was converted to Defense sword.");
        }

        if (ArsenalModule.IsEnabled && ArsenalModule.Config.InfinityPlusOne && (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon()
            || weapon.InitialParentTileIndex is Constants.DarkSwordIndex or Constants.HolyBladeIndex))
        {
            weapon.specialItem = true;
        }
    }

    private static IEnumerable<Chest> IterateAllChests()
    {
        foreach (var location1 in Game1.locations)
        {
            foreach (var @object in location1.objects.Values)
            {
                if (@object is Chest chest1)
                {
                    yield return chest1;
                }
                else if (@object.heldObject.Value is Chest chest2)
                {
                    yield return chest2;
                }
            }

            if (location1 is not BuildableGameLocation buildable)
            {
                continue;
            }

            foreach (var location2 in buildable.buildings
                         .Where(b => b.indoors.Value is not null)
                         .Select(b => b.indoors.Value))
            {
                foreach (var @object in location2.objects.Values)
                {
                    if (@object is Chest chest1)
                    {
                        yield return chest1;
                    }
                    else if (@object.heldObject.Value is Chest chest2)
                    {
                        yield return chest2;
                    }
                }
            }
        }
    }
}
