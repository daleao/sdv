namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationCheckActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.checkAction));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Forager double forage.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationCheckActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (random.NextDouble() < 0.2)
        // To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.gatherer)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R8, 0.2)])
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)])
                .InsertProfessionCheck(Farmer.gatherer + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Forager double forage bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
