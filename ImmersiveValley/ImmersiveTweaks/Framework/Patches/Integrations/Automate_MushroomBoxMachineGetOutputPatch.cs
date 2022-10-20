namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System.Reflection;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Tweex.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class MushroomBoxMachineGetOutputPatch : HarmonyPatch
{
    private static Func<object, SObject>? _getMachine;

    /// <summary>Initializes a new instance of the <see cref="MushroomBoxMachineGetOutputPatch"/> class.</summary>
    internal MushroomBoxMachineGetOutputPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality.</summary>
    [HarmonyPrefix]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (!ModEntry.Config.AgeImprovesMushroomBoxes)
            {
                return;
            }

            _getMachine ??= __instance
                .GetType()
                .RequirePropertyGetter("Machine")
                .CompileUnboundDelegate<Func<object, SObject>>();
            var machine = _getMachine(__instance);
            if (machine.heldObject.Value is not { } held)
            {
                return;
            }

            var owner = ModEntry.ProfessionsApi?.GetConfigs().LaxOwnershipRequirements == false
                ? machine.GetOwner()
                : Game1.player;
            if (!owner.professions.Contains(Farmer.botanist) && ModEntry.Config.AgeImprovesMushroomBoxes)
            {
                held.Quality = held.GetQualityFromAge();
            }
            else if (ModEntry.ProfessionsApi is not null)
            {
                held.Quality = Math.Max(ModEntry.ProfessionsApi.GetEcologistForageQuality(owner), held.Quality);
            }
            else
            {
                held.Quality = SObject.bestQuality;
            }

            if (ModEntry.Config.MushroomBoxesRewardExp)
            {
                Game1.player.gainExperience(Farmer.foragingSkill, 1);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
