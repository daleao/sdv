namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1ApplySaveFixPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1ApplySaveFixPatch()
    {
        Target = RequireMethod<Game1>(nameof(Game1.applySaveFix));
    }

    #region harmony patches

    /// <summary>Replace with custom Qi Challenge.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1ApplySaveFixTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (farmer.mailReceived.Contains("skullCave") && !farmer.hasQuest(20) && !farmer.hasOrWillReceiveMail("QiChallengeComplete"))
        /// To: if (farmer.mailReceived.Contains("skullCave")) CheckForMissingQiChallenges(farmer)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, "skullCave")
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[23]),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Game1ApplySaveFixPatch).RequireMethod(nameof(CheckForMissingQiChallenges)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding failsafe for custom Qi Challenges.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CheckForMissingQiChallenges(Farmer farmer)
    {
        if (farmer.hasOrWillReceiveMail("QiChallengeComplete")) return;

        if (ModEntry.Config.TrulyLegendaryGalaxySword)
        {
            if (!farmer.hasQuest(20) && !farmer.hasOrWillReceiveMail("QiChallengeFirst"))
                farmer.addQuest(20);
            else if (farmer.hasOrWillReceiveMail("QiChallengeFirst") &&
                     !farmer.hasQuest(ModEntry.QiChallengeFinalQuestId))
                farmer.addQuest(ModEntry.QiChallengeFinalQuestId);
        }
        else
        {
            if (!farmer.hasQuest(20)) farmer.addQuest(20);
        }
    }

    #endregion injected subroutines
}