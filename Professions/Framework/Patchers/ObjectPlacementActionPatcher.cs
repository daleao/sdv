namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectPlacementActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    /// <summary>Patch to prevent quantum bombs when detonating manually + record Arborist-planted trees.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who is not null && who.professions.Contains(<demolitionist_id>) && Config.ModKey.IsDown()) skipIntensity ...
        // After: new TemporaryAnimatedSprite( ... )
        try
        {
            helper
                .Repeat(
                    3,
                    _ =>
                    {
                        var skipIntensity = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .PatternMatch(
                                [
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                                    new CodeInstruction(
                                        OpCodes.Stfld,
                                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                                            .shakeIntensity))),
                                ])
                            .AddLabels(resumeExecution)
                            .Insert(
                                [
                                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                                ])
                            .InsertProfessionCheck(Farmer.excavator, forLocalPlayer: false)
                            .Insert(
                                [
                                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(Config))),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ProfessionsConfig).RequirePropertyGetter(nameof(ProfessionsConfig.ModKey))),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(KeybindList).RequireMethod(nameof(KeybindList.IsDown))),
                                    new CodeInstruction(OpCodes.Brtrue_S, skipIntensity),
                                ])
                            .PatternMatch(
                                [
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]), // local 11 = int idNum
                                    new CodeInstruction(
                                        OpCodes.Stfld,
                                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                                            .extraInfoForEndBehavior))),
                                ])
                            .AddLabels(skipIntensity);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting intensity skip for manually-detonated bombs.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Newobj,
                            typeof(Tree).RequireConstructor(typeof(string), typeof(int), typeof(bool))),
                    ],
                    ILHelper.SearchOption.First)
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPlacementActionPatcher).RequireMethod(nameof(RecordTreeData))),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Arborist record for Trees.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Newobj,
                            typeof(FruitTree).RequireConstructor(typeof(string), typeof(int))),
                    ],
                    ILHelper.SearchOption.First)
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPlacementActionPatcher).RequireMethod(nameof(RecordTreeData))),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Arborist record for Fruit Trees.\nHelper returned {ex}");
            return null;
        }

        // From: else return false;
        // To: else return TryFertilizeFruitTree(location, placementTile);
        // After: return tree2.fertilize();
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tree).RequireMethod(nameof(Tree.fertilize))),
                    ],
                    ILHelper.SearchOption.First)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_0)])
                .StripLabels(out var labels)
                .Remove()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPlacementActionPatcher).RequireMethod(nameof(TryFertilizeFruitTree))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Fruit Tree fertilization.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void RecordTreeData(TerrainFeature feature, Farmer? planter)
    {
        var date = Game1.game1.GetCurrentDateNumber();
        Data.Write(feature, DataKeys.DatePlanted, date.ToString());
        if (planter?.HasProfession(Profession.Arborist) == true)
        {
            Data.Write(feature, DataKeys.PlantedByArborist, true.ToString());
        }
    }

    private static bool TryFertilizeFruitTree(GameLocation location, Vector2 placementTile)
    {
        if (!location.terrainFeatures.TryGetValue(placementTile, out var feature) || feature is not FruitTree)
        {
            return false;
        }

        Data.Write(feature, DataKeys.Fertilized, true.ToString());
        return true;
    }

    #endregion injections
}
