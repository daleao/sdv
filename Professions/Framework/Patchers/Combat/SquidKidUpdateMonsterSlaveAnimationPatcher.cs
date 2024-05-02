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
internal sealed class SquidKidUpdateMonsterSlaveAnimationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SquidKidUpdateMonsterSlaveAnimationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SquidKidUpdateMonsterSlaveAnimationPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SquidKid>("updateMonsterSlaveAnimation", [typeof(GameTime)]);
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher in ambush from Squid Kid gaze.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SquidKidUpdateMonsterSlaveAnimationTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: faceGeneralDirection(base.Player.Position);
        // To: if (!base.Player.IsInAmbush()) faceGeneralDirection(base.Player.Position);
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
                .PatternMatch([new CodeInstruction(OpCodes.Ret)])
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Squid Kid eye-stalking hidden Poachers.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
