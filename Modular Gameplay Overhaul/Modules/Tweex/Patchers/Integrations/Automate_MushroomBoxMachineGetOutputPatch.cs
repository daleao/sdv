namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class MushroomBoxMachineGetOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MushroomBoxMachineGetOutputPatcher"/> class.</summary>
    internal MushroomBoxMachineGetOutputPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated Mushroom Boxes.</summary>
    [HarmonyPrefix]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            var machine = Reflector
                .GetUnboundPropertyGetter<object, SObject>(__instance, "Machine")
                .Invoke(__instance);
            if (machine.heldObject.Value is not { } || TweexModule.Config.MushroomBoxExpReward <= 0)
            {
                return;
            }

            var owner = ProfessionsModule.IsEnabled && !ProfessionsModule.Config.LaxOwnershipRequirements
                ? machine.GetOwner()
                : Game1.player;
            owner.gainExperience(Farmer.foragingSkill, (int)TweexModule.Config.MushroomBoxExpReward);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
