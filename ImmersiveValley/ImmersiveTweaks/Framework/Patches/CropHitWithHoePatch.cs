namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHitWithHoePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CropHitWithHoePatch"/> class.</summary>
    internal CropHitWithHoePatch()
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.hitWithHoe));
    }

    #region harmony patches

    /// <summary>Apply Botanist/Ecologist perk to wild ginger.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CropHitWithHoeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: SetGingerQuality(obj);
        // Between: obj = new SObject(829, 1);
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Stloc_0))
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(CropHitWithHoePatch).RequireMethod(nameof(AddGingerQuality))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while apply Ecologist/Botanist perk to hoed ginger.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutine

    private static SObject AddGingerQuality(SObject ginger)
    {
        if (!ModEntry.Config.ProfessionalForagingInGingerIsland || !Game1.player.professions.Contains(Farmer.botanist))
        {
            return ginger;
        }

        ginger.Quality = ModEntry.ProfessionsApi is null
            ? SObject.bestQuality
            : ModEntry.ProfessionsApi.GetEcologistForageQuality(Game1.player);
        return ginger;
    }

    #endregion injected subroutine
}
