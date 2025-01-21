namespace DaLion.Combat.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanMoveNowPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanMoveNowPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerCanMoveNowPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.canMoveNow));
    }

    #region harmony patches

    /// <summary>Reset animation state.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmerCanMoveNowPostfix(Farmer who)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        State.FarmerAnimating = false;
        if (Config.SlickMoves)
        {
            State.DriftVelocity = Vector2.Zero;
        }
    }

    #endregion harmony patches
}
