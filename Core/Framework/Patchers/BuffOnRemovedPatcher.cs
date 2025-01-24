namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffOnRemovedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffOnRemovedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages __instance patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BuffOnRemovedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Buff>(nameof(Buff.OnRemoved));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BuffCtorPostfix(Buff __instance)
    {
        if (__instance.id != BuffIDs.Jinxed)
        {
            return; // run original logic
        }

        MeleeWeapon.clubCooldown = 0;
        MeleeWeapon.daggerCooldown = 0;
        MeleeWeapon.defenseCooldown = 0;
    }

    #endregion harmony patches
}
