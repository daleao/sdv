namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeCollisionWithFarmerBehaviorPatcher : HarmonyPatcher
{
    private const int FARMER_INVINCIBILITY_FRAMES = 72;

    /// <summary>Initializes a new instance of the <see cref="GreenSlimeCollisionWithFarmerBehaviorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GreenSlimeCollisionWithFarmerBehaviorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper LimitBreak meter on contact with Slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        __instance.farmerPassesThrough = __instance.farmerPassesThrough || __instance.Get_Piped() != null;
        if (!__instance.currentLocation.IsEnemyArea())
        {
            return;
        }

        var who = __instance.Player;
        if (!who.IsLocalPlayer ||
            State.LimitBreak is not PiperConcerto { IsActive: false, SlimeContactTimer: <= 0 } concerto)
        {
            return;
        }

        concerto.ChargeValue += Game1.random.Next(1, 4);
        concerto.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES;
        EventManager.Enable<ConcertoUpdateTickedEvent>();
    }

    #endregion harmony patches
}
