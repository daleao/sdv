namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatcher"/> class.</summary>
    internal ObjectPerformObjectDropInActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
        this.Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>Patch to increase Artisan production + integrate Quality Artisan Products.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(
        SObject __instance, bool __state, Item dropInItem, bool probe, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is not an artisan machine, or if the machine is still empty after running the original method, then do nothing
        if (__state || !__instance.IsArtisanMachine() || __instance.heldObject.Value is not { } held ||
            dropInItem is not SObject dropIn || probe)
        {
            return;
        }

        var user = who;
        var owner = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : __instance.GetOwner();
        var r = new Random(Guid.NewGuid().GetHashCode());

        // artisan users can preserve the input quality
        if (user.HasProfession(Profession.Artisan))
        {
            // golden mayonnaise is always iridium quality
            held.Quality = __instance.ParentSheetIndex == BigCraftableIds.MayonnaiseMachine &&
                           dropIn.ParentSheetIndex == ObjectIds.GoldenEgg &&
                           !ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod")
                ? SObject.bestQuality
                : dropIn.Quality;
            if (!ProfessionsModule.Config.ArtisanGoodsAlwaysInputQuality)
            {
                if (r.NextDouble() > who.FarmingLevel / 30d)
                {
                    held.Quality = (int)((ObjectQuality)held.Quality).Decrement();
                    if (r.NextDouble() > who.FarmingLevel / 15d)
                    {
                        held.Quality = (int)((ObjectQuality)held.Quality).Decrement();
                    }
                }
            }
        }

        // artisan-owned machines work faster and may upgrade quality
        if (!owner.HasProfession(Profession.Artisan))
        {
            return;
        }

        if (held.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
        {
            held.Quality += held.Quality == SObject.highQuality ? 2 : 1;
        }

        if (owner.HasProfession(Profession.Artisan, true))
        {
            __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 4;
        }
        else
        {
            __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;
        }
    }

    /// <summary>Patch to reduce prestiged Breeder incubation time + open high-quality Gemologist geodes.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: SetGeodeTreasureQuality(this, geode_item, who);
        // Before: heldObject.Value = geode_item;
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SObject).RequireField(nameof(SObject.heldObject))),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[22]),
                        new CodeInstruction(OpCodes.Callvirt),
                    })
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[22]), // local 22 = SObject geode_item
                        new CodeInstruction(OpCodes.Ldarg_3), // arg 3 = Farmer who
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectPerformObjectDropInActionPatcher).RequireMethod(
                                nameof(SetGeodeTreasureQuality))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Gemologist geode quality.\nHelper returned {ex}");
            return null;
        }

        helper.GoTo(0);

        // From: minutesUntilReady.Value /= 2
        // To: minutesUntilReady.Value /= who.professions.Contains(100 + <breeder_id>) ? 3 : 2
        try
        {
            helper
                .Repeat(
                    3,
                    _ =>
                    {
                        var notPrestigedBreeder = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .MatchProfessionCheck(Profession.Breeder.Value)
                            .Match(new[] { new CodeInstruction(OpCodes.Ldloc_0) }, ILHelper.SearchOption.Previous)
                            .CountUntil(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, out var steps)
                            .Copy(
                                out var copy,
                                steps,
                                true,
                                true)
                            .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_2) })
                            .AddLabels(notPrestigedBreeder)
                            .Insert(copy)
                            .Move(-1)
                            .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_2) }, ILHelper.SearchOption.Previous)
                            .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Breeder.Value + 100))
                            .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                            .SetOperand(notPrestigedBreeder)
                            .Move()
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldc_I4_3),
                                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                                })
                            .Move()
                            .AddLabels(resumeExecution);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Breeder incubation bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void SetGeodeTreasureQuality(SObject crusher, SObject treasure, Farmer who)
    {
        if (who.HasProfession(Profession.Gemologist) && treasure.IsGemOrMineral() &&
            (crusher.IsOwnedBy(who) || ProfessionsModule.Config.LaxOwnershipRequirements))
        {
            treasure.Quality = who.GetGemologistMineralQuality();
        }
    }

    #endregion injected subroutines
}
