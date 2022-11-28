#pragma warning disable SA1611
namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Shared.Exceptions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingDrawInMenuPatcher"/> class.</summary>
    internal RingDrawInMenuPatcher()
    {
        this.Target = this.RequireMethod<Ring>(
            nameof(Ring.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Ring.drawInMenu"/>.</summary>
    [HarmonyReversePatch]
    internal static void RingDrawInMenuReverse(
        object instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}
#pragma warning restore SA1611
