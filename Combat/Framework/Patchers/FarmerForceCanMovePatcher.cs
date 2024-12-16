﻿namespace DaLion.Combat.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerForceCanMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerForceCanMovePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerForceCanMovePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.forceCanMove));
    }

    #region harmony patches

    /// <summary>Reset animation state.</summary>
    [HarmonyPostfix]
    private static void FarmerForceCanMovePostfix(Farmer __instance)
    {
        if (!__instance.IsLocalPlayer)
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
