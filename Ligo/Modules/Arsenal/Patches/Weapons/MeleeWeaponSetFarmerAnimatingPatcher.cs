namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSetFarmerAnimatingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSetFarmerAnimatingPatcher"/> class.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Movement speed does not affect swing speed + remove weapon enchantment OnSwing effect.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Removed: swipeSpeed -= who.addedSpeed * 40;
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.addedSpeed))))
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Sub));
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing move speed's effect to swing speed.\nHelper returned {ex}");
            return null;
        }

        // Removed: if (who.IsLocalPlayer) foreach (BaseEnchantment enchantment in enchantments) if (enchantment is BaseWeaponEnchantment) (enchantment as BaseWeaponEnchantment).OnSwing(this, who);
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))))
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Endfinally));
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing enchantment on swing effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
