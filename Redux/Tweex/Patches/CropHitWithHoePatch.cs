namespace DaLion.Redux.Tweex.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
        if (!ModEntry.Config.Tweex.ProfessionalForagingInGingerIsland || !Game1.player.professions.Contains(Farmer.botanist))
        {
            return ginger;
        }

        ginger.Quality = ModEntry.Config.EnableProfessions
            ? Game1.player.GetEcologistForageQuality()
            : SObject.bestQuality;
        return ginger;
    }

    #endregion injected subroutine
}
