namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Common.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatch"/> class.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Guaranteed crit on underground Duggy from club smash attack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: Monster.set_WasKnockedBack(true);
        // After: trajectory *= knockBackModifier;
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[6]),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                    new CodeInstruction(OpCodes.Call),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .Advance(4)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_WasKnockedBack).RequireMethod(nameof(Monster_WasKnockedBack.Set_WasKnockedBack))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording knocked back flag.\nHelper returned {ex}");
            return null;
        }

        // Injected: Monster.set_GotCrit(true);
        // After: playSound("crit");
        try
        {
            helper
                .FindNext(new CodeInstruction(OpCodes.Ldstr, "crit"))
                .Advance(3)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.Set_GotCrit))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording crit flag.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsClubSmashHittingDuggy(MeleeWeapon weapon, Monster monster)
    {
        return ModEntry.Config.Arsenal.Weapons.GroundedClubSmash && weapon.type.Value == MeleeWeapon.club &&
               weapon.isOnSpecial && monster is Duggy;
    }

    #endregion injected subroutines
}
