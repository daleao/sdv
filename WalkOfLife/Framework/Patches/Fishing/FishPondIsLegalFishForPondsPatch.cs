using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class FishPondIsLegalFishForPondsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondIsLegalFishForPondsPatch()
    {
        Original = RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishPondIsLegalFishForPondsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (fish_item.HasContextTag("fish_legendary")) ...
        /// To: if (fish_item.HasContextTag("fish_legendary") && !Game1.player.HasPrestigedProfession("Aquarist"))

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, "fish_legendary")
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var notLegal)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldstr, "Aquarist"),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.HasPrestigedProfession))),
                    new CodeInstruction(OpCodes.Brtrue_S, notLegal)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding prestiged Aquarist permission to raise legendary fish.\nHelper returned {ex}", LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}