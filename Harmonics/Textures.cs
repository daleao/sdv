namespace DaLion.Harmonics;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class Textures
{
    private static readonly Lazy<Texture2D> _infinityStones =
        new(() => ModHelper.ModContent.Load<Texture2D>($"assets/infinity_stones"));

    private static readonly Lazy<Texture2D> _patternedResonanceTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/resonance_patterned"));

    private static readonly Lazy<Texture2D> _strongerResonanceTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/resonance_stronger"));

    internal static Texture2D InfinityStones => _infinityStones.Value;

    internal static Texture2D PatternedResonanceTx => _patternedResonanceTx.Value;

    internal static Texture2D StrongerResonanceTx => _strongerResonanceTx.Value;
}
