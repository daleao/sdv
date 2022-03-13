namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.Automate;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;

#endregion using directives

[UsedImplicitly]
internal class BushMachineGetOutputPatch : BasePatch
{
    private static MethodInfo _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .MethodNamed("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    [HarmonyPostfix]
    private static void BushMachineGetOutputPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.BerryBushesRewardExp) return;

        _GetMachine ??= __instance.GetType().PropertyGetter("Machine");
        var machine = (Bush) _GetMachine.Invoke(__instance, null);
        if (machine is null || machine.size.Value >= Bush.greenTeaBush) return;

        Game1.MasterPlayer.gainExperience((int) SkillType.Foraging, 3);
    }

    #endregion harmony patches
}