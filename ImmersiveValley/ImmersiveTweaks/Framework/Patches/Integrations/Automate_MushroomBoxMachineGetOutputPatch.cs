namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Attributes;
using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class MushroomBoxMachineGetOutputPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal MushroomBoxMachineGetOutputPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine".ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality.</summary>
    [HarmonyPrefix]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (!ModEntry.Config.AgeImprovesMushroomBoxes) return;

            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
                .CompileUnboundDelegate<Func<object, SObject>>();
            var machine = _GetMachine(__instance);
            if (machine.heldObject.Value is not { } held) return;

            var owner = ModEntry.ProfessionsApi?.GetConfigs().LaxOwnershipRequirements == false
                ? machine.GetOwner()
                : Game1.player;
            if (!owner.professions.Contains(Farmer.botanist) && ModEntry.Config.AgeImprovesMushroomBoxes)
                held.Quality = held.GetQualityFromAge();
            else if (ModEntry.ProfessionsApi is not null)
                held.Quality = Math.Max(ModEntry.ProfessionsApi.GetEcologistForageQuality(owner), held.Quality);
            else
                held.Quality = SObject.bestQuality;

            if (ModEntry.Config.MushroomBoxesRewardExp)
                Game1.player.gainExperience(Farmer.foragingSkill, 1);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}