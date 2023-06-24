namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tools.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
[IgnoreWithMod("bcmpinc.HarvestWithScythe")]
internal sealed class HoeDirtPerformUseActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtPerformUseActionPatcher"/> class.</summary>
    internal HoeDirtPerformUseActionPatcher()
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.performUseAction));
    }

    #region harmony patches

    /// <summary>Allow harvest with scythe + optimize this dumb method.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? HoeDirtPerformUseActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (Game1.player.CanSickleHarvest(this.crop) && this.readyForHarvest()) { goto sickle harvest }
        // Before: if ((int)crop.harvestMethod == 0 && crop.harvest( ...
        //      -- and also --
        // Removed: if ((int)crop.harvestMethod == 1) <- this check is completely useless
        //      -- and aso --
        // Replaced: if (crop.harvest( ... )) { ... }
        // With: if (!crop.harvest( ... )) return harvestable; <- early return
        try
        {
            var sickleHarvest = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Bne_Un) })
                .GetOperand(out var @return)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                    },
                    ILHelper.SearchOption.First)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(HoeDirt).RequirePropertyGetter(nameof(HoeDirt.crop))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.CanSickleHarvest))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(HoeDirt).RequireMethod(nameof(HoeDirt.readyForHarvest))),
                        new CodeInstruction(OpCodes.Brtrue_S, sickleHarvest),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue_S) })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .SetOperand(@return)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }, ILHelper.SearchOption.Previous)
                .GetLabels(out var labels)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Bne_Un) }, out var count)
                .Remove(count)
                .AddLabels(labels.Take(1).ToArray())
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertySetter(nameof(Farmer.CanMove))),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    },
                    ILHelper.SearchOption.Previous)
                .AddLabels(sickleHarvest);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding check for sickle harvest.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
