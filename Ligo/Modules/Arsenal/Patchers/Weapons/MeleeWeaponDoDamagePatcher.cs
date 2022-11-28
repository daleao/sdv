namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
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

    /// <summary>Override `special = false` for stabby sword + inject resonance bonuses.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoDamageTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
                    new CodeInstruction(OpCodes.Ldc_I4_0), // 0 = MeleeWeapon.stabbingSword
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
                .ReplaceInstructionWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon_Stats).RequireMethod(nameof(MeleeWeapon_Stats.Get_MinDamage))))
                .Advance()
                .RemoveInstructions();
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding combined weapon min damage.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.maxDamage))))
                .ReplaceInstructionWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon_Stats).RequireMethod(nameof(MeleeWeapon_Stats.Get_MaxDamage))))
                .Advance()
                .RemoveInstructions();
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding combined weapon max damage.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.knockback))))
                .ReplaceInstructionWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon_Stats).RequireMethod(nameof(MeleeWeapon_Stats.Get_AbsoluteKnockback))))
                .Advance()
                .RemoveInstructions();
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding combined weapon knockback.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_3))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Ldarg_0))
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon_Stats).RequireMethod(nameof(MeleeWeapon_Stats.Get_EffectiveCritChance))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding combined weapon crit. chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.critMultiplier))))
                .ReplaceInstructionWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon_Stats).RequireMethod(nameof(MeleeWeapon_Stats.Get_EffectiveCritPower))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding combined weapon crit. power.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
