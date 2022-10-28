namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoDamagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoDamagePatch"/> class.</summary>
    internal MeleeWeaponDoDamagePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.DoDamage));
    }

    #region harmony patches

    /// <summary>Override `special = false` for stabby sword.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoDamageTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: isOnSpecial = false;
        // To: isOnSpecial = (type.Value == MeleeWeapon.stabbingSword && isOnSpecial);
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial))))
                .Retreat()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial))),
                    new CodeInstruction(OpCodes.And))
                .RemoveInstructions();
        }
        catch (Exception ex)
        {
            Log.E($"Failed to prevent special stabby sword override.\nHelper returned {ex}");
            return null;
        }

        // From: knockback * (1f + who.knockbackModifier)
        // To: (knockback - GetKnockbackOffset(this)) * (1f + who.knockbackModifier)

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.knockback))))
                .Advance(2)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatch).RequireMethod(nameof(GetKnockbackOffset))),
                    new CodeInstruction(OpCodes.Sub));
        }
        catch (Exception ex)
        {
            Log.E($"Failed to add knockback offset.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static float GetKnockbackOffset(MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.dagger => 0f,
            MeleeWeapon.club => 0.5f,
            MeleeWeapon.defenseSword => 0.25f,
            MeleeWeapon.stabbingSword => 0.25f,
            _ => 0f,
        };
    }

    #endregion injected subroutines
}
