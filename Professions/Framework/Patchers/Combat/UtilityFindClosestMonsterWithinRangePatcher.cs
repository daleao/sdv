namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityFindClosestMonsterWithinRangePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityFindClosestMonsterWithinRangePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal UtilityFindClosestMonsterWithinRangePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.findClosestMonsterWithinRange));
    }

    #region harmony patches

    /// <summary>Patch to make ally Slimes immune to trinkets.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? SlingshotDrawAttachmentsPrefix(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var slime = generator.DeclareLocal(typeof(GreenSlime));
            var nextCheck = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Isinst, typeof(Monster))])
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)])
                .GetOperand(out var continueLoop)
                .Move()
                .AddLabels(nextCheck)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]),
                    new CodeInstruction(OpCodes.Isinst, typeof(GreenSlime)),
                    new CodeInstruction(OpCodes.Stloc_S, slime),
                    new CodeInstruction(OpCodes.Ldloc_S, slime),
                    new CodeInstruction(OpCodes.Brfalse_S, nextCheck),
                    new CodeInstruction(OpCodes.Ldloc_S, slime),
                    new CodeInstruction(OpCodes.Call, typeof(GreenSlime_Piped).RequireMethod(nameof(GreenSlime_Piped.IsPiped))),
                    new CodeInstruction(OpCodes.Brtrue_S, continueLoop)
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Piped Slime exception.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
