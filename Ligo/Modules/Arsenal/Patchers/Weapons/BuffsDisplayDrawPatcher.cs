namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatcher : HarmonyPatcher
{
    private static readonly int BuffId = (Manifest.UniqueID + "Energized").GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffsDisplayDrawPatcher"/> class.</summary>
    internal BuffsDisplayDrawPatcher()
    {
        this.Target = this.RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Patch to draw Energized buff.</summary>
    [HarmonyPostfix]
    private static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var energized = (Game1.player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<EnergizedEnchantment>();
        if (energized is null)
        {
            return;
        }

        var (clickableTextureComponent, buff) = ___buffs.FirstOrDefault(p => p.Value.which == BuffId);
        if ((clickableTextureComponent, buff) == default((object, object)))
        {
            return;
        }

        var counter = energized.Stacks;
        b.DrawString(
            Game1.tinyFont,
            counter.ToString(),
            new Vector2(
                clickableTextureComponent.bounds.Right - (counter >= 10 ? 16 : 8),
                clickableTextureComponent.bounds.Bottom - 24),
            Color.White);
    }
}
