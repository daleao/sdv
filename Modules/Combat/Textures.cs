namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    private static Lazy<Texture2D> _gemstonesTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/objects/gemstones"));

    private static Lazy<Texture2D> _gemSocketTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets"));

    private static Lazy<Texture2D> _ringsTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/objects/rings"));

    private static Lazy<Texture2D> _shieldTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/shield"));

    private static Lazy<Texture2D> _patternedResonanceTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/resonance_patterned"));

    private static Lazy<Texture2D> _strongerResonanceTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/resonance_stronger"));

    private static Lazy<Texture2D> _energizedTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/effects/energized_hd"));

    private static Lazy<Texture2D> _tooltipsTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/interface/tooltips"));

    internal static Texture2D GemstonesTx => _gemstonesTx.Value;

    internal static Texture2D GemSocketTx => _gemSocketTx.Value;

    internal static Texture2D RingsTx => _ringsTx.Value;

    internal static Texture2D ShieldTx => _shieldTx.Value;

    internal static Texture2D PatternedResonanceTx => _patternedResonanceTx.Value;

    internal static Texture2D StrongerResonanceTx => _strongerResonanceTx.Value;

    internal static Texture2D EnergizedTx => _energizedTx.Value;

    internal static Texture2D TooltipsTx => _tooltipsTx.Value;

    internal static void Refresh(IReadOnlySet<IAssetName> names)
    {
        if (names.Any(name => name.IsEquivalentTo($"{Manifest.UniqueID}/GemstoneSockets")))
        {
            _gemSocketTx = new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/GemstoneSockets"));
        }
    }
}
