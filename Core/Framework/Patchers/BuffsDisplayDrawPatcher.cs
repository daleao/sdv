namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Core.Framework;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuffsDisplayDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BuffsDisplayDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), [typeof(SpriteBatch)]);
    }

    /// <summary>Patch to draw stackable buffs.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        foreach (var (clickableTextureComponent, buff) in ___buffs)
        {
            if (buff is not StackableBuff stackable)
            {
                continue;
            }

            var stacks = stackable.Stacks;
            b.DrawString(
                Game1.tinyFont,
                stacks.ToString(),
                new Vector2(
                    clickableTextureComponent.bounds.Right - (stacks >= 10 ? stacks >= 100 ? 24 : 16 : 8),
                    clickableTextureComponent.bounds.Bottom - 24),
                stacks >= stackable.MaxStacks ? Color.Yellow : Color.White);
        }
    }
}
