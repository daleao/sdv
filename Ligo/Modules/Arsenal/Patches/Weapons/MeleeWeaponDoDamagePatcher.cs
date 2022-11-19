namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoDamagePatcher"/> class.</summary>
    internal MeleeWeaponDoDamagePatcher()
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

        if (!ModEntry.Config.EnableRings)
        {
            return helper.Flush();
        }

        // Inject resonance stat bonuses //

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.minDamage))))
                .Advance(3)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(AddResonantDamage))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding resonance to weapon min damage.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.maxDamage))))
                .Advance(3)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(AddResonantDamage))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding resonance to weapon max damage.\nHelper returned {ex}");
            return null;
        }

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
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(AddResonantKnockback))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding resonance to weapon knockback.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_3))
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(AddResonantCritChance))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding resonance to weapon crit. chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.critMultiplier))))
                .Advance(2)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(AddResonantCritPower))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding resonance to weapon crit. power.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static float AddResonantDamage(float damage, MeleeWeapon weapon)
    {
        return damage * (1f + weapon.Read<float>(DataFields.ResonantDamage));
    }

    private static float AddResonantKnockback(float knockback, MeleeWeapon weapon)
    {
        return knockback * (1f + weapon.Read<float>(DataFields.ResonantKnockback));
    }

    private static float AddResonantCritChance(float critChance, MeleeWeapon weapon)
    {
        return critChance * (1f + weapon.Read<float>(DataFields.ResonantCritChance));
    }

    private static float AddResonantCritPower(float critPower, MeleeWeapon weapon)
    {
        return critPower * (1f + weapon.Read<float>(DataFields.ResonantCritPower));
    }

    #endregion injected subroutines
}
