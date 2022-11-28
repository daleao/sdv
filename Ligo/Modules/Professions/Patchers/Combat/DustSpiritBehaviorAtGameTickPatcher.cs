namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class DustSpiritBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DustSpiritBehaviorAtGameTickPatcher"/> class.</summary>
    internal DustSpiritBehaviorAtGameTickPatcher()
    {
        this.Target = this.RequireMethod<DustSpirit>(nameof(DustSpirit.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Dust Spirits during Ultimate.</summary>
    [HarmonyPostfix]
    private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
    {
        if (!__instance.Player.IsLocalPlayer || __instance.Player.Get_Ultimate() is not
                Ambush { IsActive: true })
        {
            return;
        }

        ___seenFarmer = false;
    }

    #endregion harmony patches
}
