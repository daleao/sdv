using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches.Mining;

[UsedImplicitly]
internal class GeodeMenuUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GeodeMenuUpdatePatch()
    {
        Original = RequireMethod<GeodeMenu>(nameof(GeodeMenu.update));
    }

    #region harmony patches

    /// <summary>Patch to increment Gemologist counter for geodes cracked at Clint's.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GeodeMenuUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
        ///		Data.IncrementField<uint>("MineralsCollected")
        ///	After: Game1.stats.GeodesCracked++;

        var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).PropertySetter(nameof(Stats.GeodesCracked)))
                )
                .Advance()
                .InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Gemologist"),
                    dontIncreaseGemologistCounter)
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModData>).PropertyGetter(nameof(PerScreen<ModData>.Value))),
                    new CodeInstruction(OpCodes.Ldstr, "MineralsCollected"),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModData).MethodNamed(nameof(ModData.Increment), new[] { typeof(string) })
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseGemologistCounter);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding Gemologist counter increment.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}