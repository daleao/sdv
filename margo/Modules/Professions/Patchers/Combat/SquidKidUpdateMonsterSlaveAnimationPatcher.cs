namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
    internal SquidKidUpdateMonsterSlaveAnimationPatcher()
    {
        this.Target = this.RequireMethod<SquidKid>("updateMonsterSlaveAnimation", new[] { typeof(GameTime) });
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
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Character).RequireMethod(
                                nameof(Character.faceGeneralDirection),
                                new[] { typeof(Vector2), typeof(int), typeof(bool) })),
                    },
                    ILHelper.SearchOption.Last)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                    },
                    ILHelper.SearchOption.Previous)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsInAmbush))),
                        new CodeInstruction(OpCodes.Brtrue_S, skip),
                    },
                    labels)
                .Match(new[] { new CodeInstruction(OpCodes.Ret) })
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
