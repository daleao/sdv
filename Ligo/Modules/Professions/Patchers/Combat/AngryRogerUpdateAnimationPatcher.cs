namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class AngryRogerUpdateAnimationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AngryRogerUpdateAnimationPatcher"/> class.</summary>
    internal AngryRogerUpdateAnimationPatcher()
    {
        this.Target = this.RequireMethod<AngryRoger>("updateAnimation", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher in ambush from Angry Roger gaze.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? AngryRogerUpdateAnimationTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: faceGeneralDirection(base.Player.getStandingPosition());
        // To: if (!base.Player.IsInAmbush()) faceGeneralDirection(base.Player.getStandingPosition());
        try
        {
            var skip = generator.DefineLabel();
            helper
                .FindLast(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Character).RequireMethod(
                            nameof(Character.faceGeneralDirection),
                            new[] { typeof(Vector2), typeof(int), typeof(bool) })))
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0))
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsInAmbush))),
                    new CodeInstruction(OpCodes.Brtrue_S, skip))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Monster).RequireMethod("resetAnimationSpeed")))
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Angry Roger eye-stalking hidden Poachers.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
