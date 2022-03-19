namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.Automate;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewValley;

using Common.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class TapperMachineResetPatch : BasePatch
{
    private static MethodInfo _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal TapperMachineResetPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine".ToType()
                .MethodNamed("Reset");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.TappersRewardExp) return;

        _GetMachine ??= __instance.GetType().PropertyGetter("Machine");
        var machine = (SObject) _GetMachine.Invoke(__instance, null);
        if (machine is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience((int) SkillType.Foraging, 5);
    }

    #endregion harmony patches
}