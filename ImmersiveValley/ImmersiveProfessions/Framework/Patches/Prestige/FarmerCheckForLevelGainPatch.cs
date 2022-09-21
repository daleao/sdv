namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCheckForLevelGainPatch : HarmonyPatch
{
    private const int PrestigeGate = 15000;

    /// <summary>Initializes a new instance of the <see cref="FarmerCheckForLevelGainPatch"/> class.</summary>
    internal FarmerCheckForLevelGainPatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.checkForLevelGain));
    }

    #region harmony patches

    /// <summary>Patch to allow level increase up to 20.</summary>
    [HarmonyPostfix]
    private static void FarmerCheckForLevelGainPostfix(ref int __result, int oldXP, int newXP)
    {
        if (!ModEntry.Config.EnablePrestige)
        {
            return;
        }

        for (var i = 1; i <= 10; ++i)
        {
            var requiredExpForThisLevel = PrestigeGate + (ModEntry.Config.RequiredExpPerExtendedLevel * i);
            if (oldXP >= requiredExpForThisLevel)
            {
                continue;
            }

            if (newXP < requiredExpForThisLevel)
            {
                return;
            }

            __result = i + 10;
        }
    }

    #endregion harmony patches
}
