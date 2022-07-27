namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.Automate;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class GenericObjectMachinePatches : DaLion.Common.Harmony.HarmonyPatch
{
    private static string? _target;

    /// <summary>Construct an instance.</summary>
    internal GenericObjectMachinePatches()
    {
        Transpiler!.after = new[] { "DaLion.ImmersiveTweaks" };
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        foreach (var target in TargetMethods())
        {
            Target = target;
            _target = target.Name;
            base.ApplyImpl(harmony);
        }
    }

    #region harmony patches

    /// <summary>Patch to apply Artisan effects to automated generic machines.</summary>
    [HarmonyTranspiler]
    [HarmonyAfter("DaLion.ImmersiveTweaks")]
    private static IEnumerable<CodeInstruction>? GenericObjectMachineGenericPullRecipeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: ApplyArtisanPerks(this.Machine, this.Location, consumable.Sample)
        /// Before: return true;

        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine`1".ToType().MakeGenericType(typeof(SObject))
                            .RequirePropertyGetter("Machine")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine".ToType()
                            .RequirePropertyGetter("Location")),
                    new CodeInstruction(_target!.Contains("Loom") ? OpCodes.Ldloc_1 : OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        "Pathoschild.Stardew.Automate.IConsumable".ToType().RequirePropertyGetter("Sample")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GenericObjectMachinePatches).RequireMethod(_target!.Contains("GeodeCrusher")
                            ? nameof(ApplyGemologistPerks)
                            : nameof(ApplyArtisanPerks)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Artisan behavior for generic Automate machines.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1".ToType()
            .MakeGenericType(typeof(SObject))
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .First(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3);
        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType()
            .RequireMethod("SetInput");
        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine".ToType()
            .RequireMethod("SetInput");
        yield return "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType()
            .RequireMethod("SetInput");
    }

    private static void ApplyArtisanPerks(SObject machine, GameLocation location, Item sample)
    {
        if (!machine.IsArtisanMachine() || !machine.heldObject.Value.IsArtisanGood() ||
            sample is not SObject input) return;

        var output = machine.heldObject.Value;
        var chest = ExtendedAutomateAPI.GetClosestContainerTo(machine, location);
        var user = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : chest.GetOwner();
        if (user.HasProfession(Profession.Artisan) || ModEntry.Config.LaxOwnershipRequirements &&
            Game1.game1.DoesAnyPlayerHaveProfession(Profession.Artisan, out _)) output.Quality = input.Quality;

        var owner = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : machine.GetOwner();
        if (!owner.HasProfession(Profession.Artisan)) return;

        if (output.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

        if (owner.HasProfession(Profession.Artisan, true))
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 4;
        else
            machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;

        if (machine.name == "Mayonnaise Machine" && input.ParentSheetIndex == 928 &&
            !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod"))
            output.Quality = SObject.bestQuality;
    }

    private static void ApplyGemologistPerks(SObject machine, GameLocation location, Item sample)
    {
        var output = machine.heldObject.Value;
        var chest = ExtendedAutomateAPI.GetClosestContainerTo(machine, location);
        var user = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : chest.GetOwner();
        if (!(user.HasProfession(Profession.Gemologist) || ModEntry.Config.LaxOwnershipRequirements &&
                Game1.game1.DoesAnyPlayerHaveProfession(Profession.Artisan, out _)) ||
            !output.IsForagedMineral() && !output.IsGemOrMineral()) return;

        output.Quality = user.GetGemologistMineralQuality();
    }

    #endregion private methods
}