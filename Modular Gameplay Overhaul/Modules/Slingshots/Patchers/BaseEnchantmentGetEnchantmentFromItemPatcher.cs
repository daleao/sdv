namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetEnchantmentFromItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetEnchantmentFromItemPatcher"/> class.</summary>
    internal BaseEnchantmentGetEnchantmentFromItemPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetEnchantmentFromItem));
    }

    #region harmony patches

    /// <summary>Allow Galaxy Soul forge into Galaxy Slingshot.</summary>
    [HarmonyPostfix]
    private static void BaseEnchantmentGetEnchantmentFromItemPostfix(ref BaseEnchantment? __result, Item? base_item, Item item)
    {
        if (SlingshotsModule.Config.EnableInfinitySlingshot &&
            base_item is Slingshot { InitialParentTileIndex: ItemIDs.GalaxySlingshot } &&
            Utility.IsNormalObjectAtParentSheetIndex(item, ItemIDs.GalaxySoul))
        {
            __result = new GalaxySoulEnchantment();
        }
    }

    /// <summary>Allow Slingshot gemstone enchantments.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetEnchantmentFromItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()))
        // To: if (base_item == null || (base_item is MeleeWeapon && !(base_item as MeleeWeapon).isScythe()) || base_item is Slingshot)
        try
        {
            var isNotMeleeWeaponButMaybeSlingshot = generator.DefineLabel();
            var canForge = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) })
                .GetOperand(out var cannotForge)
                .SetOperand(isNotMeleeWeaponButMaybeSlingshot)
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue) })
                .Move()
                .AddLabels(canForge)
                .Insert(new[] { new CodeInstruction(OpCodes.Br_S, canForge) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                        new CodeInstruction(OpCodes.Brfalse, cannotForge),
                    },
                    new[] { isNotMeleeWeaponButMaybeSlingshot });
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing slingshot gemstone enchantments.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
