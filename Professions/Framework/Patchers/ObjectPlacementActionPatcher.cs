namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPlacementActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ObjectPlacementActionPrefix(SObject __instance, int x, int y, Farmer? who)
    {
        if (__instance.ItemId != SurveyFlagId)
        {
            return true; // run original logic
        }

        if (State.SpelunkerFlag is not null)
        {
            Game1.showRedMessage(I18n.Objects_Surveyflag_Cant_Double());

            return false; // don't run original logic
        }

        if (!(who?.HasProfession(Profession.Spelunker, true) ?? false))
        {
            Game1.showRedMessage(I18n.Objects_Surveyflag_Cant_Player());
            return false; // don't run original logic
        }

        if (who.currentLocation is not MineShaft shaft)
        {
            Game1.showRedMessage(I18n.Objects_Surveyflag_Cant_Here());
            return false; // don't run original logic
        }

        State.SpelunkerFlag = __instance;
        State.SpelunkerFlagLevel = shaft.mineLevel;
        return true; // don't run original logic
    }

    /// <summary>Patch to prevent quantum bombs when detonating manually + record Arborist-planted trees.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who is not null && who.professions.Contains(<demolitionist_id>) && Config.ModKey.IsDown()) skipIntensity ...
        // After: new TemporaryAnimatedSprite( ... )
        //try
        //{
        //    helper
        //        .Repeat(
        //            3,
        //            _ =>
        //            {
        //                var skipIntensity = generator.DefineLabel();
        //                var resumeExecution = generator.DefineLabel();
        //                helper
        //                    .PatternMatch(
        //                        [
        //                            new CodeInstruction(OpCodes.Dup),
        //                            new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
        //                            new CodeInstruction(
        //                                OpCodes.Stfld,
        //                                typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
        //                                    .shakeIntensity))),
        //                        ])
        //                    .AddLabels(resumeExecution)
        //                    .Insert(
        //                        [
        //                            new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
        //                            new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
        //                            new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
        //                        ])
        //                    .InsertProfessionCheck(Farmer.excavator, forLocalPlayer: false)
        //                    .Insert(
        //                        [
        //                            new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
        //                            new CodeInstruction(
        //                                OpCodes.Call,
        //                                typeof(ProfessionsMod).RequirePropertyGetter(nameof(Config))),
        //                            new CodeInstruction(
        //                                OpCodes.Call,
        //                                typeof(ProfessionsConfig).RequirePropertyGetter(nameof(ProfessionsConfig.ModKey))),
        //                            new CodeInstruction(
        //                                OpCodes.Call,
        //                                typeof(KeybindList).RequireMethod(nameof(KeybindList.IsDown))),
        //                            new CodeInstruction(OpCodes.Brtrue_S, skipIntensity),
        //                        ])
        //                    .PatternMatch(
        //                        [
        //                            new CodeInstruction(OpCodes.Dup),
        //                            new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]), // local 11 = int idNum
        //                            new CodeInstruction(
        //                                OpCodes.Stfld,
        //                                typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
        //                                    .extraInfoForEndBehavior))),
        //                        ])
        //                    .AddLabels(skipIntensity);
        //            });
        //}
        //catch (Exception ex)
        //{
        //    Log.E($"Failed injecting intensity skip for manually-detonated bombs.\nHelper returned {ex}");
        //    return null;
        //}

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

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void RecordTreeData(TerrainFeature feature, Farmer? planter)
    {
        var date = Game1.game1.GetCurrentDateNumber();
        Data.Write(feature, DataKeys.TreeDatePlanted, date.ToString());
        if (planter?.HasProfession(Profession.Arborist) ?? false)
        {
            Data.Write(feature, DataKeys.PlantedByArborist, true.ToString());
        }
    }

    #endregion injected
}
