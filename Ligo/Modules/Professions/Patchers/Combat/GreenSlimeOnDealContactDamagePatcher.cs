namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeOnDealContactDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeOnDealContactDamagePatcher"/> class.</summary>
    internal GreenSlimeOnDealContactDamagePatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.onDealContactDamage));
    }

    #region harmony patches

    /// <summary>Patch to make Piper immune to slimed debuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GreenSlimeOnDealContactDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who.professions.Contains(<piper_id>) && !who.professions.Contains(100 + <piper_id>)) return;
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(new CodeInstruction(OpCodes.Bge_Un_S)) // find index of first branch instruction
                .GetOperand(out var returnLabel) // get return label
                .Return()
                .AddLabels(resumeExecution)
                .InsertInstructions(new CodeInstruction(OpCodes.Ldarg_1)) // arg 1 = Farmer who
                .InsertProfessionCheck(Profession.Piper.Value, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_1)) // arg 1 = Farmer who
                .InsertProfessionCheck(Profession.Piper.Value + 100, forLocalPlayer: false)
                .InsertInstructions(new CodeInstruction(OpCodes.Brfalse_S, returnLabel));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Piper slime debuff immunity.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
