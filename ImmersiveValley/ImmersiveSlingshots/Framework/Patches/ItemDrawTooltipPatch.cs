#pragma warning disable SA1611
namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using System.Text;
using DaLion.Common.Exceptions;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemDrawTooltipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ItemDrawTooltipPatch"/> class.</summary>
    internal ItemDrawTooltipPatch()
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.drawTooltip));
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Item.drawTooltip"/>.</summary>
    /// <remarks>Required by <see cref="Tool.drawTooltip"/> prefix.</remarks>
    [HarmonyReversePatch]
    internal static void ItemDrawTooltipReverse(
        object instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}
#pragma warning restore SA1611
