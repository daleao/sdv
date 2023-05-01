namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodStartMinigameEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodStartMinigameEndFunctionPatcher"/> class.</summary>
    internal FishingRodStartMinigameEndFunctionPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
    }

    #region harmony patches

    /// <summary>Patch to remove Pirate bonus treasure chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodStartMinigameEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...
        try
        {
            helper // find index of pirate check
                .MatchProfessionCheck(Farmer.pirate)
                .Move(-2)
                .Count(new[] { new CodeInstruction(OpCodes.Add) }, out var count)
                .Remove(count); // remove this check
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
