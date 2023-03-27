namespace DaLion.Overhaul.Modules.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    internal static void RevalidateAllWeapons()
    {
        Log.I(
            $"[Weapons]: Performing {(Context.IsMainPlayer ? "global" : "local")} items re-validation.");
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
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon)
                {
                    RevalidateSingleWeapon(weapon);
                }
            }
        }

        var removed = 0;
        if (WeaponsModule.IsEnabled)
        {
            foreach (var chest in IterateAllChests())
            {
                for (var i = chest.items.Count - 1; i >= 0; i--)
                {
                    if (chest.items[i] is not MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword } darkSword)
                    {
                        continue;
                    }

                    chest.items.Remove(darkSword);
                    removed++;
                }
            }
        }

        Log.I("[Weapons]: Done.");
        if (removed <= 0)
        {
            return;
        }

        Log.W($"{removed} Dark Swords were removed from Chests.");
        if (!Game1.player.hasOrWillReceiveMail("viegoCurse"))
        {
            return;
        }

        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword })
                {
                    break;
                }

                if (!Game1.player.addItemToInventoryBool(new MeleeWeapon(ItemIDs.DarkSword)))
                {
                    Log.E($"Failed adding Dark Sword to {Game1.player.Name}. Use CJB Item Spawner to obtain a new copy.");
                }
            }
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
    }

    internal static void RevalidateSingleWeapon(MeleeWeapon weapon)
    {
        weapon.RecalculateAppliedForges();
        if (!WeaponsModule.IsEnabled)
        {
            weapon.RemoveIntrinsicEnchantments();
        }
        else if (WeaponsModule.Config.InfinityPlusOne || WeaponsModule.Config.EnableRebalance)
        {
            weapon.AddIntrinsicEnchantments();
        }

        if (WeaponsModule.IsEnabled && WeaponsModule.Config.EnableStabbySwords &&
            (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex) ||
             WeaponsModule.Config.CustomStabbingSwords.Contains(weapon.Name)))
        {
            weapon.type.Value = MeleeWeapon.stabbingSword;
            Log.D($"[Weapons]: The type of {weapon.Name} was converted to Stabbing sword.");
        }
        else if ((!WeaponsModule.IsEnabled || !WeaponsModule.Config.EnableStabbySwords) &&
                 weapon.type.Value == MeleeWeapon.stabbingSword)
        {
            weapon.type.Value = MeleeWeapon.defenseSword;
            Log.D($"[Weapons]: The type of {weapon.Name} was converted to Defense sword.");
        }

        if (WeaponsModule.IsEnabled && WeaponsModule.Config.InfinityPlusOne && (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon()
            || weapon.InitialParentTileIndex is ItemIDs.DarkSword or ItemIDs.HolyBlade))
        {
            weapon.specialItem = true;
        }
    }

    internal static void AddAllIntrinsicEnchantments()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon)
                {
                    weapon.AddIntrinsicEnchantments();
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon)
                {
                    weapon.AddIntrinsicEnchantments();
                }
            }
        }
    }

    internal static void RemoveAllIntrinsicEnchantments()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon)
                {
                    weapon.RemoveIntrinsicEnchantments();
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon)
                {
                    weapon.RemoveIntrinsicEnchantments();
                }
            }
        }
    }

    internal static void ConvertAllStabbingSwords()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon { type.Value: MeleeWeapon.defenseSword } sword)
                {
                    return;
                }

                if (Collections.StabbingSwords.Contains(sword.InitialParentTileIndex) ||
                    WeaponsModule.Config.CustomStabbingSwords.Contains(sword.Name))
                {
                    sword.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon weapon && weapon.type.Value == MeleeWeapon.defenseSword &&
                    (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex) ||
                     WeaponsModule.Config.CustomStabbingSwords.Contains(weapon.Name)))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }
            }
        }
    }

    internal static void RevertAllStabbingSwords()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                {
                    sword.type.Value = MeleeWeapon.defenseSword;
                }
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                {
                    sword.type.Value = MeleeWeapon.defenseSword;
                }
            }
        }
    }

    internal static void RefreshAllWeapons(RefreshOption option)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon)
                {
                    return;
                }

                weapon.RefreshStats(option);
                weapon.Invalidate();
            });
        }
        else
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is not MeleeWeapon weapon)
                {
                    continue;
                }

                weapon.RefreshStats(option);
                weapon.Invalidate();
            }
        }
    }

    internal static IEnumerable<Chest> IterateAllChests()
    {
        for (var i = 0; i < Game1.locations.Count; i++)
        {
            var location1 = Game1.locations[i];
            foreach (var @object in location1.Objects.Values)
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

            for (var j = 0; j < buildable.buildings.Count; j++)
            {
                var building = buildable.buildings[j];
                if (building.indoors.Value is not { } location2)
                {
                    continue;
                }

                foreach (var @object in location2.Objects.Values)
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
