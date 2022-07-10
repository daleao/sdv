namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatch()
    {
        Target = RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetEnchantmentFromItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()))
        /// To: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()) || base_item is Slingshot)

        var isNotMeleeWeaponButMaybeSlingshot = generator.DefineLabel();
        var canForge = generator.DefineLabel();
        try
        {
            helper
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse)
                )
                .GetOperand(out var cannotForge)
                .SetOperand(isNotMeleeWeaponButMaybeSlingshot)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue)
                )
                .Advance()
                .AddLabels(canForge)
                .Insert(
                    new CodeInstruction(OpCodes.Br_S, canForge)
                )
                .InsertWithLabels(
                    new[] { isNotMeleeWeaponButMaybeSlingshot },
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                    new CodeInstruction(OpCodes.Brfalse, cannotForge)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing slingshot forges.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    /// <summary>Get infinity enchantment from hero soul.</summary>
    [HarmonyPostfix]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment __result, Item base_item, Item item)
    {
        if (base_item is MeleeWeapon weapon && weapon.isGalaxyWeapon() &&
            weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3 &&
            item.Name == "Hero Soul") __result = new InfinityEnchantment();
    }

    #endregion harmony patches
}