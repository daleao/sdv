namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using JetBrains.Annotations;
using StardewValley;

using Common.Extensions.Reflection;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TapperMachineResetPatch : BasePatch
{
    private delegate SObject GetMachineDelegate(object instance);

    private static GetMachineDelegate _GetMachine;

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
        if (__instance is null || !ModEntry.Config.TappersRewardExp) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CreateDelegate<GetMachineDelegate>();
        var machine = _GetMachine(__instance);
        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        owner.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}