namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTriggerClubFunctionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponTriggerClubFunctionPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.triggerClubFunction));
    }

    #region harmony patches

    /// <summary>Doubles AoE of Infinity Club's special smash move.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponTriggerClubFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (this.hasEnchantmentOfType<InfinityEnchantment>() areaOfEffect.Inflate(78, 78);
        /// After: new Rectangle((int)lastUser.Position.X - 192, lastUser.GetBoundingBox().Y - 192, 384, 384)

        var notInfinity = generator.DefineLabel();
        var aoe = generator.DeclareLocal(typeof(Rectangle));
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Newobj)
                )
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Stloc_S, aoe)
                )
                .InsertWithLabels(
                    new[] { notInfinity },
                    new CodeInstruction(OpCodes.Ldloc_S, aoe)
                )
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(MeleeWeapon).RequireMethod(nameof(MeleeWeapon.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(InfinityEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                    new CodeInstruction(OpCodes.Ldloca_S, aoe),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 178), // subtract 100 before release !!
                    new CodeInstruction(OpCodes.Ldc_I4_S, 178), // subtract 100 before release !!
                    new CodeInstruction(OpCodes.Call,
                        typeof(Rectangle).RequireMethod(nameof(Rectangle.Inflate), new[] { typeof(int), typeof(int) }))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding infinity club effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}