namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class PropagatorPopExtraHeldMushroomsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<SObject, int>? _GetSourceMushroomQuality;

    /// <summary>Construct an instance.</summary>
    internal PropagatorPopExtraHeldMushroomsPatch()
    {
        try
        {
            Target = "BlueberryMushroomMachine.Propagator".ToType().RequireMethod("PopExtraHeldMushrooms");
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
        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.IsLocalPlayer || !owner.HasProfession(Profession.Ecologist)) return;

        ModDataIO.IncrementData<uint>(Game1.player, ModData.EcologistItemsForaged.ToString());
    }

    /// <summary>Patch for Propagator output quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int popQuality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : SourceMushroomQuality);
        /// To: int popQuality = PopExtraHeldMushroomsSubroutine(this);

        try
        {
            helper
                .FindProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .Retreat()
                .GetLabels(out var labels)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(PropagatorPopExtraHeldMushroomsPatch).RequireMethod(
                            nameof(PopExtraHeldMushroomsSubroutine)))
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Blueberry's Mushroom Propagator output quality.\nHelper returned {ex}");
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

        _GetSourceMushroomQuality ??= "BlueberryMushroomMachine.Propagator".ToType()
            .RequireField("SourceMushroomQuality")
            .CompileUnboundFieldGetterDelegate<Func<SObject, int>>();
        var sourceMushroomQuality = _GetSourceMushroomQuality(propagator);
        return sourceMushroomQuality;
    }

    #endregion injected subroutines
}