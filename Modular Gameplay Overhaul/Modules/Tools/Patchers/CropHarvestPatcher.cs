namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHarvestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropHarvestPatcher"/> class.</summary>
    internal CropHarvestPatcher()
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.harvest));
        this.Transpiler!.priority = Priority.Low;
    }

    #region harmony patches

    /// <summary>Proper spring onion harvest with scythe.</summary>
    [HarmonyTranspiler]
    [HarmonyPriority(Priority.Low)]
    private static IEnumerable<CodeInstruction>? CropHarvestTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Game1.player.addItemToInventoryBool(o)) { ... }
        // To: if (this.harvestMethod.Value == 1) { Game1.createItemDebris(...); goto check if regrowAfterHarvest }
        //     else if (Game1.player.addItemToInventoryBool(o)) { ... }
        try
        {
            var doGrabHarvest = generator.DefineLabel();
            var checkRegrowthAfterHarvest = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldloc_1), new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequireMethod(nameof(Farmer.addItemToInventoryBool))),
                    })
                .StripLabels(out var labels)
                .AddLabels(doGrabHarvest)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Crop).RequireField(nameof(Crop.harvestMethod))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                        new CodeInstruction(OpCodes.Brfalse_S, doGrabHarvest),
                        new CodeInstruction(OpCodes.Ldloc_1),

                        // create origin vector
                        new CodeInstruction(OpCodes.Ldarg_1), // xTile
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 64f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Ldarg_2), // yTile
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 64f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Newobj, typeof(Vector2).RequireConstructor(typeof(float), typeof(float))),

                        new CodeInstruction(OpCodes.Ldc_I4_M1),
                        new CodeInstruction(OpCodes.Ldnull),
                        new CodeInstruction(OpCodes.Ldc_I4_M1),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequireMethod(nameof(Game1.createItemDebris))),
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Br_S, checkRegrowthAfterHarvest),
                    },
                    labels)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Crop).RequireField(nameof(Crop.regrowAfterHarvest))),
                    })
                .AddLabels(checkRegrowthAfterHarvest);
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
