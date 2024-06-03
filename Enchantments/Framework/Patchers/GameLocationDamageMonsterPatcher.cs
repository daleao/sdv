namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationDamageMonsterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            [
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Steadfast enchantment crit. to damage conversion.</summary>
    [HarmonyPrefix]
    private static void GameLocationDamageMonsterPrefix(
        ref int minDamage,
        ref int maxDamage,
        ref float critChance,
        Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon || !weapon.hasEnchantmentOfType<SteadfastEnchantment>())
        {
            return;
        }

        var k = weapon.critChance.Value * (weapon.critMultiplier.Value - 1f) / weapon.critMultiplier.Value;
        minDamage += (int)(minDamage * (1f + k));
        maxDamage += (int)(maxDamage * (1f + k));
        critChance = 0f;
    }

    #endregion harmony patches
}
