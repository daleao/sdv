namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using Common;
using Common.Integrations.WalkOfLife;
using Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAutoFireRatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetAutoFireRatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAutoFireRatePostfix(Slingshot __instance, ref float __result)
    {
        var ultimate = ModEntry.ProfessionsApi?.GetRegisteredUltimate();
        if (ultimate is not null && ultimate.Index == IImmersiveProfessions.UltimateIndex.Blossom &&
            ultimate.IsActive || !__instance.hasEnchantmentOfType<GatlingEnchantment>()) return;

        __result *= 1.5f;
    }

    #endregion harmony patches
}