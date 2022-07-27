namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.LoveOfCooking;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, RequiresMod("blueberry.LoveOfCooking"), Deprecated]
internal sealed class ModEntryEvent_DrawRegenBarPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ModEntryEvent_DrawRegenBarPatch()
    {
        Target = "LoveOfCooking.ModEntry".ToType().RequireMethod("Event_DrawRegenBar");
    }

    #region harmony patches

    /// <summary>Patch to displace food bar.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ModEntryEvent_DrawRegenBarTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Inject: if (Game1.player.get_Ultimate()?.Hud.IsVisible) topOfBar.X -= 56f;
        /// Before: e.SpriteBatch.Draw( ... )

        var resumeExecution = ilGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_2) // arg 2 = RenderingHudEventArgs e
                )
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .InsertWithLabels(
                    labels,
                    // check if Ultimate is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if Ultimate.Hud.IsVisible
                    new CodeInstruction(OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.get_Ultimate))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.Hud))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(UltimateHUD).RequirePropertyGetter(nameof(UltimateHUD.IsVisible))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // load and displace topOfBar.X
                    new CodeInstruction(OpCodes.Ldloca_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Vector2).RequireField(nameof(Vector2.X))),
                    new CodeInstruction(OpCodes.Ldc_R4, 56f), // displace by 56 pixels
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Stfld, typeof(Vector2).RequireField(nameof(Vector2.X)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Love Of Cooking's food regen bar.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}