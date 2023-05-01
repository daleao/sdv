namespace DaLion.Overhaul.Modules.Weapons.Patchers;

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

        WeaponsModule.State.FarmerAnimating = false;
        if (WeaponsModule.Config.SlickMoves)
        {
            WeaponsModule.State.DriftVelocity = Vector2.Zero;
        }
    }

    #endregion harmony patches
}
