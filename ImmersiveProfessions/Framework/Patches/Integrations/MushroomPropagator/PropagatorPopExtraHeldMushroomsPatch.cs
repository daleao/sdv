namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class PropagatorPopExtraHeldMushroomsPatch : BasePatch
{
    private static readonly FieldInfo _SourceMushroomQuality =
        "BlueberryMushroomMachine.Propagator".ToType().RequireField("SourceMushroomQuality")!;

    /// <summary>Construct an instance.</summary>
    internal PropagatorPopExtraHeldMushroomsPatch()
    {
        try
        {
            Original = "BlueberryMushroomMachine.Propagator".ToType().RequireMethod("PopExtraHeldMushrooms");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for Propagator forage increment.</summary>
    [HarmonyPostfix]
    private static void PropagatorPopExtraHeldMushroomsPostfix(SObject __instance)
    {
        if (__instance is null) return;

        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.IsLocalPlayer || !owner.HasProfession(Profession.Ecologist)) return;

        Game1.player.IncrementData<uint>(DataField.EcologistItemsForaged);
    }

    /// <summary>Patch for Propagator output quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int popQuality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : SourceMushroomQuality);
        /// To: int popQuality = PopExtraHeldMushroomsSubroutine(this);

        try
        {
            helper
                .FindProfessionCheck((int) Profession.Ecologist) // find index of ecologist check
                .Retreat()
                .GetLabels(out var labels)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(PropagatorPopExtraHeldMushroomsPatch).RequireMethod(nameof(PopExtraHeldMushroomsSubroutine)))
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Blueberry's Mushroom Propagator output quality.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int PopExtraHeldMushroomsSubroutine(SObject propagator)
    {
        var owner = Game1.getFarmerMaybeOffline(propagator.owner.Value) ?? Game1.MasterPlayer;
        if (owner.IsLocalPlayer && owner.HasProfession(Profession.Ecologist)) return owner.GetEcologistForageQuality();

        var sourceMushroomQuality = (int) _SourceMushroomQuality.GetValue(propagator)!;
        return sourceMushroomQuality;
    }

    #endregion injected subroutines
}