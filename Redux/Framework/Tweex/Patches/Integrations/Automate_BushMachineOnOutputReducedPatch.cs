namespace DaLion.Redux.Framework.Tweex.Patches;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class BushMachineOnOutputReducedPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BushMachineOnOutputReducedPatch"/> class.</summary>
    internal BushMachineOnOutputReducedPatch()
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
        if (!ModEntry.Config.Tweex.BerryBushesRewardExp)
        {
            return;
        }

        var machine = ModEntry.Reflector
            .GetUnboundPropertyGetter<object, Bush>(__instance, "Machine")
            .Invoke(__instance);
        if (machine.size.Value >= Bush.greenTeaBush)
        {
            return;
        }

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
