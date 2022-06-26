namespace DaLion.Stardew.Tools.Extensions;

#region using directives

using StardewValley;
using System;
using System.Linq;

#endregion using directives

public static class FarmerExtensions
{
    /// <summary>Temporarily set up the farmer to interact with a tile, then return it to the original state.</summary>
    /// <param name="action">The action to perform.</param>
    public static void TemporarilyFakeInteraction(this Farmer farmer, Action action)
    {
        // save current state
        var stamina = farmer.stamina;
        var position = farmer.Position;
        var facingDirection = farmer.FacingDirection;
        var currentToolIndex = farmer.CurrentToolIndex;
        var canMove = farmer.canMove; // fix player frozen due to animations when performing an action

        // perform action
        try
        {
            action();
        }
        finally
        {
            // restore previous state
            farmer.stamina = stamina;
            farmer.Position = position;
            farmer.FacingDirection = facingDirection;
            farmer.CurrentToolIndex = currentToolIndex;
            farmer.canMove = canMove;
        }
    }

    /// <summary>Cancel the current player animation if it matches one of the given IDs.</summary>
    /// <param name="animationIds">The animation IDs to detect.</param>
    public static void CancelAnimation(this Farmer who, params int[] animationIds)
    {
        var animationId = ModEntry.ModHelper.Reflection.GetField<int>(who.FarmerSprite, "currentSingleAnimation")
            .GetValue();
        if (animationIds.Any(id => id == animationId))
        {
            who.completelyStopAnimatingOrDoingAction();
            who.forceCanMove();
        }
    }
}