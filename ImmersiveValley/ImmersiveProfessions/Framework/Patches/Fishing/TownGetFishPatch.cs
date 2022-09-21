namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Harmony;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using StardewValley.Locations;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TownGetFishPatch : HarmonyPatch
{
    private const int AnglerIndex = 160;

    /// <summary>Initializes a new instance of the <see cref="TownGetFishPatch"/> class.</summary>
    internal TownGetFishPatch()
    {
        this.Target = this.RequireMethod<Town>(nameof(Town.getFish));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Angler to recatch Angler.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TownGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
        // To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...
        var checkSeason = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldc_I4, AnglerIndex))
                .AdvanceUntil(new CodeInstruction(OpCodes.Brtrue_S))
                .GetOperand(out var skipLegendary)
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Brfalse_S, checkSeason))
                .Advance()
                .AddLabels(checkSeason)
                .InsertInstructions(new CodeInstruction(OpCodes.Ldarg_S, (byte)4)) // arg 4 = Farmer who
                .InsertProfessionCheck(Profession.Angler.Value + 100, forLocalPlayer: false)
                .InsertInstructions(new CodeInstruction(OpCodes.Brfalse_S, skipLegendary));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Angler legendary fish recatch.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
