namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class PropagatorPopExtraHeldMushroomsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal PropagatorPopExtraHeldMushroomsPatch()
    {
        try
        {
            Original = "BlueberryMushroomMachine.Propagator".ToType()
                .MethodNamed("PopExtraHeldMushrooms");
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

        var who = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (!who.IsLocalPlayer || !who.HasProfession(Profession.Ecologist)) return;

        ModData.Increment<uint>(DataField.EcologistItemsForaged);
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
                .Insert(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(PropagatorPopExtraHeldMushroomsPatch).MethodNamed(
                            nameof(PopExtraHeldMushroomsSubroutine)))
                )
                .StripLabels();
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
        var who = Game1.getFarmerMaybeOffline(propagator.owner.Value) ?? Game1.MasterPlayer;
        if (who.IsLocalPlayer && who.HasProfession(Profession.Ecologist)) return who.GetEcologistForageQuality();

        var sourceMushroomQuality =
            ModEntry.ModHelper.Reflection.GetField<int>(propagator, "SourceMushroomQuality").GetValue();
        return sourceMushroomQuality;
    }

    #endregion injected subroutines
}