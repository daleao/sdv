namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestLogDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestLogDrawPatcher"/> class.</summary>
    internal QuestLogDrawPatcher()
    {
        this.Target = typeof(QuestLog).RequireMethod(nameof(QuestLog.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Draw Virtues quest objectives.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? QuestLogDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (_shownQuest != null && _shownQuest is SpecialOrder)
        // To: if (_shownQuest != null)
        //	   {
        //	       if (_shownQuest is VirtuesQuest virtues) virtues.DrawObjective(j, this, ref yPos, b);
        //		   else if (_shownQuest is SpecialOrder) { ... }
        //	   }

        try
        {
            var drawSpecialOrder = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(QuestLog).RequireField("_shownQuest")),
                        new CodeInstruction(OpCodes.Isinst, typeof(SpecialOrder)),
                    },
                    nth: 4)
                .AddLabels(drawSpecialOrder)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(QuestLog).RequireField("_shownQuest")),
                        new CodeInstruction(OpCodes.Isinst, typeof(HeroQuest)),
                        new CodeInstruction(OpCodes.Brfalse_S, drawSpecialOrder),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(QuestLog).RequireField("_shownQuest")),
                        new CodeInstruction(OpCodes.Isinst, typeof(HeroQuest)),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]), // local 8 = int j
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloca_S, helper.Locals[6]), // local 6 = int yPos
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Call,
                            typeof(HeroQuest).RequireMethod(nameof(HeroQuest.DrawObjective))),
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]) })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting draw call for Virtues quest.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
