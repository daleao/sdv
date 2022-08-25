namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class SquidKidUpdateMonsterSlaveAnimationPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SquidKidUpdateMonsterSlaveAnimationPatch()
    {
        Target = RequireMethod<SquidKid>("updateMonsterSlaveAnimation", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher in ambush from Squid Kid gaze.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SquidKidUpdateMonsterSlaveAnimationTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: faceGeneralDirection(base.Player.Position);
        /// To: if (!base.Player.IsInAmbush()) faceGeneralDirection(base.Player.Position);

        var skip = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Character).RequireMethod(nameof(Character.faceGeneralDirection),
                            new[] { typeof(Vector2), typeof(int), typeof(bool) }))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsInAmbush))),
                    new CodeInstruction(OpCodes.Brtrue_S, skip)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ret)
                )
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Squid Kid eye-stalking hidden Poachers.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}