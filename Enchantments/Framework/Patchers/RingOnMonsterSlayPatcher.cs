namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnMonsterSlayPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnMonsterSlayPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal RingOnMonsterSlayPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onMonsterSlay));
    }

    #region harmony patches

    /// <summary>Fix for vampiric ring reseting overheal.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void RingOnMonsterSlayPrefix(Ring __instance, Farmer who)
    {
        if (who.IsLocalPlayer && __instance.QualifiedItemId == QIDs.VampireRing &&
            who.CurrentTool is MeleeWeapon weapon && weapon.hasEnchantmentOfType<VampiricEnchantment>())
        {
            who.health = Math.Min(who.health + 2, (int)(who.maxHealth * 1.2f));
        }
    }

    #endregion harmony patches
}
