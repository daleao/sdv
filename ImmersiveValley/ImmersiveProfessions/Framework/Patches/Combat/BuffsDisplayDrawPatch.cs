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

using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class BuffsDisplayDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static readonly int _buffId = (ModEntry.Manifest.UniqueID + Profession.Brute).GetHashCode();

    /// <summary>Construct an instance.</summary>
    internal BuffsDisplayDrawPatch()
    {
        Target = RequireMethod<BuffsDisplay>(nameof(BuffsDisplay.draw), new[] {typeof(SpriteBatch)});
    }

    /// <summary>Patch to draw Brute Rage buff.</summary>
    [HarmonyPostfix]
    internal static void BuffsDisplayDrawPostfix(Dictionary<ClickableTextureComponent, Buff> ___buffs, SpriteBatch b)
    {
        var (key, value) = ___buffs.FirstOrDefault(p => p.Value.which == _buffId);
        if (value is null) return;

        var counter = ModEntry.PlayerState.BruteRageCounter;
        b.DrawString(Game1.tinyFont, counter.ToString(),
            new(key.bounds.Right - (counter >= 10 ? 16 : 8), key.bounds.Bottom - 24), Color.White);
    }
}