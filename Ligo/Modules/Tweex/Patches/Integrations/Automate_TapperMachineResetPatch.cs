namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class TapperMachineResetPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TapperMachineResetPatch"/> class.</summary>
    internal TapperMachineResetPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine"
            .ToType()
            .RequireMethod("Reset");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (!ModEntry.Config.Tweex.TappersRewardExp)
        {
            return;
        }

        ModEntry.Reflector
            .GetUnboundPropertyGetter<object, SObject>(__instance, "Machine")
            .Invoke(__instance)
            .GetOwner()
            .gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
