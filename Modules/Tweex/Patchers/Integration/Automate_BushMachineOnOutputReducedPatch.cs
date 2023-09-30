namespace DaLion.Overhaul.Modules.Tweex.Patchers.Integration;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate", "Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class BushMachineOnOutputReducedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushMachineOnOutputReducedPatcher"/> class.</summary>
    internal BushMachineOnOutputReducedPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine"
            .ToType()
            .RequireMethod("OnOutputReduced");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    [HarmonyPostfix]
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (TweexModule.Config.BerryBushExpReward <= 0)
        {
            return;
        }

        var machine = Reflector
            .GetUnboundPropertyGetter<object, Bush>(__instance, "Machine")
            .Invoke(__instance);
        if (machine.size.Value >= Bush.greenTeaBush)
        {
            return;
        }

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, (int)TweexModule.Config.BerryBushExpReward);
    }

    #endregion harmony patches
}
