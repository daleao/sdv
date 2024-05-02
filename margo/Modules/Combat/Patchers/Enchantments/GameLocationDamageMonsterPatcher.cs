namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Steadfast enchantment crit. to damage conversion.</summary>
    [HarmonyPrefix]
    private static void GameLocationDamageMonsterPrefix(
        ref int minDamage,
        ref int maxDamage,
        ref float critChance,
        float critMultiplier,
        Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon || !weapon.hasEnchantmentOfType<SteadfastEnchantment>())
        {
            return;
        }

        var k = weapon.DefaultCritChance() * (weapon.DefaultCritPower() - 1) / weapon.DefaultCritPower();
        minDamage += (int)(minDamage * critMultiplier * k);
        maxDamage += (int)(maxDamage * critMultiplier * k);
        critChance = 0f;
    }

    #endregion harmony patches
}
