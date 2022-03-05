namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal class BuffsDisplayDrawPatch : BasePatch
{
    private static readonly int _buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Brute;

    /// <summary>Construct an instance.</summary>
    internal BuffsDisplayDrawPatch()
    {
        Original = RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] {typeof(SpriteBatch)});
    }

    /// <summary>Patch to draw BuffsDisplay overcharge meter for Desperado.</summary>
    [HarmonyPostfix]
    internal static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var pair = ___buffs.FirstOrDefault(p => p.Value.which == _buffId);
        if (pair.Value is null) return;

        var counter = ModEntry.PlayerState.Value.BruteRageCounter;
        b.DrawString(Game1.tinyFont, counter.ToString(),
            new(pair.Key.bounds.Right - (counter >= 10 ? 16 : 8), pair.Key.bounds.Bottom - 24), Color.White);
    }
}