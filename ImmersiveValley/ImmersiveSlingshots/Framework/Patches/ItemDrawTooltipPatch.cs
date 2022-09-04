namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Common.Exceptions;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemDrawItemtipPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ItemDrawItemtipPatch()
    {
        Target = RequireMethod<Item>(nameof(Item.drawTooltip));
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Item.drawTooltip">.</summary>
    /// <remarks>Required by <see cref="Tool.drawTooltip"> prefix.</remarks>
    [HarmonyReversePatch]
    internal static void ItemDrawTooltipReverse(object instance, SpriteBatch spriteBatch, ref int x, ref int y,
        SpriteFont font, float alpha, StringBuilder? overrideText)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    #endregion harmony patches
}