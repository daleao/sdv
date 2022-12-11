namespace DaLion.Ligo.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeShakePatcher"/> class.</summary>
    internal TreeShakePatcher()
    {
        this.Target = this.RequireMethod<Tree>("shake");
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to coconuts from shaken trees.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreeShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, 0, 1f, location);
        // To: Game1.createObjectDebris(seedIndex, tileLocation.X, tileLocation.Y - 3, (tileLocation.Y + 1) * 64, GetCoconutQuality(), 1f, location);
        //     -- and again for golden coconut immediately below
        try
        {
            var callCreateObjectDebrisInst = new CodeInstruction(
                OpCodes.Call,
                typeof(Game1).RequireMethod(
                    nameof(Game1.createObjectDebris),
                    new[]
                    {
                        typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float),
                        typeof(GameLocation),
                    }));

            helper
                // the normal coconut
                .Match(new[] { callCreateObjectDebrisInst })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_0) }, ILHelper.SearchOption.Previous)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(TreeShakePatcher).RequireMethod(nameof(GetCoconutQuality))))
                .Insert(new[] { new CodeInstruction(OpCodes.Ldloc_2) })
                // the golden coconut
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 791) })
                .Match(new[] { callCreateObjectDebrisInst })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_0) }, ILHelper.SearchOption.Previous)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(TreeShakePatcher).RequireMethod(nameof(GetCoconutQuality))))
                .Insert(new[] { new CodeInstruction(OpCodes.Ldc_I4, 791) });
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying Ecologist/Botanist perk to shaken coconut.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetCoconutQuality(int seedIndex)
    {
        return seedIndex is not (Constants.CoconutIndex or Constants.GoldenCoconutIndex) ||
               !Game1.player.HasProfession(Profession.Ecologist)
            ? SObject.lowQuality
            : Game1.player.GetEcologistForageQuality();
    }

    #endregion injected subroutines
}
