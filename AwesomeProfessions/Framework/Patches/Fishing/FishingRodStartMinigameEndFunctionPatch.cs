namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishingRodStartMinigameEndFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodStartMinigameEndFunctionPatch()
    {
        Original = RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
    }

    #region harmony patches

    /// <summary>Patch to remove Pirate bonus treasure chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishingRodStartMinigameEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

        try
        {
            helper // find index of pirate check
                .FindProfessionCheck(Farmer.pirate)
                .Retreat(2)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Add) // remove this check
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}