namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Configs;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Shared.Extensions.Stardew;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    internal ObjectPlacementActionPatcher()
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

        // Injected: if (who is not null && who.professions.Contains(<demolitionist_id>) && ProfessionsModule.Config.ControlsUi.ModKey.IsDown()) skipIntensity ...
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
                            .Match(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                                    new CodeInstruction(
                                        OpCodes.Stfld,
                                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                                            .shakeIntensity))),
                                })
                            .AddLabels(resumeExecution)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                                })
                            .InsertProfessionCheck(Profession.Demolitionist.Value, forLocalPlayer: false)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(ProfessionConfig).RequirePropertyGetter(nameof(ProfessionConfig.ControlsUi))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(ControlsUiConfig).RequirePropertyGetter(
                                            nameof(ControlsUiConfig.ModKey))),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(KeybindList).RequireMethod(nameof(KeybindList.IsDown))),
                                    new CodeInstruction(OpCodes.Brtrue_S, skipIntensity),
                                })
                            .Match(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Ldloc_1),
                                    new CodeInstruction(
                                        OpCodes.Stfld,
                                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                                            .extraInfoForEndBehavior))),
                                })
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
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Newobj,
                            typeof(Tree).RequireConstructor(typeof(int), typeof(int))),
                    },
                    ILHelper.SearchOption.First)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPlacementActionPatcher).RequireMethod(nameof(RecordArboristIfNecessary))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Arborist record for Trees.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Newobj,
                            typeof(FruitTree).RequireConstructor(typeof(int))),
                    },
                    ILHelper.SearchOption.First)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPlacementActionPatcher).RequireMethod(nameof(RecordArboristIfNecessary))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Arborist record for Fruit Trees.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void RecordArboristIfNecessary(TerrainFeature feature, Farmer? planter)
    {
        if (planter?.HasProfession(Profession.Arborist) == true)
        {
            feature.Write(DataKeys.PlantedByArborist, true.ToString());
        }
    }

    #endregion injected subroutines
}
