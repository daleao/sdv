namespace DaLion.Redux.Professions.Patches.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("blueberry.LoveOfCooking")]
[Deprecated]
internal sealed class ModEntryEvent_DrawRegenBarPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ModEntryEvent_DrawRegenBarPatch"/> class.</summary>
    internal ModEntryEvent_DrawRegenBarPatch()
    {
        this.Target = "LoveOfCooking.ModEntry"
            .ToType()
            .RequireMethod("Event_DrawRegenBar");
    }

    #region harmony patches

    /// <summary>Patch to displace food bar.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ModEntryEvent_DrawRegenBarTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Inject: if (Game1.player.get_Ultimate()?.Hud.IsVisible) topOfBar.X -= 56f;
        // Before: e.SpriteBatch.Draw( ... )
        try
        {
            var resumeExecution = ilGenerator.DefineLabel();
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldarg_2)) // arg 2 = RenderingHudEventArgs e
                .StripLabels(out var labels)
                .AddLabels(resumeExecution)
                .InsertWithLabels(
                    labels,
                    // check if Ultimate is null
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if Ultimate.Hud.IsVisible
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.Hud))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(UltimateHud).RequirePropertyGetter(nameof(UltimateHud.IsVisible))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // load and displace topOfBar.X
                    new CodeInstruction(OpCodes.Ldloca_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Vector2).RequireField(nameof(Vector2.X))),
                    new CodeInstruction(OpCodes.Ldc_R4, 56f), // displace by 56 pixels
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Stfld, typeof(Vector2).RequireField(nameof(Vector2.X))));
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
