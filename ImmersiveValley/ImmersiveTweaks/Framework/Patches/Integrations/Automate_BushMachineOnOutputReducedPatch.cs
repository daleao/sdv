namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions.Reflection;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class BushMachineOnOutputReducedPatch : BasePatch
{
    private delegate Bush GetMachineDelegate(object instance);

    private static GetMachineDelegate _GetBushMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineOnOutputReducedPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .RequireMethod("OnOutputReduced");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.BerryBushesRewardExp) return;

        _GetBushMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CreateDelegate<GetMachineDelegate>();
        var machine = _GetBushMachine(__instance);
        if (machine.size.Value >= Bush.greenTeaBush) return;

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 3);
    }

    #endregion harmony patches
}