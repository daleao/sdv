namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
{
    private const int FARMER_INVINCIBILITY_FRAMES_I = 72;

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Ultimate meter on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        if (!__instance.currentLocation.IsDungeon()) return;

        var who = __instance.Player;
        if (!who.IsLocalPlayer ||
            ModEntry.PlayerState.RegisteredUltimate is not Pandemonium {IsActive: false} pandemonium ||
            ModEntry.PlayerState.SlimeContactTimer > 0) return;

        pandemonium.ChargeValue += Game1.random.Next(1, 4);
        ModEntry.PlayerState.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES_I;
    }

    #endregion harmony patches
}