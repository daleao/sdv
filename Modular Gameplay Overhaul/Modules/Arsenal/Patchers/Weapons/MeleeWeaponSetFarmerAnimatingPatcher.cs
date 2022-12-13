namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
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
        var helper = new ILHelper(original, instructions);

        // Removed: swipeSpeed -= who.addedSpeed * 40;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.addedSpeed))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Sub) }, out var count)
                .Remove(count);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing move speed's effect to swing speed.\nHelper returned {ex}");
            return null;
        }

        // From: if (who.IsLocalPlayer)
        // To: if (who.IsLocalPlayer && this.type.Value == MeleeWeapon.dagger)
        // Before: foreach (BaseEnchantment enchantment in enchantments) if (enchantment is BaseWeaponEnchantment) (enchantment as BaseWeaponEnchantment).OnSwing(this, who);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    },
                    ILHelper.SearchOption.First)
                .Move(2)
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // 1 = MeleeWeapon.dagger
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
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
