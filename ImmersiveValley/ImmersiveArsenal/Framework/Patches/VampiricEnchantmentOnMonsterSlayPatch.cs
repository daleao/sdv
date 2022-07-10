namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class VampiricEnchantmentOnMonsterSlayPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal VampiricEnchantmentOnMonsterSlayPatch()
    {
        Target = RequireMethod<VampiricEnchantment>("_OnMonsterSlay");
    }

    #region harmony patches

    /// <summary>Rebalances Vampiric enchant.</summary>
    [HarmonyPrefix]
    private static bool VampiricEnchantmentOnMonsterSlayPrefix(Monster m, GameLocation location, Farmer who)
    {
        if (!ModEntry.Config.RebalancedEnchants) return true; // run original logic

        if (Game1.random.NextDouble() > 0.5) return false; // don't run original logic

        var amount = Math.Max((int)((m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15)) * 0.05f),
            1);
        who.health = Math.Min(who.health + amount, who.maxHealth);
        location.debris.Add(new(amount, new(who.getStandingX(), who.getStandingY()), Color.Lime, 1f, who));
        Game1.playSound("healSound");
        return false; // don't run original logic
    }

    #endregion harmony patches
}