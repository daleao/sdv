namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.LoveOfCooking;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class ModEntryEvent_DrawRegenBarPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ModEntryEvent_DrawRegenBarPatch()
    {
        try
        {
            Original = "LoveOfCooking.ModEntry".ToType()
                .MethodNamed("Event_DrawRegenBar");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for Propagator output quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Inject: if (ModEntry.PlayerState.Value.SuperMode?.Gauge.IsVisible) topOfBar.X -= 56f;
        /// Before: e.SpriteBatch.Draw( ... )

        var resumeExecution = ilGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Nop),
                    new CodeInstruction(OpCodes.Ldloca_S, $"{typeof(Vector2)} (7)")
                )
                .Advance()
                .ToBuffer(3)
                .FindFirst(
                    new CodeInstruction(OpCodes.Nop),
                    new CodeInstruction(OpCodes.Ldarg_2)
                )
                .Advance()
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .Insert(
                    labels,
                    // check if SuperMode is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.SuperMode))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if SuperMode.Gauge.IsVisible
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode).PropertyGetter(nameof(SuperMode.Gauge))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.IsVisible))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution)
                )
                .InsertBuffer() // loads topOfBar.X
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R4, 56f),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Stfld, typeof(Vector2).Field(nameof(Vector2.X)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Love Of Cooking's food regen bar.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}