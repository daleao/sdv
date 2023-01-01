namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowToolSwipeEffectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowToolSwipeEffectPatcher"/> class.</summary>
    internal FarmerShowToolSwipeEffectPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showToolSwipeEffect));
    }

    #region harmony patches

    /// <summary>Infinity-colored overhead swipe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerShowToolSwipeEffectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.White))),
                    },
                    () =>
                    {
                        var useWhiteColor = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .AddLabels(useWhiteColor)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                                    new CodeInstruction(OpCodes.Brfalse_S, useWhiteColor),
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(MeleeWeaponExtensions).RequireMethod(
                                            nameof(MeleeWeaponExtensions.IsInfinityWeapon))),
                                    new CodeInstruction(OpCodes.Brfalse_S, useWhiteColor),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(Color).RequirePropertyGetter(nameof(Color.HotPink))),
                                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                                })
                            .Move()
                            .AddLabels(resumeExecution);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed coloring Infinity overhead swipe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
