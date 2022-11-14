namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Weapons.Enchantments;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatch : HarmonyPatch
{
    private static readonly int BuffId = (ModEntry.Manifest.UniqueID + "Energized").GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BuffsDisplayDrawPatch"/> class.</summary>
    internal BuffsDisplayDrawPatch()
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
