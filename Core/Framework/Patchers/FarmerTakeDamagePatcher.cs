namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerTakeDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Implement various debuff effects.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerTakeDamagePrefix(Farmer __instance, ref bool __state, ref int damage, Monster? damager)
    {
        if (damager is null)
        {
            return true; // run original logic
        }

        if (damager.IsBurning())
        {
            damage /= 2;
        }

        if (damager.IsBlinded() && Game1.random.NextBool())
        {
            damage = -1;
            var missText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Attack_Miss");
            __instance.currentLocation.debris.Add(
                new Debris(
                    missText,
                    1,
                    new Vector2(__instance.StandingPixel.X, __instance.StandingPixel.Y),
                    Color.LightGray,
                    1f,
                    0f));
            return false; // don't run original logic
        }

        if (__instance.IsFrozen())
        {
            damage *= 2;
            __instance.Defrost();
        }

        if (__instance.IsJinxed())
        {
            var defense = __instance.buffs.FloatingDefense();
            var debuff = new Buff($"{UniqueId}_Jinxed", effects: new BuffEffects() { Defense = { defense / 2f } });
            __instance.applyBuff(debuff);
            __state = true;
        }

        return true; // run original logic
    }

    /// <summary>Reset seconds-out-of-combat.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmerTakeDamagePostfix(Farmer __instance, bool __state)
    {
        if (__instance.IsLocalPlayer)
        {
            State.SecondsOutOfCombat = 0;
        }

        if (__state)
        {
            __instance.buffs.Remove($"{UniqueId}_Jinxed");
        }
    }

    #endregion harmony patches
}
