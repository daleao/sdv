namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

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
        ///		Data.IncrementField<uint>("GemologistMineralsCollected")
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
                .InsertProfessionCheckForLocalPlayer((int) Profession.Gemologist,
                    dontIncreaseGemologistCounter)
                .Insert(
                    new CodeInstruction(OpCodes.Ldstr, DataField.GemologistMineralsCollected.ToString()),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModData)
                            .MethodNamed(nameof(ModData.Increment), new[] {typeof(DataField), typeof(Farmer)})
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseGemologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Gemologist counter increment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}