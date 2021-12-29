using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Monsters;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class GreenSlimeOnDealContactDamagePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeOnDealContactDamagePatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.onDealContactDamage));
    }

    #region harmony patches

    /// <summary>Patch to make Piper immune to slimed debuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GreenSlimeOnDealContactDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<piper_id>)) return;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Bge_Un_S) // find index of first branch instruction
                )
                .GetOperand(out var returnLabel) // get return label
                .Return()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack(Utility.Professions.IndexOf("Piper"), (Label) returnLabel,
                    true);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding Piper slime debuff immunity.\nHelper returned {ex}", LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}