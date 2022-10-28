namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class VampiricEnchantmentOnMonsterSlayPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="VampiricEnchantmentOnMonsterSlayPatch"/> class.</summary>
    internal VampiricEnchantmentOnMonsterSlayPatch()
    {
        this.Target = this.RequireMethod<VampiricEnchantment>("_OnMonsterSlay");
    }

    #region harmony patches

    /// <summary>Rebalances Vampiric enchant.</summary>
    [HarmonyPrefix]
    private static bool VampiricEnchantmentOnMonsterSlayPrefix(Monster m, GameLocation location, Farmer who)
    {
        if (!ModEntry.Config.Arsenal.Weapons.OverhauledEnchants)
        {
            return true; // run original logic
        }

        var amount = Math.Max((int)((m.MaxHealth + Game1.random.Next(-m.MaxHealth / 10, m.MaxHealth / 15)) * 0.05f), 1);
        who.health = Math.Min(who.health + amount, (int)(who.maxHealth * 1.1));
        location.debris.Add(
            new Debris(amount, new Vector2(who.getStandingX(), who.getStandingY()), Color.Lime, 1f, who));
        Game1.playSound("healSound");
        return false; // don't run original logic
    }

    #endregion harmony patches
}
