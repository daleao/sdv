namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Common.Harmony;
using JetBrains.Annotations;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TapperMachineResetPatch : HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal TapperMachineResetPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine".ToType()
                .RequireMethod("Reset");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated tappers.</summary>
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (!ModEntry.Config.TappersRewardExp) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, SObject>>();
        var machine = _GetMachine(__instance);
        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}