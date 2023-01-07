namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
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

    /// <summary>Adds foraging experience for automated Tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (TweexModule.Config.TapperExpReward <= 0)
        {
            return;
        }

        Reflector
            .GetUnboundPropertyGetter<object, SObject>(__instance, "Machine")
            .Invoke(__instance)
            .GetOwner()
            .gainExperience(Farmer.foragingSkill, (int)TweexModule.Config.TapperExpReward);
    }

    #endregion harmony patches
}
