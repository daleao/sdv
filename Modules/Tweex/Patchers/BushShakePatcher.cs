namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class BushShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushShakePatcher"/> class.</summary>
    internal BushShakePatcher()
    {
        this.Target = this.RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Detects if the bush is ready for harvest.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static void BushShakePrefix(Bush __instance, ref bool __state)
    {
        if (TweexModule.Config.BerryBushExpReward > 0 && __instance.tileSheetOffset.Value == 1 &&
            !__instance.townBush.Value &&
            __instance.inBloom(Game1.GetSeasonForLocation(__instance.currentLocation), Game1.dayOfMonth) &&
            __instance.size.Value < Bush.greenTeaBush)
        {
            __state = true;
        }
    }

    /// <summary>Adds foraging experience if the bush was harvested.</summary>
    [HarmonyPostfix]
    private static void BushShakePostfix(Bush __instance, bool __state)
    {
        if (__state && __instance.tileSheetOffset.Value == 0)
        {
            Game1.player.gainExperience(Farmer.foragingSkill, (int)TweexModule.Config.BerryBushExpReward);
        }
    }

    /// <summary>Adds quality tea bushes.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1).RequireMethod(
                                nameof(Game1.createObjectDebris),
                                new[]
                                {
                                    typeof(int),
                                    typeof(int),
                                    typeof(int),
                                    typeof(int),
                                    typeof(int),
                                    typeof(float),
                                    typeof(GameLocation),
                                })),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_0) }, ILHelper.SearchOption.Previous)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .ReplaceWith(new CodeInstruction(
                    OpCodes.Call,
                    typeof(BushExtensions).RequireMethod(nameof(BushExtensions.GetQualityFromAge))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed to add quality tea leaves.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
