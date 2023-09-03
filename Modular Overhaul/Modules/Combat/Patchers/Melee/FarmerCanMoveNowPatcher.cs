namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

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
    internal FarmerCanMoveNowPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.canMoveNow));
    }

    #region harmony patches

    /// <summary>Reset animation state.</summary>
    [HarmonyPostfix]
    private static void FarmerCanMoveNowPostfix(Farmer who)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        CombatModule.State.FarmerAnimating = false;
        if (CombatModule.Config.SlickMoves)
        {
            CombatModule.State.DriftVelocity = Vector2.Zero;
        }
    }

    #endregion harmony patches
}
