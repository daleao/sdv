namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Core.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcWithinPlayerThresholdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcWithinPlayerThresholdPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal NpcWithinPlayerThresholdPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.withinPlayerThreshold), [typeof(int)]);
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in LimitBreak.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
    {
        if (__instance is not Monster { IsMonster: true } monster)
        {
            return true; // run original method
        }

        try
        {
            if (!monster.Get_Target().IsAmbushing())
            {
                return true; // run original method
            }

            __result = false;
            return false; // don't run original method
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patch
}
