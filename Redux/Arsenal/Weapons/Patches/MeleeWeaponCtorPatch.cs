namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Arsenal.Weapons.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCtorPatch"/> class.</summary>
    internal MeleeWeaponCtorPatch()
    {
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    /// <summary>Add intrinsic weapon enchantments.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponCtorPostfix(MeleeWeapon __instance)
    {
        // convert stabby swords
        if (__instance.type.Value == MeleeWeapon.defenseSword &&
            ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords &&
            ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(__instance.Name))
        {
            __instance.type.Value = MeleeWeapon.stabbingSword;
        }

        if (!ModEntry.Config.Arsenal.Weapons.InfinityPlusOneWeapons || __instance.isScythe())
        {
            return;
        }

        // apply unique enchants
        switch (__instance.InitialParentTileIndex)
        {
            case Constants.DarkSwordIndex:
                __instance.enchantments.Add(new CursedEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.HolyBladeIndex:
                __instance.enchantments.Add(new BlessedEnchantment());
                __instance.specialItem = true;
                break;
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityClubIndex:
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    /// <summary>Prevent the game from overriding stabby swords.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Removed: if ((int)type == 0) { type.Set(3); }
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))))
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))))
                .Retreat()
                .RemoveInstructionsUntil(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequireMethod(nameof(NetFieldBase<int, NetInt>.Set))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing stabby sword override.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
