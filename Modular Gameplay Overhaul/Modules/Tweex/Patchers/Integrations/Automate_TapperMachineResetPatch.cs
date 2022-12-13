namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class TapperMachineResetPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TapperMachineResetPatcher"/> class.</summary>
    internal TapperMachineResetPatcher()
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
        if (!TweexModule.Config.TappersRewardExp)
        {
            return;
        }

        Reflector
            .GetUnboundPropertyGetter<object, SObject>(__instance, "Machine")
            .Invoke(__instance)
            .GetOwner()
            .gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
