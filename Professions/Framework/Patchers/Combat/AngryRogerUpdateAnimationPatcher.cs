namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal AngryRogerUpdateAnimationPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<AngryRoger>("updateAnimation", [typeof(GameTime)]);
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
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Character).RequireMethod(
                                nameof(Character.faceGeneralDirection),
                                [typeof(Vector2), typeof(int), typeof(bool)])),
                    ],
                    ILHelper.SearchOption.Last)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                    ],
                    ILHelper.SearchOption.Previous)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsAmbushing))),
                        new CodeInstruction(OpCodes.Brtrue_S, skip),
                    ],
                    labels)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Monster).RequireMethod("resetAnimationSpeed")),
                    ])
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
