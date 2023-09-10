namespace DaLion.Chargeable.Framework.Extensions;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>
    ///     Temporarily sets up the <paramref name="farmer"/> to interact with a tile, then return it to the original
    ///     state.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="action">The action to perform.</param>
    internal static void TemporarilyFakeInteraction(this Farmer farmer, Action action)
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

    /// <summary>Cancels the <paramref name="farmer"/>'s current animation if it matches one of the given IDs.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="animationIds">The animation IDs to detect.</param>
    internal static void CancelAnimation(this Farmer farmer, params int[] animationIds)
    {
        var animationId = ModHelper.Reflection.GetField<int>(farmer.FarmerSprite, "currentSingleAnimation")
            .GetValue();
        if (animationIds.All(id => id != animationId))
        {
            return;
        }

        farmer.completelyStopAnimatingOrDoingAction();
        farmer.forceCanMove();
    }
}
