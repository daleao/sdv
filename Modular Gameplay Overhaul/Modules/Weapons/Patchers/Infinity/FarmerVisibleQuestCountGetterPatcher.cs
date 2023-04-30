namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerVisibleQuestCountGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerVisibleQuestCountGetterPatcher"/> class.</summary>
    internal FarmerVisibleQuestCountGetterPatcher()
    {
        this.Target = this.RequirePropertyGetter<Farmer>(nameof(Farmer.visibleQuestCount));
    }

    #region harmony patches

    /// <summary>Consider Virtues quest as visible.</summary>
    [HarmonyPostfix]
    private static void FarmerVisibleQuestCountGetterPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.IsLocalPlayer && WeaponsModule.State.VirtuesQuest is not null)
        {
            __result++;
        }
    }

    #endregion harmony patches
}
