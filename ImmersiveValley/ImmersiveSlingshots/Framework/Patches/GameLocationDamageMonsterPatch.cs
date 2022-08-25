namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster), new[]
        {
            typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
            typeof(float), typeof(float), typeof(bool), typeof(Farmer)
        });
    }

    #region harmony patches

    /// <summary>Apply Slingshot special smash stun.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: else if (damageAmount > 0) { ... }
        /// To: else { DoSlingshotSpecial(monster, who); if (damageAmount > 0) { ... } }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(DoSlingshotSpecial)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding slingshot special stun.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DoSlingshotSpecial(Monster monster, Farmer who)
    {
        if (who.CurrentTool is Slingshot slingshot && slingshot.get_IsOnSpecial())
            monster.stunTime = 2000;
    }

    #endregion injected subroutines
}