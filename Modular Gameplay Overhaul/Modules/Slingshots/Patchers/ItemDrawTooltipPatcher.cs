#pragma warning disable SA1611
namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Runtime.CompilerServices;
using System.Text;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemDrawTooltipPatcher"/> class.</summary>
    internal ItemDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.drawTooltip));
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="Item.drawTooltip"/>.</summary>
    /// <remarks>Required by <see cref="Tool.drawTooltip"/> prefix.</remarks>
    [HarmonyReversePatch]
    [MethodImpl(MethodImplOptions.NoInlining)]
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
