namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
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

    /// <summary>Implement blind status.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance, ref int damage, Monster damager)
    {
        if (!damager.IsBlinded() || !Game1.random.NextBool())
        {
            return true; // run original logic
        }

        damage = -1;
        var missText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Attack_Miss");
        __instance.currentLocation.debris.Add(new Debris(missText, 1, new Vector2(__instance.StandingPixel.X, __instance.StandingPixel.Y), Color.LightGray, 1f, 0f));
        return false; // don't run original logic
    }

    /// <summary>Reset seconds-out-of-combat.</summary>
    [HarmonyPostfix]
    private static void FarmerTakeDamagePostfix(Farmer __instance)
    {
        if (__instance.IsLocalPlayer)
        {
            State.SecondsOutOfCombat = 0;
        }
    }

    #endregion harmony patches
}
